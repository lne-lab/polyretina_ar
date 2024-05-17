using UnityEngine;

namespace LNE.Studies.Augmented
{
	using LabStreamingLayer;
	using PostProcessing;
	using ProstheticVision;

	public class AugmentTrialNumInlet : Inlet<string>
	{

		protected override void ProcessData(string[] data)
		{
			if (data[0].Contains("Start"))
			{
				FindObjectOfType<HeadGazeTracker>()._trialId++;
			}
		}

		public void ProcessData(string data)
		{
			ProcessData(new[] { data });
		}
	}
}
