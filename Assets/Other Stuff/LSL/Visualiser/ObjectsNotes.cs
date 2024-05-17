#pragma warning disable 649

using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
    using static ArrayExts.ArrayExtensions;

    public class ObjectsNotes : MonoBehaviour
    {
        [SerializeField]
        private Dropdown conditionDropdown;

        [SerializeField]
        private InputField[] actuals;

        [SerializeField]
        private InputField[] guesses;

        [SerializeField]
        private Dropdown cupPlacementDropdown;

        [SerializeField]
        private InputField otherNotes;

        [Space]

        [SerializeField]
        private Dropdown participantDropdown;

        private Notes[] notes;

        public void Awake()
		{
            notes = CreateArray(5, (i) => new Notes(i));
		}

		void OnApplicationQuit()
		{
            var participantId = participantDropdown.value + 1;
            var data = new AllNotes { participantId = participantId, task = "Object Recognition", notes = notes };
            for (int i = 0; i < 5; i++)
			{
                data.notes[i].condition = TagManager2.GetCondition(0, participantId, data.notes[i].trialId);
                data.notes[i].CalculateCorrectness();
			}

            var json = JsonUtility.ToJson(data, true);
            var filename = $"Objects-P{participantId:00}-notes.json";
            File.WriteAllText(UnityApp.ProjectPath + filename, json);
        }

		public void LoadNotes()
		{
            LoadNotes(conditionDropdown.value);
		}

        public void LoadNotes(int condition)
        {
            otherNotes.text = notes[condition].otherNotes;

            for (int i = 0; i < 5; i++)
			{
                actuals[i].text = notes[condition].actuals[i];
                guesses[i].text = notes[condition].guesses[i];
			}

            cupPlacementDropdown.value = notes[condition].cupPlacement;
		}

        public void ApplyNotes()
		{
            ApplyNotes(conditionDropdown.value);
		}

        public void ApplyNotes(int condition)
        {
            notes[condition].otherNotes = otherNotes.text;

            for (int i = 0; i < 5; i++)
            {
                notes[condition].actuals[i] = actuals[i].text;
                notes[condition].guesses[i] = guesses[i].text;
            }

            notes[condition].cupPlacement = cupPlacementDropdown.value;
        }

        [System.Serializable]
        public class Notes
        {
            public Notes(int i)
            {
                trialId = i;
                condition = 0;

                actuals = CreateArray(5, () => "");
                guesses = CreateArray(5, () => "");
                cupPlacement = 0;
                otherNotes = "";
            }

            public int trialId;
            public int condition;

            public string[] actuals;
            public string[] guesses;
            public int cupPlacement;
            public string otherNotes;

            public int numCorrect;

            public void CalculateCorrectness()
			{
                for (int i = 0; i < 5; i++)
				{
                    if (guesses[i] == actuals[i])
					{
                        numCorrect++;
					}
				}
			}
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
