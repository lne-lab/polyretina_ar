using UnityEngine;

namespace LNE.Studies.Augmented
{
    using LabStreamingLayer;
	using PostProcessing;
	using ProstheticVision;

	public class DataSaverInlet : Inlet<string>
	{
		[SerializeField]
		private HeadGazeTracker _data = null;

		protected override void ProcessData(string[] data)
		{
			if (data[0].Contains("End"))
			{
				_data.SaveAndFlush();
			}
		}

		public void ProcessData(string data)
		{
			ProcessData(new[] { data });
		}
	}
}
