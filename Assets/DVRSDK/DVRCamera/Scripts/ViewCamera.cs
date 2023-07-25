using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.DVRCamera
{
    public class ViewCamera : MonoBehaviour
    {
        [NonSerialized]
        public CameraTarget Target;
        [NonSerialized]
        public Camera Camera;

        private Camera PreviousCamera;

        private bool isFadeing = false;
        private float FadeTime;
        private float progress = 0f;

        public void Awake()
        {
            //GetComponent<AudioListener>().enabled = false;
            Camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (isFadeing)
            {
                var distance = Vector3.Distance(PreviousCamera.transform.position, Camera.transform.position);

                var moveDistance = distance / FadeTime * Time.deltaTime;

                var targetVector = Camera.transform.position - transform.position;
                var direction = targetVector / targetVector.magnitude;

                progress += moveDistance;
                if (progress >= distance)
                {
                    progress = 0f;
                    isFadeing = false;
                    return;
                }

                transform.position = transform.position + direction * moveDistance;
                transform.rotation = Quaternion.Lerp(PreviousCamera.transform.rotation, Camera.transform.rotation, progress / distance);
            }
            else
            {
                if (Target != null)
                {
                    transform.position = Target.transform.position;
                    transform.rotation = Target.transform.rotation;
                    Camera.fieldOfView = Target.FOV;
                    Camera.cullingMask = Target.CullingMask;
                }
            }
        }

        public void StartFade(float fadeTime)
        {
            PreviousCamera = Camera;
            FadeTime = fadeTime;
            progress = 0f;
            isFadeing = true;
        }
    }
}
