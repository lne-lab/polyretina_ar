#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
	public class Streetlab_ConfirmationInlet : Inlet<string>
	{
		[SerializeField]
		private Image button20,
						button45,
						button110,
						buttonBlack,
						buttonEdge,
						buttonNat,
						buttonEyeCalibration;

		protected override void ProcessData(string[] data)
		{
			var buttons = new Dictionary<string, Image>
			{

				{ "20", button20 },
				{ "45", button45 },
				{ "110", button110 },
				{ "Black", buttonBlack },
				{ "Edge", buttonEdge },
				{ "Nat", buttonNat },
				{ "StartEyeCalibration", buttonEyeCalibration }
			};

			foreach (var button in buttons.Values)
			{
				button.color = Color.white;
			}

			if (buttons.ContainsKey(data[0]))
			{
				buttons[data[0]].color = Color.green;
			}
		}
	}
}
