using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DVRSDK.Streaming
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioStreamingSource : MonoBehaviour
    {
        [SerializeField]
        private DVRStreaming controller;

        private void Start()
        {
            if (controller == null) controller = FindObjectOfType<DVRStreaming>();
        }

        private void OnAudioFilterRead(float[] sampleData, int channels)
        {
            if (controller && controller.IsStreaming)
            {
                GCHandle pinnedSampleData = GCHandle.Alloc(sampleData, GCHandleType.Pinned);

                // メインスレッド以外でプラグインをロードするとUnity Editor内でクラッシュする
                // 実際に呼び出してなくてもメソッドの先頭でロードされるので別メソッドに分離しておく
                CallPlugin(pinnedSampleData.AddrOfPinnedObject(), sampleData.Length, channels);

                pinnedSampleData.Free();
            }
        }

        private void CallPlugin(IntPtr buffer, int length, int channels)
        {
            NativeMethods.AddAudioSamples(buffer, length, channels);
        }

        public void StartStreaming() { }

        public void StopStreaming() { }
    }
}
