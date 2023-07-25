using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;

namespace VRStudioLab.Scripts
{
    [JsonObject]
    [MessagePackObject]
    public class MotionDataClass
    {
        [JsonProperty("GUID")] [Key(0)] public string Guid { get; set; }
        [JsonProperty("Time")] [Key(1)] public float Time { get; set; }

        [JsonProperty("Transforms")] [Key(2)] public List<ObjectTransforms> Transforms { get; set; }

    }

    [JsonObject]
    [MessagePackObject]
    public class ObjectTransforms
    {
        [JsonProperty("ObjectName")] [Key(0)] public string ObjectName { get; set; }

        [JsonProperty("value")] [Key(1)] public float Value { get; set; }
    }
}