#pragma warning disable 649

using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
    using static ArrayExts.ArrayExtensions;
    using UI.Attributes;

    public class StreetlabNotes : MonoBehaviour
    {
        [SerializeField, Path(isRelative = true)]
        private string path = "";

        [SerializeField]
        private Dropdown runDropdown;

        [Space]

        [SerializeField]
        private InputField familObsNum;

        [SerializeField]
        private InputField familRecObsNum;

        [SerializeField]
        private InputField familRecDigitNum;

        [SerializeField]
        private InputField familRecLetterNum;

        [SerializeField]
        private Toggle familPlacedWell;

        [Space]

        [SerializeField]
        private Toggle atmFound;

        [SerializeField]
        private Toggle atmScreen;

        [SerializeField]
        private Toggle atmTookBill;

        [SerializeField]
        private Toggle atmRightVal;

        [SerializeField]
        private InputField atmConfusionNum;

        [SerializeField]
        private InputField atmRevisitNum;

        [Space]

        [SerializeField]
        private Toggle postFound;

        [SerializeField]
        private Toggle postFoundStamp;

        [SerializeField]
        private Toggle postPutLetter;

        [SerializeField]
        private InputField postConfusionNum;

        [SerializeField]
        private InputField postRevisitNum;

        [Space]

        [SerializeField]
        private Toggle homeFound;

        [SerializeField]
        private InputField homeConfusionNum;

        [SerializeField]
        private InputField homeRevisitNum;

        [Space]

        [SerializeField]
        private InputField streetCrossingNum;

        [SerializeField]
        private InputField streetSteppedStrtNum;

        [SerializeField]
        private InputField streetCollisionNum;

        [Space]

        [SerializeField]
        private InputField otherNotes;

        [Space]

        [SerializeField]
        private Dropdown participantDropdown;

        [SerializeField]
        private Dropdown conditionDropdown;

        private Notes[] notes;

        public void Awake()
        {
            notes = CreateArray(12, (i) => new Notes(i));
        }

        void OnApplicationQuit()
        {
            var participantId = participantDropdown.value + 1;
            var condition = conditionDropdown.value + 1;
            var run = runDropdown.value + 1;
            var data = new AllNotes { participantId = participantId, condition = condition, notes = notes };
            for (int i = 0; i < 12; i++)
            {
                data.notes[i].runId = run;
            }

            var json = JsonUtility.ToJson(data, true);
            var filename = $"StreetlabStudy-P{participantId:00}-notes.json";
            File.WriteAllText(path + filename, json);
        }

        public void LoadNotes()
        {
            LoadNotes(runDropdown.value);
        }

        public void LoadNotes(int run)
        {
            otherNotes.text = notes[run].otherNotes;

            familObsNum.text = notes[run].familObsNum.ToString();
            familRecObsNum.text = notes[run].familRecObsNum.ToString();
            familRecDigitNum.text = notes[run].familRecDigitNum.ToString();
            familRecLetterNum.text = notes[run].familRecLetterNum.ToString();
            familPlacedWell.isOn = notes[run].familPlacedWell;

            atmFound.isOn = notes[run].atmFound;
            atmScreen.isOn = notes[run].atmScreen;
            atmTookBill.isOn = notes[run].atmTookBill;
            atmRightVal.isOn = notes[run].atmRightVal;
            atmConfusionNum.text = notes[run].atmConfusionNum.ToString();
            atmRevisitNum.text = notes[run].atmRevisitNum.ToString();

            postFound.isOn = notes[run].postFound;
            postFoundStamp.isOn = notes[run].postFoundStamp;
            postPutLetter.isOn = notes[run].postPutLetter;
            postConfusionNum.text = notes[run].postConfusionNum.ToString();
            postRevisitNum.text = notes[run].postRevisitNum.ToString();

            homeFound.isOn = notes[run].homeFound;
            homeConfusionNum.text = notes[run].homeConfusionNum.ToString();
            homeRevisitNum.text = notes[run].homeRevisitNum.ToString();

            streetCrossingNum.text = notes[run].streetCrossingNum.ToString();
            streetSteppedStrtNum.text = notes[run].streetSteppedStrtNum.ToString();
            streetCollisionNum.text = notes[run].streetCollisionNum.ToString();
        }

        public void ApplyNotes()
        {
            ApplyNotes(runDropdown.value);
        }

        public void ApplyNotes(int run)
        {
            notes[run].otherNotes = otherNotes.text;
            
            notes[run].familObsNum = int.Parse(familObsNum.text);
            notes[run].familRecObsNum = int.Parse(familRecObsNum.text);
            notes[run].familRecDigitNum = int.Parse(familRecDigitNum.text);
            notes[run].familRecLetterNum = int.Parse(familRecLetterNum.text);
            notes[run].familPlacedWell = familPlacedWell.isOn;

            notes[run].atmFound = atmFound.isOn;
            notes[run].atmScreen = atmScreen.isOn;
            notes[run].atmTookBill = atmTookBill.isOn;
            notes[run].atmRightVal = atmRightVal.isOn;
            notes[run].atmConfusionNum = int.Parse(atmConfusionNum.text);
            notes[run].atmRevisitNum = int.Parse(atmRevisitNum.text);

            notes[run].postFound = postFound.isOn;
            notes[run].postFoundStamp = postFoundStamp.isOn;
            notes[run].postPutLetter = postPutLetter.isOn;
            notes[run].postConfusionNum = int.Parse(postConfusionNum.text);
            notes[run].postRevisitNum = int.Parse(postRevisitNum.text);

            notes[run].homeFound = homeFound.isOn; 
            notes[run].homeConfusionNum = int.Parse(homeConfusionNum.text);
            notes[run].homeRevisitNum = int.Parse(homeRevisitNum.text);

            notes[run].streetCrossingNum = int.Parse(streetCrossingNum.text);
            notes[run].streetSteppedStrtNum = int.Parse(streetSteppedStrtNum.text);
            notes[run].streetCollisionNum = int.Parse(streetCollisionNum.text);
        }

        [System.Serializable]
        public class Notes
        {
            public Notes(int i)
            {
                runId = i;
                condition = 0;

                familObsNum = 0;
                familRecObsNum = 0;
                familRecDigitNum = 0;
                familRecLetterNum = 0;
                familPlacedWell = false;

                atmFound = false;
                atmScreen = false;
                atmTookBill = false;
                atmRightVal = false;
                atmConfusionNum = 0;
                atmRevisitNum = 0;

                postFound = false;
                postFoundStamp = false;
                postPutLetter = false;
                postConfusionNum = 0;
                postRevisitNum = 0;

                homeFound = false;
                homeConfusionNum = 0;
                homeRevisitNum = 0;

                streetCrossingNum = 0;
                streetSteppedStrtNum = 0;
                streetCollisionNum = 0;

                otherNotes = "";
            }

            public int runId;
            public int condition;

            public int familObsNum;
            public int familRecObsNum;
            public int familRecDigitNum;
            public int familRecLetterNum;
            public bool familPlacedWell;

            public bool atmFound;
            public bool atmScreen;
            public bool atmTookBill;
            public bool atmRightVal;
            public int atmConfusionNum;
            public int atmRevisitNum;

            public bool postFound;
            public bool postFoundStamp;
            public bool postPutLetter;
            public int postConfusionNum;
            public int postRevisitNum;

            public bool homeFound;
            public int homeConfusionNum;
            public int homeRevisitNum;

            public int streetCrossingNum;
            public int streetSteppedStrtNum;
            public int streetCollisionNum;

            public string otherNotes;
        }

        [System.Serializable]
        public class AllNotes
        {
            public int participantId;
            public int condition;

            public Notes[] notes;
        }
    }
}
