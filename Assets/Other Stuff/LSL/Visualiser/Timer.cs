#pragma warning disable 649

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private Slider slider;

        [SerializeField]
        private Text timeBox;

        [SerializeField]
        private UnityEvent onTimerStarted;

        [SerializeField]
        private UnityEvent onTimerFinished;

        private bool on = false;

        private float time
		{
            get
			{
                return slider.value;
			}

			set
			{
                slider.value = value;

                var min = (int)value / 60;
                var sec = (int)value % 60;
                timeBox.text = $"{min:00}:{sec:00}";

            }
		}

        void Update()
		{
            if (on)
			{
                time -= Time.deltaTime;
                if (time <= 1)
                {
                    on = false;
                    time = 0;
                    onTimerFinished.Invoke();
				}
			}
		}

        public void Begin()
        {
            Begin(slider.value);
        }

        public void Begin(float time)
		{
            on = true;
            this.time = Mathf.Floor(time) + DateTime.Now.Millisecond / 1000f;
		}
    }
}
