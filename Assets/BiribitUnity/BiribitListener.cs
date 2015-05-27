using UnityEngine;
using System.Collections;

public interface BiribitListener
{
	void PlayerJoined(uint id);
	void PlayerLeaved(uint id);

	void Connected();
	void Disconnected();
}
