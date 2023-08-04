# MuscleCompressor

- This project can handle Unity Humanoid animations externally using [muscle](https://docs.unity3d.com/2017.4/Documentation/Manual/MuscleDefinitions.html) instead of the conventional .anim.
- It includes a sample scene for PCVR that works as a QueTra-based motion capture that actually reads out and saves the motion when wearing VR equipment such as Meta Quest.
- Data size comparison of 1 minute of Humanoid recording at 60 fps; MuscleCompressor format is lighter than existing formats (up to 96% compression for .anim)

![image](https://github.com/gree/MuscleCompressor/blob/readme-imgs/imgs/MuscleCompressor.png?raw=true)


## Environment
- Unity 2021.3.19f1
- [Oculus Integration 53.1](https://developer.oculus.com/downloads/package/unity-integration/53.1) (Separate import)
- [Final IK](https://assetstore.unity.com/?q=Final%20IK&orderBy=1) (Separate import)
- Windows 10 x64
- Meta Quest 2

When starting up for the first time, you will be prompted to start up in SafeMode due to the lack of FinalIK and Oculus Integration, but please "Ignore" and proceed.

## How to use (Motion Loading)

https://github.com/gree/MuscleCompressor/assets/5110708/b8c7f5ca-2493-4c2c-a33a-3b40ab4ab0d9

### Runtime load
1. Open this project with Unity 2021.3.19 and import Oculus Integration and Final IKfrom Package Manager
2. Go and open the scene at Assets/VRStudioLab/Scenes/LoadSceneSample.unity
3. If prompted import TMP essentials and extras, then and play the scene
4. After click the button "Load Motion", a file browser will open. Go to the motion strage location (Assets/StreamingAssets/Motion) and select .data file.
5. Motion is applied to the avatar in the scene

Sample motions are included in Assets/StreamingAssets/Motion for testing.

### Convert to animation clips
1. (Open this project with Unity 2021.3.19 and import Oculus Integration and Final IK from Package Manager)
2. Click VRStudioLab>Bytes2Anim in the menu at the top of Unity
3. A file browser will open. Go to the motion storage location (Assets/StreamingAssets/Motion) and select .data file.
4. Converted clip is saved in Assets/VRStudioLab/Animations

## How to use (Motion Recording)

https://github.com/gree/MuscleCompressor/assets/5110708/8520bcc3-f59d-43ad-b7a9-d236f2846591

1. (Open this project with Unity 2021.3.19 and import Oculus Integration and Final IK from Package Manager)
2. Go and open the scene at Assets/VRStudioLab/Scenes/RecordSceneSample.unity
3. If prompted import TMP essentials and extras, then and play the scene
4. Add a Recording Datas in Recording Manager (From this you can control the motion duration and name)
5. Play the scene wearing VR Device
6. After an avatar is loaded, push the READY button. The motion recorded for Duration seconds is saved in Assets/StreamingAssets/Motion

## Citation
This code is extracted from AI-Assisted Avatar Fashion Show, SIGGRAPH 2023 Posters.
```
@inproceedings{10.1145/3588028.3603660,
author = {Kohyama, Kai and Berthault, Alexandre and Kato, Takuma and Shirai, Akihiko},
title = {AI-Assisted Avatar Fashion Show: Word-to-Clothing Texture Exploration and Motion Synthesis for Metaverse UGC},
year = {2023},
isbn = {9798400701528},
publisher = {Association for Computing Machinery},
address = {New York, NY, USA},
url = {https://doi.org/10.1145/3588028.3603660},
doi = {10.1145/3588028.3603660},
booktitle = {ACM SIGGRAPH 2023 Posters},
articleno = {14},
numpages = {2},
location = {Los Angeles, CA, USA},
series = {SIGGRAPH '23}
}
```

