using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MirrorLayerMaskOverride : MonoBehaviour
{
    public LayerMask MirrorLayerMask = ~0;
}
