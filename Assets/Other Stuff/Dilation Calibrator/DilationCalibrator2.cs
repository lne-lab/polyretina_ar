#pragma warning disable 649

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using LNE.ProstheticVision;

public class DilationCalibrator2 : MonoBehaviour
{
    /*
     * Editor fields
     */

    [SerializeField]
    private int fov = 15;

    [SerializeField]
    private float recordTime = 20;

    [SerializeField]
    private DilationCalibrator2 fifteenRecorder;

    /*
     * Private fields
     */

    private bool recording = false;
    private bool resting = false;
    private List<Data> data = new List<Data>();

    /*
     * Public/Private Properties
     */

    private RandomActivator RandomActivator => Prosthesis.Instance.ExternalProcessor as RandomActivator;
    private EpiretinalImplant Implant => Prosthesis.Instance.Implant as EpiretinalImplant;

    public List<Data> Results => data;
    public int Run { get; private set; } = 0;
    public float Brightness { get; set; } = 1;

    public float LastBrightness => data.Last().brightness;
    public float LastDilation => data.Last().AverageDilation;

    public float RecordTime => recordTime;

    public float CondAverageDilation
    {
        get
        {
            var first = Mathf.Max(data.Count - 5, 0);

            var averageAverageDilation = 0f;
            for (int i = first; i < data.Count; i++)
            {
                averageAverageDilation += data[i].AverageDilation;
            }

            return averageAverageDilation / Mathf.Min(data.Count, 5);
        }
    }

    public float CondAverageBrightness
    {
        get
        {
            var first = Mathf.Max(data.Count - 5, 0);

            var averageAverageBrightness = 0f;
            for (int i = first; i < data.Count; i++)
            {
                averageAverageBrightness += data[i].brightness;
            }

            return averageAverageBrightness / Mathf.Min(data.Count, 5);
        }
    }

    public double[] ConfidenceInterval
    {
        get
        {
            var dilation15 = fifteenRecorder.CondAverageDilation;
            var lowerBound = dilation15 - dilation15 * 0.05;
            var upperBound = dilation15 + dilation15 * 0.05;

            double[] confInterval = { lowerBound, upperBound };
            return confInterval;
        }
    }

    /*
     * Unity Callbacks
     */

    void FixedUpdate()
    {
        if (recording)
		{
            data.Last().dilation.Add(EyeGaze.GetPupilDilation());

            if (data.Last().dilation.Count == recordTime * 50)
			{
                RandomActivator.on = false;
                recording = false;
                PrintInfo();

                resting = true;
                Invoke(nameof(Print20SecondWarning), 20);
			}
		}
    }

    void OnApplicationQuit()
	{

	}

    /*
     * Public Methods
     */

    public float CalculateBrightness()
	{
        if (Run == 0)
		{
            return 15f / fov;
		}

        var dilation15 = fifteenRecorder.CondAverageDilation;
        var ratio = LastDilation / dilation15;

        return LastBrightness * ratio;
	}

    public void StartRecording(float customBrightness = -1)
	{
        // return early if already recording or still resting
        if (recording || resting)
            return;

        if (customBrightness > 0)
        {
            Brightness = customBrightness;
        }
        else
		{
            Brightness = CalculateBrightness();
		}

        data.Add(new Data { run = ++Run, fov = fov, brightness = Brightness, startTime = Time.time });

        if (fov < 47)
        {
            Implant.brightness = Mathf.Clamp(Brightness, 0, 1);
            RandomActivator.brightness = 1;
        }
        else
        {
            Implant.brightness = 1;
            RandomActivator.brightness = Mathf.Clamp(Brightness, 0, 1);
		}

        RandomActivator.on = true;
        recording = true;
	}

    public void PrintInfo()
	{
        Debug.Log($"Fov: {fov}   |   Run: {Run}   |     Brt: {LastBrightness:N3}   |  CurDil: {LastDilation:N3}  |  AveDil: {CondAverageDilation:N3}  | D15: {fifteenRecorder.CondAverageDilation:N3}   |   ConfIval: {ConfidenceInterval[0]:N3}, {ConfidenceInterval[1]:N3}  |   Sug: {CalculateBrightness():N3}");
	}

    public void Print20SecondWarning()
	{
        Debug.Log("You may start the next exposure.");
        resting = false;
    }

    /*
     * Private classes
     */

    public class Data
	{
        public int run              = 1;
        public int fov              = 15;
        public float brightness     = 1;
        public List<float> dilation = new List<float>();

        public float startTime;

        public float AverageDilation => /*dilation.Average()*/BetterAverageDilation;

        public float BetterAverageDilation
		{
            get
            {
                var dilationCopy = new List<float>(dilation);

                dilationCopy.RemoveRange(0, 250);
                dilationCopy.RemoveAll((d) => d < 0);

                return dilationCopy.Average();
			}
		}
    }
}
