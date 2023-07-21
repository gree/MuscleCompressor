using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DVRSDK.Avatar.Tracking.Oculus
{
    public class OculusVRTracker : MonoBehaviour, ITracker
    {
        [Header("親に指定したいTransform。TrackingSpaceを指定してください")]
        [SerializeField]
        private Transform trackersParent = null;
        public Transform TrackersParent => trackersParent;

        [SerializeField] Vector3 leftHandTransform;
        [SerializeField] Vector3 rightHandTransform;
        [SerializeField] Quaternion leftHandRotation = Quaternion.Euler(0,0,180);
        [SerializeField] Quaternion rightHandRotation = Quaternion.Euler(0,180,0);

        // 外部からTransformを指定されたときはそちらをそのまま使用する。無い場合は自動で作成して割り当てる
        [Header("使用したい部位をすべて定義します。すべてのSourceTransformを埋めてください")]
        public TrackerTarget[] TrackerTargets = new TrackerTarget[]
        {
            new TrackerTarget { TrackerPosition = TrackerPositions.Head, UseDeviceType = TrackingDeviceType.HMD },
            new TrackerTarget { TrackerPosition = TrackerPositions.LeftHand, UseDeviceType = TrackingDeviceType.Controller },
            new TrackerTarget { TrackerPosition = TrackerPositions.RightHand, UseDeviceType = TrackingDeviceType.Controller },
        };

        public TrackerTarget GetTrackerTarget(TrackerPositions trackerPosition) => TrackerTargets.FirstOrDefault(d => d.TrackerPosition == trackerPosition && d.PoseIsValid);

        public Vector3 GetIKOffsetPosition(TrackerPositions targetPosition, TrackingDeviceType deviceType)
        {
            if (targetPosition == TrackerPositions.LeftHand && deviceType == TrackingDeviceType.Controller)
            {
                //return new Vector3(-0.015f, 0.01f, -0.1f);
                return leftHandTransform;
            }
            else if (targetPosition == TrackerPositions.RightHand && deviceType == TrackingDeviceType.Controller)
            {
                //return new Vector3(0.015f, 0.01f, -0.1f);
                return rightHandTransform;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public Quaternion GetIKOffsetRotation(TrackerPositions targetPosition, TrackingDeviceType deviceType)
        {
            if (targetPosition == TrackerPositions.LeftHand && deviceType == TrackingDeviceType.Controller)
            {
                return leftHandRotation;
            }
            else if (targetPosition == TrackerPositions.RightHand && deviceType == TrackingDeviceType.Controller)
            {
                return rightHandRotation;
            }
            else
            {
                return Quaternion.identity;
            }
        }

        private void Start()
        {
            if (trackersParent == null) Debug.LogError("Please set TrackersParent");
            foreach (var trackerTarget in TrackerTargets)
            {
                if (trackerTarget.TargetTransform == null)
                {
                    var newTarget = new GameObject(trackerTarget.TrackerPosition.ToString());
                    if (trackersParent == null) trackersParent = transform;
                    newTarget.transform.SetParent(TrackersParent, false);
                    trackerTarget.TargetTransform = newTarget.transform;
                }
            }
        }

        private void Update()
        {
            UpdateAllTrackerData();
        }

        private void UpdateAllTrackerData()
        {
            // 全トラッカーの位置データ更新
            foreach (var trackerTarget in TrackerTargets)
            {
                if (trackerTarget.SourceTransform != null) // スキップ設定、外部CameraRig等ですでに位置データ処理しているときを想定
                {
                    trackerTarget.TargetTransform.localPosition = trackerTarget.SourceTransform.localPosition;
                    trackerTarget.TargetTransform.localRotation = trackerTarget.SourceTransform.localRotation;
                    trackerTarget.PoseIsValid = true;
                }
            }
        }

        /// <summary>
        /// 自動で指定したデバイスを指定した部位に割り当て
        /// </summary>
        /// <param name="worldForwardVector">HmdTransform.forward</param>
        /// <param name="worldUpVector">HmdTransform.up</param>
        public void AutoAttachTrackerTargets(Vector3? worldForwardVector = null, Vector3? worldUpVector = null)
        {
            // 頭
            var trackerTarget = TrackerTargets.FirstOrDefault(d => d.TrackerPosition == TrackerPositions.Head);
            trackerTarget.DeviceIndex = 0;
            trackerTarget.UseDeviceType = TrackingDeviceType.HMD;

            var forward = worldForwardVector ?? trackerTarget.TargetTransform.forward;
            var up = worldUpVector ?? trackerTarget.TargetTransform.up;

            // 左手
            trackerTarget = TrackerTargets.FirstOrDefault(d => d.TrackerPosition == TrackerPositions.LeftHand);
            trackerTarget.DeviceIndex = 0;
            trackerTarget.UseDeviceType = TrackingDeviceType.Controller;

            // 右手
            trackerTarget = TrackerTargets.FirstOrDefault(d => d.TrackerPosition == TrackerPositions.RightHand);
            trackerTarget.DeviceIndex = 0;
            trackerTarget.UseDeviceType = TrackingDeviceType.Controller;
        }

        // forwardVectorとupVectorには正面方向(hmdTransform.forward, hmdTransform.up)を入れる
        private Vector3 RecenterPoint(Vector3 forwardVector, Vector3 upVector, Vector3 pos)
        {
            var rotation = Quaternion.LookRotation(forwardVector, upVector);
            var frontRotation = Quaternion.identity;
            var diffRotation = frontRotation * Quaternion.Inverse(rotation);
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, diffRotation, Vector3.one);
            //Matrix4x4 inverse = matrix.inverse;
            return matrix.MultiplyPoint3x4(pos);
        }
    }
}
