using System;
using System.Runtime.InteropServices;

namespace DVRSDK.Streaming
{
    internal static class NativeMethods
    {
        internal struct Settings
        {
            public int Width;
            public int Height;
            public int FrameRate;
            public int BitRate;
        }

        [DllImport("DVRStreamingPlugin")]
        public static extern void SetTextureFromUnity(IntPtr texture, int w, int h);

        [DllImport("DVRStreamingPlugin")]
        public static extern IntPtr GetRenderEventFunc();

        [DllImport("DVRStreamingPlugin", CharSet = CharSet.Ansi)]
        public static extern void SetLogFunction(IntPtr logFunc, IntPtr logWarningFunc, IntPtr logErrorFunc);

        [DllImport("DVRStreamingPlugin")]
        public static extern void AddAudioSamples(IntPtr Samples, int Length, int Channel);

        [DllImport("DVRStreamingPlugin", CharSet = CharSet.Ansi)]
        public static extern int StartStreaming(string ServerUrl, ref Settings settings);

        [DllImport("DVRStreamingPlugin")]
        public static extern int StopStreaming();

        [DllImport("DVRStreamingPlugin")]
        public static extern int Pause(bool pause);
    }
}
