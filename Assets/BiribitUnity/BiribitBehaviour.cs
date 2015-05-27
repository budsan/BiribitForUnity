using UnityEngine;
using System.Collections;

public class BiribitBehaviour : MonoBehaviour
{
	protected BiribitManager manager;

	public uint PlayerSlot;
	protected bool LocalPlayer ()
	{
		return PlayerSlot == manager.GetLocalPlayer();
	}

	public virtual void Start()
	{
		manager = BiribitManager.ManagerInstance;
		manager.AddBehaviour(this);
	}

	void OnDestroy()
	{
		manager.RemoveBehaviour(this);
	}

	protected void Send(byte[] data, BiribitClient.ReliabilityBitmask mask = BiribitClient.ReliabilityBitmask.Unreliable)
	{
		manager.Send(data, mask);
	}

	public virtual void NewIncomingData(byte[] data)
	{

	}

	public virtual void NetworkUpdate()
	{

	}
}
