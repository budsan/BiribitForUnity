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

		/// brbt_Unreliable -> 0
		Unreliable = 0,

		/// brbt_Reliable -> 1
		Reliable = 1,

		/// brbt_Ordered -> 2
		Ordered = 2,

		/// brbt_ReliableOrdered -> 3
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
	protected struct brbt_ServerInfo_array
	{

		/// brbt_ServerInfo*
		public IntPtr arr;

		/// unsigned int
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ServerConnection
	{

		/// brbt_id_t->unsigned int
		public uint id;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;

		/// unsigned int
		public uint ping;
	}

	[StructLayout(LayoutKind.Sequential)]
	protected struct brbt_ServerConnection_array
	{

		/// brbt_ServerConnection*
		public IntPtr arr;

		/// unsigned int
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RemoteClient
	{

		/// brbt_id_t->unsigned int
		public uint id;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string appid;
	}

	[StructLayout(LayoutKind.Sequential)]
	protected struct brbt_RemoteClient_array
	{

		/// brbt_RemoteClient*
		public IntPtr arr;

		/// unsigned int
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	protected struct brbt_ClientParameters
	{

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string name;

		/// char*
		[MarshalAs(UnmanagedType.LPStr)]
		public string appid;
	}

	[StructLayout(LayoutKind.Sequential)]
	protected struct brbt_Room
	{

		/// brbt_id_t->unsigned int
		public uint id;

		/// unsigned int
		public uint slots_size;

		/// brbt_id_t*
		public IntPtr slots;
	}

	[StructLayout(LayoutKind.Sequential)]
	protected struct brbt_Room_array
	{

		/// brbt_Room*
		public IntPtr arr;

		/// unsigned int
		public uint size;
	}

	[StructLayout(LayoutKind.Sequential)]
	protected struct brbt_Received
	{

		/// brbt_time_t->unsigned int
		public uint when;

		/// brbt_id_t->unsigned int
		public uint connection;

		/// brbt_id_t->unsigned int
		public uint room_id;

		/// unsigned char
		public byte slot_id;

		/// unsigned int
		public uint data_size;

		/// void*
		public IntPtr data;
	}

	protected partial class NativeMethods
	{
		const string DllName = "BiribitForUnity";

		/// Return Type: brbt_Client
		[DllImport(DllName, EntryPoint = "brbt_CreateClient")]
		public static extern uint brbt_CreateClient();


		/// Return Type: void
		///param0: brbt_Client
		[DllImport(DllName, EntryPoint = "brbt_DeleteClient")]
		public static extern void brbt_DeleteClient(uint client);


		/// Return Type: void
		///client: brbt_Client
		///addr: char*
		///port: unsigned short
		///password: char*
		[DllImport(DllName, EntryPoint = "brbt_Connect")]
		public static extern void brbt_Connect(uint client, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] string addr, ushort port, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] string password);


		/// Return Type: void
		///client: brbt_Client
		///id_con: brbt_id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_Disconnect")]
		public static extern void brbt_Disconnect(uint client, uint id_con);


		/// Return Type: void
		///client: brbt_Client
		[DllImport(DllName, EntryPoint = "brbt_DisconnectAll")]
		public static extern void brbt_DisconnectAll(uint client);


		/// Return Type: void
		///client: brbt_Client
		///port: unsigned short
		[DllImport(DllName, EntryPoint = "brbt_DiscoverOnLan")]
		public static extern void brbt_DiscoverOnLan(uint client, ushort port);


		/// Return Type: void
		///client: brbt_Client
		[DllImport(DllName, EntryPoint = "brbt_ClearDiscoverInfo")]
		public static extern void brbt_ClearDiscoverInfo(uint client);


		/// Return Type: void
		///client: brbt_Client
		[DllImport(DllName, EntryPoint = "brbt_RefreshDiscoverInfo")]
		public static extern void brbt_RefreshDiscoverInfo(uint client);


		/// Return Type: brbt_ServerInfo_array
		///client: brbt_Client
		///revision: unsigned int*
		[DllImport(DllName, EntryPoint = "brbt_GetDiscoverInfo")]
		public static extern brbt_ServerInfo_array brbt_GetDiscoverInfo(uint client, ref uint revision);


		/// Return Type: brbt_ServerConnection_array
		///client: brbt_Client
		///revision: unsigned int*
		[DllImport(DllName, EntryPoint = "brbt_GetConnections")]
		public static extern brbt_ServerConnection_array brbt_GetConnections(uint client, ref uint revision);


		/// Return Type: brbt_RemoteClient_array
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		///revision: unsigned int*
		[DllImport(DllName, EntryPoint = "brbt_GetRemoteClients")]
		public static extern brbt_RemoteClient_array brbt_GetRemoteClients(uint client, uint id_conn, ref uint revision);


		/// Return Type: brbt_id_t->unsigned int
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_GetLocalClientId")]
		public static extern uint brbt_GetLocalClientId(uint client, uint id_conn);


		/// Return Type: void
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		///parameters: brbt_ClientParameters
		[DllImport(DllName, EntryPoint = "brbt_SetLocalClientParameters")]
		public static extern void brbt_SetLocalClientParameters(uint client, uint id_conn, brbt_ClientParameters parameters);


		/// Return Type: void
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_RefreshRooms")]
		public static extern void brbt_RefreshRooms(uint client, uint id_conn);


		/// Return Type: brbt_Room_array
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		///revision: unsigned int*
		[DllImport(DllName, EntryPoint = "brbt_GetRooms")]
		public static extern brbt_Room_array brbt_GetRooms(uint client, uint id_conn, ref uint revision);


		/// Return Type: void
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		///num_slots: unsigned int
		[DllImport(DllName, EntryPoint = "brbt_CreateRoom")]
		public static extern void brbt_CreateRoom(uint client, uint id_conn, uint num_slots);


		/// Return Type: void
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		///num_slots: unsigned int
		///slot_to_join_id: unsigned int
		[DllImport(DllName, EntryPoint = "brbt_CreateRoomAndJoinSlot")]
		public static extern void brbt_CreateRoomAndJoinSlot(uint client, uint id_conn, uint num_slots, uint slot_to_join_id);


		/// Return Type: void
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		///room_id: brbt_id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_JoinRoom")]
		public static extern void brbt_JoinRoom(uint client, uint id_conn, uint room_id);


		/// Return Type: void
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		///room_id: brbt_id_t->unsigned int
		///slot_id: unsigned int
		[DllImport(DllName, EntryPoint = "brbt_JoinRoomAndSlot")]
		public static extern void brbt_JoinRoomAndSlot(uint client, uint id_conn, uint room_id, uint slot_id);


		/// Return Type: brbt_id_t->unsigned int
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_GetJoinedRoomId")]
		public static extern uint brbt_GetJoinedRoomId(uint client, uint id_conn);


		/// Return Type: unsigned int
		///client: brbt_Client
		///id_conn: brbt_id_t->unsigned int
		[DllImport(DllName, EntryPoint = "brbt_GetJoinedRoomSlot")]
		public static extern uint brbt_GetJoinedRoomSlot(uint client, uint id_conn);


		/// Return Type: void
		///client: brbt_Client
		///id_con: brbt_id_t->unsigned int
		///data: void*
		///size: unsigned int
		///mask: brbt_ReliabilityBitmask
		[DllImport(DllName, EntryPoint = "brbt_SendToRoom")]
		public static extern void brbt_SendToRoom(uint client, uint id_con, IntPtr data, uint size, ReliabilityBitmask mask);


		/// Return Type: unsigned int
		///client: brbt_Client
		[DllImport(DllName, EntryPoint = "brbt_GetDataSizeOfNextReceived")]
		public static extern uint brbt_GetDataSizeOfNextReceived(uint client);


		/// Return Type: brbt_Received*
		///client: brbt_Client
		[DllImport(DllName, EntryPoint = "brbt_PullReceived")]
		public static extern IntPtr brbt_PullReceived(uint client);
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
				catch (Exception) {
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

	private uint m_client;
	private ArrayHolder<ServerInfo> m_serverInfoArray = new ArrayHolder<ServerInfo>();
	private ArrayHolder<ServerConnection> m_serverConnectionArray = new ArrayHolder<ServerConnection>();
	private ArrayHolder<RemoteClient> m_remoteClientArray = new ArrayHolder<RemoteClient>();
	private ArrayHolder<brbt_Room> m_roomArray = new ArrayHolder<brbt_Room>();
	private Room[] m_rooms = new Room[0];
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

	public BiribitClient()
	{

	}

	~BiribitClient()
	{
		if (m_client != 0)
			NativeMethods.brbt_DeleteClient(m_client);
	}

	public uint GetClientPtr()
	{
		if (m_client == 0)
			m_client = NativeMethods.brbt_CreateClient();

		return m_client;
	}

	public void Connect(string address, ushort port = 0, string password = "")
	{
		NativeMethods.brbt_Connect(GetClientPtr(), address, port, password);
	}

	public void Disconnect(uint connectionId)
	{
		NativeMethods.brbt_Disconnect(GetClientPtr(), connectionId);
	}

	public void Disconnect()
	{
		NativeMethods.brbt_DisconnectAll(GetClientPtr());
	}

	public void DiscoverOnLan(ushort port = 0)
	{
		NativeMethods.brbt_DiscoverOnLan(GetClientPtr(), port);
	}

	public void ClearDiscoverInfo()
	{
		NativeMethods.brbt_ClearDiscoverInfo(GetClientPtr());
	}

	public void RefreshDiscoverInfo()
	{
		NativeMethods.brbt_RefreshDiscoverInfo(GetClientPtr());
	}

	public ServerInfo[] GetDiscoverInfo()
	{
		uint revision = 0;
		brbt_ServerInfo_array array = NativeMethods.brbt_GetDiscoverInfo(GetClientPtr(), ref revision);
		return m_serverInfoArray.GetArray(revision, array.arr, array.size) ;
	}

	public ServerConnection[] GetConnections()
	{
		uint revision = 0;
		brbt_ServerConnection_array array = NativeMethods.brbt_GetConnections(GetClientPtr(), ref revision);
		return m_serverConnectionArray.GetArray(revision, array.arr, array.size);
	}

	public RemoteClient[] GetRemoteClients(uint connectionId)
	{
		bool changed = false;
		uint revision = 0;
		brbt_RemoteClient_array array = NativeMethods.brbt_GetRemoteClients(GetClientPtr(), connectionId, ref revision);
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
		return NativeMethods.brbt_GetLocalClientId(GetClientPtr(), connectionId);
	}

	public void SetLocalClientParameters(uint connectionId, string clientName, string appId = "")
	{
		brbt_ClientParameters parameters = new brbt_ClientParameters();
		parameters.name = clientName;
		parameters.appid = appId;
		NativeMethods.brbt_SetLocalClientParameters(GetClientPtr(), connectionId, parameters);
	}

	public void RefreshRooms(uint connectionId)
	{
		NativeMethods.brbt_RefreshRooms(GetClientPtr(), connectionId);
	}

	public Room[] GetRooms(uint connectionId)
	{
		bool changed = false;
		uint revision = 0;
		brbt_Room_array array = NativeMethods.brbt_GetRooms(GetClientPtr(), connectionId, ref revision);
		brbt_Room[] rooms = m_roomArray.GetArray(revision, array.arr, array.size, out changed);
		if (changed)
		{
			m_rooms = new Room[rooms.Length];
			for(int i = 0; i < rooms.Length; i++)
			{
				int[] temp = new int[rooms[i].slots_size];
				Marshal.Copy(rooms[i].slots, temp, 0, temp.Length);

				m_rooms[i].id = rooms[i].id;
				m_rooms[i].slots = new uint[temp.Length];
				for (int j = 0; j < temp.Length; j++)
					m_rooms[i].slots[j] = (uint) temp[j];
				
			}
		}

		return m_rooms;
	}

	public void CreateRoom(uint connectionId, uint slotsCount)
	{
		NativeMethods.brbt_CreateRoom(GetClientPtr(), connectionId, slotsCount);
	}

	public void CreateRoom(uint connectionId, uint slotsCount, uint jointSlot)
	{
		NativeMethods.brbt_CreateRoomAndJoinSlot(GetClientPtr(), connectionId, slotsCount, jointSlot);
	}

	public void JoinRoom(uint connectionId, uint roomId)
	{
		NativeMethods.brbt_JoinRoom(GetClientPtr(), connectionId, roomId);
	}

	public void JoinRoom(uint connectionId, uint roomId, uint jointSlot)
	{
		NativeMethods.brbt_JoinRoomAndSlot(GetClientPtr(), connectionId, roomId, jointSlot);
	}

	public uint GetJoinedRoomId(uint connectionId)
	{
		return NativeMethods.brbt_GetJoinedRoomId(GetClientPtr(), connectionId);
	}

	public uint GetJoinedRoomSlot(uint connectionId)
	{
		return NativeMethods.brbt_GetJoinedRoomSlot(GetClientPtr(), connectionId);
	}

	public void SendToRoom(uint connectionId, byte[] data, uint numBytes, ReliabilityBitmask mask = ReliabilityBitmask.Unreliable)
	{
		m_sendBuffer.Ensure(numBytes);
		Marshal.Copy(data, 0, m_sendBuffer.ptr, (int) numBytes);
		NativeMethods.brbt_SendToRoom(GetClientPtr(), connectionId, m_sendBuffer.ptr, numBytes, mask);
	}

	public void SendToRoom(uint connectionId, byte[] data, ReliabilityBitmask mask = ReliabilityBitmask.Unreliable)
	{
		SendToRoom(connectionId, data, (uint) data.Length, mask);
	}

	public Received PullReceived()
	{
		IntPtr ptr = NativeMethods.brbt_PullReceived(GetClientPtr());
		if (ptr == IntPtr.Zero)
			return null;

		brbt_Received recv = (brbt_Received) Marshal.PtrToStructure(ptr, typeof(brbt_Received));
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
