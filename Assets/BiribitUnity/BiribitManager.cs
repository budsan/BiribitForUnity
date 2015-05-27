using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using Silver;


public class BiribitManager : MonoBehaviour
{
	[SerializeField]
	private float networkFramerate = 25.0f;
	public float NetworkFramerate
	{
		get
		{
			return networkFramerate;
		}
		set
		{
			deltaTime = 1.0f / value;
			networkFramerate = value;
		}
	}

	private float AccumUpdateTime = 0.0f;

	private float tempTime = 0;
	private float tempFrames = 0;

	private float deltaTime = 0.0f;
	public float NetworkDeltaTime
	{
		get
		{
			return deltaTime;
		}
	}

	static private BiribitManager m_managerInstance = null;
	static private BiribitClient m_instance = null;

	static public BiribitManager ManagerInstance
	{
		get
		{
			if (m_managerInstance == null)
			{
				GameObject manager = new GameObject("BiribitManager");
				manager.hideFlags = HideFlags.HideInHierarchy;
				manager.AddComponent<BiribitManager>();
			}

			return m_managerInstance;
		}
	}

	static public BiribitClient ClientInstance
	{
		get
		{
			BiribitManager manager = ManagerInstance;
			if (m_instance == null)
				m_instance = new BiribitClient();
				
			return m_instance;
		}
	}

	protected partial class NativeMethods
	{
		const string DllNameClient = "BiribitClient";

		[DllImport(DllNameClient, EntryPoint = "BiribitClientClean")]
		public static extern bool Clean();

		[DllImport(DllNameClient, EntryPoint = "BiribitClientAddLogCallback")]
		public static extern bool ClientAddLogCallback(IntPtr func);

		[DllImport(DllNameClient, EntryPoint = "BiribitClientDelLogCallback")]
		public static extern bool ClientDelLogCallback(IntPtr func);
	}

	private delegate void ClientLogCallbackDelegate(string msg);
	private ClientLogCallbackDelegate m_clientCallback = null;
	private IntPtr m_clientCallbackPtr;

	private void DebugLog(string msg)
	{
		Debug.Log(msg);
	}

	private void Awake()
	{
		NativeMethods.Clean();
		m_clientCallback = new ClientLogCallbackDelegate(DebugLog);
		m_clientCallbackPtr = Marshal.GetFunctionPointerForDelegate(m_clientCallback);
		NativeMethods.ClientAddLogCallback(m_clientCallbackPtr);

		if (m_managerInstance == null)
			m_managerInstance = this;
		else
			Destroy(this);

		m_joinedRoom.id = BiribitClient.UnassignedId;
		m_joinedRoom.slots = new uint[0];
	}

	private void Start()
	{
		deltaTime = 1.0f / networkFramerate;
		networkFramerate = deltaTime;
	}

	private void OnDestroy()
	{
		if (m_managerInstance == this)
			m_managerInstance = null;

		NativeMethods.ClientDelLogCallback(m_clientCallbackPtr);
	}

	private void OnApplicationQuit()
	{
		if (m_instance != null)
			m_instance.FreeClientPtr();

		NativeMethods.Clean();
	}


	private bool m_connected = false;
	private uint m_connectionId = BiribitClient.UnassignedId;
	public bool IsConnected()
	{
		return m_connected;
	}

	private bool m_joined = false;
	public bool HasJoinedRoom()
	{
		return m_joined;
	}

	private uint m_localPlayer = 0;
	public uint GetLocalPlayer()
	{
		return m_localPlayer;
	}

	private BiribitClient.Room m_joinedRoom;
	public uint GetMaxPlayers()
	{
		if (HasJoinedRoom())
			return (uint)m_joinedRoom.slots.Length;

		return 0;
	}

	public void AddBehaviour(BiribitBehaviour behaviour)
	{
		List<BiribitBehaviour> behaviours;
		if (!m_behaviours.TryGetValue(behaviour.PlayerSlot, out behaviours))
		{
			behaviours = new List<BiribitBehaviour>();
			m_behaviours.Add(behaviour.PlayerSlot, behaviours);
		}

		behaviours.Add(behaviour);
	}

	public void RemoveBehaviour(BiribitBehaviour behaviour)
	{
		List<BiribitBehaviour> behaviours;
		if (m_behaviours.TryGetValue(behaviour.PlayerSlot, out behaviours))
		{
			behaviours.Remove(behaviour);
			if (behaviours.Count == 0)
				m_behaviours.Remove(behaviour.PlayerSlot);
		}
	}

	public void Send(byte[] data, BiribitClient.ReliabilityBitmask mask = BiribitClient.ReliabilityBitmask.Unreliable)
	{
		if (m_connectionId != BiribitClient.UnassignedId)
			ClientInstance.SendToRoom(m_connectionId, data, mask);
	}

	List<BiribitListener> m_listeners = new List<BiribitListener>();
	public void AddListener(BiribitListener listener)
	{
		m_listeners.Add(listener);
	}

	public void RemoveListener(BiribitListener listener)
	{
		m_listeners.Remove(listener);
	}

	private void NotifyPlayerJoined(uint slot)
	{
		foreach (BiribitListener listener in m_listeners)
			listener.PlayerJoined(slot);
	}

	private void NotifyPlayerLeaved(uint slot)
	{
		foreach (BiribitListener listener in m_listeners)
			listener.PlayerLeaved(slot);
	}

	private void NotifyConnected()
	{
		foreach (BiribitListener listener in m_listeners)
			listener.Connected();
	}

	private void NotifyDisconnected()
	{
		foreach (BiribitListener listener in m_listeners)
			listener.Disconnected();
	}

	private Dictionary<uint, List<BiribitBehaviour>> m_behaviours = new Dictionary<uint,List<BiribitBehaviour>>();
	private void FixedUpdate()
	{
		BiribitClient client = ClientInstance;
		BiribitClient.ServerConnection[] connections = client.GetConnections();
		bool connected = connections.Length > 0;

		if (!m_connected && connected)
			NotifyConnected();
		else if (m_connected && !connected)
			NotifyDisconnected();

		m_connected = connected;
		if (m_connected)
		{
			m_connectionId = connections[0].id;
			BiribitClient.Room[] rooms = client.GetRooms(m_connectionId);
			
			uint joinedRoom = client.GetJoinedRoomId(m_connectionId);
			uint roomPos = 0;
			for (; roomPos < rooms.Length && rooms[roomPos].id != joinedRoom; roomPos++);
			m_joined = roomPos < rooms.Length;
			if (m_joined)
			{
				BiribitClient.Room room = rooms[roomPos];
				m_localPlayer = client.GetJoinedRoomSlot(m_connectionId);
				for (uint i = 0; i < room.slots.Length; i++ )
				{
					uint nowslot = room.slots[i];
					uint beforeslot = BiribitClient.UnassignedId;
					if (i < m_joinedRoom.slots.Length)
						beforeslot = m_joinedRoom.slots[i];

					if (beforeslot != nowslot)
					{
						if (beforeslot != BiribitClient.UnassignedId)
							NotifyPlayerLeaved(i);

						if (nowslot != BiribitClient.UnassignedId)
							NotifyPlayerJoined(i);
					}
				}

				m_joinedRoom = room;
				BiribitClient.Received recv = client.PullReceived();
				while ((recv = client.PullReceived()) != null)
				{
					if (recv.slot_id == m_localPlayer)
						continue;

					List<BiribitBehaviour> behaviours;
					if (m_behaviours.TryGetValue(recv.slot_id, out behaviours))
					{
						foreach (BiribitBehaviour behaviour in behaviours)
						{
							behaviour.NewIncomingData(recv.data);
						}
					}
				}

				AccumUpdateTime += Time.fixedDeltaTime;
				while (AccumUpdateTime >= deltaTime)
				{
					List<BiribitBehaviour> behaviours;
					if (m_behaviours.TryGetValue(m_localPlayer, out behaviours))
					{
						foreach (var behaviour in behaviours)
						{
							behaviour.NetworkUpdate();
						}
					}

					tempFrames++;
					AccumUpdateTime -= deltaTime;
				}
			}
			else
			{
				m_joinedRoom.id = BiribitClient.UnassignedId;
				AccumUpdateTime = 0.0f;
			}
		}
		else
		{
			m_connectionId = BiribitClient.UnassignedId;
		}
	}
}

