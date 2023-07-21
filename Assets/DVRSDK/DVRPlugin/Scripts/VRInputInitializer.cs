using DVRSDK.Plugins.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(1)]
public class VRInputInitializer : MonoBehaviour
{
    [Header("Required Components")]
    public VRInputModule vrInputModule;
    public VRCursor vrCursor;

    [Header("Dependency Components")]
    public Transform VRCursorRayTransform;
    public Component buttonInputInterfaceComponent;

    private void Awake()
    {
        vrInputModule.rayTransform = VRCursorRayTransform;
        vrCursor.rayTransform = VRCursorRayTransform;

        Globals.buttonInputInterface = buttonInputInterfaceComponent.GetComponent<ButtonInputInterface>();
    }
}
