using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LNE.IO;
using LNE.UI.Attributes;

public class TrackerLocationTest : MonoBehaviour
{
    [SerializeField, Path(isFile=true)]
    private string _filePath = "";

    [SerializeField]
    private Transform head = default;

    [SerializeField]
    private Transform room1 = default;

    [SerializeField]
    private Transform room2 = default;

    [SerializeField]
    private Transform torso = default;

    [SerializeField]
    private Transform atm = default;

    [SerializeField]
    private Transform post = default;

    [SerializeField]
    private Transform market = default;

    [SerializeField]
    private Transform crossing1 = default;

    [SerializeField]
    private Transform crossing2 = default;

    private CSV trackerDataFile;

    private int frameCounter;

    // Start is called before the first frame update
    void Start()
    {
        trackerDataFile = new CSV();
        trackerDataFile.Separator = ';';
        trackerDataFile.LoadWStream(_filePath);

        frameCounter = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (frameCounter > trackerDataFile.Height-2)
            return;

        var headPosX = trackerDataFile.GetCell<float>("HeadPosX", frameCounter);
        var headPosY = trackerDataFile.GetCell<float>("HeadPosY", frameCounter);
        var headPosZ = trackerDataFile.GetCell<float>("HeadPosZ", frameCounter);
        var room1X = trackerDataFile.GetCell<float>("roomTracker1X", frameCounter);
        var room1Y = trackerDataFile.GetCell<float>("roomTracker1Y", frameCounter);
        var room1Z = trackerDataFile.GetCell<float>("roomTracker1Z", frameCounter);
        var room2X = trackerDataFile.GetCell<float>("roomTracker2X", frameCounter);
        var room2Y = trackerDataFile.GetCell<float>("roomTracker2Y", frameCounter);
        var room2Z = trackerDataFile.GetCell<float>("roomTracker2Z", frameCounter);
        var torsoX = trackerDataFile.GetCell<float>("torsoTrackerX", frameCounter);
        var torsoY = trackerDataFile.GetCell<float>("torsoTrackerY", frameCounter);
        var torsoZ = trackerDataFile.GetCell<float>("torsoTrackerZ", frameCounter);
        var atmX = trackerDataFile.GetCell<float>("atmTrackerX", frameCounter);
        var atmY = trackerDataFile.GetCell<float>("atmTrackerY", frameCounter);
        var atmZ = trackerDataFile.GetCell<float>("atmTrackerZ", frameCounter);
        var postX = trackerDataFile.GetCell<float>("postTrackerX", frameCounter);
        var postY = trackerDataFile.GetCell<float>("postTrackerY", frameCounter);
        var postZ = trackerDataFile.GetCell<float>("postTrackerZ", frameCounter);
        var marketX = trackerDataFile.GetCell<float>("marketTrackerX", frameCounter);
        var marketY = trackerDataFile.GetCell<float>("marketTrackerY", frameCounter);
        var marketZ = trackerDataFile.GetCell<float>("marketTrackerZ", frameCounter);
        var crossing1X = trackerDataFile.GetCell<float>("crossingTracker1X", frameCounter);
        var crossing1Y = trackerDataFile.GetCell<float>("crossingTracker1Y", frameCounter);
        var crossing1Z = trackerDataFile.GetCell<float>("crossingTracker1Z", frameCounter);
        var crossing2X = trackerDataFile.GetCell<float>("crossingTracker2X", frameCounter);
        var crossing2Y = trackerDataFile.GetCell<float>("crossingTracker2Y", frameCounter);
        var crossing2Z = trackerDataFile.GetCell<float>("crossingTracker2Z", frameCounter);

        head.position = new Vector3(headPosX, headPosY, headPosZ);
        room1.position = new Vector3(room1X, room1Y, room1Z);
        room2.position = new Vector3(room2X, room2Y, room2Z);
        torso.position = new Vector3(torsoX, torsoY, torsoZ);
        atm.position = new Vector3(atmX, atmY, atmZ);
        post.position = new Vector3(postX, postY, postZ);
        market.position = new Vector3(marketX, marketY, marketZ);
        crossing1.position = new Vector3(crossing1X, crossing1Y, crossing1Z);
        crossing2.position = new Vector3(crossing2X, crossing2Y, crossing2Z);

        frameCounter++;
    }
}
