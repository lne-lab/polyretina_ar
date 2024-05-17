using UnityEngine;
using UnityEngine.UI;

using LNE.IO;
using LNE.ProstheticVision;

public class CalibrationManager : MonoBehaviour
{
    [SerializeField]
    private DilationCalibrator2[] calibrators = null;

    [SerializeField]
    private Dropdown visionIndex = null;

    [SerializeField]
    private InputField brightnessInput = null;

    [SerializeField]
    private Material crosshairMat = null;

    private int index => visionIndex.value;

    private DilationCalibrator2 Active => calibrators[index];

    void Start()
	{
        Active.Print20SecondWarning();
	}

	//void Update()
	//{
	//	if (Input.GetKeyDown(KeyCode.Return))
	//	{
	//		var useCustom = float.TryParse(brightnessInput.text, out float brightness);
	//		Active.StartRecording(useCustom ? brightness : -1);

	//		brightnessInput.text = "";
	//	}

	//	if (Input.GetKeyDown(KeyCode.Space))
	//	{
	//		Active.StartRecording(1);
	//	}
	//}

	void OnApplicationQuit()
    {
        // full data set
        var csv_full = new CSV();
        csv_full.AppendRow("fov", "run", "brightness", "dilation");

        foreach (var calibrator in calibrators)
        {
            foreach (var result in calibrator.Results)
            {
                foreach (var dilation in result.dilation)
                {
                    csv_full.AppendRow(result.fov, result.run, result.brightness, dilation);
                }
            }
        }

        csv_full.SaveWStream(LNE.UnityApp.ProjectPath + "dilation_full.csv");

        // averaged data
        var csv_ave = new CSV();
        csv_ave.AppendRow("fov", "run", "brightness", "dilation", "startTime");

        foreach (var calibrator in calibrators)
        {
            foreach (var result in calibrator.Results)
            {
                csv_ave.AppendRow(result.fov, result.run, result.brightness, result.AverageDilation, result.startTime);
            }
        }

        csv_ave.SaveWStream(LNE.UnityApp.ProjectPath + "dilation_ave.csv");


        // averaged data for each condition
        var csv_ave_cond = new CSV();
        csv_ave_cond.AppendRow("fov", "run", "brightness", "dilation");

        foreach (var calibrator in calibrators)
        {
            csv_ave_cond.AppendRow(
                calibrator.Results[0].fov, 
                calibrator.CondAverageBrightness, 
                calibrator.CondAverageDilation
            );
        }

        csv_ave_cond.SaveWStream(LNE.UnityApp.ProjectPath + "dilation_ave_cond.csv");
    }

    public void StartRecordingCustom()
	{
        var useCustom = float.TryParse(brightnessInput.text, out float brightness);
        brightnessInput.text = "";

        if (useCustom == false)
        {
            brightness = Active.CalculateBrightness();
        }

        LNE.Threading.CallbackManager.InvokeOnce(1, () => { Active.StartRecording(brightness); });

        // set crosshair colour
        crosshairMat.SetColor("_colour", new Color(brightness, brightness, brightness, 1));

        // set crosshair colour to black after ~20 seconds
        LNE.Threading.CallbackManager.InvokeOnce(Active.RecordTime+1, () => {
            crosshairMat.SetColor("_colour", new Color(0, 0, 0, 1));
        });
    }

    public void StartRecordingFullBrightness()
    {
        LNE.Threading.CallbackManager.InvokeOnce(1, () => { Active.StartRecording(1); });

        // set crosshair colour
        crosshairMat.SetColor("_colour", new Color(1, 1, 1, 1));

        // set crosshair colour to black after ~20 seconds
        LNE.Threading.CallbackManager.InvokeOnce(Active.RecordTime + 1, () => {
            crosshairMat.SetColor("_colour", new Color(0, 0, 0, 1));
        });
    }

	public void UpdateFieldOfView()
	{
        if (index < 4)
        {
            (Prosthesis.Instance.Implant as EpiretinalImplant).on = true;
            (Prosthesis.Instance.Implant as EpiretinalImplant).fieldOfView = new[] { 15, 25, 35, 45 }[index];

            (Prosthesis.Instance.ExternalProcessor as RandomActivator).interrupt = true;
        }
        else
        {
            (Prosthesis.Instance.Implant as EpiretinalImplant).on = false;

            (Prosthesis.Instance.ExternalProcessor as RandomActivator).interrupt = false;
        }
	}
}
