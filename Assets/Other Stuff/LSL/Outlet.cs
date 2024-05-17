#pragma warning disable 649

using System;
using System.Collections.Generic;
using UnityEngine;
using LSL;

namespace LNE.LabStreamingLayer
{
	public abstract class Outlet<T> : MonoBehaviour
	{
		/*
		 * Editor fields
		 */

		[SerializeField]
		private string _streamName = "";

		[SerializeField]
		private float _sampleRate = 0;

		[SerializeField]
		private bool _onStart = false;

		/*
		 * Private fields
		 */

		private liblsl.StreamInfo info;
		private liblsl.StreamOutlet outlet;

		private T[] data;

		private Dictionary<Type, Action<T[]>> pushSampleOfType;

		private const int VARIABLE_SRATE = 0;

		/*
		 * Private properties
		 */

		private liblsl.channel_format_t ChannelFormat
		{
			get
			{
				return new Dictionary<Type, liblsl.channel_format_t> {
					{ typeof(char),		liblsl.channel_format_t.cf_int8 },
					{ typeof(string),	liblsl.channel_format_t.cf_string },
					{ typeof(float),	liblsl.channel_format_t.cf_float32 },
					{ typeof(double),	liblsl.channel_format_t.cf_double64 },
					{ typeof(short),	liblsl.channel_format_t.cf_int16 },
					{ typeof(int),		liblsl.channel_format_t.cf_int32 }
				}
				[typeof(T)];
			}
		}

		/*
		 * Unity callbacks
		 */

		protected virtual void Start()
		{
			if (_onStart == false)
				return;

			Init(_streamName, _sampleRate);
		}

		protected virtual void FixedUpdate()
		{ 
			if (_sampleRate == VARIABLE_SRATE)
				return;

			var updateCount = Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
			var updatesPerFrame = Mathf.RoundToInt(1 / (_sampleRate * Time.fixedDeltaTime));
			
			if (updateCount % updatesPerFrame == 0)
			{
				PushSample();
			}
		}

		/*
		 * Public methods
		 */

		public void Init(string streamName, float sampleRate)
		{
			_streamName = streamName;
			_sampleRate = Mathf.Min(sampleRate, 1 / Time.fixedDeltaTime);

			info = new liblsl.StreamInfo(
				_streamName,
				StreamType,
				NumChannels,
				_sampleRate,
				ChannelFormat,
				OutletName
			);

			DefineMetaData(info);

			outlet = new liblsl.StreamOutlet(info);

			data = new T[NumChannels];

			pushSampleOfType = new Dictionary<Type, Action<T[]>> {
				{ typeof(char),		(data) => outlet?.push_sample(data as char[]) },
				{ typeof(string),	(data) => outlet?.push_sample(data as string[]) },
				{ typeof(float),	(data) => outlet?.push_sample(data as float[]) },
				{ typeof(double),	(data) => outlet?.push_sample(data as double[]) },
				{ typeof(short),	(data) => outlet?.push_sample(data as short[]) },
				{ typeof(int),		(data) => outlet?.push_sample(data as int[]) }
			};
		}

		public void PushSample()
		{
			UpdateData(data);
			pushSampleOfType[typeof(T)](data);
		}

		/*
		 * Abstract / Virtual interface
		 */

		public abstract string StreamType { get; }
		public abstract int NumChannels { get; }
		public virtual string OutletName => Application.productName;

		protected abstract void DefineMetaData(liblsl.StreamInfo info);

		protected abstract void UpdateData(T[] data);
	}
}
