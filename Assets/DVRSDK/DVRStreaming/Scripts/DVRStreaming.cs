using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace DVRSDK.Streaming
{
    public class DVRStreaming : MonoBehaviour
    {
        [Header("Streaming Source")]
        public VideoStreamingSource videoSource;
        public AudioStreamingSource audioSource;

        [Header("Streaming Setting")]
        public string ServerUrl;
        public int Width = 1280;
        public int Height = 720;
        public int FrameRate = 30;
        public int VideoBitRate = 5000000;

        [Header("Event Handlers")]
        public UnityEvent OnStartStreaming;
        public UnityEvent OnStopStreaming;
        public ErrorEvent OnError;

        public bool IsStreaming { get; private set; }

        private void Awake()
        {
            System.Action<string> logFunction = (text) => Debug.Log(text);
            System.Action<string> logWarningFunction = (text) => Debug.LogWarning(text);
            System.Action<string> logErrorFunction = (text) => Debug.LogError(text);
            NativeMethods.SetLogFunction(
                Marshal.GetFunctionPointerForDelegate(logFunction),
                Marshal.GetFunctionPointerForDelegate(logWarningFunction),
                Marshal.GetFunctionPointerForDelegate(logErrorFunction));
        }

        private int CheckStartStreaming(int errorCode)
        {
            if (errorCode != 0)
            {
                OnError.Invoke(errorCode);
                return errorCode;
            }

            GL.IssuePluginEvent(NativeMethods.GetRenderEventFunc(), 0);

            IsStreaming = true;

            videoSource?.StartStreaming();
            audioSource?.StartStreaming();

            OnStartStreaming.Invoke();

            return errorCode;
        }

        public int StartStreaming()
        {
            NativeMethods.Settings settings = new NativeMethods.Settings {
                Width = Width,
                Height = Height,
                FrameRate = FrameRate,
                BitRate = VideoBitRate,
            };
            int errorCode = NativeMethods.StartStreaming(ServerUrl, ref settings);

            return CheckStartStreaming(errorCode);
        }

        public async Task<int> StartStreamingAsync()
        {
            NativeMethods.Settings settings = new NativeMethods.Settings
            {
                Width = Width,
                Height = Height,
                FrameRate = FrameRate,
                BitRate = VideoBitRate,
            };
            int errorCode = await Task.Run<int>(() => NativeMethods.StartStreaming(ServerUrl, ref settings));

            return CheckStartStreaming(errorCode);
        }

        public int StopStreaming()
        {
            IsStreaming = false;

            OnStopStreaming.Invoke();

            videoSource?.StopStreaming();
            audioSource?.StopStreaming();

            GL.IssuePluginEvent(NativeMethods.GetRenderEventFunc(), 2);

            int result = NativeMethods.StopStreaming();
            if (result != 0)
            {
                OnError.Invoke(result);
            }

            return result;
        }
    }
}
