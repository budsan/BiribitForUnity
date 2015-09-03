using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class BiribitManager : MonoBehaviour, Biribit.ClientListener
{
	protected class ConnectionInfo
	{
		public Biribit.Native.RemoteClient[] remoteClients = new Biribit.Native.RemoteClient[0];
		public Biribit.Room[] rooms = new Biribit.Room[0];

		public uint joined_room;
		public byte joined_room_slot;

		public List<Biribit.Entry> entries = new List<Biribit.Entry>();

		public void Clear()
		{
			remoteClients = new Biribit.Native.RemoteClient[0];
			rooms = new Biribit.Room[0];
			joined_room = 0;
			joined_room_slot = 0;
			ClearEntries();
		}

		public void ClearEntries()
		{
			if (entries.Count > 0)
				entries.RemoveRange(0, entries.Count);
		}
	};

	private Biribit.Client m_client = new Biribit.Client();
	private delegate void ClientLogCallbackDelegate(string msg);
	private ClientLogCallbackDelegate m_clientCallback = null;
	private IntPtr m_clientCallbackPtr;

	private Biribit.Native.ServerInfo[] m_serverInfo = new Biribit.Native.ServerInfo[0];
	private Biribit.Native.Connection[] m_connection = new Biribit.Native.Connection[0];
	private List<ConnectionInfo> m_connectionInfo = new List<ConnectionInfo>();

	public Biribit.Native.ServerInfo[] ServerInfo { get { return m_serverInfo; } }
	public Biribit.Native.Connection[] Connections { get { return m_connection; } }

	public Biribit.Native.RemoteClient[] RemoteClients(uint connectionId) {
		return m_connectionInfo[(int) connectionId].remoteClients;
	}

	public Biribit.Room[] Rooms(uint connectionId) {
		return m_connectionInfo[(int)connectionId].rooms;
	}
	public uint JoinedRoom(uint connectionId) {
		return m_connectionInfo[(int)connectionId].joined_room;
	}

	public uint JoinedRoomSlot(uint connectionId) {
		return m_connectionInfo[(int)connectionId].joined_room_slot;
	}

	static private BiribitManager m_instance = null;
	static public BiribitManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				GameObject manager = new GameObject("BiribitManager");
				manager.AddComponent<BiribitManager>();
			}

			return m_instance;
		}
	}

	private partial class NativeMethods
	{
		const string DllNameClient = "BiribitClient";

		[DllImport(DllNameClient, EntryPoint = "BiribitClientClean")]
		public static extern bool Clean();

		[DllImport(DllNameClient, EntryPoint = "BiribitClientAddLogCallback")]
		public static extern bool ClientAddLogCallback(IntPtr func);

		[DllImport(DllNameClient, EntryPoint = "BiribitClientDelLogCallback")]
		public static extern bool ClientDelLogCallback(IntPtr func);
	}

	private void DebugLog(string msg)
	{
		Debug.Log(msg);
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
			NativeMethods.Clean();

			m_clientCallback = new ClientLogCallbackDelegate(DebugLog);
			m_clientCallbackPtr = Marshal.GetFunctionPointerForDelegate(m_clientCallback);
			NativeMethods.ClientAddLogCallback(m_clientCallbackPtr);
			m_client.AddListener(this);
		}
		else
		{
			Destroy(this);
		}
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
			m_client.DelListener(this);
		}
			
		NativeMethods.ClientDelLogCallback(m_clientCallbackPtr);
	}

	private void OnApplicationQuit()
	{
		m_client.FreeClientPtr();
		NativeMethods.Clean();
	}

	private void Update()
	{
		m_client.Update();
	}

	//-----------------------------------------------------------------------//

	public Biribit.Client Client
	{
		get
		{
			return m_client;
		}
	}

	//-----------------------------------------------------------------------//


	public void OnGetServerInfo(Biribit.Native.ServerInfo_array array)
	{
		m_serverInfo = Biribit.Interop.PtrToArray<Biribit.Native.ServerInfo>(array.arr, array.size);
	}

	public void OnGetConnection(Biribit.Native.Connection_array array)
	{
		m_connection = Biribit.Interop.PtrToArray<Biribit.Native.Connection>(array.arr, array.size);
	}

	public void OnGetRemoteClient(uint connectionId, Biribit.Native.RemoteClient_array array)
	{
		Debug.Log("connectionId: " + connectionId);
		while (m_connectionInfo.Count < connectionId)
			m_connectionInfo.Add(new ConnectionInfo());

		m_connectionInfo[(int) connectionId].remoteClients = Biribit.Interop.PtrToArray<Biribit.Native.RemoteClient>(array.arr, array.size);
	}

	public void OnGetRoom(uint connectionId, Biribit.Native.Room_array array)
	{
		Debug.Log("connectionId: " + connectionId);
		while (m_connectionInfo.Count < connectionId)
			m_connectionInfo.Add(new ConnectionInfo());

		m_connectionInfo[(int)connectionId].rooms = Biribit.Interop.NativeToArray(array);
	}

	public void OnErrorEvent(ref Biribit.Native.ErrorEvent evnt)
	{
		Debug.LogError(Enum.GetName(typeof(Biribit.Native.ErrorId), evnt.which));
	}

	public void OnServerListEvent(ref Biribit.Native.ServerListEvent evnt)
	{
		m_client.GetServerList();
	}

	public void OnConnectionEvent(ref Biribit.Native.ConnectionEvent evnt)
	{
		m_client.GetConnections();
		switch(evnt.type)
		{
			case Biribit.Native.ConnectionEventType.TYPE_NEW_CONNECTION:
				while (m_connectionInfo.Count < evnt.connection.id)
					m_connectionInfo.Add(new ConnectionInfo());
				Debug.Log("Successfully connected!");
				break;
			case Biribit.Native.ConnectionEventType.TYPE_DISCONNECTION:
				m_connectionInfo[(int) evnt.connection.id].Clear();
				Debug.Log("Disconnected!");
				break;
			case Biribit.Native.ConnectionEventType.TYPE_NAME_UPDATED:
				Debug.Log("Welcome to " + evnt.connection.name + "!");
				break;
		}
	}

	public void OnServerStatusEvent(ref Biribit.Native.ServerStatusEvent evnt)
	{

	}

	public void OnRemoteClientEvent(ref Biribit.Native.RemoteClientEvent evnt)
	{
		m_client.GetRemoteClients(evnt.connection);
		string appId = evnt.client.appid;
		if (string.IsNullOrEmpty(appId))
			appId = "None";

		switch (evnt.type)
		{
			case Biribit.Native.RemoteClientEventType.TYPE_CLIENT_UPDATED:
				Debug.Log((evnt.self != 0 ? "Your data" : "Client's data") +
					" has been updated ID: " + evnt.client.id +
					". Name: " + evnt.client.name +
					", AppId: " + appId); 
				break;
			case Biribit.Native.RemoteClientEventType.TYPE_CLIENT_DISCONNECTED:
				Debug.Log((evnt.self != 0 ? "Your have" : "Client has") +
					" been disconnected ID: " + evnt.client.id +
					". Name: " + evnt.client.name +
					", AppId: " + appId);

				break;
		}
	}

	public void OnRoomListEvent(ref Biribit.Native.RoomListEvent evnt)
	{
		m_connectionInfo[(int)evnt.connection].rooms = Biribit.Interop.NativeToArray(evnt.rooms);
		Debug.Log("Room list updated!");
	}

	public void OnJoinedRoomEvent(ref Biribit.Native.JoinedRoomEvent evnt)
	{
		uint connectionId = evnt.connection;

		ConnectionInfo info = m_connectionInfo[(int) connectionId];
		info.joined_room = evnt.room_id;
		info.joined_room_slot = evnt.slot_id;
		info.ClearEntries();

		Debug.Log("Joined room " + info.joined_room + ", slot " + info.joined_room_slot + ".");
	}

	public void OnBroadcastEvent(ref Biribit.Native.BroadcastEvent evnt)
	{

	}

	public void OnEntriesEvent(ref Biribit.Native.EntriesEvent evnt)
	{

	}
}

