using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DVRSDK.DVRCamera;
using UnityEditorInternal;
using UnityEngine.UI;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditorScript : Editor
{
    public CameraManager _target;
    private ReorderableList SceneList;

    private Dictionary<string, ReorderableList> innerListDict = new Dictionary<string, ReorderableList>();

    private void OnEnable()
    {
        _target = (CameraManager)target;

        SceneList = new ReorderableList(_target.SceneList, typeof(List<SceneSetting>), true, true, true, true);

        SceneList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "SceneList");
        SceneList.drawElementCallback = DrawElementCallback;
        SceneList.onReorderCallback = OnReorderCallback;
        SceneList.elementHeightCallback = ElementHeightCallback;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Space();

        _target.EnableAutoSequence = EditorGUILayout.Toggle("EnableAutoSequence", _target.EnableAutoSequence);
        if (_target.EnableAutoSequence)
        {
            _target.SwitchTime = EditorGUILayout.FloatField("SwitchTime", _target.SwitchTime);
        }
        _target.useRenderTexture = EditorGUILayout.Toggle("UseRenderTexture", _target.useRenderTexture);
        if (_target.useRenderTexture)
        {
            _target.switcher = (RawImage)EditorGUILayout.ObjectField(_target.switcher, typeof(RawImage), true);
        }
        _target.createScreen = EditorGUILayout.Toggle("CreateScreen", _target.createScreen);

        EditorGUILayout.Space();

        SceneList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_target, "Change Property");
            EditorUtility.SetDirty(_target);
        }
    }

    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        var data = _target.SceneList[index];

        var offsetRect = rect;
        offsetRect.y += EditorGUIUtility.singleLineHeight;
        offsetRect.height = EditorGUIUtility.singleLineHeight;

        data.OverrideDefaultSwitchTime = EditorGUI.Toggle(offsetRect, "OverrideDefaultSwitchTime", data.OverrideDefaultSwitchTime);
        if (data.OverrideDefaultSwitchTime)
        {
            offsetRect.y += EditorGUIUtility.singleLineHeight;
            data.SwitchTime = EditorGUI.FloatField(offsetRect, "SwitchTime", data.SwitchTime);
        }

        offsetRect.y += EditorGUIUtility.singleLineHeight;
        data.FadeTransition = EditorGUI.Toggle(offsetRect, "FadeTransition", data.FadeTransition);
        if (data.FadeTransition)
        {
            offsetRect.y += EditorGUIUtility.singleLineHeight;
            data.FadeTime = EditorGUI.FloatField(offsetRect, "FadeTime", data.FadeTime);
        }
        offsetRect.y += EditorGUIUtility.singleLineHeight;

        var so = serializedObject.FindProperty("SceneList").GetArrayElementAtIndex(index);
        var listKey = so.propertyPath;

        ReorderableList cameraList;

        if (innerListDict.ContainsKey(listKey))
        {
            cameraList = innerListDict[listKey];
        }
        else
        {
            cameraList = new ReorderableList(data.CameraList, typeof(List<CameraSetting>), true, true, true, true)
            {
                drawHeaderCallback = (rect2) => EditorGUI.LabelField(rect2, "CameraList"),
                drawElementCallback = (rect2, index2, isActive2, isFocused2) =>
                {
                    var offsetRect2 = rect2;
                    offsetRect2.y += EditorGUIUtility.singleLineHeight;
                    offsetRect2.height = EditorGUIUtility.singleLineHeight * 2;

                    data.CameraList[index2].ViewPosition = EditorGUI.RectField(offsetRect2, data.CameraList[index2].ViewPosition);

                    offsetRect2.y += EditorGUIUtility.singleLineHeight * 2;
                    offsetRect2.height = EditorGUIUtility.singleLineHeight;
                    data.CameraList[index2].CameraTarget = (CameraTarget)EditorGUI.ObjectField(offsetRect2, data.CameraList[index2].CameraTarget, typeof(CameraTarget), true);
                },
                elementHeight = EditorGUIUtility.singleLineHeight * 5,
            };
            innerListDict[listKey] = cameraList;
        }

        rect.y += EditorGUIUtility.singleLineHeight * 5;
        cameraList.DoList(rect);
    }

    private void OnReorderCallback(ReorderableList reorderableList)
    {
        innerListDict.Clear();
    }

    private float ElementHeightCallback(int index)
    {
        var listHeight = _target.SceneList[index].CameraList.Count * EditorGUIUtility.singleLineHeight * 6;
        return EditorGUIUtility.singleLineHeight * 6 + listHeight;
    }
}
