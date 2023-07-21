using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.DVRCamera
{
    public class ObjectTrackingCamera : ScriptCamera
    {
        [SerializeField]
        private Transform TargetTransform;

        private bool isPlaying = false;

        public override void Update()
        {
            if (isPlaying)
            {
                if (TargetTransform != null)
                {
                    transform.LookAt(TargetTransform);
                }
            }
        }

        public override void Play()
        {
            isPlaying = true;
        }

        public override void Stop()
        {
            isPlaying = false;
        }
    }
}
