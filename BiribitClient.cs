using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Biribit.Native;

namespace Biribit
{
	namespace Native
	{

		public enum UNASSIGNED
		{

			/// UNASSIGNED_ID -> 0
			UNASSIGNED_ID = 0,
		}

		public enum BoolValues
		{

			/// FALSE -> 0
			FALSE = 0,

			/// TRUE -> 1
			TRUE = 1,
		}

		public enum ReliabilityBitmask
		{

			/// UNRELIABLE -> 0
			UNRELIABLE = 0,

			/// RELIABLE -> 1
			RELIABLE = 1,

			/// ORDERED -> 2
			ORDERED = 2,

			/// RELIABLEORDERED -> 3
			RELIABLEORDERED = 3,
		}

		public enum ErrorId
		{

			/// ERROR_CODE -> 0
			ERROR_CODE = 0,

			WARN_CLIENT_NAME_IN_USE,

			WARN_CANNOT_LIST_ROOMS_WITHOUT_APPID,

			WARN_CANNOT_CREATE_ROOM_WITHOUT_APPID,

			WARN_CANNOT_CREATE_ROOM_WITH_WRONG_SLOT_NUMBER,

			WARN_CANNOT_CREATE_ROOM_WITH_TOO_MANY_SLOTS,

			WARN_CANNOT_JOIN_WITHOUT_ROOM_ID,

			WARN_CANNOT_JOIN_TO_UNEXISTING_ROOM,

			WARN_CANNOT_JOIN_TO_OTHER_APP_ROOM,

			WARN_CANNOT_JOIN_TO_OCCUPIED_SLOT,

			WARN_CANNOT_JOIN_TO_INVALID_SLOT,

			WARN_CANNOT_JOIN_TO_FULL_ROOM,
		}

		public enum ConnectionEventType
		{

			TYPE_NEW_CONNECTION,

			TYPE_DISCONNECTION,

			TYPE_NAME_UPDATED,
		}

		public enum RemoteClientEventType
		{

			TYPE_CLIENT_UPDATED,

			TYPE_CLIENT_DISCONNECTED,
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
		public struct ServerInfo_array
		{

			/// ServerInfo*
			public IntPtr arr;

			/// unsigned int
			public uint size;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Connection
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
		public struct Connection_array
		{

			/// Connection*
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
		public struct RemoteClient_array
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
		public struct Room
		{

			/// id_t->unsigned int
			public uint id;

			/// unsigned int
			public uint slots_size;

			/// id_t*
			public IntPtr slots;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Room_array
		{

			/// Room*
			public IntPtr arr;

			/// unsigned int
			public uint size;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Entry
		{

			/// id_t->unsigned int
			public uint id;

			/// slot_id_t->unsigned char
			public byte slot_id;

			/// unsigned int
			public uint data_size;

			/// void*
			public IntPtr data;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ErrorEvent
		{

			/// ErrorId
			public ErrorId which;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ServerListEvent
		{

			/// id_t->unsigned int
			public uint dummy;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ConnectionEvent
		{

			/// ConnectionEventType
			public ConnectionEventType type;

			/// Connection
			public Connection connection;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ServerStatusEvent
		{

			/// id_t->unsigned int
			public uint connection;

			/// RemoteClient_array
			public RemoteClient_array clients;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RemoteClientEvent
		{

			/// RemoteClientEventType
			public RemoteClientEventType type;

			/// id_t->unsigned int
			public uint connection;

			/// RemoteClient
			public RemoteClient client;

			/// bool->unsigned char
			public byte self;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RoomListEvent
		{

			/// id_t->unsigned int
			public uint connection;

			/// Room_array
			public Room_array rooms;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct JoinedRoomEvent
		{

			/// id_t->unsigned int
			public uint connection;

			/// id_t->unsigned int
			public uint room_id;

			/// slot_id_t->unsigned char
			public byte slot_id;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct BroadcastEvent
		{

			/// id_t->unsigned int
			public uint connection;

			/// time_t->unsigned int
			public uint when;

			/// id_t->unsigned int
			public uint room_id;

			/// slot_id_t->unsigned char
			public byte slot_id;

			/// unsigned int
			public uint data_size;

			/// void*
			public IntPtr data;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct EntriesEvent
		{

			/// id_t->unsigned int
			public uint connection;

			/// id_t->unsigned int
			public uint room_id;
		}

		/// Return Type: void
		///param0: ErrorEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ErrorEvent_callback(ref ErrorEvent param0);

		/// Return Type: void
		///param0: ServerListEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ServerListEvent_callback(ref ServerListEvent param0);

		/// Return Type: void
		///param0: ConnectionEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ConnectionEvent_callback(ref ConnectionEvent param0);

		/// Return Type: void
		///param0: ServerStatusEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ServerStatusEvent_callback(ref ServerStatusEvent param0);

		/// Return Type: void
		///param0: RemoteClientEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void RemoteClientEvent_callback(ref RemoteClientEvent param0);

		/// Return Type: void
		///param0: RoomListEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void RoomListEvent_callback(ref RoomListEvent param0);

		/// Return Type: void
		///param0: JoinedRoomEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void JoinedRoomEvent_callback(ref JoinedRoomEvent param0);

		/// Return Type: void
		///param0: BroadcastEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void BroadcastEvent_callback(ref BroadcastEvent param0);

		/// Return Type: void
		///param0: EntriesEvent*
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void EntriesEvent_callback(ref EntriesEvent param0);

		[StructLayout(LayoutKind.Sequential)]
		public struct EventCallbackTable
		{

			/// ErrorEvent_callback
			public ErrorEvent_callback error;

			/// ServerListEvent_callback
			public ServerListEvent_callback server_list;

			/// ConnectionEvent_callback
			public ConnectionEvent_callback connection;

			/// ServerStatusEvent_callback
			public ServerStatusEvent_callback server_status;

			/// RemoteClientEvent_callback
			public RemoteClientEvent_callback remote_client;

			/// RoomListEvent_callback
			public RoomListEvent_callback room_list;

			/// JoinedRoomEvent_callback
			public JoinedRoomEvent_callback joined_room;

			/// BroadcastEvent_callback
			public BroadcastEvent_callback broadcast;

			/// EntriesEvent_callback
			public EntriesEvent_callback entries;
		}

		/// Return Type: void
		///param0: ServerInfo_array
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ServerInfo_callback(ServerInfo_array param0);

		/// Return Type: void
		///param0: Connection_array
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Connection_callback(Connection_array param0);

		/// Return Type: void
		///param0: RemoteClient_array
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void RemoteClient_callback(RemoteClient_array param0);

		/// Return Type: void
		///param0: Room_array
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Room_callback(Room_array param0);

		public partial class NativeMethods
		{
			private const string DllName = "BiribitForUnity";

			/// Return Type: Client->void*
			[DllImport(DllName, EntryPoint = "brbt_CreateClient")]
			public static extern IntPtr CreateClient();


			/// Return Type: Client->void*
			[DllImport(DllName, EntryPoint = "brbt_GetClientInstance")]
			public static extern IntPtr GetClientInstance();


			/// Return Type: void
			///client: Client->void*
			[DllImport(DllName, EntryPoint = "brbt_DeleteClient")]
			public static extern void DeleteClient(HandleRef client);


			/// Return Type: void
			///client: Client->void*
			///addr: char*
			///port: unsigned short
			///password: char*
			[DllImport(DllName, EntryPoint = "brbt_Connect")]
			public static extern void Connect(HandleRef client, [In()] [MarshalAs(UnmanagedType.LPStr)] string addr, ushort port, [In()] [MarshalAs(UnmanagedType.LPStr)] string password);


			/// Return Type: void
			///client: Client->void*
			///id_con: id_t->unsigned int
			[DllImport(DllName, EntryPoint = "brbt_Disconnect")]
			public static extern void Disconnect(HandleRef client, uint id_con);


			/// Return Type: void
			///client: Client->void*
			[DllImport(DllName, EntryPoint = "brbt_DisconnectAll")]
			public static extern void DisconnectAll(HandleRef client);


			/// Return Type: void
			///client: Client->void*
			///port: unsigned short
			[DllImport(DllName, EntryPoint = "brbt_DiscoverServersOnLAN")]
			public static extern void DiscoverServersOnLAN(HandleRef client, ushort port);


			/// Return Type: void
			///client: Client->void*
			[DllImport(DllName, EntryPoint = "brbt_ClearServerList")]
			public static extern void ClearServerList(HandleRef client);


			/// Return Type: void
			///client: Client->void*
			[DllImport(DllName, EntryPoint = "brbt_RefreshServerList")]
			public static extern void RefreshServerList(HandleRef client);


			/// Return Type: void
			///client: Client->void*
			///callback: ServerInfo_callback
			[DllImport(DllName, EntryPoint = "brbt_GetServerList")]
			public static extern void GetServerList(HandleRef client, ServerInfo_callback callback);


			/// Return Type: void
			///client: Client->void*
			///callback: Connection_callback
			[DllImport(DllName, EntryPoint = "brbt_GetConnections")]
			public static extern void GetConnections(HandleRef client, Connection_callback callback);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			///callback: RemoteClient_callback
			[DllImport(DllName, EntryPoint = "brbt_GetRemoteClients")]
			public static extern void GetRemoteClients(HandleRef client, uint id_conn, RemoteClient_callback callback);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			///callback: Room_callback
			[DllImport(DllName, EntryPoint = "brbt_GetRooms")]
			public static extern void GetRooms(HandleRef client, uint id_conn, Room_callback callback);


			/// Return Type: void
			///client: Client->void*
			[DllImport(DllName, EntryPoint = "brbt_UpdateCallbacks")]
			public static extern void UpdateCallbacks(HandleRef client);


			/// Return Type: id_t->unsigned int
			///client: Client->void*
			///id_conn: id_t->unsigned int
			[DllImport(DllName, EntryPoint = "brbt_GetLocalClientId")]
			public static extern uint GetLocalClientId(HandleRef client, uint id_conn);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			///parameters: ClientParameters
			[DllImport(DllName, EntryPoint = "brbt_SetLocalClientParameters")]
			public static extern void SetLocalClientParameters(HandleRef client, uint id_conn, ClientParameters parameters);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			[DllImport(DllName, EntryPoint = "brbt_RefreshRooms")]
			public static extern void RefreshRooms(HandleRef client, uint id_conn);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			///num_slots: slot_id_t->unsigned char
			[DllImport(DllName, EntryPoint = "brbt_CreateRoom")]
			public static extern void CreateRoom(HandleRef client, uint id_conn, byte num_slots);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			///num_slots: slot_id_t->unsigned char
			///slot_to_join_id: slot_id_t->unsigned char
			[DllImport(DllName, EntryPoint = "brbt_CreateRoomAndJoinSlot")]
			public static extern void CreateRoomAndJoinSlot(HandleRef client, uint id_conn, byte num_slots, byte slot_to_join_id);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			///num_slots: slot_id_t->unsigned char
			[DllImport(DllName, EntryPoint = "brbt_JoinRandomOrCreateRoom")]
			public static extern void JoinRandomOrCreateRoom(HandleRef client, uint id_conn, byte num_slots);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			///room_id: id_t->unsigned int
			[DllImport(DllName, EntryPoint = "brbt_JoinRoom")]
			public static extern void JoinRoom(HandleRef client, uint id_conn, uint room_id);


			/// Return Type: void
			///client: Client->void*
			///id_conn: id_t->unsigned int
			///room_id: id_t->unsigned int
			///slot_id: slot_id_t->unsigned char
			[DllImport(DllName, EntryPoint = "brbt_JoinRoomAndSlot")]
			public static extern void JoinRoomAndSlot(HandleRef client, uint id_conn, uint room_id, byte slot_id);


			/// Return Type: id_t->unsigned int
			///client: Client->void*
			///id_conn: id_t->unsigned int
			[DllImport(DllName, EntryPoint = "brbt_GetJoinedRoomId")]
			public static extern uint GetJoinedRoomId(HandleRef client, uint id_conn);


			/// Return Type: unsigned int
			///client: Client->void*
			///id_conn: id_t->unsigned int
			[DllImport(DllName, EntryPoint = "brbt_GetJoinedRoomSlot")]
			public static extern uint GetJoinedRoomSlot(HandleRef client, uint id_conn);


			/// Return Type: void
			///client: Client->void*
			///id_con: id_t->unsigned int
			///data: void*
			///size: unsigned int
			///mask: ReliabilityBitmask
			[DllImport(DllName, EntryPoint = "brbt_SendBroadcast")]
			public static extern void SendBroadcast(HandleRef client, uint id_con, IntPtr data, uint size, ReliabilityBitmask mask);


			/// Return Type: void
			///client: Client->void*
			///id_con: id_t->unsigned int
			///data: void*
			///size: unsigned int
			[DllImport(DllName, EntryPoint = "brbt_SendEntry")]
			public static extern void SendEntry(HandleRef client, uint id_con, IntPtr data, uint size);


			/// Return Type: void
			///client: Client->void*
			///table: EventCallbackTable*
			[DllImport(DllName, EntryPoint = "brbt_PullEvents")]
			public static extern void PullEvents(HandleRef client, ref EventCallbackTable table);


			/// Return Type: id_t->unsigned int
			///client: Client->void*
			///id_con: id_t->unsigned int
			[DllImport(DllName, EntryPoint = "brbt_GetEntriesCount")]
			public static extern uint GetEntriesCount(HandleRef client, uint id_con);


			/// Return Type: Entry*
			///client: Client->void*
			///id_con: id_t->unsigned int
			///id_entry: id_t->unsigned int
			[DllImport(DllName, EntryPoint = "brbt_GetEntry")]
			public static extern IntPtr GetEntry(HandleRef client, uint id_con, uint id_entry);

		}
	}

	public class Interop
	{
		static public Room[] NativeToArray(Native.Room_array array)
		{
			Native.Room[] native_rooms = PtrToArray<Native.Room>(array.arr, array.size);
			Room[] rooms = new Room[native_rooms.Length];
			for (int i = 0; i < rooms.Length; i++)
				rooms[i] = new Room(native_rooms[i]);

			return rooms;
		}

		static public T[] PtrToArray<T>(IntPtr ptrArray, uint size)
		{
			T[] array = new T[size];
			if (ptrArray != IntPtr.Zero)
			{
				for (int i = 0; i < array.Length; i++)
				{
					IntPtr ptrArrayIt = new IntPtr(ptrArray.ToInt64() + i * Marshal.SizeOf(typeof(T)));
					array[i] = (T)Marshal.PtrToStructure(ptrArrayIt, typeof(T));
				}
			}

			return array;
		}
	}

	public struct Room
	{
		public uint id;
		public uint[] slots;

		public Room(Native.Room native)
		{
			int[] temp = new int[native.slots_size];
			Marshal.Copy(native.slots, temp, 0, temp.Length);
					
			id = native.id;
			slots = new uint[temp.Length];
			for (int j = 0; j < temp.Length; j++) {
				// Save the existing bit pattern, but interpret it as an unsigned integer.
				slots[j] = BitConverter.ToUInt32(BitConverter.GetBytes(temp[j]), 0);
			}
		}
	}

	public class BroadcastEvent
	{
		public uint connection;
		public uint when;
		public uint room_id;
		public byte slot_id;
		public byte[] data;

		public BroadcastEvent(Native.BroadcastEvent native)
		{
			connection = native.connection;
			when = native.when;
			room_id = native.room_id;
			slot_id = native.slot_id;
			data = new byte[native.data_size];

			if (native.data_size > 0)
				Marshal.Copy(native.data, data, 0, (int)native.data_size);
		}
	}

	public struct Entry
	{
		public uint id;
		public byte slot_id;
		public byte[] data;

		public Entry(Native.Entry native)
		{
			id = native.id;
			slot_id = native.slot_id;
			data = new byte[native.data_size];

			if (native.data_size > 0)
				Marshal.Copy(native.data, data, 0, (int)native.data_size);
		}
	}

	public interface ClientListener
	{
		void OnGetServerInfo(Native.ServerInfo_array array);
		void OnGetConnection(Native.Connection_array array);
		void OnGetRemoteClient(uint connectionId, Native.RemoteClient_array array);
		void OnGetRoom(uint connectionId, Native.Room_array array);

		void OnErrorEvent(ref Native.ErrorEvent evnt);
		void OnServerListEvent(ref Native.ServerListEvent evnt);
		void OnConnectionEvent(ref Native.ConnectionEvent evnt);
		void OnServerStatusEvent(ref Native.ServerStatusEvent evnt);
		void OnRemoteClientEvent(ref Native.RemoteClientEvent evnt);
		void OnRoomListEvent(ref Native.RoomListEvent evnt);
		void OnJoinedRoomEvent(ref Native.JoinedRoomEvent evnt);
		void OnBroadcastEvent(ref Native.BroadcastEvent evnt);
		void OnEntriesEvent(ref Native.EntriesEvent evnt);
	}

	public class Client
	{
		public const uint UnassignedId = (uint) Native.UNASSIGNED.UNASSIGNED_ID;

		//-----------------------------------------------------------------------//

		private HandleRef m_client;
		private NativeBuffer m_sendBuffer = new NativeBuffer();
		private Native.EventCallbackTable m_table;
		private HashSet<ClientListener> m_listeners = new HashSet<ClientListener>();

		//-----------------------------------------------------------------------//

		public Client()
		{
			m_client = new HandleRef(this, IntPtr.Zero);

			m_table.error = OnErrorEvent;
			m_table.server_list = OnServerListEvent;
			m_table.connection = OnConnectionEvent;
			m_table.server_status = OnServerStatusEvent;
			m_table.remote_client = OnRemoteClientEvent;
			m_table.room_list = OnRoomListEvent;
			m_table.joined_room = OnJoinedRoomEvent;
			m_table.broadcast = OnBroadcastEvent;
			m_table.entries = OnEntriesEvent;
		}

		public void AddListener(ClientListener listener)
		{
			m_listeners.Add(listener);
		}

		public void DelListener(ClientListener listener)
		{
			m_listeners.Remove(listener);
		}

		protected HandleRef GetClientPtr()
		{
			if (m_client.Handle == IntPtr.Zero)
				m_client = new HandleRef(this, Native.NativeMethods.GetClientInstance());

			return m_client;
		}

		public void FreeClientPtr()
		{
			if (m_client.Handle != IntPtr.Zero)
			{
				Native.NativeMethods.DeleteClient(m_client);
				m_client = new HandleRef(this, IntPtr.Zero);
			}
		}

		public void Connect(string address, ushort port = 0, string password = "")
		{
			Native.NativeMethods.Connect(GetClientPtr(), address, port, password);
		}

		public void Disconnect(uint connectionId)
		{
			Native.NativeMethods.Disconnect(GetClientPtr(), connectionId);
		}

		public void Disconnect()
		{
			Native.NativeMethods.DisconnectAll(GetClientPtr());
		}

		public void DiscoverServersOnLAN(ushort port = 0)
		{
			Native.NativeMethods.DiscoverServersOnLAN(GetClientPtr(), port);
		}

		public void ClearServerList()
		{
			Native.NativeMethods.ClearServerList(GetClientPtr());
		}

		public void RefreshServerList()
		{
			Native.NativeMethods.RefreshServerList(GetClientPtr());
		}

		public void GetServerList()
		{
			Native.NativeMethods.GetServerList(GetClientPtr(), OnGetServerInfo);
		}

		public void GetConnections()
		{
			Native.NativeMethods.GetConnections(GetClientPtr(), OnGetConnection);
		}

		public void GetRemoteClients(uint connectionId)
		{
			Native.NativeMethods.GetRemoteClients(GetClientPtr(), connectionId, (Native.RemoteClient_array array) => {
				OnGetRemoteClient(connectionId, array);
			});
		}

		public void GetRooms(uint connectionId)
		{
			Native.NativeMethods.GetRooms(GetClientPtr(), connectionId, (Native.Room_array array) => {
				OnGetRoom(connectionId, array);
			});
		}

		public void Update()
		{
			Native.NativeMethods.PullEvents(GetClientPtr(), ref m_table);
			Native.NativeMethods.UpdateCallbacks(GetClientPtr());

			if (m_ex != null) {
				Exception ex = m_ex;
				m_ex = null;
				throw ex;
			}
		}

		public uint GetLocalClientId(uint connectionId)
		{
			return Native.NativeMethods.GetLocalClientId(GetClientPtr(), connectionId);
		}

		public void SetLocalClientParameters(uint connectionId, string clientName, string appId = "")
		{
			Native.ClientParameters parameters = new Native.ClientParameters();
			parameters.name = clientName;
			parameters.appid = appId;
			Native.NativeMethods.SetLocalClientParameters(GetClientPtr(), connectionId, parameters);
		}

		public void RefreshRooms(uint connectionId)
		{
			Native.NativeMethods.RefreshRooms(GetClientPtr(), connectionId);
		}

		public void CreateRoom(uint connectionId, byte slotsCount)
		{
			Native.NativeMethods.CreateRoom(GetClientPtr(), connectionId, slotsCount);
		}

		public void CreateRoom(uint connectionId, byte slotsCount, byte jointSlot)
		{
			Native.NativeMethods.CreateRoomAndJoinSlot(GetClientPtr(), connectionId, slotsCount, jointSlot);
		}

		public void JoinRandomOrCreateRoom(uint connectionId, byte slotsCount)
		{
			Native.NativeMethods.JoinRandomOrCreateRoom(GetClientPtr(), connectionId, slotsCount);
		}

		public void JoinRoom(uint connectionId, uint roomId)
		{
			Native.NativeMethods.JoinRoom(GetClientPtr(), connectionId, roomId);
		}

		public void JoinRoom(uint connectionId, uint roomId, byte jointSlot)
		{
			Native.NativeMethods.JoinRoomAndSlot(GetClientPtr(), connectionId, roomId, jointSlot);
		}

		public uint GetJoinedRoomId(uint connectionId)
		{
			return Native.NativeMethods.GetJoinedRoomId(GetClientPtr(), connectionId);
		}

		public uint GetJoinedRoomSlot(uint connectionId)
		{
			return Native.NativeMethods.GetJoinedRoomSlot(GetClientPtr(), connectionId);
		}

		public void SendBroadcast(uint connectionId, byte[] data, uint numBytes, Native.ReliabilityBitmask mask = Native.ReliabilityBitmask.UNRELIABLE)
		{
			m_sendBuffer.Ensure(numBytes);
			Marshal.Copy(data, 0, m_sendBuffer.ptr, (int)numBytes);
			Native.NativeMethods.SendBroadcast(GetClientPtr(), connectionId, m_sendBuffer.ptr, numBytes, mask);
		}

		public void SendBroadcast(uint connectionId, byte[] data, Native.ReliabilityBitmask mask = Native.ReliabilityBitmask.UNRELIABLE)
		{
			SendBroadcast(connectionId, data, (uint)data.Length, mask);
		}

		public void SendEntry(uint connectionId, byte[] data, uint numBytes)
		{
			m_sendBuffer.Ensure(numBytes);
			Marshal.Copy(data, 0, m_sendBuffer.ptr, (int)numBytes);
			Native.NativeMethods.SendEntry(GetClientPtr(), connectionId, m_sendBuffer.ptr, numBytes);
		}

		public void SendEntry(uint connectionId, byte[] data)
		{
			SendEntry(connectionId, data, (uint)data.Length);
		}

		public uint GetEntriesCount(uint connectionId)
		{
			return Native.NativeMethods.GetEntriesCount(GetClientPtr(), connectionId);
		}

		public Native.Entry GetEntry(uint connectionId, uint entryId)
		{
			IntPtr ptr = Native.NativeMethods.GetEntry(GetClientPtr(), connectionId, entryId);
			return (Native.Entry)Marshal.PtrToStructure(ptr, typeof(Native.Entry));
		}

		//-------------------------------------------------------------------//

		private Exception m_ex = null;
		private void PrintException(Exception ex) {
			if (m_ex == null)
				m_ex = ex;
		}

		private void OnGetServerInfo(Native.ServerInfo_array array)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnGetServerInfo(array); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnGetConnection(Native.Connection_array array)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnGetConnection(array); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnGetRemoteClient(uint connectionId, Native.RemoteClient_array array)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnGetRemoteClient(connectionId, array); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnGetRoom(uint connectionId, Native.Room_array array)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnGetRoom(connectionId, array); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnErrorEvent(ref Native.ErrorEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnErrorEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnServerListEvent(ref Native.ServerListEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnServerListEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnConnectionEvent(ref Native.ConnectionEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnConnectionEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnServerStatusEvent(ref Native.ServerStatusEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnServerStatusEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnRemoteClientEvent(ref Native.RemoteClientEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnRemoteClientEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnRoomListEvent(ref Native.RoomListEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnRoomListEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnJoinedRoomEvent(ref Native.JoinedRoomEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnJoinedRoomEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnBroadcastEvent(ref Native.BroadcastEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnBroadcastEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}

		private void OnEntriesEvent(ref Native.EntriesEvent evnt)
		{
			foreach (ClientListener listener in m_listeners)
			{
				try { listener.OnEntriesEvent(ref evnt); }
				catch (Exception ex) { PrintException(ex); }
			}
		}
	}
}
