using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

public class BiribitClient
{
	public const uint UnassignedId = 0; 

	public enum ReliabilityBitmask
	{

		/// Unreliable -> 0
		Unreliable = 0,

		/// Reliable -> 1
		Reliable = 1,

		/// Ordered -> 2
		Ordered = 2,

		/// ReliableOrdered -> 3
		ReliableOrdered = 3,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ServerInfo
	{

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string addr;

		/// unsigned int
		public uint ping;

		/// unsigned short
		public ushort port;

		/// unsigned char
		public byte passwordProtected;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ServerInfoArray
	{

		/// ServerInfo*
		public IntPtr arr;

		/// unsigned int
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ServerConnection
	{

		/// id_t->unsigned int
		public uint id;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;

		/// unsigned int
		public uint ping;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ServerConnectionArray
	{

		/// ServerConnection*
		public IntPtr arr;

		/// unsigned int
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RemoteClient
	{

		/// id_t->unsigned int
		public uint id;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string appid;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RemoteClientArray
	{

		/// RemoteClient*
		public IntPtr arr;

		/// unsigned int
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ClientParameters
	{

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string appid;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NativeRoom
	{

		/// id_t->unsigned int
		public uint id;

		/// unsigned int
		public uint slots_size;

		/// id_t*
		public IntPtr slots;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RoomArray
	{

		/// NativeRoom*
		public IntPtr arr;

		/// unsigned int
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NativeReceived
	{

		/// time_t->unsigned int
		public uint when;

		/// id_t->unsigned int
		public uint connection;

		/// id_t->unsigned int
		public uint room_id;

		/// unsigned char
		public byte slot_id;

		/// unsigned int
		public uint data_size;

		/// void*
		public IntPtr data;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Entry
	{

		/// id_t->unsigned int
		public uint id;

		/// unsigned char
		public byte slot_id;

		/// unsigned int
		public uint data_size;

		/// void*
		public IntPtr data;
	}

	public partial class NativeMethods
	{
		private const string DllName = "BiribitForUnity";

		/// Return Type: Client
		[DllImport(DllName, EntryPoint = "brbt_CreateClient")]
		public static extern IntPtr CreateClient();


		/// Return Type: Client
		[DllImport(DllName, EntryPoint = "brbt_GetClientInstance")]
		public static extern IntPtr GetClientInstance();


		/// Return Type: void
		///client: Client
		[DllImport(DllName, EntryPoint = "brbt_DeleteClient")]
		public static extern void DeleteClient(HandleRef client);


		/// Return Type: void
		///client: Client
		///addr: char*
		///port: unsigned short
		///password: char*
		[DllImport(DllName, EntryPoint = "brbt_Connect")]
		public static extern void Connect(HandleRef client, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] string addr, ushort port, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] string password);


		/// Return Type: void
		///client: Client
		///id_con: id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_Disconnect")]
		public static extern void Disconnect(HandleRef client, uint id_con);


		/// Return Type: void
		///client: Client
		[DllImport(DllName, EntryPoint = "brbt_DisconnectAll")]
		public static extern void DisconnectAll(HandleRef client);


		/// Return Type: void
		///client: Client
		///port: unsigned short
		[DllImport(DllName, EntryPoint = "brbt_DiscoverOnLan")]
		public static extern void DiscoverOnLan(HandleRef client, ushort port);


		/// Return Type: void
		///client: Client
		[DllImport(DllName, EntryPoint = "brbt_ClearDiscoverInfo")]
		public static extern void ClearDiscoverInfo(HandleRef client);


		/// Return Type: void
		///client: Client
		[DllImport(DllName, EntryPoint = "brbt_RefreshDiscoverInfo")]
		public static extern void RefreshDiscoverInfo(HandleRef client);


		/// Return Type: ServerInfoArray
		///client: Client
		///revision: unsigned int*
		[DllImport(DllName, EntryPoint = "brbt_GetDiscoverInfo")]
		public static extern ServerInfoArray GetDiscoverInfo(HandleRef client, ref uint revision);


		/// Return Type: ServerConnectionArray
		///client: Client
		///revision: unsigned int*
		[DllImport(DllName, EntryPoint = "brbt_GetConnections")]
		public static extern ServerConnectionArray GetConnections(HandleRef client, ref uint revision);


		/// Return Type: RemoteClientArray
		///client: Client
		///id_conn: id_t->unsigned int
		///revision: unsigned int*
		[DllImport(DllName, EntryPoint = "brbt_GetRemoteClients")]
		public static extern RemoteClientArray GetRemoteClients(HandleRef client, uint id_conn, ref uint revision);


		/// Return Type: id_t->unsigned int
		///client: Client
		///id_conn: id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_GetLocalClientId")]
		public static extern uint GetLocalClientId(HandleRef client, uint id_conn);


		/// Return Type: void
		///client: Client
		///id_conn: id_t->unsigned int
		///parameters: ClientParameters
		[DllImport(DllName, EntryPoint = "brbt_SetLocalClientParameters")]
		public static extern void SetLocalClientParameters(HandleRef client, uint id_conn, ClientParameters parameters);


		/// Return Type: void
		///client: Client
		///id_conn: id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_RefreshRooms")]
		public static extern void RefreshRooms(HandleRef client, uint id_conn);


		/// Return Type: RoomArray
		///client: Client
		///id_conn: id_t->unsigned int
		///revision: unsigned int*
		[DllImport(DllName, EntryPoint = "brbt_GetRooms")]
		public static extern RoomArray GetRooms(HandleRef client, uint id_conn, ref uint revision);


		/// Return Type: void
		///client: Client
		///id_conn: id_t->unsigned int
		///num_slots: unsigned int
		[DllImport(DllName, EntryPoint = "brbt_CreateRoom")]
		public static extern void CreateRoom(HandleRef client, uint id_conn, uint num_slots);


		/// Return Type: void
		///client: Client
		///id_conn: id_t->unsigned int
		///num_slots: unsigned int
		///slot_to_join_id: unsigned int
		[DllImport(DllName, EntryPoint = "brbt_CreateRoomAndJoinSlot")]
		public static extern void CreateRoomAndJoinSlot(HandleRef client, uint id_conn, uint num_slots, uint slot_to_join_id);


		/// Return Type: void
		///client: Client
		///id_conn: id_t->unsigned int
		///num_slots: unsigned int
		[DllImport(DllName, EntryPoint = "brbt_JoinRandomOrCreateRoom")]
		public static extern void JoinRandomOrCreateRoom(HandleRef client, uint id_conn, uint num_slots);


		/// Return Type: void
		///client: Client
		///id_conn: id_t->unsigned int
		///room_id: id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_JoinRoom")]
		public static extern void JoinRoom(HandleRef client, uint id_conn, uint room_id);


		/// Return Type: void
		///client: Client
		///id_conn: id_t->unsigned int
		///room_id: id_t->unsigned int
		///slot_id: unsigned int
		[DllImport(DllName, EntryPoint = "brbt_JoinRoomAndSlot")]
		public static extern void JoinRoomAndSlot(HandleRef client, uint id_conn, uint room_id, uint slot_id);


		/// Return Type: id_t->unsigned int
		///client: Client
		///id_conn: id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_GetJoinedRoomId")]
		public static extern uint GetJoinedRoomId(HandleRef client, uint id_conn);


		/// Return Type: unsigned int
		///client: Client
		///id_conn: id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_GetJoinedRoomSlot")]
		public static extern uint GetJoinedRoomSlot(HandleRef client, uint id_conn);


		/// Return Type: void
		///client: Client
		///id_con: id_t->unsigned int
		///data: void*
		///size: unsigned int
		///mask: ReliabilityBitmask
		[DllImport(DllName, EntryPoint = "brbt_SendBroadcast")]
		public static extern void SendBroadcast(HandleRef client, uint id_con, IntPtr data, uint size, ReliabilityBitmask mask);


		/// Return Type: void
		///client: Client
		///id_con: id_t->unsigned int
		///data: void*
		///size: unsigned int
		[DllImport(DllName, EntryPoint = "brbt_SendEntry")]
		public static extern void SendEntry(HandleRef client, uint id_con, IntPtr data, uint size);


		/// Return Type: unsigned int
		///client: Client
		[DllImport(DllName, EntryPoint = "brbt_GetDataSizeOfNextReceived")]
		public static extern uint GetDataSizeOfNextReceived(HandleRef client);


		/// Return Type: Received*
		///client: Client
		[DllImport(DllName, EntryPoint = "brbt_PullReceived")]
		public static extern IntPtr PullReceived(HandleRef client);


		/// Return Type: id_t->unsigned int
		///client: Client
		///id_con: id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_GetEntriesCount")]
		public static extern uint GetEntriesCount(HandleRef client, uint id_con);


		/// Return Type: Entry*
		///client: Client
		///id_con: id_t->unsigned int
		///id_entry: id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_GetEntry")]
		public static extern IntPtr GetEntry(HandleRef client, uint id_con, uint id_entry);

	}


	//-----------------------------------------------------------------------//

	static protected T[] GetArray<T>(IntPtr ptrArray, uint size)
	{
		T[] array = new T[size];
		if (ptrArray != IntPtr.Zero)
		{
			for (int i = 0; i < array.Length; i++)
			{
				IntPtr ptrArrayIt = new IntPtr(ptrArray.ToInt64() + i * Marshal.SizeOf(typeof(T)));
				try {
					array[i] = (T) Marshal.PtrToStructure(ptrArrayIt, typeof(T));
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log("EX:" + ex.Message + "\n" + ex.StackTrace);
					return new T[0];
				}
			}
		}
		
		return array;
	}

	//-----------------------------------------------------------------------//

	private class ArrayHolder<T>
	{
		T[] array = new T[0];
		uint revision = 0;

		public T[] GetArray(uint newRevision, IntPtr ptrArray, uint size)
		{
			if (revision != newRevision)
			{
				revision = newRevision;
				array = BiribitClient.GetArray<T>(ptrArray, size);
			}

			return array;
		}

		public T[] GetArray(uint newRevision, IntPtr ptrArray, uint size, out bool changed)
		{
			if (revision != newRevision)
			{
				changed = true;
				revision = newRevision;
				array = BiribitClient.GetArray<T>(ptrArray, size);
			}
			else
				changed = false;

			return array;
		}
	}

	//-----------------------------------------------------------------------//

	private HandleRef m_client;
	private ArrayHolder<ServerInfo> m_serverInfoArray = new ArrayHolder<ServerInfo>();
	private ArrayHolder<ServerConnection> m_serverConnectionArray = new ArrayHolder<ServerConnection>();
	private ArrayHolder<RemoteClient> m_remoteClientArray = new ArrayHolder<RemoteClient>();
	private ArrayHolder<NativeRoom> m_roomArray = new ArrayHolder<NativeRoom>();

	//-----------------------------------------------------------------------//

	private NativeBuffer m_sendBuffer = new NativeBuffer();
	private Dictionary<uint, int> m_remoteClientById = new Dictionary<uint, int>();

	//-----------------------------------------------------------------------//

	public struct Room
	{
		public uint id;
		public uint[] slots;
	}

	public class Received
	{
		public uint when;
		public uint connection;
		public uint room_id;
		public byte slot_id;
		public byte[] data;
	}

	private Room[] m_rooms = new Room[0];

	//-----------------------------------------------------------------------//

	public BiribitClient()
	{
		m_client = new HandleRef(this, IntPtr.Zero);
	}

	protected HandleRef GetClientPtr()
	{
		if (m_client.Handle == IntPtr.Zero)
			m_client = new HandleRef(this, NativeMethods.GetClientInstance());

		return m_client;
	}

	public void FreeClientPtr()
	{
		if (m_client.Handle != IntPtr.Zero) {
			NativeMethods.DeleteClient(m_client);
			m_client = new HandleRef(this, IntPtr.Zero);
		}
	}

	public void Connect(string address, ushort port = 0, string password = "")
	{
		NativeMethods.Connect(GetClientPtr(), address, port, password);
	}

	public void Disconnect(uint connectionId)
	{
		NativeMethods.Disconnect(GetClientPtr(), connectionId);
	}

	public void Disconnect()
	{
		NativeMethods.DisconnectAll(GetClientPtr());
	}

	public void DiscoverOnLan(ushort port = 0)
	{
		NativeMethods.DiscoverOnLan(GetClientPtr(), port);
	}

	public void ClearDiscoverInfo()
	{
		NativeMethods.ClearDiscoverInfo(GetClientPtr());
	}

	public void RefreshDiscoverInfo()
	{
		NativeMethods.RefreshDiscoverInfo(GetClientPtr());
	}

	public ServerInfo[] GetDiscoverInfo()
	{
		uint revision = 0;
		ServerInfoArray array = NativeMethods.GetDiscoverInfo(GetClientPtr(), ref revision);
		return m_serverInfoArray.GetArray(revision, array.arr, array.size) ;
	}

	public ServerConnection[] GetConnections()
	{
		uint revision = 0;
		ServerConnectionArray array = NativeMethods.GetConnections(GetClientPtr(), ref revision);
		return m_serverConnectionArray.GetArray(revision, array.arr, array.size);
	}

	public RemoteClient[] GetRemoteClients(uint connectionId)
	{
		bool changed = false;
		uint revision = 0;
		RemoteClientArray array = NativeMethods.GetRemoteClients(GetClientPtr(), connectionId, ref revision);
		RemoteClient[] remoteClient = m_remoteClientArray.GetArray(revision, array.arr, array.size, out changed);
		if (changed)
		{
			m_remoteClientById.Clear();
			for (int i = 0; i < remoteClient.Length; i++)
				m_remoteClientById.Add(remoteClient[i].id, i);
		}
		return remoteClient;
	}

	public int GetRemoteClientArrayPos(uint clientId)
	{
		int client = -1;
		m_remoteClientById.TryGetValue(clientId, out client);
		return client;
	}

	public uint GetLocalClientId(uint connectionId)
	{
		return NativeMethods.GetLocalClientId(GetClientPtr(), connectionId);
	}

	public void SetLocalClientParameters(uint connectionId, string clientName, string appId = "")
	{
		ClientParameters parameters = new ClientParameters();
		parameters.name = clientName;
		parameters.appid = appId;
		NativeMethods.SetLocalClientParameters(GetClientPtr(), connectionId, parameters);
	}

	public void RefreshRooms(uint connectionId)
	{
		NativeMethods.RefreshRooms(GetClientPtr(), connectionId);
	}

	public Room[] GetRooms(uint connectionId)
	{
		bool changed = false;
		uint revision = 0;
		RoomArray array = NativeMethods.GetRooms(GetClientPtr(), connectionId, ref revision);
		NativeRoom[] rooms = m_roomArray.GetArray(revision, array.arr, array.size, out changed);
		if (changed)
		{
			m_rooms = new Room[rooms.Length];
			for(int i = 0; i < rooms.Length; i++)
			{
				int[] temp = new int[rooms[i].slots_size];
				Marshal.Copy(rooms[i].slots, temp, 0, temp.Length);
				m_rooms[i] = new Room();
				m_rooms[i].id = rooms[i].id;
				m_rooms[i].slots = new uint[temp.Length];
				for (int j = 0; j < temp.Length; j++)
					m_rooms[i].slots[j] = (uint)temp[j];
			}
		}

		return m_rooms;
	}

	public void CreateRoom(uint connectionId, uint slotsCount)
	{
		NativeMethods.CreateRoom(GetClientPtr(), connectionId, slotsCount);
	}

	public void CreateRoom(uint connectionId, uint slotsCount, uint jointSlot)
	{
		NativeMethods.CreateRoomAndJoinSlot(GetClientPtr(), connectionId, slotsCount, jointSlot);
	}

	public void JoinRandomOrCreateRoom(uint connectionId, uint slotsCount)
	{
		NativeMethods.JoinRandomOrCreateRoom(GetClientPtr(), connectionId, slotsCount);
	}

	public void JoinRoom(uint connectionId, uint roomId)
	{
		NativeMethods.JoinRoom(GetClientPtr(), connectionId, roomId);
	}

	public void JoinRoom(uint connectionId, uint roomId, uint jointSlot)
	{
		NativeMethods.JoinRoomAndSlot(GetClientPtr(), connectionId, roomId, jointSlot);
	}

	public uint GetJoinedRoomId(uint connectionId)
	{
		return NativeMethods.GetJoinedRoomId(GetClientPtr(), connectionId);
	}

	public uint GetJoinedRoomSlot(uint connectionId)
	{
		return NativeMethods.GetJoinedRoomSlot(GetClientPtr(), connectionId);
	}

	public void SendBroadcast(uint connectionId, byte[] data, uint numBytes, ReliabilityBitmask mask = ReliabilityBitmask.Unreliable)
	{
		m_sendBuffer.Ensure(numBytes);
		Marshal.Copy(data, 0, m_sendBuffer.ptr, (int) numBytes);
		NativeMethods.SendBroadcast(GetClientPtr(), connectionId, m_sendBuffer.ptr, numBytes, mask);
	}

	public void SendBroadcast(uint connectionId, byte[] data, ReliabilityBitmask mask = ReliabilityBitmask.Unreliable)
	{
		SendBroadcast(connectionId, data, (uint) data.Length, mask);
	}

	public Received PullReceived()
	{
		IntPtr ptr = NativeMethods.PullReceived(GetClientPtr());
		if (ptr == IntPtr.Zero)
			return null;

		NativeReceived recv = (NativeReceived) Marshal.PtrToStructure(ptr, typeof(NativeReceived));
		Received result = new Received();
		result.when = recv.when;
		result.connection = recv.connection;
		result.room_id = recv.room_id;
		result.slot_id = recv.slot_id;
		result.data = new byte[recv.data_size];

		if (recv.data_size > 0)
			Marshal.Copy(recv.data, result.data, 0, (int) recv.data_size);

		return result;
	}
}
