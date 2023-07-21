using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.DVRCamera
{
    public class MovingCamera : ScriptCamera
    {
        public Transform[] Transforms;
        public bool Loop = true;
        public bool ApplyPosition = true;
        public bool ApplyRotation = true;
        public float TotalTime = 5f;

        private bool isPlaying = false;
        private int CurrentIndex = 0;
        private float progress = 0f;
        private float allDistance;

        private void Init()
        {
            progress = 0.0f;
            CurrentIndex = 0;
            allDistance = 0f;
            ResetPosition();

            for (int i = 0; i < Transforms.Length - 1; i++)
            {
                allDistance += Vector3.Distance(Transforms[i].position, Transforms[i + 1].position);

                var camera = Transforms[i].GetComponent<Camera>();
                if(camera != null) camera.enabled = false;
            }
        }

        public override void Update()
        {
            if (isPlaying)
            {
                if (Transforms != null)
                {
                    var moveDistance = allDistance / TotalTime * Time.deltaTime;

                    var targetVector = Transforms[CurrentIndex + 1].position - transform.position;
                    var direction = targetVector / targetVector.magnitude;

                    var targetDistance = Vector3.Distance(Transforms[CurrentIndex].position, Transforms[CurrentIndex + 1].position);

                    progress += moveDistance;
                    if (progress >= targetDistance)
                    {
                        ++CurrentIndex;
                        progress = 0f;

                        if (CurrentIndex >= Transforms.Length - 1)
                        {
                            if (!Loop)
                            {
                                isPlaying = false;
                                return;
                            }

                            CurrentIndex = 0;
                            ResetPosition();
                        }
                    }

                    if (ApplyPosition) transform.position = transform.position + direction * moveDistance;
                    if (ApplyRotation) transform.rotation = Quaternion.LerpUnclamped(Transforms[CurrentIndex].rotation, Transforms[CurrentIndex + 1].rotation, progress / targetDistance);
                }
            }
        }

        public override void Play()
        {
            Init();
            isPlaying = true;
        }

        public override void Stop()
        {
            isPlaying = false;
        }

        private void ResetPosition()
        {
            if (ApplyPosition) transform.position = Transforms[0].position;
            if (ApplyRotation) transform.rotation = Transforms[0].rotation;
        }
    }
}
