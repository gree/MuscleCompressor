using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVRSDK.Streaming
{
    [RequireComponent(typeof(Camera))]
    public class VideoStreamingSource : MonoBehaviour
    {
        [SerializeField]
        private DVRStreaming controller;
        private Camera spectatorCamera;
        private Coroutine callPluginAtEndOfFrames;

        void Awake()
        {
            if (controller == null) controller = FindObjectOfType<DVRStreaming>();
            spectatorCamera = GetComponent<Camera>();

            enabled = false;
        }

        void OnEnable()
        {
            RenderTexture renderTexture = new RenderTexture(controller.Width, controller.Height, 32);
            renderTexture.name = "Spectator Camera";

            spectatorCamera.targetTexture = renderTexture;
            spectatorCamera.enabled = true;

            callPluginAtEndOfFrames = StartCoroutine(CallPluginAtEndOfFrames());
        }

        void OnDisable()
        {
            StopCoroutine(callPluginAtEndOfFrames);

            RenderTexture renderTexture = spectatorCamera.targetTexture;
            spectatorCamera.targetTexture = null;
            Destroy(renderTexture);

            spectatorCamera.enabled = false;
        }

        private IEnumerator CallPluginAtEndOfFrames()
        {
            RenderTexture renderTexture = spectatorCamera.targetTexture;
            NativeMethods.SetTextureFromUnity(renderTexture.GetNativeTexturePtr(), renderTexture.width, renderTexture.height);

            while (true)
            {
                yield return new WaitForEndOfFrame();

                GL.IssuePluginEvent(NativeMethods.GetRenderEventFunc(), 1);
            }
        }

        public void StartStreaming()
        {
            enabled = true;
        }

        public void StopStreaming()
        {
            enabled = false;
        }
    }
}
