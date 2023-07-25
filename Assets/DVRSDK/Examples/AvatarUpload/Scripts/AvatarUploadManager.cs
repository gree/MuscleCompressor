using DVRSDK.Auth;
using DVRSDK.Avatar;
using System.Threading.Tasks;
using UniGLTF;
using UnityEngine;
using VRM;
#if UNIVRM_0_71_EXPORTER || UNIVRM_0_75_EXPORTER || UNIVRM_0_79_EXPORTER
using VRMShaders;
#endif


namespace DVRSDK.Test
{
    public class AvatarUploadManager : MonoBehaviour
    {
        [SerializeField]
        private DMMVRConnectUI dmmVRConnectUI;

        [SerializeField]
        private GameObject avatarUploadPanel;

        [SerializeField]
        private AvatarLicenseView avatarLicenseView;

        private VRMLoader vrmLoader;

        private void InitializeLoader()
        {
            vrmLoader?.Dispose();
            vrmLoader = new VRMLoader();
        }

        public void ShowUploadPanel()
        {
            dmmVRConnectUI.ChangePanel(avatarUploadPanel);
        }

        public void CloseUploadPanel()
        {
            dmmVRConnectUI.ChangePanel(UIPanelType.Logout);
        }

        public async void UploadFromFile()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string vrmFilePath = WindowsDialogs.OpenFileDialog("Select VRM File", ".vrm");
            if (string.IsNullOrWhiteSpace(vrmFilePath) == false)
            {
                await avatarLicenseView.ShowPanelFromFileAsync(vrmFilePath, async meta =>
                {
                    avatarLicenseView.HidePanel();
                    var vrmData = await VRMLoader.ReadAllBytesAsync(vrmFilePath);
                    await UploadAvatarAsync(meta.Title, vrmData, ConvertTexture2DToPng(meta.Thumbnail));
                },
                () => avatarLicenseView.HidePanel());
            }
#endif
        }

        public void UploadFromPrefab()
        {
            var vrm = FindObjectOfType<VRMMeta>().gameObject;
            if (vrm == null)
            {
                dmmVRConnectUI.SetLog("Can't find VRM in scene.");
                return;
            }

            avatarLicenseView.ShowPanelFromPrefab(vrm, async meta =>
            {
                avatarLicenseView.HidePanel();
                await UploadAvatarAsync(meta.Title, ConvertVRMPrefabToBinary(vrm), ConvertTexture2DToPng(meta.Thumbnail));
            },
            () => avatarLicenseView.HidePanel());
        }

        public async void LoadVRMFile()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string vrmFilePath = WindowsDialogs.OpenFileDialog("Select VRM File", ".vrm");
            if (string.IsNullOrWhiteSpace(vrmFilePath) == false)
            {
                InitializeLoader();
                await vrmLoader.LoadVrmModelFromFileAsync(vrmFilePath);
                vrmLoader.ShowMeshes();
            }
#endif
        }

        private byte[] ConvertTexture2DToPng(Texture2D from)
        {
            return from.EncodeToPNG();
        }

        public byte[] ConvertVRMPrefabToBinary(GameObject vrm)
        {
            if (vrm == null) return null;

#if UNIVRM_LEGACY_EXPORTER
            var gltf = new glTF();
            using (var exporter = new VRMExporter(gltf))
            {
                exporter.Prepare(vrm);

                exporter.Export(MeshExportSettings.Default);
            }
            return gltf.ToGlbBytes();
#elif UNIVRM_0_71_EXPORTER
            var gltf = VRMExporter.Export(MeshExportSettings.Default, vrm, _ => false);
            return gltf.ToGlbBytes();
#elif UNIVRM_0_75_EXPORTER
            var gltf = VRMExporter.Export(MeshExportSettings.Default, vrm, new RuntimeTextureSerializer());
            return gltf.ToGlbBytes();
#elif UNIVRM_0_79_EXPORTER
            var gltf = VRMExporter.Export(new GltfExportSettings(), vrm, new RuntimeTextureSerializer());
            return gltf.ToGlbBytes();
#else
            return null;
#endif
        }

        private async Task UploadAvatarAsync(string avatarName, byte[] vrmData, byte[] thumbnailData)
        {
            dmmVRConnectUI.SetLog("Uploading...");
            await Authentication.Instance.Okami.UploadVRM(avatarName, vrmData, thumbnailData);
            dmmVRConnectUI.SetLog("Upload finished.");
        }
    }
}
