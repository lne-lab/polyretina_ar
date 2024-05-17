using UnityEngine;
using UnityEngine.XR;

public class ViveSystemInformation : MonoBehaviour
{
	void Start()
	{
		InputDevices.deviceConnected	+= (device) => Debug.Log($"+ {device.name}: {device.role}");
		InputDevices.deviceDisconnected	+= (device) => Debug.Log($"- {device.name}: {device.role}");
	}
}
