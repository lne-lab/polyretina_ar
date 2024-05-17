using System;
using System.Collections.Generic;
using UnityEngine;

using LNE.IO;
using LNE.ProstheticVision;
using LNE.UI.Attributes;

public enum FieldOfView { _20, _45, Full };

public class HeadGazeTracker : MonoBehaviour
{
	[SerializeField, Path]
	private string _filePath = "";

	[SerializeField]
	private string _participantId = "";

	[SerializeField]
	public int _trialId = 0;

	[SerializeField]
	private FieldOfView _fieldOfView = FieldOfView._45;

	[SerializeField]
	private StereoTargetEyeMask _targetEye = StereoTargetEyeMask.Right;

	[SerializeField]
	private Tracker _headset = default;

	[SerializeField]
	private Tracker _roomTracker1 = default;

	[SerializeField]
	private Tracker _roomTracker2 = default;

	[SerializeField]
	private Tracker _torsoTracker = default;

	[SerializeField]
	private Tracker _postTracker = default;

	[SerializeField]
	private Tracker _atmTracker = default;

	[SerializeField]
	private Tracker _marketTracker = default;

	[SerializeField]
	private Tracker _crossingTracker1 = default;

	[SerializeField]
	private Tracker _crossingTracker2 = default;


	private CSV savefile;
	private int frameId;

	private EpiretinalImplant implant => (Prosthesis.Instance.Implant as EpiretinalImplant);

	void Start()
	{
		savefile = new CSV();
		savefile.AppendRow("participantid", "trialid", "fieldofview", "targeteye", "frameid", "date", "time",
			"headposx", "headposy", "headposz",
			"headrotx", "headroty", "headrotz", "headrotw",
			"gazex", "gazey", "gazez", "dilation",
			"roomtracker1x", "roomtracker1y", "roomtracker1z",
			"roomtracker2x", "roomtracker2y", "roomtracker2z",
			"torsotrackerx", "torsotrackery", "torsotrackerz",
			"torsotrackerrotx", "torsotrackerroty", "torsotrackerrotz", "torsotrackerrotw",
			"posttrackerx", "posttrackery", "posttrackerz",
			"atmtrackerx", "atmtrackery", "atmtrackerz",
			"marketTrackerX", "marketTrackerY", "marketTrackerZ",
			"crossingTracker1X", "crossingTracker1Y", "crossingTracker1Z",
			"crossingTracker2X", "crossingTracker2Y", "crossingTracker2Z",
			"cGazeConvergDist",
			"cGazeConvergDistVal",
			"cGazeValidataBitMask",
			"cGazeOpenness",
			"cGazeDirectionNormalisedX", "cGazeDirectionNormalisedY", "cGazeDirectionNormalisedZ",
			"cGazeOriginX", "cGazeOriginY", "cGazeOriginZ",
			"cGazePupilDiam",
			"cGazePupilPositionSensorAreaX", "cGazePupilPositionSensorAreaY",
			"rGazeValidataBitMask",
			"rGazeOpenness",
			"rGazeDirectionNormalisedX", "rGazeDirectionNormalisedY", "rGazeDirectionNormalisedZ",
			"rGazeOriginX", "rGazeOriginY", "rGazeOriginZ",
			"rGazePupilDiam",
			"rGazePupilPositionSensorAreaX", "rGazePupilPositionSensorAreaY",
			"lGazeValidataBitMask",
			"lGazeOpenness",
			"lGazeDirectionNormalisedX", "lGazeDirectionNormalisedY", "lGazeDirectionNormalisedZ",
			"lGazeOriginX", "lGazeOriginY", "lGazeOriginZ",
			"lGazePupilDiam",
			"lGazePupilPositionSensorAreaX", "lGazePupilPositionSensorAreaY"
			);

		frameId = 0;

		switch (_fieldOfView)
		{
			case FieldOfView._20:	implant.fieldOfView = 20;	break;
			case FieldOfView._45:	implant.fieldOfView = 45;	break;
			case FieldOfView.Full:	implant.fieldOfView = 110;	break;
		}

		implant.targetEye = _targetEye;
	}

	void FixedUpdate()
	{
		var date = DateTime.Now.ToString("dd.MM.yy HH:mm:ss:FF");
		var ray = EyeGaze.GetViveProRay();
		var rotation = _headset.transform.rotation;
		var position = _headset.transform.position;

		// Retreive all gaze variables I can get
		var cGazeConvergDist = EyeGaze.eyeData.verbose_data.combined.convergence_distance_mm;
		var cGazeConvergDistVal = EyeGaze.eyeData.verbose_data.combined.convergence_distance_validity;
		var cGazeValidataBitMask = EyeGaze.eyeData.verbose_data.combined.eye_data.eye_data_validata_bit_mask;
		var cGazeOpenness = EyeGaze.eyeData.verbose_data.combined.eye_data.eye_openness;
		var cGazeDirectionNormalisedX = EyeGaze.eyeData.verbose_data.combined.eye_data.gaze_direction_normalized.x;
		var cGazeDirectionNormalisedY = EyeGaze.eyeData.verbose_data.combined.eye_data.gaze_direction_normalized.y;
		var cGazeDirectionNormalisedZ = EyeGaze.eyeData.verbose_data.combined.eye_data.gaze_direction_normalized.z;
		var cGazeOriginX = EyeGaze.eyeData.verbose_data.combined.eye_data.gaze_origin_mm.x;
		var cGazeOriginY = EyeGaze.eyeData.verbose_data.combined.eye_data.gaze_origin_mm.y;
		var cGazeOriginZ = EyeGaze.eyeData.verbose_data.combined.eye_data.gaze_origin_mm.z;
		var cGazePupilDiam = EyeGaze.eyeData.verbose_data.combined.eye_data.pupil_diameter_mm;
		var cGazePupilPositionSensorAreaX = EyeGaze.eyeData.verbose_data.combined.eye_data.pupil_position_in_sensor_area.x;
		var cGazePupilPositionSensorAreaY = EyeGaze.eyeData.verbose_data.combined.eye_data.pupil_position_in_sensor_area.y;

		var rGazeValidataBitMask = EyeGaze.eyeData.verbose_data.right.eye_data_validata_bit_mask;
		var rGazeOpenness = EyeGaze.eyeData.verbose_data.right.eye_openness;
		var rGazeDirectionNormalisedX = EyeGaze.eyeData.verbose_data.right.gaze_direction_normalized.x;
		var rGazeDirectionNormalisedY = EyeGaze.eyeData.verbose_data.right.gaze_direction_normalized.y;
		var rGazeDirectionNormalisedZ = EyeGaze.eyeData.verbose_data.right.gaze_direction_normalized.z;
		var rGazeOriginX = EyeGaze.eyeData.verbose_data.right.gaze_origin_mm.x;
		var rGazeOriginY = EyeGaze.eyeData.verbose_data.right.gaze_origin_mm.y;
		var rGazeOriginZ = EyeGaze.eyeData.verbose_data.right.gaze_origin_mm.z;
		var rGazePupilDiam = EyeGaze.eyeData.verbose_data.right.pupil_diameter_mm;
		var rGazePupilPositionSensorAreaX = EyeGaze.eyeData.verbose_data.right.pupil_position_in_sensor_area.x;
		var rGazePupilPositionSensorAreaY = EyeGaze.eyeData.verbose_data.right.pupil_position_in_sensor_area.y;

		var lGazeValidataBitMask = EyeGaze.eyeData.verbose_data.left.eye_data_validata_bit_mask;
		var lGazeOpenness = EyeGaze.eyeData.verbose_data.left.eye_openness;
		var lGazeDirectionNormalisedX = EyeGaze.eyeData.verbose_data.left.gaze_direction_normalized.x;
		var lGazeDirectionNormalisedY = EyeGaze.eyeData.verbose_data.left.gaze_direction_normalized.y;
		var lGazeDirectionNormalisedZ = EyeGaze.eyeData.verbose_data.left.gaze_direction_normalized.z;
		var lGazeOriginX = EyeGaze.eyeData.verbose_data.left.gaze_origin_mm.x;
		var lGazeOriginY = EyeGaze.eyeData.verbose_data.left.gaze_origin_mm.y;
		var lGazeOriginZ = EyeGaze.eyeData.verbose_data.left.gaze_origin_mm.z;
		var lGazePupilDiam = EyeGaze.eyeData.verbose_data.left.pupil_diameter_mm;
		var lGazePupilPositionSensorAreaX = EyeGaze.eyeData.verbose_data.left.pupil_position_in_sensor_area.x;
		var lGazePupilPositionSensorAreaY = EyeGaze.eyeData.verbose_data.left.pupil_position_in_sensor_area.y;

		savefile.AppendRow(
			_participantId,
			_trialId,
			_fieldOfView,
			_targetEye,
			frameId++,
			date,
			Time.time,
			position.x, position.y, position.z,
			rotation.x, rotation.y, rotation.z, rotation.w,
			ray.direction.x, ray.direction.y, ray.direction.z,
			EyeGaze.GetPupilDilation(),
			_roomTracker1.transform.position.x, _roomTracker1.transform.position.y, _roomTracker1.transform.position.z,
			_roomTracker2.transform.position.x, _roomTracker2.transform.position.y, _roomTracker2.transform.position.z,
			_torsoTracker.transform.position.x, _torsoTracker.transform.position.y, _torsoTracker.transform.position.z,
			_torsoTracker.transform.rotation.x, _torsoTracker.transform.rotation.y, _torsoTracker.transform.rotation.z, _torsoTracker.transform.rotation.w,
			_postTracker.transform.position.x, _postTracker.transform.position.y, _postTracker.transform.position.z,
			_atmTracker.transform.position.x, _atmTracker.transform.position.y, _atmTracker.transform.position.z,
			_marketTracker.transform.position.x, _marketTracker.transform.position.y, _marketTracker.transform.position.z,
			_crossingTracker1.transform.position.x, _crossingTracker1.transform.position.y, _crossingTracker1.transform.position.z,
			_crossingTracker2.transform.position.x, _crossingTracker2.transform.position.y, _crossingTracker2.transform.position.z,
			cGazeConvergDist,
			cGazeConvergDistVal,
			cGazeValidataBitMask,
			cGazeOpenness,
			cGazeDirectionNormalisedX, cGazeDirectionNormalisedY, cGazeDirectionNormalisedZ,
			cGazeOriginX, cGazeOriginY, cGazeOriginZ,
			cGazePupilDiam,
			cGazePupilPositionSensorAreaX, cGazePupilPositionSensorAreaY,
			rGazeValidataBitMask,
			rGazeOpenness,
			rGazeDirectionNormalisedX, rGazeDirectionNormalisedY, rGazeDirectionNormalisedZ,
			rGazeOriginX, rGazeOriginY, rGazeOriginZ,
			rGazePupilDiam,
			rGazePupilPositionSensorAreaX, rGazePupilPositionSensorAreaY,
			lGazeValidataBitMask,
			lGazeOpenness,
			lGazeDirectionNormalisedX, lGazeDirectionNormalisedY, lGazeDirectionNormalisedZ,
			lGazeOriginX, lGazeOriginY, lGazeOriginZ,
			lGazePupilDiam,
			lGazePupilPositionSensorAreaX, lGazePupilPositionSensorAreaY
		);
	}

	void OnApplicationQuit()
	{
		SaveAndFlush();
	}

	public void SaveAndFlush()
	{
		savefile.SaveWStream(_filePath + $"{_participantId}_{_fieldOfView}_{_trialId}.csv");
		savefile.Clear();
	}
}
