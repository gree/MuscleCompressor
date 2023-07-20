# MuscleCompressor

# English
- This project can handle Unity Humanoid animations externally using Muscle instead of the conventional .anim.
- It includes a sample scene that works as a motion capture that actually saves and reads out the motion when wearing VR equipment such as Meta Quest.
- Data size comparison of 1 minute of Humanoid recording at 60 fps; MuscleCompressor format is lighter than existing formats (up to 96% compression for .anim)

![image](https://github.com/gree/MuscleCompressor/blob/readme-imgs/imgs/MuscleCompressor.png?raw=true)



## Environment
- Unity 2021.3.19f1
- Oculus Integration 53.1 (Separate import)
- Final IK (Separate import)

## How to use (Motion Recording)

https://github.com/gree/MuscleCompressor/raw/readme-imgs/imgs/Muscle-Recording.mp4

https://github.com/reality-inc/MuscleCompressor/assets/79638033/ea09b26e-78f9-4207-9065-9bfe0bf4d212

1. Open this project with Unity 2021.3.19 and import Oculus Integration and Final IK
2. Go to Assets/VRStudioLab/Scenes/RecordSceneSample.unity and add Recording Datas in Recording Manager
3. Play the scene wearing VR Device
4. After an avatar is loaded, push the READY button. The motion recorded for Duration seconds is saved in Assets/StreamingAssets/Motion

## How to use (Motion Loading)

https://github.com/gree/MuscleCompressor/raw/readme-imgs/imgs/MuscleLoading.mp4

### Runtime load
1. (Open this project with Unity 2021.3.19 and import Oculus Integration and Final IK)
2. Go to Assets/VRStudioLab/Scenes/LoadSceneSample.unity and play the scene
3. After click the button "Load Motion", a file browser will open. Go to the motion strage location (Assets/StreamingAssets/Motion) and select .data file.
4. Motion is applied to the avatar in the scene

### Convert to animation clips
1. (Open this project with Unity 2021.3.19 and import Oculus Integration and Final IK)
2. Click VRStudioLab>Bytes2Anim in the menu at the top of Unity
3. A file browser will open. Go to the motion strage location (Assets/StreamingAssets/Motion) and select .data file.
6. Converted clip is saved in Assets/VRStudioLab/Animations


