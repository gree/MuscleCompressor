using DVRSDK.Test;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DynamicOVRLipSync))]
public class LipSyncInitializer : MonoBehaviour
{
    [SerializeField]
    private DMMVRConnectUI dmmVRConnectUI;

    private DynamicOVRLipSync dynamicOVRLipSync;

    private void Awake()
    {
        dynamicOVRLipSync = GetComponent<DynamicOVRLipSync>();

        dmmVRConnectUI.OnAvatarLoadedAction += OnAvatarLoaded;
    }

    private void OnAvatarLoaded(GameObject model)
    {
        dynamicOVRLipSync.ImportVRMmodel(model);
    }
}
