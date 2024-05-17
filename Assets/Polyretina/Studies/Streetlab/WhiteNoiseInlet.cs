using UnityEngine;

namespace LNE.Studies.Augmented
{
	using LabStreamingLayer;
	using PostProcessing;
	using ProstheticVision;

	public class WhiteNoiseInlet : Inlet<string>
	{

		protected override void ProcessData(string[] data)
		{
			if (data[0].Contains("End"))
			{
				FindObjectOfType<AudioSource>().Play();
			}

			if (data[0].Contains("Start"))
			{
				FindObjectOfType<AudioSource>().Stop();
			}
		}

		public void ProcessData(string data)
		{
			ProcessData(new[] { data });
		}
	}
}
