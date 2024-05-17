#pragma warning disable 649

using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
	using UI.Attributes;

    public class NotepadSaver : MonoBehaviour
	{
		[SerializeField]
		private InputField notepad;

		[SerializeField, Path(isRelative = true)]
		private string savePath;

		[Space]

		[SerializeField]
		private Dropdown taskDropdown;

		[SerializeField]
		private Dropdown participantDropdown;

		private void OnApplicationQuit()
		{
			var tasks = new[] { "Objects", "Avoidance", "Navigation" };

			var filename = $"{tasks[taskDropdown.value]}-P{participantDropdown.value+1:00}.txt";

#if UNITY_EDITOR
			File.WriteAllText(UnityApp.DataPath + savePath + filename, notepad.text);
#else
			File.WriteAllText(UnityApp.ProjectPath + filename, notepad.text);
#endif
		}
	}
}
