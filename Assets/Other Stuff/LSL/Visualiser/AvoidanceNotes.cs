#pragma warning disable 649

using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
    using static ArrayExts.ArrayExtensions;

    public class AvoidanceNotes : MonoBehaviour
    {
        [SerializeField]
        private Dropdown conditionDropdown;

        [SerializeField]
        private InputField hitCount;

        [SerializeField]
        private InputField otherNotes;

        [Space]

        [SerializeField]
        private Dropdown participantDropdown;

        private Notes[] notes;

        public void Awake()
		{
            hitCount.text = "0";

            notes = CreateArray(5, (i) => new Notes(i));
		}

        void OnApplicationQuit()
        {
            var participantId = participantDropdown.value + 1;
            var data = new AllNotes { participantId = participantId, task = "Obstacle Avoidance", notes = notes };
            for (int i = 0; i < 5; i++)
            {
                data.notes[i].condition = TagManager2.GetCondition(1, participantId, data.notes[i].trialId);
            }

            var json = JsonUtility.ToJson(data, true);
            var filename = $"Avoidance-P{participantId:00}-notes.json";
            File.WriteAllText(UnityApp.ProjectPath + filename, json);
        }

        public void IncreaseHitCount()
		{
            hitCount.text = (int.Parse(hitCount.text) + 1).ToString();
		}

        public void LoadNotes()
        {
            LoadNotes(conditionDropdown.value);
        }

        public void LoadNotes(int condition)
        {
            otherNotes.text = notes[condition].otherNotes;
            hitCount.text = notes[condition].hitCount.ToString();
        }

        public void ApplyNotes()
        {
            ApplyNotes(conditionDropdown.value);
        }

        public void ApplyNotes(int condition)
        {
            notes[condition].otherNotes = otherNotes.text;
            notes[condition].hitCount = int.Parse(hitCount.text);
        }

        [System.Serializable]
        public class Notes
		{
            public Notes(int i)
            {
                trialId = i;
                condition = 0;

                hitCount = 0;
                otherNotes = "";
			}

            public int trialId;
            public int condition;

            public int hitCount;
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
