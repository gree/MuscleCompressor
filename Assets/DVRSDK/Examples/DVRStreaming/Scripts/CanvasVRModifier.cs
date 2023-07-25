using UnityEngine;
using UnityEngine.UI;

public class CanvasVRModifier : MonoBehaviour
{
    private GameObject pointer;
    private void Awake()
    {
        var graphicRaycaster = GetComponent<GraphicRaycaster>();
        if (graphicRaycaster != null)
        {
            graphicRaycaster.enabled = false;
            var ovrRaycaster = gameObject.AddComponent<OVRRaycaster>();
            ovrRaycaster.pointer = pointer;
            ovrRaycaster.blockingObjects = OVRRaycaster.BlockingObjects.All;
        }
    }
}
