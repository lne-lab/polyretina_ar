using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LNE.EyeTracking
{
	using ProstheticVision;
	using VectorExts;

    public class EyeGazeEffect : MonoBehaviour
	{
		[SerializeField]
		private Material _material = null;

		void Update()
		{
			_material.SetVector("_position", EyeGaze.Screen.AddXY(.5f));
		}
	}
}
