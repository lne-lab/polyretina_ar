#pragma warning disable 649

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer
{
	public class SignalInlet : Inlet<float>
	{
		[SerializeField]
		private int _streamChannel = 0;

		[SerializeField]
		private float _timeLength = 60;

		[SerializeField]
		private float _yMinLimit = 1;

		[SerializeField]
		private float _yMaxLimit = -1;

		[SerializeField]
		private Rect _area = default;

		[Space]

		[SerializeField]
		private LineRenderer _line = null;

		[SerializeField]
		private Text _maxLabel;

		[SerializeField]
		private Text _minLabel;

		private List<Vector3> points = new List<Vector3>();
		private float prevTime = 0;

		protected override void Initialise()
		{
			_line.positionCount = 0;
		}

		protected override void ProcessData(float[] data)
		{
			if (data.Length <= _streamChannel)
			{
				throw new Exception($"Requested stream channel {_streamChannel}, but only {data.Length} channels found.");
			}
			
			var value = data[_streamChannel];

			SlidePoints();
			AddNextPoint(value);
			var ps = TransformPoints();
			_line.positionCount = ps.Length;
			_line.SetPositions(ps);
		}

		private void SlidePoints()
		{
			var deltaTime = (Time.time - prevTime) *  (1 / _timeLength);
			prevTime = Time.time;

			for (int i = 0; i < points.Count; i++)
			{
				var point = new Vector2(points[i].x - deltaTime, points[i].y);
				if (point.x > _area.x)
				{
					points[i] = point;
				}
				else
				{
					points.RemoveAt(i);
					i--;
				}
			}
		}

		private void AddNextPoint(float value)
		{
			points.Add(new Vector2(
				_area.x + _area.width,
				value
			));
		}

		private Vector3[] TransformPoints()
		{
			var ps = points.ToArray();

			var minPoint = _yMinLimit < _yMaxLimit ? _yMinLimit : float.MaxValue;
			var maxPoint = _yMinLimit < _yMaxLimit ? _yMaxLimit : float.MinValue;
			foreach (var point in ps)
			{
				minPoint = Mathf.Min(minPoint, point.y);
				maxPoint = Mathf.Max(maxPoint, point.y);
			}

			if (_minLabel != null) _minLabel.text = $"{minPoint:0.00}";
			if (_maxLabel != null) _maxLabel.text = $"{maxPoint:0.00}";

			var mean = (minPoint + maxPoint) / 2;
			var range = minPoint != maxPoint ? .49f / (maxPoint - mean) : 1;
			var toArea = _area.y + _area.height / 2;

			for (int i = 0; i < ps.Length; i++)
			{
				ps[i].y -= mean;	// center on zero
				ps[i].y *= range;   // make extremes touch -.49 and +.49
				ps[i].y *= _area.height;
				ps[i].y += toArea;	// translate to graph area
			}

			return ps;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			var center = new Vector3(
				_area.x + _area.width / 2,
				_area.y + _area.height / 2,
				0
			);

			var size = new Vector3(
				_area.width,
				_area.height,
				.001f
			);

			Gizmos.color = new Color(1, .2f, .1f);
			Gizmos.DrawWireCube(center, size);
		}
#endif
	}
}
