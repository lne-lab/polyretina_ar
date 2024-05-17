#pragma warning disable 649

using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
	using static ArrayExts.ArrayExtensions;

	public class NavigationNotes : MonoBehaviour
	{
		[SerializeField]
		private Dropdown conditionDropdown;

		[SerializeField]
		private Button successButton;

		[SerializeField]
		private InputField otherNotes;

		[Space]

		[SerializeField]
		private Dropdown participantDropdown;

		private Notes[] notes;

		public void Awake()
		{
			notes = CreateArray(5, (i) => new Notes { trialId = i });
		}

		public void SwapSuccess()
		{
			SwapSuccess(conditionDropdown.value);
		}

		public void SwapSuccess(int condition)
		{
			notes[condition].success = !notes[condition].success;

			if (notes[condition].success)
			{
				successButton.GetComponent<Image>().color = new Color(0.5f, 1, 0.5f);
			}
			else
			{
				successButton.GetComponent<Image>().color = new Color(1, 1, 1);
			}
		}

		public void LoadNotes()
		{
			LoadNotes(conditionDropdown.value);
		}

		public void LoadNotes(int condition)
		{
			otherNotes.text = notes[condition].otherNotes;

			if (notes[condition].success)
			{
				successButton.GetComponent<Image>().color = new Color(0.5f, 1, 0.5f);
			}
			else
			{
				successButton.GetComponent<Image>().color = new Color(1, 1, 1);
			}
		}

		public void ApplyNotes()
		{
			ApplyNotes(conditionDropdown.value);
		}

		public void ApplyNotes(int condition)
		{
			notes[condition].otherNotes = otherNotes.text;
		}

		void OnApplicationQuit()
		{
			var participantId = participantDropdown.value + 1;
			var data = new AllNotes { participantId = participantId, task = "Navigation", notes = notes };
			for (int i = 0; i < 5; i++)
			{
				data.notes[i].condition = TagManager2.GetCondition(2, participantId, data.notes[i].trialId);
			}

			var json = JsonUtility.ToJson(data, true);
			var filename = $"Navigation-P{participantId:00}-notes.json";
			File.WriteAllText(UnityApp.ProjectPath + filename, json);
		}

		[System.Serializable]
		public class Notes
		{
			public int trialId;
			public int condition;

			public bool success;
			public string otherNotes;
		}

		[System.Serializable]
		public class AllNotes
		{
			public int participantId;
			public string task;

			public Notes[] notes;
		}
	}
}
