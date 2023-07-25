using DVRSDK.Avatar.Tracking;
using DVRSDK.Plugins.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DVRSDK.Avatar
{
    public class HandTracking_Button : MonoBehaviour
    {
        private const bool LEFT = true;
        private const bool RIGHT = false;

        public float AnimationSpeed = 20.0f;

        private VRMHandController handController;

        private Dictionary<bool, float> pointBlend = new Dictionary<bool, float> { { LEFT, 0.0f }, { RIGHT, 0.0f } };
        private Dictionary<bool, float> thumbsUpBlend = new Dictionary<bool, float> { { LEFT, 0.0f }, { RIGHT, 0.0f } };
        private Dictionary<bool, float> otherBlend = new Dictionary<bool, float> { { LEFT, 0.0f }, { RIGHT, 0.0f } };

        private List<int> rock = new List<int> { -70, -90, -90, 0, -90, -90, -90, 0, -90, -90, -90, 0, -90, -90, -90, 0, -90, -90, 2, 10, -70, -90, -90, 0, -90, -90, -90, 0, -90, -90, -90, 0, -90, -90, -90, 0, -90, -90, 2, 10 };
        private List<int> paper = new List<int> { 3, 0, 4, -15, 0, 4, 2, -7, 0, 5, 1, 0, -2, -1, 0, 4, 10, 10, 10, 10, 3, 0, 4, -15, 0, 4, 2, -7, 0, 5, 1, 0, -2, -1, 0, 4, 10, 10, 10, 10 };
        public List<float> rockRoll = new List<float> { -38.9f, -27.7f, -23.7f, -22f, -22.94f, -12.82f, -17.5f, -1.8f, 0, 0, 0, 19.08f, 0, 0, 0, -38.9f, -27.7f, -23.7f, -22f, -22.94f, -12.82f, -17.5f, -1.8f, 0, 0, 0, 19.08f, 0, 0, 0 };
        public List<float> paperRoll = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };

        public List<int> FixedHandAngles = null;
        private List<Vector3> fixedHandAnglesDefault;
        public void LoadModel(GameObject prefab)
        {
            if (handController != null) DestroyImmediate(handController);
            handController = prefab.AddComponent<VRMHandController>();
            var animator = prefab.GetComponent<Animator>();
            handController.SetDefaultAngle(animator);
        }

        public void SetFixedHandAngles(List<int> angles)
        {
            FixedHandAngles = angles;
            if (angles == null)
            {
                fixedHandAnglesDefault = null;
            }
            else
            {
                fixedHandAnglesDefault = handController.CalcHandEulerAngles(angles);
            }
        }

        private List<Vector3> rockdefault = null;
        private List<Vector3> paperdefault = null;

        private void Update()
        {
            UpdateAnimStates();
        }

        private float CalcBlendValue(bool isDown, float value)
        {
            float rateDelta = Time.deltaTime * AnimationSpeed;
            float sign = isDown ? 1.0f : -1.0f;
            return Mathf.Clamp01(value + rateDelta * sign);
        }

        private void UpdateAnimStates()
        {
            if (handController == null) return;
            if (fixedHandAnglesDefault != null)
            {
                handController.SetHandEulerAngles(true, true, fixedHandAnglesDefault);
                return;
            }

            if (rockdefault == null || paperdefault == null)
            {
                UpdateAngleValue();
            }
            var ts = new List<float>();

            AddAngles(LEFT, ts);
            AddAngles(RIGHT, ts);

            handController.SetHandEulerAngles(true, true, handController.eulersLerps(rockdefault, paperdefault, ts));
        }

        private void AddAngles(bool isLeft, List<float> ts)
        {
            var isOther = !ButtonManager.Instance.GetKeyState(isLeft, KeyNames.Grip);
            var isIndex = !ButtonManager.Instance.GetKeyState(isLeft, KeyNames.Trigger);
            var isThumb = !(ButtonManager.Instance.GetKeyState(isLeft, KeyNames.Stick) |
                            ButtonManager.Instance.GetKeyState(isLeft, KeyNames.Select) |
                            ButtonManager.Instance.GetKeyState(isLeft, KeyNames.Cancel));
            otherBlend[isLeft] = CalcBlendValue(isOther, otherBlend[isLeft]);
            pointBlend[isLeft] = CalcBlendValue(isIndex, pointBlend[isLeft]);
            thumbsUpBlend[isLeft] = CalcBlendValue(isThumb, thumbsUpBlend[isLeft]);

            var otherValue = otherBlend[isLeft];
            ts.AddRange(Enumerable.Repeat(otherValue, 9));
            var indexValue = pointBlend[isLeft];
            ts.Add(indexValue);
            ts.Add(indexValue);
            ts.Add(indexValue);
            var thumbsUp = thumbsUpBlend[isLeft];
            ts.Add(thumbsUp);
            ts.Add(thumbsUp);
            ts.Add(thumbsUp);
        }

        public void UpdateAngleValue()
        {
            rockdefault = handController.CalcHandEulerAngles(rock);
            paperdefault = handController.CalcHandEulerAngles(paper);

            if (rockdefault == null) return;

            for (int i = 0; i < rockdefault.Count; i++)
            {
                rockdefault[i] = new Vector3(rockdefault[i].x + rockRoll[i], rockdefault[i].y, rockdefault[i].z);
                paperdefault[i] = new Vector3(paperdefault[i].x + paperRoll[i], paperdefault[i].y, paperdefault[i].z);
            }
        }
    }
}
