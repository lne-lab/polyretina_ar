using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer
{
	public class EyeGazeInlet : Inlet<float>
	{
		[SerializeField]
		private Material _material = null;

		[SerializeField]
		private Text _positionText = null;

		[SerializeField]
		private Text _dilationText = null;

		protected override void ProcessData(float[] data)
		{
			var position = new Vector2(data[0] + 0.5f, data[1] + 0.5f);
			var dilation = data[2];

			_material.SetVector("_position", position);
			_material.SetFloat("_dilation", dilation);

			_positionText.text = $" X: {position.x:0.00}, Y: {position.y:0.00}";
			_dilationText.text = $" Dilation: {dilation:0.00}mm";
		}
	}
}
