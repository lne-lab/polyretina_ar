#pragma warning disable 649

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
	using ArrayExts;
	using UI.Attributes;

	public class TagManager2 : MonoBehaviour
	{
		[SerializeField, Path(isRelative = true)]
		private string path = "";

		[SerializeField]
		private Dropdown participant = null;

		[SerializeField]
		private Dropdown task = null;

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

		private Tag[] objectTags = null;
		private Tag[] avoidanceTags = null;
		private Tag[] navigationTags = null;

		private int tagIndex = 0;
		private List<int> sentTags = new List<int>();

		private static readonly (int, int, int, int, int)[] conditions = new (int, int, int, int, int)[]
		{
			(-1, 15, 25, 35, 45),			(15, 25, 35, 45, -1),           (25, 35, 45, -1, 15),
			(15, 25, 35, 45, -1),			(25, 35, 45, -1, 15),           (35, 45, -1, 15, 25),
			(25, 35, 45, -1, 15),			(35, 45, -1, 15, 25),           (45, -1, 15, 25, 35),
			(35, 45, -1, 15, 25),			(45, -1, 15, 25, 35),           (-1, 15, 25, 35, 45),
			(45, -1, 15, 25, 35),			(-1, 15, 25, 35, 45),           (15, 25, 35, 45, -1),

			(45, 35, 25, 15, -1),			(35, 25, 15, -1, 45),			(25, 15, -1, 45, 35),
			(35, 25, 15, -1, 45),			(25, 15, -1, 45, 35),			(15, -1, 45, 35, 25),
			(25, 15, -1, 45, 35),			(15, -1, 45, 35, 25),			(-1, 45, 35, 25, 15),
			(15, -1, 45, 35, 25),			(-1, 45, 35, 25, 15),			(45, 35, 25, 15, -1),
			(-1, 45, 35, 25, 15),			(45, 35, 25, 15, -1),			(35, 25, 15, -1, 45),

			(-1, 25, 45, 15, 35),			(25, 45, 15, 35, -1),			(45, 15, 35, -1, 25),
			(25, 45, 15, 35, -1),			(45, 15, 35, -1, 25),			(15, 35, -1, 25, 45),
			(45, 15, 35, -1, 25),			(15, 35, -1, 25, 45),			(35, -1, 25, 45, 15),
			(15, 35, -1, 25, 45),			(35, -1, 25, 45, 15),			(-1, 25, 45, 15, 35),
			(35, -1, 25, 45, 15),			(-1, 25, 45, 15, 35),			(25, 45, 15, 35, -1),

			(45, 15, 35, -1, 25),			(15, 35, -1, 25, 45),			(35, -1, 25, 45, 15),
			(15, 35, -1, 25, 45),			(35, -1, 25, 45, 15),			(-1, 25, 45, 15, 35),
			(35, -1, 25, 45, 15),			(-1, 25, 45, 15, 35),			(25, 45, 15, 35, -1),
			(-1, 25, 45, 15, 35),			(25, 45, 15, 35, -1),			(45, 15, 35, -1, 25),
			(25, 45, 15, 35, -1),			(45, 15, 35, -1, 25),			(15, 35, -1, 25, 45),

			(45, 25, -1, 35, 15),			(25, -1, 35, 15, 45),			(-1, 35, 15, 45, 25),
			(25, -1, 35, 15, 45),			(-1, 35, 15, 45, 25),			(35, 15, 45, 25, -1),
			(-1, 35, 15, 45, 25),			(35, 15, 45, 25, -1),			(15, 45, 25, -1, 35),
			(35, 15, 45, 25, -1),			(15, 45, 25, -1, 35),			(45, 25, -1, 35, 15),
			(15, 45, 25, -1, 35),			(45, 25, -1, 35, 15),			(25, -1, 35, 15, 45)
		};

		public static int GetCondition(int taskIndex, int participantId, int trialId)
		{
			var conditionIndex = (participantId - 1) * 3 + taskIndex;

			var trials = conditions[conditionIndex];
			switch (trialId)
			{
				case 0: return trials.Item1;
				case 1: return trials.Item2;
				case 2: return trials.Item3;
				case 3: return trials.Item4;
				case 4: return trials.Item5;
				default:
					throw new System.Exception();
			}
		}

		private int participantId => participant.value;

		private Tag[] selectedTags
		{
			get
			{
				switch (task.value)
				{
					case 0:
						return objectTags;
					case 1:
						return avoidanceTags;
					case 2:
						return navigationTags;
					default:
						return default;
				}
			}
		}

		void Start()
		{
#if UNITY_EDITOR
			var path = UnityApp.DataPath + this.path;
#else
			var path = UnityApp.ProjectPath;
#endif

			objectTags = LoadTags(path + "object tags.csv");
			avoidanceTags = LoadTags(path + "avoidance tags.csv");
			navigationTags = LoadTags(path + "navigation tags.csv");

			RandomiseConditions();
		}

		public void RandomiseConditions()
		{
			objectTags = RandomiseConditions(objectTags);
			avoidanceTags = RandomiseConditions(avoidanceTags);
			navigationTags = RandomiseConditions(navigationTags);

			ShowTags();
		}

		public void ShowTags()
		{
			tagListBox.text = "";

			var n = tagIndex < numLines ? 0 : numLines;

			for (int i = n; i < selectedTags.Length; i++)
			{
				var tag = selectedTags[i];

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

			if (selectedTags.Length < numLines)
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
			tagIndex = Mathf.Min(tagIndex + 1, selectedTags.Length - 1);

			// automatically open appropriate notes
			var label = selectedTags[tagIndex].label;
			if (label.Contains("Start condition1")) notesConditionDropdown.value = 0;
			if (label.Contains("Start condition2")) notesConditionDropdown.value = 1;
			if (label.Contains("Start condition3")) notesConditionDropdown.value = 2;
			if (label.Contains("Start condition4")) notesConditionDropdown.value = 3;
			if (label.Contains("Start condition5")) notesConditionDropdown.value = 4;

			ShowTags();
		}

		public void SendTag()
		{
			outlet.PushSample(selectedTags[tagIndex].FullLabel);
			sentTags.Add(tagIndex);

			// send vision instruction if tag had one
			var fov = selectedTags[tagIndex].fov;
			if (fov != 0)
			{
				if (fov == -1)
				{
					visionOutlet.PushSample("Edge");
				}
				else
				{
					visionOutlet.PushSample(fov.ToString());
				}
			}

			// start timer if the tag had one
			var timerLength = selectedTags[tagIndex].timerLength;
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

		private Tag[] RandomiseConditions(Tag[] tags)
		{
			var conditionOrder = conditions[participantId * 3 + task.value];
			var replacements = new (string, int)[]
			{
				("condition1", conditionOrder.Item1),
				("condition2", conditionOrder.Item2),
				("condition3", conditionOrder.Item3),
				("condition4", conditionOrder.Item4),
				("condition5", conditionOrder.Item5)
			};

			for (int i = 0; i < tags.Length; i++)
			{
				foreach (var replacement in replacements)
				{
					if (tags[i].label == $"Start {replacement.Item1}")
					{
						tags[i].fov = replacement.Item2;
					}
				}
			}

			return tags;
		}
	}

	public class Tag
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
