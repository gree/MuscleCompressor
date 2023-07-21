using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace VRStudioLab.Scripts
{
    public class RecordCueHandler : MonoBehaviour
    {
        public Motion2BytesHumanoid m2b;
        public float duration;
        [SerializeField] private GameObject button;
        [SerializeField] private GameObject recText;

        public void StartRecord()
        {
            m2b?.StartRecord();
            recText.SetActive(true);
        }

        public void EndRecord()
        {
            m2b?.EndRecord();
            recText.SetActive(false);
            button.SetActive(true);
        }

        public void RecordWithTimer()
        {
            StartRecord();
            Invoke(nameof(EndRecord), duration);
        }
    }
}
