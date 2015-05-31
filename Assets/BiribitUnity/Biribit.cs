using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System;

public class Biribit : MonoBehaviour {

	static private Biribit m_instance;
	static private BiribitClient m_client = null;
	static public BiribitClient Instance
	{
		get
		{
			if (m_instance == null)
			{
				GameObject manager = new GameObject("Biribit");
				manager.hideFlags = HideFlags.HideInHierarchy;
				manager.AddComponent<Biribit>();
			}

			if (m_client == null)
				m_client = new BiribitClient();
				
			return m_client;
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
		if (m_instance == null)
		{
			m_instance = this;
			NativeMethods.Clean();
			m_clientCallback = new ClientLogCallbackDelegate(DebugLog);
			m_clientCallbackPtr = Marshal.GetFunctionPointerForDelegate(m_clientCallback);
			NativeMethods.ClientAddLogCallback(m_clientCallbackPtr);
		}
		else
		{
			Destroy(this);
		}	
	}

	private void OnDestroy()
	{
		if (m_instance == this)
			m_instance = null;

		NativeMethods.ClientDelLogCallback(m_clientCallbackPtr);
	}

	private void OnApplicationQuit()
	{
		m_client.FreeClientPtr();
		NativeMethods.Clean();
	}
}
