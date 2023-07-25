using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.DVRCamera
{
    public class Smoother : ScriptCamera
    {
        [SerializeField, Range(0, 1)]
        private float linear = 0.5f;
        private Vector3 previousPosition;
        private Quaternion previousRotation;

        public override void LateUpdate()
        {
            Vector3 newPosition = transform.position;
            Quaternion newRotation = transform.rotation;

            if (previousRotation != null)
            {
                newPosition = Vector3.Lerp(previousPosition, newPosition, linear);
                newRotation = Quaternion.Lerp(previousRotation, newRotation, linear);

                transform.position = newPosition;
                transform.rotation = newRotation;
            }

            previousPosition = newPosition;
            previousRotation = newRotation;
        }
    }
}
