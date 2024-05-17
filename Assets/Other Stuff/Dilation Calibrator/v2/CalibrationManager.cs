using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.DilationCalibration
{
    using ArrayExts;
    using IO;
    using UI.Attributes;
    using ProstheticVision;

    public class CalibrationManager : Singleton<CalibrationManager>
    {
        /*
         * Editor fields
         */

        [Header("Settings")]

        [SerializeField, Path]
        private string _path = null;

        [SerializeField]
        private string _participant = "Jake";

        [SerializeField, Range(0, 100)]
        private int _phosphenePercentage = 30;

        [SerializeField]
        private Color _adaptationColour = Color.black;

        [SerializeField, CustomLabel(label = "Text Update Frequency")]
        private int _updateFrequency = 50;

        [SerializeField]
        private int _windowLength = 100;

        [SerializeField]
        private int _subWindowLength = 10;

        [Header("Graph Settings")]

        [SerializeField]
        private int _minutes;

        [SerializeField]
        private int _seconds;

        [SerializeField]
        private bool _includeBlinks;

        [SerializeField, CustomLabel(label = "Update Frequency")]
        private int _graphUpdateFrequency = 10;

        [Header("UI Elements")]

        [SerializeField]
        private InputField _participantLabel = null;

        [SerializeField]
        private InputField _phosphenePercentageLabel = null;

        [SerializeField]
        private Text _totalRuntimeLabel = null;

        [SerializeField]
        private Dropdown _fovDropdown = null;

        [SerializeField]
        private Dropdown _brightnessDropdown = null;

        [SerializeField]
        private InputField _customBrightness = null;

        [SerializeField]
        private Button _startStopButton = null;

        [SerializeField]
        private InputField _trialLength = null;

        [SerializeField]
        private Dropdown _trial1Dropdown = null;

        [SerializeField]
        private Dropdown _trial2Dropdown = null;

        [SerializeField]
        private Text _fov1 = null;

        [SerializeField]
        private Text _brightness1 = null;

        [SerializeField]
        private Text _runtime1 = null;

        [SerializeField]
        private Text _dilation1 = null;

        [SerializeField]
        private Text _average1 = null;

        [SerializeField]
        private Text _variance1 = null;

        [SerializeField]
        private Text _rnAverage1 = null;

        [SerializeField]
        private Text _fov2 = null;

        [SerializeField]
        private Text _brightness2 = null;

        [SerializeField]
        private Text _runtime2 = null;

        [SerializeField]
        private Text _dilation2 = null;

        [SerializeField]
        private Text _average2 = null;

        [SerializeField]
        private Text _variance2 = null;

        [SerializeField]
        private Text _rnAverage2 = null;

        [SerializeField]
        private Text _comparisonLabel = null;

        [SerializeField]
        private Text _timeSinceLastTrial = null;

        [SerializeField]
        private Text _recordingLabel = null;

        [SerializeField]
        private Material _crosshairMat = null;

        [SerializeField]
        private Image _varianceTick1 = null;

        [SerializeField]
        private Image _averageTick1 = null;

        [SerializeField]
        private Image _varianceTick2 = null;

        [SerializeField]
        private Image _averageTick2 = null;

        /*
         * Private fields
         */

        private List<Trial> trials = new List<Trial>();

        private Coroutine stopTrialCoroutine = null;

        private float trialLength = 0xffffffff;

        /*
         * Public properties
         */

        public List<Trial> Trials => trials;

        public Trial LastTrial => Trials.Count > 0 ? Trials.Last() : null;

        public string Participant => _participant;
        public int PhosphenePercentage => _phosphenePercentage;

        public int WindowLength => _windowLength;
        public int SubWindowLength => _subWindowLength;

        /*
         * Private properties
         */

        private RandomActivator Processor => Prosthesis.Instance.ExternalProcessor as RandomActivator;
        private EpiretinalImplant Implant => Prosthesis.Instance.Implant as EpiretinalImplant;

        private List<Data> Data => EyeRecorder.Instance.Data;

        private bool IsRecording => LastTrial != null && LastTrial.IsRecording;

        /*
         * Unity callbacks
         */

        void Start()
		{
            // display participants name
            _participantLabel.text = $"{_participant}";

            // display phosphene percentage and set value in processor
            _phosphenePercentageLabel.text = $"{_phosphenePercentage}%";
            Processor.percentageActive = _phosphenePercentage;

            // set adaption colour as cameras background colour (will be shown when prosthesis is turned off)
            Prosthesis.Instance.Camera.backgroundColor = _adaptationColour;

            // start coroutine to update GUI @ _updateFrequency
            StartCoroutine(UpdateSelectedTrials_Coroutine());

            // set graph and gui settings
            var graph = FindObjectOfType<DilationGraph>();
            graph.Minutes = _minutes;
            graph.Seconds = _seconds;
            graph.IncludeBlinks = _includeBlinks;
            graph.UpdateFrequency = _graphUpdateFrequency;
		}

        void FixedUpdate()
		{
            // update total runtime GUI
            _totalRuntimeLabel.text = MinutesAndSeconds(Time.time);

            // update time since last trial GUI
            if (LastTrial != null && LastTrial.IsRecording == false)
			{
                var time = Time.time - Data[LastTrial.LastFrame].time;
                _timeSinceLastTrial.text = $"Time since last trial: {MinutesAndSeconds(time)}";
            }
            else
            {
                _timeSinceLastTrial.text = $"Time since last trial: 0m 0s";
            }

            UpdateSelectedTrialRuntime(_trial1Dropdown, _runtime1);
            UpdateSelectedTrialRuntime(_trial2Dropdown, _runtime2);
        }

		void OnApplicationQuit()
		{
            var csv = new CSV();
            csv.AppendRow("participant", "percentage", "fov", "trial", "brightness", "startime", "runtime", "total_average", "window_average", "window_variance", "subwindow1_average", "subwindow2_average", "subwindow3_average", "subwindow4_average", "subwindow5_average");

            foreach (var trial in trials)
			{
                var aves = trial.GetSubWindowAverages();
                var aves_5 = ArrayExtensions.CreateArray(5, (i) =>  i < aves.Length ? aves[i] : 0f);

                csv.AppendRow(
                    Participant,
                    trial.PhosphenePercentage,
                    trial.FieldOfView,
                    trial.TrialIndex+1,
                    trial.Brightness,
                    trial.StartTime,
                    trial.Runtime,
                    trial.Average,
                    trial.GetRollingAverage(),
                    trial.GetRollingVariance(),
                    aves_5[0],
                    aves_5[1],
                    aves_5[2],
                    aves_5[3],
                    aves_5[4]
                );
			}

            csv.SaveWStream(_path + $"Dilation_Average-{System.DateTime.Now:HH_mm_ss}.csv");
        }

        IEnumerator UpdateSelectedTrials_Coroutine()
        {
            while (Application.isPlaying)
            {
                yield return new WaitForSeconds(1f / _updateFrequency);

                UpdateSelectedTrial(
                    _trial1Dropdown,
                    _fov1,
                    _brightness1,
                    _dilation1,
                    _average1,
                    _variance1,
                    _rnAverage1,
                    _varianceTick1
                );

                UpdateSelectedTrial(
                    _trial2Dropdown,
                    _fov2,
                    _brightness2,
                    _dilation2,
                    _average2,
                    _variance2,
                    _rnAverage2,
                    _varianceTick2
                );

                // update comparison
                if (_trial1Dropdown.value < trials.Count && _trial2Dropdown.value < trials.Count)
                {
                    var trial1 = trials[_trial1Dropdown.value];
                    var trial2 = trials[_trial2Dropdown.value];

                    if (trial1 != trial2)
                    {
                        var ave1 = trial1.GetRollingAverage();
                        var ave2 = trial2.GetRollingAverage();
                        var comparison = $"|{ave1:N2} - {ave2:N2}| ";
                        if (Mathf.Abs(ave1 - ave2) < 0.3)
                        {
                            comparison += $"< 0.3 (Similar)";
                            _averageTick1.enabled = trial1.Runtime > _windowLength && trial2.Runtime > _windowLength;
                            _averageTick2.enabled = trial1.Runtime > _windowLength && trial2.Runtime > _windowLength;
                        }
                        else
                        {
                            comparison += $"> 0.3 (Not similar)";
                            _averageTick1.enabled = false;
                            _averageTick2.enabled = false;
                        }

                        _comparisonLabel.text = comparison;
                    }
                    else
					{
                        _comparisonLabel.text = "";
					}
                }
            }
        }

        /*
         * Public methods
         */

        public void SetSuggestedDropdownOption()
		{
            var suggestedBrightness = 1f;

            if (_fovDropdown.value == 1)
            {
                suggestedBrightness = 1;
            }
            else if (_fovDropdown.value == 2)
            {
                suggestedBrightness = .6f;
            }
            else if (_fovDropdown.value == 3)
            {
                suggestedBrightness = .4f;
            }
            else if (_fovDropdown.value == 4)
            {
                suggestedBrightness = .3f;
            }
            else if (_fovDropdown.value == 5)
            {
                suggestedBrightness = .1f;
            }
            else if (_fovDropdown.value == 0)
            {
                suggestedBrightness = 1;
            }

            var prevValue = _brightnessDropdown.value;
            _brightnessDropdown.ClearOptions();
            _brightnessDropdown.AddOptions(new List<string> { "Full Brightness", $"Linear ({suggestedBrightness * 100:N0}%)", "Custom" });
            _brightnessDropdown.value = prevValue;
        }

        public void StartStopTrial()
		{
            if (LastTrial != null && LastTrial.IsRecording)
			{
                StopTrial();
			}
            else
			{
                StartTrial();
			}
		}

        public void StartTrial()
        {
            if (LastTrial != null && LastTrial.IsRecording)
            {
                return;
            }

            // set fov
            if (_fovDropdown.value == 1) // 15
			{
                Processor.on = true;
                Processor.interrupt = true;
                Processor.useRandom = true;
                Implant.on = true;
                Implant.fieldOfView = 15;
            }
            else if (_fovDropdown.value == 2) // 25
            {
                Processor.on = true;
                Processor.interrupt = true;
                Processor.useRandom = true;
                Implant.on = true;
                Implant.fieldOfView = 25;
            }
            else if (_fovDropdown.value == 3) // 35
            {
                Processor.on = true;
                Processor.interrupt = true;
                Processor.useRandom = true;
                Implant.on = true;
                Implant.fieldOfView = 35;
            }
            else if (_fovDropdown.value == 4) // 45
            {
                Processor.on = true;
                Processor.interrupt = true;
                Processor.useRandom = true;
                Implant.on = true;
                Implant.fieldOfView = 45;
            }
            else if (_fovDropdown.value == 5) // baseline
            {
                Processor.on = true;
                Processor.interrupt = false;
                Processor.useRandom = false;
                Implant.on = false;
                Implant.fieldOfView = 110;
            }
            else if (_fovDropdown.value == 0) // adaptation
            {
                Processor.on = false;
                Implant.on = false;
                Implant.fieldOfView = 0;
            }

            // set brightness
            if (_brightnessDropdown.value == 0) // full brightness
            {
                Implant.brightness = 1;
                Processor.brightness = 1;
			}
            else if (_brightnessDropdown.value == 1) // suggested brightness
			{
                if (_fovDropdown.value == 1)
				{
                    Implant.brightness = 1;
                    Processor.brightness = 1;
                }
                else if (_fovDropdown.value == 2)
                {
                    Implant.brightness = .6f;
                    Processor.brightness = 1;
                }
                else if (_fovDropdown.value == 3)
                {
                    Implant.brightness = .4f;
                    Processor.brightness = 1;
                }
                else if (_fovDropdown.value == 4)
                {
                    Implant.brightness = .3f;
                    Processor.brightness = 1;
                }
                else if (_fovDropdown.value == 5)
                {
                    Implant.brightness = 1;
                    Processor.brightness = .1f;
                }
                else if (_fovDropdown.value == 0)
                {
                    Implant.brightness = 1;
                    Processor.brightness = 1;
                }
			}
            else if (_brightnessDropdown.value == 2) // custom brightness
			{
                if (_fovDropdown.value == 5)
                {
                    Processor.brightness = float.Parse(_customBrightness.text) / 100;
                    Implant.brightness = 1;
                }
                else
                {
                    Processor.brightness = 1;
                    Implant.brightness = float.Parse(_customBrightness.text) / 100;
                }
			}

            // wait for a little bit to create trial so eye recorder has time to create
            // the first frame
            StartCoroutine(CreateTrial_Coroutine());

            // begin coroutine to stop trial
            stopTrialCoroutine = StartCoroutine(StopTrial_Coroutine());

            // change button to stop button
            _startStopButton.GetComponentInChildren<Text>().text = "Stop trial";
            _startStopButton.GetComponent<Image>().color = new Color(1, .9f, .9f);
            _recordingLabel.gameObject.SetActive(true);
        }

        public void HideCustomBrightness()
		{
            _customBrightness.gameObject.SetActive(_brightnessDropdown.value == 2);
		}

        IEnumerator CreateTrial_Coroutine()
		{
            yield return new WaitForFixedUpdate();

            trials.Add(new Trial());
            var options = new List<string>(trials.ToArray().Convert((trial) => trial.Name));

            _trial1Dropdown.ClearOptions();
            _trial1Dropdown.AddOptions(options);
            _trial1Dropdown.value = trials.Count - 1;

            //options.RemoveAt(options.Count - 1);

            //if (options.Count > 0)
            //{
            //    var prevOption = _trial2Dropdown.value;
            //    _trial2Dropdown.ClearOptions();
            //    _trial2Dropdown.AddOptions(options);
            //    _trial2Dropdown.value = prevOption;
            //}
        }

        public void StopTrial()
		{
            // cancel coroutine planning to stop trial if we've already stopped it
            if (stopTrialCoroutine != null)
            {
                StopCoroutine(stopTrialCoroutine);
            }

            LastTrial.StopRecording();

            Processor.on = false;
            Implant.on = true;

            // change button to start button
            _startStopButton.GetComponentInChildren<Text>().text = "Start trial";
            _startStopButton.GetComponent<Image>().color = new Color(.95f, 1, .9f);
            _recordingLabel.gameObject.SetActive(false);

            // add newest trial to comparison dropdown
            var options = new List<string>(trials.ToArray().Convert((trial) => trial.Name));
            var prevOption = _trial2Dropdown.value;
            _trial2Dropdown.ClearOptions();
            _trial2Dropdown.AddOptions(options);
            _trial2Dropdown.value = prevOption;
        }

        IEnumerator StopTrial_Coroutine()
        {
            yield return new WaitForFixedUpdate();

            while (RuntimeIsLessThanTrialLength())
            {
                yield return null;
            }

            StopTrial();
            FindObjectOfType<DilationGraph>().AddChangeBar();
		}

        public void UpdateTrialLength()
        {
            var success = float.TryParse(_trialLength.text, out trialLength);
            if (success == false)
			{
                trialLength = 0xffffffff;
			}
        }

        public void AddPercentage()
		{
            var number = int.Parse(_phosphenePercentageLabel.text);

            number = Mathf.Clamp(number, 0, 100);

            _phosphenePercentageLabel.contentType = InputField.ContentType.Standard;
            _phosphenePercentageLabel.text = $"{number}%";
            _phosphenePercentageLabel.contentType = InputField.ContentType.IntegerNumber;
		}

        public void SetPhosphenePercentage()
		{
            var percent = int.Parse(_phosphenePercentageLabel.text.Substring(0, _phosphenePercentageLabel.text.Length - 1));
            _phosphenePercentage = percent;
            Processor.percentageActive = percent;
        }

        public void SetFieldOfView()
		{
            // set fov
            if (_fovDropdown.value == 1) // 15
            {
                Implant.on = true;
                Implant.fieldOfView = 15;
            }
            else if (_fovDropdown.value == 2) // 25
            {
                Implant.on = true;
                Implant.fieldOfView = 25;
            }
            else if (_fovDropdown.value == 3) // 35
            {
                Implant.on = true;
                Implant.fieldOfView = 35;
            }
            else if (_fovDropdown.value == 4) // 45
            {
                Implant.on = true;
                Implant.fieldOfView = 45;
            }
        }

        public void SetBrightness()
        {
            // set brightness
            if (_brightnessDropdown.value == 0) // full brightness
            {
                Implant.brightness = 1;
                Processor.brightness = 1;
            }
            else if (_brightnessDropdown.value == 1) // suggested brightness
            {
                if (_fovDropdown.value == 1)
                {
                    Implant.brightness = 1;
                    Processor.brightness = 1;
                }
                else if (_fovDropdown.value == 2)
                {
                    Implant.brightness = .6f;
                    Processor.brightness = 1;
                }
                else if (_fovDropdown.value == 3)
                {
                    Implant.brightness = .4f;
                    Processor.brightness = 1;
                }
                else if (_fovDropdown.value == 4)
                {
                    Implant.brightness = .3f;
                    Processor.brightness = 1;
                }
                else if (_fovDropdown.value == 5)
                {
                    Implant.brightness = 1;
                    Processor.brightness = .1f;
                }
                else if (_fovDropdown.value == 0)
                {
                    Implant.brightness = 1;
                    Processor.brightness = 1;
                }
            }
            else if (_brightnessDropdown.value == 2) // custom brightness
            {
                var brightness = _customBrightness.text != "" ? float.Parse(_customBrightness.text) : 0;

                if (_fovDropdown.value == 5)
                {
                    Processor.brightness = brightness / 100;
                    Implant.brightness = 1;
                }
                else
                {
                    Processor.brightness = 1;
                    Implant.brightness = brightness / 100;
                }
            }
        }

        public void SetParticipantName(string name)
		{
            _participant = name;
		}

        /*
         * Private methods
         */

        private void UpdateSelectedTrial(Dropdown trialDropdown,
                                            Text fov,
                                            Text brightness,
                                            Text dilation,
                                            Text average,
                                            Text variance,
                                            Text rnAverage,
                                            Image varianceTick)
        {
            if (trialDropdown.options.Count > 0 && trialDropdown.value < trials.Count)
            {
                var selectedTrial = trials[trialDropdown.value];

                fov.text = $"Field of View: <b>{selectedTrial.FieldOfView}</b>";
                brightness.text = $"Brightness: <b>{selectedTrial.Brightness * 100}%</b>";
                dilation.text = $"Dilation: <b>{selectedTrial.Dilation:N2}</b>";
                average.text = $"Average: <b>{selectedTrial.Average:N2}</b>";

                //selectedTrial.RollingInfo(out float v, out float a);
                var v = selectedTrial.GetRollingVariance();
                var a = selectedTrial.GetRollingAverage();
                variance.text = $"R  Variance: <b>{v:N2}</b>";
                rnAverage.text = $"R  Average: <b>{a:N2}</b>";

                varianceTick.enabled = selectedTrial.Runtime > _windowLength && selectedTrial.CheckVariance(v);
            }
        }

        private void UpdateSelectedTrialRuntime(Dropdown trialDropdown,
                                                Text runtime)
        {
            if (trialDropdown.options.Count > 0 && trialDropdown.value < trials.Count)
            {
                var selectedTrial = trials[trialDropdown.value];
                
                runtime.text = $"Runtime: <b>{MinutesAndSeconds(selectedTrial.Runtime)}</b>";
            }
        }

        private string MinutesAndSeconds(float time)
        {
            var minutes = (int)(time / 60);
            var seconds = time % 60;

            return $"{minutes}m {seconds:N1}s";
        }

        private bool RuntimeIsLessThanTrialLength()
		{
            return LastTrial.Runtime < trialLength;
        }
    }

    public class Trial
	{
        private int firstFrame;
        private int lastFrame;

        public int FirstFrame => firstFrame;
        public int LastFrame => lastFrame >= 0 ? lastFrame : Data.Count - 1;

        public int TrialIndex { get; private set; }
        public string Name => $"FoV={FieldOfView} ({TrialIndex}) @ {Brightness * 100}%";

        public int PhosphenePercentage => Data[FirstFrame].phosphenePercentage;
        public float FieldOfView => Data[FirstFrame].fieldOfView;
        public float Brightness => Data[FirstFrame].brightness;
        public float StartTime => Data[FirstFrame].time;
        public float Runtime => Data[LastFrame].time - Data[FirstFrame].time;
        public float Dilation => Data[LastFrame].dilation_right;
        
        public float Average
		{
            get
			{
                try
                {
                    // remove first 5 seconds
                    var count = Mathf.Max(LastFrame - FirstFrame - 250, 0);

                    return Data .GetRange(FirstFrame, count)
                                .ToArray()
                                .Convert((datum) => datum.dilation_right)
                                .Remove((dilation) => dilation < 0)
                                .Average();
                }
                catch (System.InvalidOperationException)
				{
                    return 0;
				}
			}
		}

        public bool IsRecording => lastFrame == -1;

        private List<Data> Data => EyeRecorder.Instance.Data;

        public Trial()
		{
            firstFrame = Data.Count-1;
            lastFrame = -1;

            TrialIndex = FindTrialIndex();
		}

        public void StopRecording()
		{
            if (IsRecording)
			{
                lastFrame = Data.Count - 1;
			}
		}

        public void RollingInfo(out float variance, out float average)
        {
            try
            {
                var framesWanted = CalibrationManager.Instance.WindowLength * 50;
                var framesAvailable = Mathf.Min(framesWanted, LastFrame - FirstFrame);
                var frames = Data.GetRange(FirstFrame, framesAvailable);

                var data = frames.ToArray()
                                    .Convert((datum) => datum.dilation_right)
                                    .Remove((dilation) => dilation < 0);

                average = data.Average();

                var varsum = 0f;
                foreach (var datum in data)
                {
                    varsum += Mathf.Pow(datum - average, 2);
                }

                variance = varsum / data.Length;
            }
            catch (System.InvalidOperationException)
            {
                variance = 0;
                average = 0;
			}
        }

        public float GetRollingAverage()
		{
            try
            {
                var framesWanted = CalibrationManager.Instance.WindowLength * 50;
                var framesAvailable = Mathf.Min(framesWanted, LastFrame - FirstFrame);
                var frames = Data.GetRange(LastFrame - framesAvailable, framesAvailable);

                var data = frames.ToArray()
                                    .Convert((datum) => datum.dilation_right)
                                    .Remove((dilation) => dilation < 0);

                return data.Average();

            }
            catch (System.InvalidOperationException)
            {
                return 0;
            }
        }

        public float GetRollingVariance()
        {
            try
            {
                var cm = CalibrationManager.Instance;

                var framesWanted = cm.WindowLength * 50;
                var framesAvailable = Mathf.Min(framesWanted, LastFrame - FirstFrame);
                var frames = Data.GetRange(LastFrame - framesAvailable, framesAvailable);

                var averages = new List<float>();
                var numSubWindowsFrames = cm.SubWindowLength * 50;
                for (int i = 0; i <= (frames.Count - numSubWindowsFrames); i += numSubWindowsFrames)
                {
                    var subWindowFrames = frames.GetRange(i, numSubWindowsFrames)
                                                .ToArray()
                                                .Convert((datum) => datum.dilation_right)
                                                .Remove((dilation) => dilation < 0);

                    var average = subWindowFrames.Average();
                    averages.Add(average);
                }

                var averages_arr = averages.ToArray();
                var variance = Mathf.Max(averages_arr) - Mathf.Min(averages_arr);

                return variance;
            }
            catch (System.InvalidOperationException)
			{
                return 0;
			}
        }

        public float[] GetSubWindowAverages()
        {
            try
            {
                var cm = CalibrationManager.Instance;

                var framesWanted = cm.WindowLength * 50;
                var framesAvailable = Mathf.Min(framesWanted, LastFrame - FirstFrame);
                var frames = Data.GetRange(LastFrame - framesAvailable, framesAvailable);

                var averages = new List<float>();
                var numSubWindowsFrames = cm.SubWindowLength * 50;
                for (int i = 0; i <= (frames.Count - numSubWindowsFrames); i += numSubWindowsFrames)
                {
                    var subWindowFrames = frames.GetRange(i, numSubWindowsFrames)
                                                .ToArray()
                                                .Convert((datum) => datum.dilation_right)
                                                .Remove((dilation) => dilation < 0);

                    var average = subWindowFrames.Average();
                    averages.Add(average);
                }

                var averages_arr = averages.ToArray();
                return averages_arr;
            }
            catch (System.InvalidOperationException)
            {
                return default;
            }
        }

        public bool CheckVariance(float variance)
		{
            return variance < 0.3f;
		}

        private int FindTrialIndex()
		{
            var index = 0;

            var trials = CalibrationManager.Instance.Trials;
            foreach (var trial in trials)
            {
                if (trial.FieldOfView == FieldOfView)
                {
                    index++;
                }
            }

            return index;
        }
    }
}
