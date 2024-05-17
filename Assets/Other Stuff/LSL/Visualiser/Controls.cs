#pragma warning disable 649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LNE.LabStreamingLayer.Visualiser
{
    public class Controls : MonoBehaviour
    {
        [SerializeField]
        private new Camera camera = null;

        [Header("Zoom Positions")]

        [SerializeField]
        private Vector2 topRight = default;

        [SerializeField]
        private Vector2 bottomRight = default;

        [SerializeField]
        private Vector2 bottomLeft = default;

        [SerializeField]
        private Vector2 topLeft = default;

        [Space]

        [SerializeField]
        private Button[] buttons;

        private Vector2 initialPos = default;
        private bool zoomed = false;

        void Start()
		{
            initialPos = camera.transform.position;
		}

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
			{
                var button = GetPressedButton();
                if (button != null)
                {
                    button.action.Invoke();
                }
                else
				{
                    Zoom();
				}
			}
        }

        public void TestControls(string message)
		{
            Debug.Log(message);
		}

        private Button GetPressedButton()
        {
            var mousePos = new Vector2(
                Input.mousePosition.x / Screen.width * 2 - 1,
                Input.mousePosition.y / Screen.height * 2 - 1
            );

            mousePos *= camera.orthographicSize;
            mousePos += new Vector2(camera.transform.position.x, camera.transform.position.y);

            foreach (var button in buttons)
			{
                if (button.area.Contains(mousePos))
				{
                    return button;
				}
			}

            return null;
		}

        private void Zoom()
        {
            if (zoomed)
            {
                camera.orthographicSize = 1;
                camera.transform.position = initialPos;

                zoomed = false;
            }
            else
            {
                camera.orthographicSize = 0.5f;

                var mousePos = new Vector2(
                    Input.mousePosition.x / Screen.width,
                    Input.mousePosition.y / Screen.height
                );

                if (mousePos.x > 0.5 && mousePos.y > 0.5)
                {
                    camera.transform.position = topLeft;
                }
                else if (mousePos.x > 0.5 && mousePos.y < 0.5)
                {
                    camera.transform.position = bottomLeft;
                }
                else if (mousePos.x < 0.5 && mousePos.y < 0.5)
                {
                    camera.transform.position = bottomRight;
                }
                else if (mousePos.x < 0.5 && mousePos.y > 0.5)
                {
                    camera.transform.position = topRight;
                }

                zoomed = true;
            }
        }

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			foreach (var button in buttons)
            {
                var center = new Vector3(
                    button.area.x + button.area.width / 2,
                    button.area.y + button.area.height / 2,
                    0
                );

                var size = new Vector3(
                    button.area.width,
                    button.area.height,
                    .001f
                );

                Gizmos.color = new Color(1, .2f, .1f);
                Gizmos.DrawWireCube(center, size);
            }
		}
# endif

		[Serializable]
        public class Button
		{
            public string name;
            public Rect area;
            public UnityEvent action;
		}
    }
}
