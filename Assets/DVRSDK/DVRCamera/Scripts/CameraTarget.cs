using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.DVRCamera
{
    public class CameraTarget : MonoBehaviour
    {
        [NonSerialized]
        public float FOV;
        [NonSerialized]
        public int CullingMask;

        private ScriptCamera[] scriptCameras;

        private void Awake()
        {
            var camera = GetComponent<Camera>();
            FOV = camera.fieldOfView;
            CullingMask = camera.cullingMask;
            camera.enabled = false;
            var audioListener = GetComponent<AudioListener>();
            if (audioListener != null) audioListener.enabled = false;
            scriptCameras = GetComponents<ScriptCamera>();
        }

        public void Play()
        {
            foreach (var scriptCamera in scriptCameras)
            {
                scriptCamera.Play();
            }
        }

        public void Stop()
        {
            foreach (var scriptCamera in scriptCameras)
            {
                scriptCamera.Stop();
            }
        }
    }
}
