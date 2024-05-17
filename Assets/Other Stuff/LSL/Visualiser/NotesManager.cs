#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LNE.LabStreamingLayer.Visualiser
{
    public class NotesManager : MonoBehaviour
    {
        [SerializeField]
        private ObjectsNotes objectsNotes;

        [SerializeField]
        private AvoidanceNotes avoidanceNotes;

        [SerializeField]
        private NavigationNotes navigationNotes;

        void Awake()
        {
            objectsNotes.Awake();
            avoidanceNotes.Awake();
            navigationNotes.Awake();
        }

        public void ShowNotes(int task)
        {
            objectsNotes.gameObject.SetActive(false);
            avoidanceNotes.gameObject.SetActive(false);
            navigationNotes.gameObject.SetActive(false);

            switch (task)
            {
                case 0:
                    objectsNotes.gameObject.SetActive(true);
                    break;
                case 1:
                    avoidanceNotes.gameObject.SetActive(true);
                    break;
                case 2:
                    navigationNotes.gameObject.SetActive(true);
                    break;
            }
		}
    }
}
