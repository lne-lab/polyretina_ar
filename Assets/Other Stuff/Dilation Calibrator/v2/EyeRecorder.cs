using System.Collections.Generic;
using UnityEngine;

namespace LNE.DilationCalibration
{
	using IO;
	using ProstheticVision;
	using UI.Attributes;

	public class EyeRecorder : Singleton<EyeRecorder>
	{
		[SerializeField, Path]
		private string _path;

		private List<Data> data = new List<Data>();

		public List<Data> Data => data;

		private RandomActivator Processor => Prosthesis.Instance.ExternalProcessor as RandomActivator;
		private EpiretinalImplant Implant => Prosthesis.Instance.Implant as EpiretinalImplant;

		void FixedUpdate()
		{
			var pos_right = EyeGaze.GetVivePro(StereoTargetEyeMask.Right);
			var dil_right = EyeGaze.GetPupilDilation(StereoTargetEyeMask.Right);
			var pos_left = EyeGaze.GetVivePro(StereoTargetEyeMask.Left);
			var dil_left = EyeGaze.GetPupilDilation(StereoTargetEyeMask.Left);

			var fov = 15;
			if (Processor.on == false && Implant.on == false)
			{
				fov = 0;
			}
			else if (Processor.on == false && Implant.on == true)
			{
				fov = -1;
			}
			else if (Processor.on == true && Implant.on == false)
			{
				fov = 110;
			}
			else if (Processor.on == true && Implant.on == true)
			{
				fov = (int)Implant.fieldOfView;
			}

			data.Add(new Data
			{
				phosphenePercentage = CalibrationManager.Instance.PhosphenePercentage,
				time = Time.time,
				position_right = pos_right,
				dilation_right = dil_right,
				position_left = pos_left,
				dilation_left = dil_left,
				brightness = Implant.fieldOfView < 100 ? Implant.brightness : Processor.brightness,
				fieldOfView = fov
			});
		}

		void OnApplicationQuit()
		{
			SaveData();
		}

		private void SaveData()
		{
			var csv = new CSV();
			csv.AppendRow("participant", "percentage", "time", "pos_x_right", "pos_y_right", "dil_right", "pos_x_left", "pos_y_left", "dil_left", "brightness", "fov", "trial");
			foreach (var datum in data)
			{
				csv.AppendRow(
					CalibrationManager.Instance.Participant,
					datum.phosphenePercentage,
					datum.time,
					datum.position_right.x,
					datum.position_right.y,
					datum.dilation_right,
					datum.position_left.x,
					datum.position_left.y,
					datum.dilation_left,
					datum.brightness,
					datum.fieldOfView,
					0
				);
			}

			csv = AddTrialNumbers(csv);

			csv.SaveWStream(_path + $"Dilation-{System.DateTime.Now:HH_mm_ss}.csv");
		}

		private CSV AddTrialNumbers(CSV csv)
		{
			var idx = new Dictionary<int, int> { { -1, 0 }, { 0, 0 }, { 15, 0 }, { 25, 0 }, { 35, 0 }, { 45, 0 }, { 110, 0 } };

			csv.SetCell("trial", 1, 1);

			for (int i = 2; i < csv.Height; i++)
			{
				var fov0 = csv.GetCell<int>("fov", i - 1);
				var fov1 = csv.GetCell<int>("fov", i);

				if (i == 2 || fov0 != fov1)
				{
					idx[fov1]++;
				}

				csv.SetCell("trial", i, idx[fov1]);
			}

			return csv;
		}
	}

	public class Data
	{
		public int phosphenePercentage;

		public float time;

		public Vector2 position_right;
		public float dilation_right;
		public Vector2 position_left;
		public float dilation_left;

		public float brightness;

		public int fieldOfView;

		public float GetDilation(StereoTargetEyeMask targetEye)
		{
			return targetEye == StereoTargetEyeMask.Left ? dilation_left : dilation_right;
		}
	}
}
