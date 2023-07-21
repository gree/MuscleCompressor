using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VRStudioLab.Scripts
{
    public class RecordingManager : MonoBehaviour
    {
        [HideInInspector] public Motion2BytesHumanoid motion2BytesHumanoid; //DMMVRConnectUI.csから動的にアタッチ

        [SerializeField] private bool isHighCompression;
        [SerializeField] private TextMeshProUGUI recordingText;
        [SerializeField] private RecordCueHandler recordCueHandler;
        [SerializeField] private List<RecordingDataClass> RecordingDatas;
        private int i;

        void Start()
        {
            i = -1;
        }

        public void SwitchEmotion()
        {
            Debug.Log("Switched");
            i++;
            SetRecordingData(RecordingDatas[i]);
            motion2BytesHumanoid.isHighCompression = isHighCompression;
        }

        private void SetRecordingData(RecordingDataClass data)
        {
            recordingText.text = $"{data.Text} for {data.Duration} seconds";
            motion2BytesHumanoid.fileName = data.Name;
            recordCueHandler.duration = data.Duration;
        }

        [System.Serializable]
        public class RecordingDataClass
        {
            public string Name;
            public string Text;
            public float Duration;
        }
    }
}

