using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePack;
using UnityEditor;
using UnityEngine;

namespace VRStudioLab.Scripts
{
    public class Motion2BytesHumanoid : MonoBehaviour
    {
        [SerializeField] private bool enableLog = true;
        [SerializeField] private Animator animator;
        [SerializeField] private int targetFps = 60;
        [SerializeField] private string guid;
        [SerializeField] private string externalSaveLocation = Application.streamingAssetsPath + "/Motion";
        public string fileName;
        public bool isHighCompression;
        private MotionDataClass _frame = new();
        private CancellationTokenSource _cts = new();
        public float startTime;
        private List<MotionDataClass> Frames = new List<MotionDataClass>();

        private void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            guid = Guid.NewGuid().ToString();
        }

        public void StartRecord()
        {
            Frames = new List<MotionDataClass>();
            _cts = new CancellationTokenSource();
            startTime = Time.time;
            guid = Guid.NewGuid().ToString();

            RecordMuscleFrameDataAsync().Forget();
        }

        public void EndRecord()
        {
            _cts.Cancel();
            SaveJson();
            Debug.Log($"Send save request:{guid}");
        }

        private void OnApplicationQuit()
        {
            _cts.Cancel();
            Debug.Log($"Send save request:{guid}");
        }

        private void SaveJson()
        {
            for (var i = 0; i <= 10; i++)
            {
                var directory = new DirectoryInfo($"{externalSaveLocation}");
                var fileNamePath = $"{fileName}_{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}_{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}.data";

                try
                {
                    var data = MessagePackSerializer.Serialize(Frames);
                    MessagePackSerializer.ConvertToJson(data);
                    Debug.Log(data[0]);
                    File.WriteAllBytes($"{directory}/{fileNamePath}", data);
                    Debug.Log($"Complete Save({fileNamePath})");
                    break;
                }
                catch (IOException)
                {
                    if (i < 10)
                        Debug.LogError($"Failed({fileNamePath})... retry saving...");
                    else
                    {
                        Debug.Log($"I tried 10 loops of saves but failed...");
                        throw;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    throw;
                }
            }
        }

        private async UniTaskVoid RecordMuscleFrameDataAsync()
        {
            var humanPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
            var humanPose = new HumanPose();

            try
            {
                while (true)
                {
                    _cts.Token.ThrowIfCancellationRequested();
                    humanPoseHandler.GetHumanPose(ref humanPose);

                    var animations = new List<ObjectTransforms>();

                    foreach (var muscle in HumanTrait.MuscleName.Select((value, index) => new { value, index }))
                    {
                        if (isHighCompression)
                        {
                            animations.Add(new ObjectTransforms
                            { ObjectName = "", Value = humanPose.muscles[muscle.index] });
                            continue;
                        }

                        // muscle定義名とanimationプロパティ名の対応付け
                        switch (muscle.index)
                        {
                            case 55:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Thumb.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 56:
                                animations.Add(new ObjectTransforms
                                { ObjectName = "LeftHand.Thumb.Spread", Value = humanPose.muscles[muscle.index] });
                                break;
                            case 57:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Thumb.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 58:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Thumb.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 59:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Index.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 60:
                                animations.Add(new ObjectTransforms
                                { ObjectName = "LeftHand.Index.Spread", Value = humanPose.muscles[muscle.index] });
                                break;
                            case 61:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Index.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 62:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Index.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 63:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Middle.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 64:
                                animations.Add(new ObjectTransforms
                                { ObjectName = "LeftHand.Middle.Spread", Value = humanPose.muscles[muscle.index] });
                                break;
                            case 65:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Middle.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 66:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Middle.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 67:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Ring.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 68:
                                animations.Add(new ObjectTransforms
                                { ObjectName = "LeftHand.Ring.Spread", Value = humanPose.muscles[muscle.index] });
                                break;
                            case 69:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Ring.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 70:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Ring.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 71:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Little.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 72:
                                animations.Add(new ObjectTransforms
                                { ObjectName = "LeftHand.Little.Spread", Value = humanPose.muscles[muscle.index] });
                                break;
                            case 73:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Little.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 74:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "LeftHand.Little.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 75:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Thumb.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 76:
                                animations.Add(new ObjectTransforms
                                { ObjectName = "RightHand.Thumb.Spread", Value = humanPose.muscles[muscle.index] });
                                break;
                            case 77:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Thumb.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 78:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Thumb.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 79:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Index.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 80:
                                animations.Add(new ObjectTransforms
                                { ObjectName = "RightHand.Index.Spread", Value = humanPose.muscles[muscle.index] });
                                break;
                            case 81:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Index.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 82:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Index.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 83:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Middle.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 84:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Middle.Spread",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 85:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Middle.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 86:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Middle.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 87:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Ring.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 88:
                                animations.Add(new ObjectTransforms
                                { ObjectName = "RightHand.Ring.Spread", Value = humanPose.muscles[muscle.index] });
                                break;
                            case 89:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Ring.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 90:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Ring.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 91:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Little.1 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 92:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Little.Spread",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 93:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Little.2 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            case 94:
                                animations.Add(new ObjectTransforms
                                {
                                    ObjectName = "RightHand.Little.3 Stretched",
                                    Value = humanPose.muscles[muscle.index]
                                });
                                break;
                            default:
                                animations.Add(new ObjectTransforms
                                { ObjectName = muscle.value, Value = humanPose.muscles[muscle.index] });
                                break;
                        }
                        
                        
                    }

                    _frame = new MotionDataClass
                    {
                        Guid = guid,
                        Time = Time.time - startTime,
                        Transforms = animations,
                    };

                    Frames.Add(_frame);

                    _frame = new MotionDataClass();
                    await UniTask.Delay(TimeSpan.FromSeconds((float)1 / targetFps));
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}