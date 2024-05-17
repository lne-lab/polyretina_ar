using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
    public class QuestionnaireButton : MonoBehaviour
    {
        [SerializeField]
        private TagOutlet visionOutlet = null;

        [SerializeField]
        private Dropdown participant = null;

        [SerializeField]
        private Dropdown task = null;

        public void SendTag()
		{
            visionOutlet.PushSample($"Questionnaire|{participant.value}|{task.value}");
		}
    }
}
