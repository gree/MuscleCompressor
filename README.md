# MuscleCompressor

# English
- This project can handle Unity Humanoid animations externally using [muscle](https://docs.unity3d.com/2017.4/Documentation/Manual/MuscleDefinitions.html) instead of the conventional .anim.
- It includes a sample scene for PCVR that works as a motion capture that actually saves and reads out the motion when wearing VR equipment such as Meta Quest.
- Data size comparison of 1 minute of Humanoid recording at 60 fps; MuscleCompressor format is lighter than existing formats (up to 96% compression for .anim)

![image](https://github.com/gree/MuscleCompressor/blob/readme-imgs/imgs/MuscleCompressor.png?raw=true)



## Environment
- Unity 2021.3.19f1
- [Oculus Integration 53.1](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) (Separate import)
- [Final IK](https://assetstore.unity.com/?q=Final%20IK&orderBy=1) (Separate import)
- Windows 10 x64
- Meta Quest 2

## How to use (Motion Recording)

https://github.com/gree/MuscleCompressor/assets/5110708/8520bcc3-f59d-43ad-b7a9-d236f2846591

1. Open this project with Unity 2021.3.19 and import Oculus Integration and Final IK from Package Manager
2. Go to Assets/VRStudioLab/Scenes/RecordSceneSample.unity and add Recording Datas in Recording Manager
3. Play the scene wearing VR Device
4. After an avatar is loaded, push the READY button. The motion recorded for Duration seconds is saved in Assets/StreamingAssets/Motion

## How to use (Motion Loading)

https://github.com/gree/MuscleCompressor/assets/5110708/b8c7f5ca-2493-4c2c-a33a-3b40ab4ab0d9

### Runtime load
1. (Open this project with Unity 2021.3.19 and import Oculus Integration and Final IKfrom Package Manager)
2. Go to Assets/VRStudioLab/Scenes/LoadSceneSample.unity and play the scene
3. After click the button "Load Motion", a file browser will open. Go to the motion strage location (Assets/StreamingAssets/Motion) and select .data file.
4. Motion is applied to the avatar in the scene

Sample motions are included in Assets/StreamingAssets/Motion for testing.

### Convert to animation clips
1. (Open this project with Unity 2021.3.19 and import Oculus Integration and Final IK from Package Manager)
2. Click VRStudioLab>Bytes2Anim in the menu at the top of Unity
3. A file browser will open. Go to the motion strage location (Assets/StreamingAssets/Motion) and select .data file.
6. Converted clip is saved in Assets/VRStudioLab/Animations


