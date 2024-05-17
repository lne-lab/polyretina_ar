using System;
using UnityEngine;
using UnityEngine.UI;

public class LiveTime : MonoBehaviour
{
	public Text text;

	void Update()
	{
		text.text = DateTime.Now.ToString("HH:mm:ss");
	}
}
