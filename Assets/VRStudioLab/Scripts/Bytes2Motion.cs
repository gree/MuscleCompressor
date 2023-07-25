using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using MessagePack;
using Cysharp.Threading.Tasks;

namespace VRStudioLab.Scripts
{
    public class Bytes2Motion : MonoBehaviour
    {
        public string path { get; set; }
        [SerializeField] private Animator animator;
        [SerializeField] private float playSpeed = 1.0f;

        private HumanPose humanPose;
        private HumanPoseHandler handler;
        private float[] tempHumanPose;
        private float recentTime;
        private bool isHandlerable = false;
        private Coroutine muscleCoroutine;

        void Start()
        {
            recentTime = 0;
            tempHumanPose = new float[102];

            if (GetComponent<Button>() != null)
            {
                var button = GetComponent<Button>();
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open Motion Data", "", "data", false);
            path = paths[0];
            InitHandler();
            LoadBytes(path);
        }

        public void InitHandler()
        {
            handler = new HumanPoseHandler(animator.avatar, animator.transform);
            handler.GetHumanPose(ref humanPose);
            Debug.Log("Initialized");
        }

        public void LoadBytes(string path, bool autoload = true)
        {
            var bytes = File.ReadAllBytes(path);
            var data = MessagePackSerializer.Deserialize<List<MotionDataClass>>(bytes);

            if (muscleCoroutine != null)
            {
                StopCoroutine(muscleCoroutine);
            }

            muscleCoroutine = StartCoroutine(LoadMotion(data));
        }

        IEnumerator LoadMotion(List<MotionDataClass> data)
        {
            recentTime = 0;
            isHandlerable = true;

            foreach (var frame in data.Select((value, index) => new { value, index }))
            {
                foreach (var objectTransforms in frame.value.Transforms.Select((value, index) => new { value, index }))
                {
                    tempHumanPose[objectTransforms.index] = objectTransforms.value.Value;
                }
                // ãLò^ÇµÇΩéûä‘Ç…í«Ç¢Ç¬Ç≠Ç‹Ç≈ë“ã@
                yield return new WaitUntil(() => recentTime * playSpeed >= frame.value.Time);
            }
        }

        // HumanPoseHandlerÇ…ÇÊÇÈëÄçÏÇÕLateUpdateÇ≈çsÇ§
        void LateUpdate()
        {
            recentTime += Time.deltaTime;

            if (isHandlerable == true)
            {
                handler.GetHumanPose(ref humanPose);

                for (int i = 0; i < 95; i++)
                {
                    humanPose.muscles[i] = tempHumanPose[i];
                }

                handler.SetHumanPose(ref humanPose);
            }
        }
    }
}
