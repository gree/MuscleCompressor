using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DVRSDK.Auth;
using DVRSDK.Avatar;
using DVRSDK.Serializer;
using DVRSDK.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using DVRSDK.Auth.Okami.Models;
using VRStudioLab.Scripts;
using Cysharp.Threading.Tasks;

namespace DVRSDK.Test
{
    public class DMMVRConnectUI : MonoBehaviour
    {
        [SerializeField]
        private string VRMpath = "/StreamingAssets/VRM/AvatarSample_Gvrm";

        //[NonSerialized]
        private GameObject CurrentModel = null;

        [SerializeField]
        private Text statusText;

        [SerializeField]
        private Text codeText;

        [SerializeField]
        private Image thumnbnailTexture;

        [SerializeField]
        private Text userNameText;

        [SerializeField]
        private bool useAutoLogin = false;

        [SerializeField]
        private bool skipAvatarLoad = false;

        [SerializeField]
        private AvatarListViewManager avatarListViewManager;

        [SerializeField]
        private List<UIPanelItem> Panels;

        [SerializeField] 
        private RecordCueHandler _cueHandler;

        [SerializeField]
        private RecordingManager recordingManager;

        public VRMLoader vrmLoader = new VRMLoader();

        private Dictionary<ApiRequestErrors, string> apiRequestErrorMessages = new Dictionary<ApiRequestErrors, string> {
            { ApiRequestErrors.Unknown,"Unknown request error" },
            { ApiRequestErrors.Forbidden, "Request forbidden" },
            { ApiRequestErrors.Unregistered, "User unregistered" },
            { ApiRequestErrors.Unverified, "User email unverified" },
        };

        private CurrentUserModel currentUser = null;

        private readonly string avatarPageUrl = "https://connect.vrlab.dmm.com/user/avatars/";
        private readonly string userSignupPageUrl = "https://connect.vrlab.dmm.com/user/signup/";

        public void ChangePanel(UIPanelType type)
        {
            Panels.ForEach(d => d.PanelObject?.SetActive(false));

            if (type != UIPanelType.Other)
            {
                Panels.FirstOrDefault(d => d.Type == type)?.PanelObject?.SetActive(true);
            }
        }

        public void ChangePanel(GameObject panelObject)
        {
            ChangePanel(UIPanelType.Other);
            panelObject.SetActive(true);
        }


        public void SetLog(string message)
        {
            if (statusText != null) statusText.text = message;

            Debug.Log(message);
        }

        private async void Start()
        {
            ChangePanel(UIPanelType.Login);

            if (useAutoLogin)
            {
                //TryAutoLogin();
            }
            
            //VRMpath = $"{Application.streamingAssetsPath}/VRM/belleURP.vrm";
            if (VRMpath == string.Empty)
                VRMpath = $"{Application.streamingAssetsPath}/VRM/AvatarSample_G.vrm";
            
            LoadLocalVRM();
        }

        private void InitializeAuth()
        {
            var sdkSettings = Resources.Load<SdkSettings>("SdkSettings");
            var client_id = sdkSettings.client_id;
            var config = new DVRAuthConfiguration(client_id, new UnitySettingStore(), new UniWebRequest(), new NewtonsoftJsonSerializer());
            Authentication.Instance.Init(config);
        }

        public async void TryAutoLogin()
        {
            var canAutoLogin = await Authentication.Instance.TryAutoLogin(onAuthSuccess: OnAuthSuccess);

            Debug.Log("LoginTest: " + canAutoLogin);
        }

        public void DoLogin()
        {
            SetLog("Wait a moment, please");

            DisposeCurrentModel();

            Authentication.Instance.Authorize(
                openBrowser: (OpenBrowserResponse response) =>
                {
#if !UNITY_ANDROID
                    Application.OpenURL(response.VerificationUri);
#endif
                    SetLog("");
                    codeText.text = response.UserCode;
                    ChangePanel(UIPanelType.Code);
                },
                onAuthSuccess: OnAuthSuccess,
                onAuthError: exception =>
                {
                    SetLog(exception.Message);
                    ChangePanel(UIPanelType.Login);
                });
        }

        private async void OnAuthSuccess(bool isSuccess)
        {
            if (isSuccess)
            {
                try
                {
                    await GetUserInformation();
                    var avatarList = await GetAvatarList();
                    if (skipAvatarLoad)
                    {
                        SetLog("Login success");
                        ChangePanel(UIPanelType.Logout);
                    }
                    else if (avatarList == null || avatarList.Count == 0)
                    {
                        OnAvatarNotRegistered();
                    }
                    else if (avatarList.Count == 1)
                    {
                        await LoadAvatar(avatarList.First());
                    }
                    else
                    {
                        ShowAvatarList(avatarList);
                    }
                }
                catch (ApiRequestException ex)
                {
                    SetLog(apiRequestErrorMessages[ex.ErrorType]);
                    OnUserNotSignup();
                }
            }
            else
            {
                OnLoginFailed();
            }
        }

        public void DoLogout()
        {
            DisposeCurrentModel();

            Authentication.Instance.DoLogout();
            SetLog("Logout");
            ChangePanel(UIPanelType.Login);
        }

        private async Task<List<AvatarModel>> GetAvatarList()
        {
            SetLog("Loading Avatar List.\nWait a moment,please.");
            try
            {
                var avatarList = await Authentication.Instance.Okami.GetAvatarsAsync();
                return avatarList;
            }
            catch (ApiRequestException ex)
            {
                SetLog(apiRequestErrorMessages[ex.ErrorType]);
            }

            return null;
        }

        private async Task<bool> LoadAvatar(AvatarModel avatarModel)
        {
            SetLog("Loading VRM.\nWait a moment,please.");
            try
            {
                CurrentModel = await Authentication.Instance.Okami.LoadAvatarVRMAsync(avatarModel, vrmLoader.LoadVRMModelFromConnectAsync) as GameObject;
            }
            catch (ApiRequestException ex)
            {
                SetLog(apiRequestErrorMessages[ex.ErrorType]);
            }

            if (CurrentModel != null)
            {
                OnAvatarLoaded(CurrentModel);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ShowAvatarList(List<AvatarModel> models)
        {
            SetLog("Please select avatar");
            avatarListViewManager.SelectModelAction -= SelectModel;
            avatarListViewManager.SelectModelAction += SelectModel;
            avatarListViewManager.UpdateList(models);
            ChangePanel(UIPanelType.AvatarSelect);
        }

        private async void SelectModel(AvatarModel model) => await LoadAvatar(model);

        [ContextMenu("LoadLocalVRM")]
        public async void LoadLocalVRM()
        {
#if UNITY_STANDALONE_WIN
            //var path = WindowsDialogs.OpenFileDialog("Open VRM File", ".vrm");
            
            // StreamingAssets内のVRMを指定
            var path = Application.streamingAssetsPath + VRMpath;
            if (path != null)
            {
                CurrentModel = await vrmLoader.LoadVrmModelFromFileAsync(path);
                OnAvatarLoaded(CurrentModel);

                // avatarにmotion2bytesHumanoidsを適用
                //attachM2B.AttachM2B();
            }

            
#endif
            
        }

        public Action<GameObject> OnAvatarLoadedAction;

        private void OnAvatarLoaded(GameObject model)
        {
            OnAvatarLoadedAction?.Invoke(model);
            SetLog("VRM Loaded");
            ChangePanel(UIPanelType.Logout);
            var m2b = model.AddComponent<Motion2BytesHumanoid>();

            if (recordingManager != null)
            {
                recordingManager.motion2BytesHumanoid = m2b;
            }

            _cueHandler.m2b = m2b;
        }

        private void OnLoginFailed()
        {
            SetLog("Login Failed");
            ChangePanel(UIPanelType.Login);
        }

        private void OnAvatarNotRegistered()
        {
            SetLog("Avatar not registered");
            ChangePanel(UIPanelType.NoAvatar);
        }

        public void OpenAvatarRegisterPage()
        {
            Application.OpenURL(avatarPageUrl);
        }

        private void OnUserNotSignup()
        {
            SetLog("Please user signup");
            ChangePanel(UIPanelType.NoUser);
        }

        public void OpenUserSignupPage()
        {
            Application.OpenURL(userSignupPageUrl);
        }

        public void ShowVRM()
        {
            vrmLoader?.ShowMeshes();
            SetLog("VRM shown");
        }

        public void AddAutoBlink()
        {
            vrmLoader?.AddAutoBlinkComponent();
            SetLog("Autoblink Added");
        }

        public void SetupFirstPerson(Camera firstPersonCamera)
        {
            vrmLoader?.SetupFirstPerson(firstPersonCamera);
        }

        public void DisposeCurrentModel()
        {
            if (CurrentModel != null)
            {
                vrmLoader.Dispose();
                SetLog("VRM Disposed");
            }
        }

        private async Task GetUserInformation()
        {
            currentUser = await Authentication.Instance.Okami.GetCurrentUserAsync();
            if (currentUser != null)
            {
                var imageBinary = await Authentication.Instance.Okami.GetUserThumbnailAsync(currentUser);
                if (imageBinary != null)
                {
                    if (thumnbnailTexture != null)
                    {
                        var texture = new Texture2D(1, 1);
                        texture.LoadImage(imageBinary);
                        thumnbnailTexture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    }
                }

                if (userNameText != null)
                {
                    userNameText.text = currentUser.name;
                }
            }
        }
    }

    public enum UIPanelType
    {
        Login,
        Code,
        Logout,
        NoAvatar,
        NoUser,
        AvatarSelect,
        Other,
    }

    [Serializable]
    public class UIPanelItem
    {
        public UIPanelType Type;
        public GameObject PanelObject;
    }
}
