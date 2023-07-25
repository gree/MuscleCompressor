using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.DVRCamera
{
    public class ZoomingCamera : ScriptCamera
    {
        [Range(0,100)]
        public float StartZoomValue = 90f;
        [Range(0, 100)]
        public float EndZoomValue = 40f;
        public float TotalTime = 5f;

        private bool isPlaying = false;
        private float currentTime;
        private CameraTarget cameraTarget;
        private float previousFov;
        public override void Update()
        {
            if (isPlaying)
            {
                currentTime += Time.deltaTime;
                var ratio = currentTime / TotalTime;
                cameraTarget.FOV = Mathf.LerpUnclamped(StartZoomValue, EndZoomValue, ratio);

                if (ratio >= 1.0f)
                {
                    isPlaying = false;
                }
            }
        }

        public override void Play()
        {
            cameraTarget = base.gameObject.GetComponent<CameraTarget>();
            previousFov = cameraTarget.FOV;
            currentTime = 0f;
            isPlaying = true;
        }

        public override void Stop()
        {
            isPlaying = false;
            cameraTarget.FOV = previousFov;
        }
    }
}
