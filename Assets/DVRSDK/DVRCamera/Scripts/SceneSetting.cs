using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.DVRCamera
{
    [Serializable]
    public class SceneSetting
    {
        public bool OverrideDefaultSwitchTime;
        public float SwitchTime;
        public bool FadeTransition;
        public float FadeTime;
        public List<CameraSetting> CameraList = new List<CameraSetting>() { new CameraSetting() };

        public void Apply()
        {
            foreach (var camera in CameraManager.Instance.ViewCameras)
            {
                camera.Camera.enabled = false;
            }

            foreach (var cameraSetting in CameraList)
            {
                cameraSetting.SetTargetCamera();
            }
        }

        public void Stop()
        {
            foreach (var cameraSetting in CameraList)
            {
                cameraSetting.Stop();
            }
        }
    }
}
