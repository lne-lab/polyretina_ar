using System;
using System.Collections.Generic;
using UnityEngine;
using LSL;

namespace LNE.LabStreamingLayer
{
    public abstract class Inlet<T> : MonoBehaviour
    {
        [SerializeField]
        private string _streamName = "";

        [SerializeField]
        private string _streamType = "";

        private liblsl.StreamInlet inlet;
        private T[] data;

        private Dictionary<Type, Func<bool>> pullSampleOfType;

		void Start()
        {
            pullSampleOfType = new Dictionary<Type, Func<bool>> {
                { typeof(char),     () => inlet.pull_sample(data as char[], 0) > 0 },
                { typeof(string),   () => inlet.pull_sample(data as string[], 0) > 0 },
                { typeof(float),    () => inlet.pull_sample(data as float[], 0) > 0 },
                { typeof(double),   () => inlet.pull_sample(data as double[], 0) > 0 },
                { typeof(short),    () => inlet.pull_sample(data as short[], 0) > 0 },
                { typeof(int),      () => inlet.pull_sample(data as int[], 0) > 0 }
            };

            Initialise();
        }

		void Update()
		{
            if (inlet == null)
			{
                var streamInfo = ResolveStream();
                if (streamInfo != null)
                {
                    data = new T[streamInfo.channel_count()];

                    inlet = new liblsl.StreamInlet(streamInfo);
                    inlet.open_stream();
                }
			}
            else
			{
                while (pullSampleOfType[typeof(T)]())
                {
                    ProcessData(data);
                }
			}
		}

        private liblsl.StreamInfo ResolveStream()
		{
            var streamInfos = liblsl.resolve_stream("type", _streamType, 1, 0);

            if (streamInfos.Length == 0)
			{
                return null;
			}

            if (_streamName == "")
			{
                return streamInfos[0];
			}

            foreach (var streamInfo in streamInfos)
			{
                if (streamInfo.name() == _streamName)
				{
                    return streamInfo;
				}
			}

            return null;
		}

        protected virtual void Initialise() { /* Intentionally empty */ }

        protected abstract void ProcessData(T[] data);
    }
}
