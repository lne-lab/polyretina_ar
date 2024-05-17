using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LNE.IO;
using LNE.ProstheticVision;

using static LNE.Threading.CallbackManager;

public class DilationCalibrator : MonoBehaviour
{
	// view 15 for 20 seconds
	// 10 second pause
	// calculate average dilation

	private CSV csv;
	private int run;

	private EpiretinalImplant implant => Prosthesis.Instance.Implant as EpiretinalImplant;

	void Awake()
	{
		csv = new CSV();
		csv.AppendRow("run", "fov", "dilation", "brightness");

		run = 0;
	}

	void Start()
	{
		run++;

		InvokeOnce(10,	() => { implant.fieldOfView = 15; });
		InvokeOnce(30,	() => { implant.fieldOfView = 0; });
		InvokeOnce(40,	() => { implant.fieldOfView = 25; });
		InvokeOnce(60,	() => { implant.fieldOfView = 0; });
		InvokeOnce(70,	() => { implant.fieldOfView = 35; });
		InvokeOnce(90,	() => { implant.fieldOfView = 0; });
		InvokeOnce(100, () => { implant.fieldOfView = 45; });
		InvokeOnce(120, () => { implant.fieldOfView = 0; });
		InvokeOnce(130, () => { implant.on = false; implant.fieldOfView = 100; });
		InvokeOnce(150, () => { implant.on = true; implant.fieldOfView = 0; });
		InvokeOnce(150, Start);
	}

	void FixedUpdate()
	{
		csv.AppendRow(run, implant.fieldOfView, EyeGaze.GetPupilDilation(), implant.brightness);
	}

	void View15()
	{
		implant.fieldOfView = 15;
	}

	private void CalculateBrightness(int fov)
	{
		var data = csv.Where<int>("run", (i) => i == run).And<int>("fov", (j) => j == fov);

	}
}
