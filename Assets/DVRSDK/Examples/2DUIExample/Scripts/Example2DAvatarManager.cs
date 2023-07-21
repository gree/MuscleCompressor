using UnityEngine;

namespace DVRSDK.Test
{
    public class Example2DAvatarManager : MonoBehaviour
    {
        [SerializeField]
        private DMMVRConnectUI dmmVRConnectUI;

        private void Awake()
        {
            dmmVRConnectUI.OnAvatarLoadedAction += OnAvatarLoaded;
        }

        private void OnAvatarLoaded(GameObject model)
        {
            dmmVRConnectUI.ShowVRM();
            dmmVRConnectUI.AddAutoBlink();
        }
    }
}
