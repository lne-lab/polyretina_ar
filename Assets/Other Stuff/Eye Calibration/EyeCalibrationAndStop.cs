#if VIVE_PRO_EYE
using System.Collections;
using UnityEngine;

using ViveSR.anipal.Eye;

namespace LNE.EyeTracking
{
	public class EyeCalibrationAndStop : MonoBehaviour
	{
		void Start()
		{
			UnityApp.StartCoroutine(WaitForEyeTracking());
		}

		IEnumerator WaitForEyeTracking()
		{
			while (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING)
			{
				yield return null;
			}

			LaunchEyeCalibration();
			UnityApp.Quit();
		}

		public void LaunchEyeCalibration()
		{
			SRanipal_Eye_v2.LaunchEyeCalibration();
		}
	}
}
#endif