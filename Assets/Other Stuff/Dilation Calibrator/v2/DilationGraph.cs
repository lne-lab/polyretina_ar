using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.DilationCalibration
{
	using ArrayExts;
	using ProstheticVision;

    public class DilationGraph : MonoBehaviour
    {
		[SerializeField]
		private StereoTargetEyeMask _targetEye = StereoTargetEyeMask.Left;

		[SerializeField]
		private float _graphWidth = 4;

		[SerializeField]
		private bool _includeBlinks = false;

		[SerializeField]
		private int _minutes = 5;

		[SerializeField]
		private int _seconds = 0;

		[SerializeField]
		private int _updateFrequency = 50;

		[SerializeField]
		private GameObject _changeBarParent = null;

		[SerializeField]
		private GameObject _changeBarTemplate = null;

		private List<GameObject> changeBars = new List<GameObject>();
		private bool isStartBar = true;

		public int Minutes
		{
			get { return _minutes; }
			set { _minutes = value; }
		}

		public int Seconds
		{
			get { return _seconds; }
			set { _seconds = value; }
		}

		public bool IncludeBlinks
		{
			get { return _includeBlinks; }
			set { _includeBlinks = value; }
		}

		public int UpdateFrequency
		{
			get { return _updateFrequency; }
			set { _updateFrequency = value; }
		}

		private EpiretinalImplant Implant => Prosthesis.Instance.Implant as EpiretinalImplant;
		private LineRenderer Line => GetComponent<LineRenderer>();

		void Start()
		{
			StartCoroutine(UpdateGraph_Coroutine());
		}

		IEnumerator UpdateGraph_Coroutine()
		{
			while (Application.isPlaying)
			{
				yield return new WaitForSeconds(1f / _updateFrequency);
				UpdateGraph();
				UpdateChangeBars();
			}
		}

		void UpdateGraph()
		{
			var count = (_minutes * 60 + _seconds) * 50;
			var data = GetLastData(count);

			Line.positionCount = data.Count;
			Line.SetPositions(data.ToArray().Convert((i, datum) =>
			{
				var x = i / (count / _graphWidth) - (_graphWidth / 2);
				var y = datum.GetDilation(_targetEye);

				if (_includeBlinks == false && y < 0)
				{
					y = 4;
					for (int j = i-1; j >= 0; j--)
					{
						var d = data[j].GetDilation(_targetEye);
						if (d > 0)
						{
							y = d;
							break;
						}
					}
				}

				y -= 5;	// average dil=4, center around 0
				y /= 3;	// limit extremes to -1 -> 1

				return new Vector3(x, y, 0);
			}));
		}

		void UpdateChangeBars()
		{
			float numPoints = EyeRecorder.Instance.Data.Count;
			float maxPoints = (_minutes * 60 + _seconds) * 50;

			if (numPoints < maxPoints)
			{
				return;
			}

			for (int i = 0; i < changeBars.Count; i++)
			{
				var rt = changeBars[i].transform as RectTransform;

				// calculate speed
				float time = (_minutes * 60 + _seconds) * _updateFrequency;
				float distance = 1500;

				rt.anchoredPosition -= new Vector2(distance / time, 0);

				if (rt.anchoredPosition.x < -1500)
				{
					Destroy(changeBars[i]);
					changeBars.RemoveAt(i--);
				}
			}
		}

		public void ChangeEye(int eye)
		{
			switch (eye)
			{
				case 0:
					_targetEye = StereoTargetEyeMask.Left;
					break;
				case 1:
					_targetEye = StereoTargetEyeMask.Right;
					break;
			}
		}

		public void AddChangeBar()
		{
			var changeBar = Instantiate(_changeBarTemplate, _changeBarParent.transform);
			changeBar.SetActive(true);
			var texts = changeBar.GetComponentsInChildren<Text>();
			texts[0].text = $"{Implant.fieldOfView}";
			texts[1].text = isStartBar ? "       >" : "<       ";

			changeBars.Add(changeBar);

			isStartBar = !isStartBar;

			// set position
			float numPoints = EyeRecorder.Instance.Data.Count;
			float maxPoints = (_minutes * 60 + _seconds) * 50;

			if (numPoints < maxPoints)
			{
				var ratio = numPoints / maxPoints;
				var position = 1500 * ratio;
				(changeBar.transform as RectTransform).anchoredPosition = new Vector2(-1500 + position, 0);
			}
		}

		private List<Data> GetLastData(int count)
		{
			var allData = EyeRecorder.Instance.Data;
			if (allData.Count < count)
			{
				return allData;
			}
			else
			{
				return allData.GetRange(allData.Count - count, count);
			}
		}
	}
}
