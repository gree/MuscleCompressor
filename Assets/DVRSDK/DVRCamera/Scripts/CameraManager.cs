using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DVRSDK.DVRCamera
{
    public class CameraManager : MonoBehaviour
    {
        public bool EnableAutoSequence;
        public float SwitchTime;
        public bool useRenderTexture = true;
        public RawImage switcher;
        public bool createScreen = true;
        public int TextureWidth = 1920;
        public int TextureHeight = 1080;

        public List<SceneSetting> SceneList = new List<SceneSetting>() { new SceneSetting() };
        public int CurrentIndex = 0;

        private SceneSetting oldScene;

        [NonSerialized]
        public RenderTexture RenderTexture;

        private Coroutine autoSequenceCoroutine;

        [NonSerialized]
        public List<ViewCamera> ViewCameras = new List<ViewCamera>();

        private static CameraManager instance;
        public static CameraManager Instance => instance;

        private void Awake()
        {
            instance = this;

            int cameraCount = SceneList.Max(d => d.CameraList.Count);

            for (int i = 0; i < cameraCount; i++)
            {
                var viewCameraObject = new GameObject($"ViewCamera{i + 1}");
                viewCameraObject.transform.parent = transform;
                viewCameraObject.AddComponent<Camera>();
                var viewCamera = viewCameraObject.AddComponent<ViewCamera>();
                ViewCameras.Add(viewCamera);

                foreach (var scene in SceneList)
                {
                    if (scene.CameraList.Count > i)
                    {
                        scene.CameraList[i].ViewCamera = viewCamera;
                    }
                }
            }
        }

        private void Start()
        {
            Init();
        }

        public void PlayPause()
        {
            EnableAutoSequence = !EnableAutoSequence;
            SwitchScene(CurrentIndex);
        }

        public void Next()
        {
            SwitchScene(CurrentIndex + 1);
        }

        public void Prev()
        {
            SwitchScene(CurrentIndex - 1);
        }

        public void SwitchScene(int index = 0)
        {
            if (autoSequenceCoroutine != null)
            {
                StopCoroutine(autoSequenceCoroutine);
                SceneList[CurrentIndex].Stop();
                autoSequenceCoroutine = null;
            }

            if (index >= SceneList.Count) index = 0;
            else if (index <= -1) index = SceneList.Count - 1;

            CurrentIndex = index;

            if (EnableAutoSequence)
            {
                autoSequenceCoroutine = StartCoroutine(AutoSequence());
            }
            else
            {
                SceneApply(CurrentIndex);
            }
        }

        private void Init()
        {
            if (useRenderTexture)
            {
                RenderTexture = new RenderTexture(TextureWidth, TextureHeight, 24);

                foreach (var camera in ViewCameras)
                {
                    camera.Camera.targetTexture = RenderTexture;
                }

                if (createScreen) CreateScreen();

                if (switcher != null) switcher.texture = RenderTexture;
            }

            if(EnableAutoSequence) SwitchScene();
        }

        private void SceneApply(int index)
        {
            if (oldScene != null)
            {
                oldScene.Stop();
            }
            if (SceneList[index].FadeTransition)
            {
                foreach (var camera in ViewCameras)
                {
                    camera.StartFade(SceneList[index].FadeTime);
                }
            }

            SceneList[index].Apply();
            oldScene = SceneList[index];
        }

        private void CreateScreen()
        {
            var canvasObject = new GameObject("Canvas");
            canvasObject.transform.parent = transform;
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();

            var textureObject = new GameObject("Texture");
            var rect = textureObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            textureObject.transform.parent = canvasObject.transform;
            var texture = textureObject.AddComponent<RawImage>();
            texture.texture = RenderTexture;

            rect.sizeDelta = Vector2.zero;
            rect.localPosition = Vector3.zero;
        }

        IEnumerator AutoSequence()
        {
            while (true)
            {
                var switchTime = SwitchTime;
                if (SceneList[CurrentIndex].OverrideDefaultSwitchTime)
                {
                    switchTime = SceneList[CurrentIndex].SwitchTime;
                }

                SceneApply(CurrentIndex);

                yield return new WaitForSeconds(switchTime);

                if (CurrentIndex >= SceneList.Count - 1)
                {
                    CurrentIndex = 0;
                }
                else
                {
                    CurrentIndex++;
                }
            }
        }
    }
}
