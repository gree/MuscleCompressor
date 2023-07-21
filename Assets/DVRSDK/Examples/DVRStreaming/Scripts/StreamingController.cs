using DVRSDK.Streaming;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamingController : MonoBehaviour
{
    public DVRStreaming DVRStreaming;

    public void StartStreaming()
    {
        DVRStreaming?.StartStreamingAsync();
    }

    public void StopStreaming()
    {
        DVRStreaming?.StopStreaming();
    }

    public void OnStartStreaming()
    {
        Debug.Log("StreamingController.OnStartStreaming");
    }

    public void OnStopStreaming()
    {
        Debug.Log("StreamingController.OnStopStreaming");
    }

    public void OnError(int errorCode)
    {
        Debug.Log($"StreamingController.OnError({errorCode})");
    }
}
