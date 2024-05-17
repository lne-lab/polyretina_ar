using UnityEngine;

namespace LNE.DilationCalibration
{
	using ProstheticVision;

    public class EyeGUI : MonoBehaviour
	{
		[SerializeField]
		private StereoTargetEyeMask _targetEye = StereoTargetEyeMask.Left;

		[SerializeField]
		private Material _material = null;

		void FixedUpdate()
		{
			var position = EyeGaze.GetVivePro(_targetEye);
			var dilation = EyeGaze.GetPupilDilation(_targetEye);

			_material.SetVector("_position", position);
			_material.SetFloat("_dilation", dilation);
		}

		public void ChangeEye(int eye)
		{
			switch (eye)
			{
				case 0:
					_targetEye = StereoTargetEyeMask.Left;
					break;
				case 1:
					_targetEye = StereoTargetEyeMask.Right;
					break;
			}
		}
	}
}
