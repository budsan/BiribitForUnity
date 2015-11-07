using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class BiribitRemoteClientNotFoundException : Exception
{
	public BiribitRemoteClientNotFoundException() { }
	public BiribitRemoteClientNotFoundException(string message) : base(message) { }
	public BiribitRemoteClientNotFoundException(string message, Exception inner) : base(message, inner) { }
}

public class BiribitRoomNotFoundException : Exception
{
	public BiribitRoomNotFoundException() {}
	public BiribitRoomNotFoundException(string message) : base(message) {}
	public BiribitRoomNotFoundException(string message, Exception inner) : base(message, inner) {}
}

public interface BiribitConnectionView
{
	Biribit.Native.RemoteClient[] RemoteClients { get; }
	Biribit.Room[] Rooms { get; }

	int GetRemoteClientIndex(uint id);
	int GetRoomIndex(uint id);

	Biribit.Native.RemoteClient GetRemoteClient(uint id);
	Biribit.Room GetRoom(uint id);

	uint LocalId { get; }
	uint JoinedRoomId { get; }
	byte JoinedRoomSlotId { get; }

	Biribit.Room JoinedRoom { get; }
	Biribit.Native.RemoteClient GetRemoteClientFromSlot(uint slotId);
}

public interface BiribitManagerListener
{
	void OnServerListUpdated();

	void OnConnected(uint connectionId);
	void OnDisconnected(uint connectionId);

	void OnJoinedRoom(uint connectionId, uint roomId, byte slotId);

	void OnJoinedRoomPlayerJoined(uint connectionId, uint clientId, byte slotId);
	void OnJoinedRoomPlayerLeft(uint connectionId, uint clientId, byte slotId);

	void OnBroadcast(Biribit.BroadcastEvent evnt);
	void OnEntriesChanged(uint connectionId);
	void OnLeaveRoom(uint connectionId);

	List<Biribit.Entry> Entries { get; }
}

public class BiribitManager : MonoBehaviour
{
	protected class ConnectionInfo : BiribitConnectionView
	{
		private Biribit.Native.RemoteClient[] remoteClients = new Biribit.Native.RemoteClient[0];
		private int[] remoteClientsIndex = new int[0];

		private Biribit.Room[] rooms = new Biribit.Room[0];
		private int[] roomsIndex = new int[0];

		public uint local_id;
		public uint joined_room;
		public byte joined_room_slot;
		public uint[] lastSlotsState = new uint[0];

		public Biribit.Native.RemoteClient[] RemoteClients
		{
			get { return remoteClients; }
			set { remoteClients = value; RebuildRemoteClientsIndex(); }
		}

		public Biribit.Room[] Rooms
		{
			get { return rooms; }
			set { rooms = value; RebuildRoomsIndex(); }
		}

		public int GetRemoteClientIndex(uint id)
		{
			if (id < 0 && id >= remoteClientsIndex.Length)
				return -1;
			else
				return remoteClientsIndex[id];
		}

		public int GetRoomIndex(uint id)
		{
			if (id < 0 && id >= roomsIndex.Length)
				return -1;
			else
				return roomsIndex[id];
		}

		public Biribit.Room GetRoom(uint id)
		{
			int index = GetRoomIndex(id);
			if (index < 0)
				throw new BiribitRoomNotFoundException();

			return rooms[index];
		}

		public Biribit.Native.RemoteClient GetRemoteClient(uint id)
		{
			int index = GetRemoteClientIndex(id);
			if (index < 0)
				throw new BiribitRemoteClientNotFoundException();

			return remoteClients[index];
		}

		public uint LocalId { get { return local_id; } }
		public uint JoinedRoomId { get { return joined_room; } }
		public byte JoinedRoomSlotId { get { return joined_room_slot; } }

		public Biribit.Room JoinedRoom { get { return GetRoom(JoinedRoomId); } }
		public Biribit.Native.RemoteClient GetRemoteClientFromSlot(uint slotId)
		{
			return GetRemoteClient(JoinedRoom.slots[slotId]);
		}

		private List<Biribit.Entry> entries = new List<Biribit.Entry>();
		public List<Biribit.Entry> Entries
		{
			get { return entries; }
		}

		private void RebuildRemoteClientsIndex()
		{
			uint length = 0;
			foreach (Biribit.Native.RemoteClient remoteclient in remoteClients)
				if (length < remoteclient.id)
					length = remoteclient.id;

			remoteClientsIndex = new int[length];
			for (int index = 0; index < remoteClientsIndex.Length; index++)
				remoteClientsIndex[index] = -1;

			for (int index = 0; index < remoteClients.Length; index++)
				remoteClientsIndex[remoteClients[index].id] = index;
		}

		private void RebuildRoomsIndex()
		{
			uint length = 0;
			foreach (Biribit.Room room in rooms)
				if (length < room.id)
					length = room.id;

			roomsIndex = new int[length];
			for (int index = 0; index < roomsIndex.Length; index++)
				roomsIndex[index] = -1;

			for (int index = 0; index < rooms.Length; index++)
				roomsIndex[rooms[index].id] = index;
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
			for (int i = 0; i < lastSlotsState.Length; i++)
				lastSlotsState[i] = Biribit.Client.UnassignedId;
		}

		public void ClearEntries()
		{
			entries.Clear();
		}
	};

	private Biribit.Client m_client = new Biribit.Client();
	private ClientListener m_clientlistener = null;
	private delegate void ClientLogCallbackDelegate(string msg);
	private ClientLogCallbackDelegate m_clientCallback = null;
	private IntPtr m_clientCallbackPtr;
	private HashSet<BiribitManagerListener> m_listeners = new HashSet<BiribitManagerListener>();

	private Biribit.Native.ServerInfo[] m_serverInfo = new Biribit.Native.ServerInfo[0];
	private Biribit.Native.Connection[] m_connection = new Biribit.Native.Connection[0];
	private List<ConnectionInfo> m_connectionInfo = new List<ConnectionInfo>();

	public Biribit.Native.ServerInfo[] ServerInfo { get { return m_serverInfo; } }
	public Biribit.Native.Connection[] Connections { get { return m_connection; } }

	public BiribitConnectionView GetConnection(uint connectionId) {
		return m_connectionInfo[(int)connectionId];
	}

	public void AddListener(BiribitManagerListener listener) {
		m_listeners.Add(listener);
	}

	public void DelListener(BiribitManagerListener listener) {
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
			DontDestroyOnLoad(transform.gameObject);
			m_instance = this;
			NativeMethods.Clean();

			m_clientCallback = new ClientLogCallbackDelegate(DebugLog);
			m_clientCallbackPtr = Marshal.GetFunctionPointerForDelegate(m_clientCallback);
			NativeMethods.ClientAddLogCallback(m_clientCallbackPtr);

			m_clientlistener = new ClientListener(this);
			m_client.ClearListeners();
			m_client.AddListener(m_clientlistener);
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

		foreach (BiribitManagerListener listener in m_listeners)
			try { listener.OnServerListUpdated(); }
			catch (Exception ex) { PrintException(ex); }
	}

	private void OnGetConnection(Biribit.Native.Connection_array array)
	{
		m_connection = Biribit.Interop.PtrToArray<Biribit.Native.Connection>(array.arr, array.size);
	}

	private void OnGetRemoteClient(uint connectionId, Biribit.Native.RemoteClient_array array)
	{
		while (m_connectionInfo.Count <= connectionId)
			m_connectionInfo.Add(new ConnectionInfo());

		m_connectionInfo[(int)connectionId].RemoteClients = Biribit.Interop.PtrToArray<Biribit.Native.RemoteClient>(array.arr, array.size);
	}

	private void OnGetRoom(uint connectionId, Biribit.Native.Room_array array)
	{
		while (m_connectionInfo.Count <= connectionId)
			m_connectionInfo.Add(new ConnectionInfo());

		m_connectionInfo[(int)connectionId].Rooms = Biribit.Interop.NativeToArray(array);
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
				foreach (BiribitManagerListener listener in m_listeners)
					try { listener.OnConnected(evnt.connection.id); }
					catch (Exception ex) { PrintException(ex); }
				break;
			case Biribit.Native.ConnectionEventType.TYPE_DISCONNECTION:
				m_connectionInfo[(int) evnt.connection.id].Clear();
				Debug.Log("Disconnected!");
				foreach (BiribitManagerListener listener in m_listeners)
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
			Biribit.Room joined = info.GetRoom(info.joined_room);
			for (int i = 0; i < joined.slots.Length; i++)
			{
				if (info.lastSlotsState[i] != joined.slots[i])
				{
					if (info.lastSlotsState[i] == Biribit.Client.UnassignedId)
						foreach (BiribitManagerListener listener in m_listeners)
							try { listener.OnJoinedRoomPlayerJoined(evnt.connection, joined.slots[i], (byte) i); }
							catch (Exception ex) { PrintException(ex); }
					else if (joined.slots[i] == Biribit.Client.UnassignedId)
						foreach (BiribitManagerListener listener in m_listeners)
							try { listener.OnJoinedRoomPlayerLeft(evnt.connection, info.lastSlotsState[i], (byte)i); }
							catch (Exception ex) { PrintException(ex); }

					info.lastSlotsState[i] = joined.slots[i];
				}
			}
		}

		info.Rooms = rooms;
	}

	private void OnJoinedRoomEvent(ref Biribit.Native.JoinedRoomEvent evnt)
	{
		uint connectionId = evnt.connection;

		ConnectionInfo info = m_connectionInfo[(int) connectionId];
		info.joined_room = evnt.room_id;
		info.joined_room_slot = evnt.slot_id;
		info.ClearEntries();

		if (info.joined_room > Biribit.Client.UnassignedId)
		{
			Biribit.Room joined = info.Rooms[info.GetRoomIndex(info.joined_room)];
			if (info.lastSlotsState.Length < joined.slots.Length)
				info.lastSlotsState = new uint[joined.slots.Length];

			for (int i = 0; i < joined.slots.Length; i++)
				info.lastSlotsState[i] = joined.slots[i];

			foreach (BiribitManagerListener listener in m_listeners)
				try { listener.OnJoinedRoom(evnt.connection, evnt.room_id, evnt.slot_id); }
				catch (Exception ex) { PrintException(ex); }
		}
			
		else
		{
			foreach (BiribitManagerListener listener in m_listeners)
				try { listener.OnLeaveRoom(evnt.connection); }
				catch (Exception ex) { PrintException(ex); }
		}	
	}

	private void OnBroadcastEvent(ref Biribit.Native.BroadcastEvent native_evnt)
	{
		Biribit.BroadcastEvent envt = new Biribit.BroadcastEvent(native_evnt);
		foreach (BiribitManagerListener listener in m_listeners)
			try { listener.OnBroadcast(envt); }
			catch (Exception ex) { PrintException(ex); }
	}

	private void OnEntriesEvent(ref Biribit.Native.EntriesEvent evnt)
	{
		uint count = m_client.GetEntriesCount(evnt.connection);
		var info = m_connectionInfo[(int) evnt.connection];
		for (uint i = (uint) (info.Entries.Count + 1); i <= count; i++) {
			Biribit.Native.Entry native_entry = m_client.GetEntry(evnt.connection, i);
			if (native_entry.data == IntPtr.Zero)
				break;

			info.Entries.Add(new Biribit.Entry(native_entry));
		}

		foreach (BiribitManagerListener listener in m_listeners)
			try { listener.OnEntriesChanged(evnt.connection); }
			catch (Exception ex) { PrintException(ex); }
	}

	private Exception m_ex = null;
	private void PrintException(Exception ex)
	{
		if (m_ex == null)
			m_ex = ex;
	}
}

