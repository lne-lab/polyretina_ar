#if VIVE_PRO_EYE
using System.Collections;
using UnityEngine;

using ViveSR.anipal.Eye;

namespace LNE.EyeTracking
{
	using Threading;

	public class EyeCalibration : MonoBehaviour
	{
		[SerializeField]
		private KeyCode confirm = KeyCode.LeftControl;

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

			CallbackManager.InvokeOnce(.1f, () =>
			{
				if (Input.GetKey(confirm))
				{
					LaunchEyeCalibration();
				}
			});
		}

		public void LaunchEyeCalibration()
		{
			SRanipal_Eye_v2.LaunchEyeCalibration();
		}
	}
}
#endif
