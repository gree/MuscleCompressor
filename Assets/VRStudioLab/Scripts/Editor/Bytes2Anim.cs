using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MessagePack;
using static System.Int32;
using SFB;
using UnityEngine;
using UnityEditor;
namespace VRStudioLab.Scripts.Editor
{
    public class Bytes2Anim : EditorWindow
    {
        [MenuItem("VRStudioLab/Byte2Anim")]
        public static void Byte2Anim()
        {
            var extension = new[] {new ExtensionFilter("data file", "data")};
            var path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extension, false);
            var bytes = File.ReadAllBytes(path[0]);
            var data = MessagePackSerializer.Deserialize<List<MotionDataClass>>(bytes);
            Debug.Log(data);
            var curves = new AnimationCurve[102];
            var clip = new AnimationClip {legacy = false};
            foreach (var frame in data.Select((value, index) => new {value, index}))
            {
                foreach (var objectTransforms in frame.value.Transforms.Select((value, index) => new {value, index}))
                {
                    var keyframe = new Keyframe(frame.value.Time, objectTransforms.value.Value);
                    if (curves[objectTransforms.index] == null)
                    {
                        curves[objectTransforms.index] = new AnimationCurve(keyframe);
                    }
                    else
                    {
                        curves[objectTransforms.index].AddKey(keyframe);
                    }

                    clip.SetCurve("", typeof(Animator), $"{objectTransforms.value.ObjectName}", curves[objectTransforms.index]);}
            }
            if (!Directory.Exists("Assets/VRStudioLab/Animations")) Directory.CreateDirectory("Assets/VRStudioLab/Animations");
            var directory = new DirectoryInfo("Assets/VRStudioLab/Animations/");
            var max = directory.GetFiles($"???.anim")
                .Select(info => Regex.Match(info.Name, @"(\d{3})\.anim"))
                .Where(match => match.Success)
                .Select(match => Parse(match.Groups[1].Value))
                .DefaultIfEmpty(0)
                .Max();
            var fileName = $"{max + 1:d3}.anim";
            AssetDatabase.CreateAsset(clip,$"Assets/VRStudioLab/Animations/{fileName}");
        }
    }
}
