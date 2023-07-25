using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.DVRCamera
{
    [DefaultExecutionOrder(-1)]
    public class CameraShake : ScriptCamera
    {
        public float NoiseSpeed = 0.6f;
        public float NoiseCoeff = 1.4f;

        private bool isPlaying = false;
        private Quaternion targetRotation;

        public override void Update()
        {
            transform.rotation = targetRotation;
        }

        public override void LateUpdate()
        {
            if (isPlaying)
            {
                var t = Time.time * NoiseSpeed;
                var nx = Mathf.PerlinNoise(t, t + 5.0f) * NoiseCoeff;
                var ny = Mathf.PerlinNoise(t + 10.0f, t + 15.0f) * NoiseCoeff;
                var nz = Mathf.PerlinNoise(t + 25.0f, t + 20.0f) * NoiseCoeff * 0.5f;
                var noise = new Vector3(nx, ny, nz);

                var noiseRot = Quaternion.Euler(noise.x, noise.y, noise.z);
                transform.rotation = transform.rotation * noiseRot;
            }
        }

        public override void Play()
        {
            isPlaying = true;
            targetRotation = transform.rotation;
        }

        public override void Stop()
        {
            isPlaying = false;
        }
    }
}
