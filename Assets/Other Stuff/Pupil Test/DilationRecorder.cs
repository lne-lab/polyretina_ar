using System;
using UnityEngine;

namespace LNE.PupilTest
{
	using UI.Attributes;
	using IO;
	using ProstheticVision;

	public class DilationRecorder : MonoBehaviour
	{
		[Path]
		public string path;

		public OscillatingColour oscillator;

		private CSV csv;

		void Start()
		{
			csv = new CSV();
			csv.AppendRow("date", "time", "colour", "size");
		}

		void FixedUpdate()
		{
			csv.AppendRow(
				DateTime.UtcNow.ToString("dd.MM.yyyy"),
				DateTime.UtcNow.ToString("HH:mm:ss.ff"),
				oscillator.GetColour().r,
				EyeGaze.GetPupilDilation()
			);
		}

		void OnApplicationQuit()
		{
			csv.SaveWStream(path + DateTime.UtcNow.ToString("dd-MM-yyyy HH-mm-ss") + " pupil_dilation.csv");
		}
	}
}
