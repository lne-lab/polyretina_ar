# polyretina_ar

This project simulates the artificial vision provided by a POLYRETINA epi-retinal prosthesis using Unity, C# and HLSL/Cg. Simulations can be viewed in real-time in augmented reality (using the VIVE Pro Eye head-mounted display).

## Setup

1. Download Unity (Polyretina AR has only been tested on version 2019.2.16f1).
2. Download Polyretina AR, either by:
   - Downloading the zipped files from Github,
   - Forking the repository, or
   - Downloading the latest release
   
   Downloading the zipped files or forking the repository will give you the whole Unity project which can then be opened using Unity. Downloading the latest release will give you a Unity Package file which can be imported into an existing Unity project.
3. After opening/importing the project, there will be potentially be some warning/error messages. You can ignore them by clicking clear in the console window. They will not appear again.

### Viewing the simulation in the Vive Pro Eye

1. Install the Vive Pro Eye eye-tracking software.
   - Follow the links [here](https://developer.vive.com/resources/knowledgebase/vive-sranipal-sdk/).
   - Run the installer and download the SDK.
   - Unzip the SDK and open **02_Unity/Vive-SRanipal-Unity-Plugin** to import it into the the Unity project.
2. Click on the Polyretina dropdown menu -> Settings.
   - Check the Vive Pro Eye option under Eye Tracking SDKs
   - Check the VRInput Support option
3. Make sure the **SRanipal Eye Framework** GameObject is enabled.
4. Make sure **Virtual Reality Supported** is enabled in Unity's XR Settings.
5. Navigate to Polyretina -> Studies -> Streetlab and run the Streetlab scene.
6. Select the **Remote** resolution from the Game tab for more accurate viewing in the Unity Editor.
7. To control the AR Streetlab Scene, use the control software in Other Stuff -> LSL -> Visualiser -> Streetlab_ControlCenter. Create a build and, use another computer via Wifi to make notes, control and change the simulation during the study.

### Changing Prosthetic Vision parameters

1. Select the **Prosthetic Vision** GameObject in the Hierarchy tab.
2. Scroll down the Prosthesis script in the Inspector tab.
3. Adjust the settings as you see fit (changes made while the simulation is running will not be saved).

## Using your own prosthesis design

1. Add a value to the ElectrodePattern enum (Assets/Polyretina/SPV/Scripts/ElectrodePattern.cs) to represent your design.
2. Add a method to the ElectrodePatternExtensions class (same file) that returns the x, y positions (μm) of each electrode in your design. See Polyretina(ElectrodeLayout layout, float fov) for an example.
3. Add a value to the ElectrodeLayout enum (Assets/Polyretina/SPV/Scripts/ElectrodeLayout.cs) to represent the diameter and pitch of your design.
4. Add cases to the switch statements in the methods ToValue() and ToAnatomicalValue() that return the diameter and pitch of the electrodes in your design.
   - If unsure, then have the ToAnatomicalValue() method return the same values as the ToValue() method.
5. Click on the Poylretina dropdown menu -> Preprocessed Data.
   - Choose your design pattern and layout as well as an output resolution/field of view by choosing a headset model.
   - Click start and choose a location to save the file.
   - Repeat this for both the phosphene and axon data types.
6. Create or duplicate an existing implant.
7. Assign the phosphene and axon data textures to appropriate parts of the Preprocessed Data section in the implant configuration window.

## Using your own External Processors/Implants

External Processors/Implants are explained [here](https://github.com/lne-lab/polyretina_ar/tree/master/Assets/Polyretina/SPV), under **Renderers**.

1. Extended either the ExternalProcessor.cs or Implant.cs base classes, implenting your own image processing logic.
   - Overridable methods include: Start, Update, GetDimensions and OnRenderImage.
   - Add a **CreateAssetMenu** attribute to your class (See [EpiretinalImplant.cs](https://github.com/lne-lab/polyretina_ar/blob/master/Assets/Polyretina/SPV/Scripts/Epiretinal/EpiretinalImplant.cs) as an example).
2. Right click the Project tab -> Create and then follow the menu set in your CreateAssetMenu attribute to create the asset.
4. Add the Prosthesis component to the Scene's camera (if not already done).
5. Assign the asset as either the External Processor or Implant of the Prosthesis component.