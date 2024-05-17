#if VIVE_PRO_EYE
using UnityEngine;
using UnityEditor;

using ViveSR.anipal.Eye;

namespace LNE.UI
{
	public class EyeCalibrationStarter : MonoBehaviour
	{
		[MenuItem("Polyretina/Calibrate Eyes &c", priority = WindowPriority.eyeCalibration)]
		public static void StartEyeCalibration()
		{
			SRanipal_Eye_v2.LaunchEyeCalibration();
		}
	}
}
#endif
