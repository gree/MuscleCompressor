using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace DVRSDK.DVRCamera
{
    [Serializable]
    public class CameraSetting
    {
        public Rect ViewPosition = new Rect(0, 0, 1, 1);
        [NonSerialized]
        public ViewCamera ViewCamera;
        public CameraTarget CameraTarget;
#if UNITY_POST_PROCESSING_STACK_V2
        private FieldInfo[] PostProcessLayerFields = typeof(PostProcessLayer).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif

        public void SetTargetCamera()
        {
            if (ViewCamera != null && CameraTarget != null)
            {
                ViewCamera.Target = CameraTarget;

#if UNITY_POST_PROCESSING_STACK_V2
                if (CameraTarget.TryGetComponent<PostProcessLayer>(out var srcPostProcessLayer))
                {
                    PostProcessLayer destPostProcessLayer;
                    if (!ViewCamera.gameObject.TryGetComponent(out destPostProcessLayer))
                        destPostProcessLayer = ViewCamera.gameObject.AddComponent<PostProcessLayer>();

                    foreach (var field in PostProcessLayerFields)
                    {
                        if ((field.IsPublic && !field.IsDefined(typeof(NonSerializedAttribute))) ||
                            field.IsDefined(typeof(SerializeField)))
                            field.SetValue(destPostProcessLayer, field.GetValue(srcPostProcessLayer));
                    }

                    destPostProcessLayer.volumeTrigger = ViewCamera.transform;
                }
                else
                {
                    UnityEngine.Object.Destroy(ViewCamera.Camera.gameObject.GetComponent<PostProcessLayer>());
                }
#endif

                var camera = ViewCamera.Camera;
                camera.rect = ViewPosition;
                camera.cullingMask = CameraTarget.CullingMask;
                camera.enabled = true;
                CameraTarget.Play();
            }
        }

        public void Stop()
        {
            if (CameraTarget != null)
            {
                CameraTarget.Stop();
            }
        }
    }
}
