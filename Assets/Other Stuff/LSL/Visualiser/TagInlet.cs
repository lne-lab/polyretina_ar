using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer
{
	public class TagInlet : Inlet<string>
	{
		[SerializeField]
		private Text _display = null;

		[SerializeField]
		private int _maxLines = 25;

		protected override void ProcessData(string[] data)
		{
			var time = DateTime.Now.ToString("HH:mm:ss");

			_display.text += $"\n [{time}] {data[0]}";

			if (_display.text.Count((c) => c == '\n') > _maxLines)
			{
				var firstNewline = _display.text.IndexOf('\n') + 1;
				_display.text = _display.text.Remove(0, firstNewline);
			}
		}
	}
}
