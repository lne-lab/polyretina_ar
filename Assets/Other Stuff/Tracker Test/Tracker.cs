using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public enum TrackerID { None, _D3C53228, _C08A0477, _8D9CD6CB, _0106A410, _B8AE5A31, _EF2246B5, _C0640482, _D987B353, _5C85C788, _60968DC9, _36370D71, _A3F10B46, _75F1F203, _43C820D0, _656CC634, _2E8C9CAA, _2D3BE031, Headset };

public class Tracker : MonoBehaviour
{
	[SerializeField]
	private string _trackerRole = string.Empty;

	[SerializeField]
	private TrackerID _trackerId = TrackerID.None;

	private InputDevice? device = null;

	void Start()
	{
		if (_trackerId == TrackerID.Headset)
		{
			var devices = new List<InputDevice>();
			InputDevices.GetDevices(devices);
			var headset = devices.First((device) => device.name == "VIVE_Pro MV");
			OnDeviceConnected(headset);
		}
		else
		{
			InputDevices.deviceConnected += OnDeviceConnected;
			InputDevices.deviceDisconnected += OnDeviceDisconnected;
		}
	}

	void Update()
    {
		if (device.HasValue)
		{
			device.Value.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
			device.Value.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

			transform.position = position;
			transform.rotation = rotation;
		}
	}

	private void OnDeviceConnected(InputDevice device)
	{
		if (IsCorrect(device))
		{
			this.device = device;
			Debug.Log($"{_trackerId} connected.");
		}
	}

	private void OnDeviceDisconnected(InputDevice device)
	{
		if (IsCorrect(device))
		{
			this.device = null;
			Debug.Log($"{_trackerId} disconnected.");
		}
	}

	private bool IsCorrect(InputDevice device)
	{
		if (_trackerId == TrackerID.Headset)
		{
			return device.name == "VIVE_Pro MV";
		}
		else
		{
			return device.name.Contains($"{_trackerId}".Replace('_', '-'));
		}
	}
}
