using DVRSDK.Avatar;
using DVRSDK.Plugins.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

namespace DVRSDK.Avatar
{
    public class VRFaceBlendShapeController : MonoBehaviour
    {
        private FaceController faceController;
        private int lastStickPointIndex = 0;
        private BlendShapePreset[] blendShapePresets = new BlendShapePreset[] { BlendShapePreset.Neutral, BlendShapePreset.Sorrow, BlendShapePreset.Joy, BlendShapePreset.Fun, BlendShapePreset.Angry };

        public void LoadModel(GameObject model)
        {
            faceController = model.GetComponent<FaceController>();
            if (faceController == null)
            {
                faceController = model.AddComponent<FaceController>();
                faceController.ImportVRMmodel(model);
            }
        }

        private void Start()
        {
            ButtonManager.Instance.AxisChangedEvent += ButtonInputInterface_AxisChangedEvent;
        }

        private void ButtonInputInterface_AxisChangedEvent(object sender, AxisEventArgs e)
        {
            if (e.IsLeft == false)
            {
                int stickPointIndex = GetStickPointIndex(e.Value.x, e.Value.y);
                if (stickPointIndex != lastStickPointIndex)
                {
                    lastStickPointIndex = stickPointIndex;

                    if (faceController == null) return;

                    faceController.SetFace(blendShapePresets[stickPointIndex], 1.0f, true);
                }
            }
        }

        // ＼ 1 ／
        // 4( 0 )2
        // ／ 3 ＼
        private int GetStickPointIndex(float x, float y)
        {
            int index = 0;
            var point_distance = x * x + y * y;
            var r = 2.0f / 5.0f; // 半径
            var r2 = r * r;
            if (point_distance < r2) // 円内
            {
                return 0;
            }
            var points = new UPoint[] {
                new UPoint { x = 0, y = 0.5f },
                new UPoint { x = 0.5f, y = 0 },
                new UPoint { x = 0, y = -0.5f },
                new UPoint { x = -0.5f, y = 0 },
            };
            float minLength = float.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                var p = points[i];
                float length = Mathf.Sqrt(Mathf.Pow(x - p.x, 2) + Mathf.Pow(y - p.y, 2));
                if (minLength > length)
                {
                    minLength = length;
                    index = i + 1;
                }
            }
            return index;
        }
    }

    public struct UPoint
    {
        public float x;
        public float y;
    }
}
