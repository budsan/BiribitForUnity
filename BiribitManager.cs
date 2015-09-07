using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public interface BiribitListener
{
	void OnConnected(uint connectionId);
	void OnDisconnected(uint connectionId);

	void OnJoinedRoom(uint connectionId, uint roomId, byte slotId);

	void OnJoinedRoomPlayerJoined(uint connectionId, uint clientId, byte slotId);
	void OnJoinedRoomPlayerLeft(uint connectionId, uint clientId, byte slotId);

	void OnBroadcast(Biribit.BroadcastEvent evnt);
	void OnEntriesChanged(uint connectionId);
	void OnLeaveRoom(uint connectionId);
}

public class BiribitManager : MonoBehaviour
{
	protected class ConnectionInfo
	{
		public Biribit.Native.RemoteClient[] remoteClients = new Biribit.Native.RemoteClient[0];
		public List<int> remoteClientsById = new List<int>();

		public Biribit.Room[] rooms = new Biribit.Room[0];

		public uint local_id;
		public uint joined_room;
		public byte joined_room_slot;
		public uint[] current_slots = new uint[0];

		public List<Biribit.Entry> entries = new List<Biribit.Entry>();

		public void UpdateIds()
		{
			for (int i = 0; i < remoteClientsById.Count; i++)
				remoteClientsById[i] = -1;

			for (int i = 0; i < remoteClients.Length; i++)
			{
				uint id = remoteClients[i].id;
				while (id >= remoteClientsById.Count)
					remoteClientsById.Add(-1);

				remoteClientsById[(int) id] = i;
			}
				
		}

		public void Clear()
		{
			remoteClients = new Biribit.Native.RemoteClient[0];
			rooms = new Biribit.Room[0];
			joined_room = 0;
			joined_room_slot = 0;

			ClearSlots();
			ClearEntries();
		}

		public void ClearSlots()
		{
			for (int i = 0; i < current_slots.Length; i++)
				current_slots[i] = Biribit.Client.UnassignedId;
		}

		public void ClearEntries()
		{
			if (entries.Count > 0)
				entries.RemoveRange(0, entries.Count);
		}
	};

	private Biribit.Client m_client = new Biribit.Client();
	private ClientListener m_clientlistener = null;
	private delegate void ClientLogCallbackDelegate(string msg);
	private ClientLogCallbackDelegate m_clientCallback = null;
	private IntPtr m_clientCallbackPtr;
	private HashSet<BiribitListener> m_listeners = new HashSet<BiribitListener>();

	private Biribit.Native.ServerInfo[] m_serverInfo = new Biribit.Native.ServerInfo[0];
	private Biribit.Native.Connection[] m_connection = new Biribit.Native.Connection[0];
	private List<ConnectionInfo> m_connectionInfo = new List<ConnectionInfo>();

	public Biribit.Native.ServerInfo[] ServerInfo { get { return m_serverInfo; } }
	public Biribit.Native.Connection[] Connections { get { return m_connection; } }

	public Biribit.Native.RemoteClient[] RemoteClients(uint connectionId) {
		return m_connectionInfo[(int) connectionId].remoteClients;
	}

	public int RemoteClients(uint connectionId, uint id)
	{
		int pos = -1;
		ConnectionInfo info = m_connectionInfo[(int)connectionId];
		if (id < info.remoteClientsById.Count)
			pos = info.remoteClientsById[(int)id];

		return pos;
	}

	public Biribit.Room[] Rooms(uint connectionId) {
		return m_connectionInfo[(int)connectionId].rooms;
	}

	public uint LocalClientId(uint connectionId) {
		return m_connectionInfo[(int)connectionId].local_id;
	}

	public uint JoinedRoom(uint connectionId) {
		return m_connectionInfo[(int)connectionId].joined_room;
	}

	public uint JoinedRoomSlot(uint connectionId) {
		return m_connectionInfo[(int)connectionId].joined_room_slot;
	}

	public List<Biribit.Entry> JoinedRoomEntries(uint connectionId) {
		return m_connectionInfo[(int)connectionId].entries;
	}

	public void AddListener(BiribitListener listener)
	{
		m_listeners.Add(listener);
	}

	public void DelListener(BiribitListener listener)
	{
		m_listeners.Remove(listener);
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

			if (m_clientlistener == null) {
				m_clientlistener = new ClientListener(this);
				m_client.AddListener(m_clientlistener);
			}
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

			if (m_clientlistener != null)
				m_client.DelListener(m_clientlistener);
		}

		m_client.FreeClientPtr();
		NativeMethods.ClientDelLogCallback(m_clientCallbackPtr);
	}

	private void OnApplicationQuit()
	{
		NativeMethods.Clean();
	}

	private void Update()
	{
		m_client.Update();

		if (m_ex != null) {
			Exception ex = m_ex;
			m_ex = null;
			throw ex;
		}
	}

	//-----------------------------------------------------------------------//

	public void Connect(string address, ushort port = 0, string password = "") {
		m_client.Connect(address, port, password);
	}

	public void Disconnect(uint connectionId) {
		m_client.Disconnect(connectionId);
	}

	public void Disconnect() {
		m_client.Disconnect();
	}

	public void DiscoverServersOnLAN(ushort port = 0) {
		m_client.DiscoverServersOnLAN(port);
	}

	public void ClearServerList() {
		m_client.ClearServerList();
	}

	public void RefreshServerList() {
		m_client.RefreshServerList();
	}

	public void SetLocalClientParameters(uint connectionId, string clientName, string appId = "") {
		m_client.SetLocalClientParameters(connectionId, clientName, appId);
	}

	public void RefreshRooms(uint connectionId) {
		m_client.RefreshRooms(connectionId);
	}

	public void CreateRoom(uint connectionId, byte slotsCount) {
		m_client.CreateRoom(connectionId, slotsCount);
	}

	public void CreateRoom(uint connectionId, byte slotsCount, byte jointSlot) { 
		m_client.CreateRoom(connectionId, slotsCount, jointSlot);
	}

	public void JoinRandomOrCreateRoom(uint connectionId, byte slotsCount) {
		m_client.JoinRandomOrCreateRoom(connectionId, slotsCount);
	}

	public void JoinRoom(uint connectionId, uint roomId) {
		m_client.JoinRoom(connectionId, roomId);
	}

	public void JoinRoom(uint connectionId, uint roomId, byte jointSlot) {
		m_client.JoinRoom(connectionId, roomId, jointSlot);
	}

	public void SendBroadcast(uint connectionId, byte[] data, uint numBytes, Biribit.Native.ReliabilityBitmask mask = Biribit.Native.ReliabilityBitmask.UNRELIABLE) {
		m_client.SendBroadcast(connectionId, data, numBytes, mask);
	}

	public void SendBroadcast(uint connectionId, byte[] data, Biribit.Native.ReliabilityBitmask mask = Biribit.Native.ReliabilityBitmask.UNRELIABLE) {
		m_client.SendBroadcast(connectionId, data, mask);
	}

	public void SendEntry(uint connectionId, byte[] data, uint numBytes){
		m_client.SendEntry(connectionId, data, numBytes);
	}

	public void SendEntry(uint connectionId, byte[] data){
		m_client.SendEntry(connectionId, data);
	}


	//-----------------------------------------------------------------------//

	/// <summary>
	/// Making BiribitManager to implement Biribit.ClientListener is not desirable
	/// because it would make posible to call interface methods from outside. For
	/// avoid it, this private class ClientListener implements Biribit.ClientListener
	/// and calls BiribitManager methods which are private.
	/// </summary>
	private class ClientListener : Biribit.ClientListener
	{
		private BiribitManager parent;
		public ClientListener(BiribitManager _parent) { parent = _parent; }

		public void OnGetServerInfo(Biribit.Native.ServerInfo_array array) { parent.OnGetServerInfo(array); }
		public void OnGetConnection(Biribit.Native.Connection_array array) { parent.OnGetConnection(array); }
		public void OnGetRemoteClient(uint connectionId, Biribit.Native.RemoteClient_array array) { parent.OnGetRemoteClient(connectionId, array); }
		public void OnGetRoom(uint connectionId, Biribit.Native.Room_array array) { parent.OnGetRoom(connectionId, array); }

		public void OnBroadcastEvent(ref Biribit.Native.BroadcastEvent evnt) { parent.OnBroadcastEvent(ref evnt); }
		public void OnConnectionEvent(ref Biribit.Native.ConnectionEvent evnt) { parent.OnConnectionEvent(ref evnt); }
		public void OnEntriesEvent(ref Biribit.Native.EntriesEvent evnt) { parent.OnEntriesEvent(ref evnt); }
		public void OnErrorEvent(ref Biribit.Native.ErrorEvent evnt) { parent.OnErrorEvent(ref evnt); }
		public void OnJoinedRoomEvent(ref Biribit.Native.JoinedRoomEvent evnt) { parent.OnJoinedRoomEvent(ref evnt); }
		public void OnRemoteClientEvent(ref Biribit.Native.RemoteClientEvent evnt) { parent.OnRemoteClientEvent(ref evnt); }
		public void OnRoomListEvent(ref Biribit.Native.RoomListEvent evnt) { parent.OnRoomListEvent(ref evnt); }
		public void OnServerListEvent(ref Biribit.Native.ServerListEvent evnt) { parent.OnServerListEvent(ref evnt); }
		public void OnServerStatusEvent(ref Biribit.Native.ServerStatusEvent evnt) { parent.OnServerStatusEvent(ref evnt); }
	}

	private void OnGetServerInfo(Biribit.Native.ServerInfo_array array)
	{
		m_serverInfo = Biribit.Interop.PtrToArray<Biribit.Native.ServerInfo>(array.arr, array.size);
	}

	private void OnGetConnection(Biribit.Native.Connection_array array)
	{
		m_connection = Biribit.Interop.PtrToArray<Biribit.Native.Connection>(array.arr, array.size);
	}

	private void OnGetRemoteClient(uint connectionId, Biribit.Native.RemoteClient_array array)
	{
		while (m_connectionInfo.Count <= connectionId)
			m_connectionInfo.Add(new ConnectionInfo());

		m_connectionInfo[(int)connectionId].remoteClients = Biribit.Interop.PtrToArray<Biribit.Native.RemoteClient>(array.arr, array.size);
		m_connectionInfo[(int)connectionId].UpdateIds();
    }

	private void OnGetRoom(uint connectionId, Biribit.Native.Room_array array)
	{
		while (m_connectionInfo.Count <= connectionId)
			m_connectionInfo.Add(new ConnectionInfo());

		m_connectionInfo[(int)connectionId].rooms = Biribit.Interop.NativeToArray(array);
	}

	private void OnErrorEvent(ref Biribit.Native.ErrorEvent evnt)
	{
		Debug.LogError(Enum.GetName(typeof(Biribit.Native.ErrorId), evnt.which));
	}

	private void OnServerListEvent(ref Biribit.Native.ServerListEvent evnt)
	{
		m_client.GetServerList();
	}

	private void OnConnectionEvent(ref Biribit.Native.ConnectionEvent evnt)
	{
		m_client.GetConnections();
		switch(evnt.type)
		{
			case Biribit.Native.ConnectionEventType.TYPE_NEW_CONNECTION:
				while (m_connectionInfo.Count <= evnt.connection.id)
					m_connectionInfo.Add(new ConnectionInfo());

				Debug.Log("Successfully connected! Id: " + evnt.connection.id);
				foreach (BiribitListener listener in m_listeners)
					try { listener.OnConnected(evnt.connection.id); }
					catch (Exception ex) { PrintException(ex); }
				break;
			case Biribit.Native.ConnectionEventType.TYPE_DISCONNECTION:
				m_connectionInfo[(int) evnt.connection.id].Clear();
				Debug.Log("Disconnected!");
				foreach (BiribitListener listener in m_listeners)
					try { listener.OnDisconnected(evnt.connection.id); }
					catch (Exception ex) { PrintException(ex); }
				break;
			case Biribit.Native.ConnectionEventType.TYPE_NAME_UPDATED:
				Debug.Log("Welcome to " + evnt.connection.name + "!");
				break;
		}
	}

	private void OnServerStatusEvent(ref Biribit.Native.ServerStatusEvent evnt)
	{

	}

	private void OnRemoteClientEvent(ref Biribit.Native.RemoteClientEvent evnt)
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
		
		if (evnt.self != 0)
		{
			while (m_connectionInfo.Count <= evnt.connection)
				m_connectionInfo.Add(new ConnectionInfo());

			m_connectionInfo[(int)evnt.connection].local_id = evnt.client.id;
		}
	}

	private void OnRoomListEvent(ref Biribit.Native.RoomListEvent evnt)
	{
		ConnectionInfo info = m_connectionInfo[(int)evnt.connection];
		Biribit.Room[] rooms = Biribit.Interop.NativeToArray(evnt.rooms);
		Debug.Log("Room list updated!");

		if (info.joined_room > Biribit.Client.UnassignedId)
		{
			Biribit.Room joined = rooms[info.joined_room];
			for (int i = 0; i < joined.slots.Length; i++)
			{
				if (info.current_slots[i] != joined.slots[i])
				{
					if (info.current_slots[i] == Biribit.Client.UnassignedId)
						foreach (BiribitListener listener in m_listeners)
							try { listener.OnJoinedRoomPlayerJoined(evnt.connection, joined.slots[i], (byte) i); }
							catch (Exception ex) { PrintException(ex); }
					else if (joined.slots[i] == Biribit.Client.UnassignedId)
						foreach (BiribitListener listener in m_listeners)
							try { listener.OnJoinedRoomPlayerLeft(evnt.connection, info.current_slots[i], (byte)i); }
							catch (Exception ex) { PrintException(ex); }

					info.current_slots[i] = joined.slots[i];
				}
			}
		}

		info.rooms = rooms;
	}

	private void OnJoinedRoomEvent(ref Biribit.Native.JoinedRoomEvent evnt)
	{
		uint connectionId = evnt.connection;

		ConnectionInfo info = m_connectionInfo[(int) connectionId];
		info.joined_room = evnt.room_id;
		info.joined_room_slot = evnt.slot_id;
		info.ClearEntries();

		Biribit.Room joined = info.rooms[info.joined_room];
		if (info.current_slots.Length < joined.slots.Length)
			info.current_slots = new uint[joined.slots.Length];

		for (int i = 0; i < joined.slots.Length; i++)
			info.current_slots[i] = joined.slots[i];

		Debug.Log("Joined room " + info.joined_room + ", slot " + info.joined_room_slot + ".");

		if (info.joined_room > Biribit.Client.UnassignedId)
			foreach (BiribitListener listener in m_listeners)
				try { listener.OnJoinedRoom(evnt.connection, evnt.room_id, evnt.slot_id); }
				catch (Exception ex) { PrintException(ex); }
		else
			foreach (BiribitListener listener in m_listeners)
				try { listener.OnLeaveRoom(evnt.connection); }
				catch (Exception ex) { PrintException(ex); }
	}

	private void OnBroadcastEvent(ref Biribit.Native.BroadcastEvent native_evnt)
	{
		Biribit.BroadcastEvent envt = new Biribit.BroadcastEvent(native_evnt);
		foreach (BiribitListener listener in m_listeners)
			try { listener.OnBroadcast(envt); }
			catch (Exception ex) { PrintException(ex); }
	}

	private void OnEntriesEvent(ref Biribit.Native.EntriesEvent evnt)
	{
		uint count = m_client.GetEntriesCount(evnt.connection);
		var info = m_connectionInfo[(int) evnt.connection];
		for (uint i = (uint) info.entries.Count; i <= count; i++) {
			Biribit.Native.Entry native_entry = m_client.GetEntry(evnt.connection, i);
			info.entries.Add(new Biribit.Entry(native_entry));
		}

		foreach (BiribitListener listener in m_listeners)
			try { listener.OnEntriesChanged(evnt.connection); }
			catch (Exception ex) { PrintException(ex); }
	}

	private Exception m_ex = null;
	private void PrintException(Exception ex)
	{
		if (m_ex != null)
			m_ex = ex;
	}
}

