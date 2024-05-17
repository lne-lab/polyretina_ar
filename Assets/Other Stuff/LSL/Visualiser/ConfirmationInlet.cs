#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
	public class ConfirmationInlet : Inlet<string>
	{
		[SerializeField]
		private Image	buttonNat,
						button15,
						button25,
						button35,
						button45,
						buttonEdge,
						buttonQuestionnaire;

		protected override void ProcessData(string[] data)
		{
			var buttons = new Dictionary<string, Image>
			{
				{ "Nat", buttonNat },
				{ "15", button15 },
				{ "25", button25 },
				{ "35", button35 },
				{ "45", button45 },
				{ "Edge", buttonEdge },
				{ "Questionnaire", buttonQuestionnaire }
			};

			foreach (var button in buttons.Values)
			{
				button.color = Color.white;
			}

			if (buttons.ContainsKey(data[0]))
			{
				buttons[data[0]].color = Color.green;
			}
			else if (data[0] == "30")
			{
				// special case for 30 degrees
				button25.color = Color.green;
				button35.color = Color.green;
			}
		}
	}
}
