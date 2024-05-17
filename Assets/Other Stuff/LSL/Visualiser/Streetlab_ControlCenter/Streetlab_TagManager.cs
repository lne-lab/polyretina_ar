#pragma warning disable 649

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
	using ArrayExts;
	using UI.Attributes;

	public class Streetlab_TagManager : MonoBehaviour
	{
		[SerializeField, Path(isRelative = true)]
		private string path = "";

		[SerializeField]
		private Dropdown participant = null;

		[SerializeField]
		private Text tagListBox = null;

		[SerializeField]
		private Text pageNumberBox = null;

		[SerializeField]
		private TagOutlet outlet = null;

		[SerializeField]
		private int numLines = 28;

		[SerializeField]
		private Timer timer;

		[Space]

		[SerializeField]
		private TagOutlet visionOutlet;

		[SerializeField]
		private Dropdown notesConditionDropdown;

		private Tag[] streetlabTags = null;

		private int tagIndex = 0;
		private List<int> sentTags = new List<int>();

		private int participantId => participant.value;

		void Start()
		{
#if UNITY_EDITOR
			var path = UnityApp.DataPath + this.path;
#else
			var path = UnityApp.ProjectPath;
#endif

			streetlabTags = LoadTags(path + "streetlab tags.csv");

			ShowTags();
		}

		public void ShowTags()
		{
			tagListBox.text = "";

			var n = tagIndex < numLines ? 0 : numLines;

			for (int i = n; i < streetlabTags.Length; i++)
			{
				var tag = streetlabTags[i];

				if (i == tagIndex)
				{
					tagListBox.text += "<color=#ffff00ff>";
				}
				else if (sentTags.Contains(i))
				{
					tagListBox.text += "<color=#ffffff8f>";
				}

				tagListBox.text += tag.FullLabel;

				if (sentTags.Contains(i) || i == tagIndex)
				{
					tagListBox.text += "</color>";
				}

				tagListBox.text += "\n";
			}

			if (streetlabTags.Length < numLines)
			{
				pageNumberBox.text = "Page 1 of 1";
			}
			else if (n == 0)
			{
				pageNumberBox.text = "Page 1 of 2";
			}
			else
			{
				pageNumberBox.text = "Page 2 of 2";
			}
		}

		public void PreviousTag()
		{
			tagIndex = Mathf.Max(tagIndex - 1, 0);
			ShowTags();
		}

		public void NextTag()
		{
			tagIndex = Mathf.Min(tagIndex + 1, streetlabTags.Length - 1);

			// automatically open appropriate notes
			var label = streetlabTags[tagIndex].label;
			if (label.Contains("Start familiarisation")) notesConditionDropdown.value = 0;
			if (label.Contains("Start black")) notesConditionDropdown.value = 0;
			if (label.Contains("Start 1stExposure")) notesConditionDropdown.value = 0;
			if (label.Contains("Start training1")) notesConditionDropdown.value = 1;
			if (label.Contains("Start training2")) notesConditionDropdown.value = 2;
			if (label.Contains("Start training3")) notesConditionDropdown.value = 3;
			if (label.Contains("Start training4")) notesConditionDropdown.value = 4;
			if (label.Contains("Start training5")) notesConditionDropdown.value = 5;
			if (label.Contains("Start training6")) notesConditionDropdown.value = 6;
			if (label.Contains("Start training7")) notesConditionDropdown.value = 7;
			if (label.Contains("Start training8")) notesConditionDropdown.value = 8;
			if (label.Contains("Start training9")) notesConditionDropdown.value = 9;
			if (label.Contains("Start training10")) notesConditionDropdown.value = 10;
			if (label.Contains("Start test")) notesConditionDropdown.value = 11;

			ShowTags();
		}

		public void SendTag()
		{
			outlet.PushSample(streetlabTags[tagIndex].FullLabel);
			sentTags.Add(tagIndex);

			// send vision instruction if tag had one
			var fov = streetlabTags[tagIndex].fov;
			if (fov != 0)
			{
				if (fov == -10)
				{
					var conds = new [] { 20, 45, 110 };
					visionOutlet.PushSample(conds[notesConditionDropdown.value].ToString());
				}
				else if (fov == -1)
				{
					visionOutlet.PushSample("Edge");
				}
				else
				{
					visionOutlet.PushSample(fov.ToString());
				}
			}

			// start timer if the tag had one
			var timerLength = streetlabTags[tagIndex].timerLength;
			if (timerLength > 0)
			{
				timer.Begin(timerLength * 60);
			}
		}

		private Tag[] LoadTags(string path)
		{
			var lines = File.ReadAllLines(path).Where((line) => 
				line.Length > 0 && line[0] != '#'
			);

			var tags = new List<Tag>();
			foreach (var line in lines)
			{
				tags.Add(LineToTag(line));
			}

			return tags.ToArray();
		}

		private Tag LineToTag(string line)
		{
			return new Tag { 
				label = GetLabel(line), 
				timerLength = GetParameter(line, 't'),
				fov = GetParameter(line, 'v')
			};
		}

		private string GetLabel(string line)
		{
			return line.Split('|')[0].Trim();
		}

		private int GetParameter(string line, char parameter)
		{
			var i = line.IndexOf($"{parameter}:");
			if (i > 0)
			{
				var p = (line + " ").Substring(i + 2, 2).Trim();
				var success = int.TryParse(p, out int value);
				if (success)
				{
					return value;
				}
			}

			return 0;
		}
	}

	public class Streetlab_Tag
	{
		public string label;
		public int timerLength;
		public int fov;

		public string FullLabel
		{
			get
			{
				var label = $" {this.label}";

				if (label.Contains("condition"))
				{
					if (fov == -1)
					{
						label += " (Edge)";
					}
					else if (fov == 0)
					{
						label += $" (Black)";
					}
					else if (fov > 0)
					{
						label += $" ({fov})";
					}
				}

				return label;
			}
		}
	}
}
