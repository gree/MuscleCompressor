using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DVRSDK.Streaming;
using System.Threading.Tasks;
using DVRSDK.Auth;
using DVRSDK.Utilities;
using DVRSDK.Serializer;
using UnityEngine.UI;
using DVRSDK.Auth.Okami.Models;

public class DVRStreamingHandler : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button getStreamButton;
    [SerializeField] private Dropdown dropdown;
    [SerializeField] private InputField urlText;
    [SerializeField] private DVRStreaming streaming;

    private Dictionary<ApiRequestErrors, string> apiRequestErrorMessages = new Dictionary<ApiRequestErrors, string> {
        { ApiRequestErrors.Unknown,"Unknown request error" },
        { ApiRequestErrors.Forbidden, "Request forbidden" },
        { ApiRequestErrors.Unregistered, "User unregistered" },
        { ApiRequestErrors.Unverified, "User email unverified" },
    };

    private List<StreamKeyModel> currentStreamKeys = new List<StreamKeyModel> { };

    private bool disableValueChanged = false;

    private void Start()
    {
        loginButton.onClick.AddListener(() => DoLogin());
        getStreamButton.onClick.AddListener(() => GetStreamKeyAsync());
        dropdown.onValueChanged.AddListener(index => {
            if(index != 0)
            {
                disableValueChanged = true;
                var streamUrl = (dropdown.options[index] as StreamKeyOptionData).Model.url;
                urlText.text = streamUrl;
                streaming.ServerUrl = streamUrl;
            }
        });
        urlText.onValueChanged.AddListener(value => {
            if (disableValueChanged)
            {
                disableValueChanged = false;
                return;
            }
            dropdown.value = 0;
            streaming.ServerUrl = value;
        });
    }

    public void DoLogin()
    {
        var sdkSettings = Resources.Load<SdkSettings>("SdkSettings");
        var client_id = sdkSettings.client_id;
        var config = new DVRAuthConfiguration(client_id, new UnitySettingStore(), new UniWebRequest(), new NewtonsoftJsonSerializer());
        Authentication.Instance.Init(config);

        Authentication.Instance.Authorize(
            openBrowser: (OpenBrowserResponse response) =>
            {
#if UNITY_EDITOR
                Application.OpenURL(response.VerificationUri);
#endif
                Debug.Log("Need Login");
                Debug.Log("https://vrlab.link/");
                Debug.Log("Enter Code " + response.UserCode);
            },
            onAuthSuccess: isSuccess =>
            {
                if (isSuccess)
                {
                    Debug.Log("Login Success!");
                }
                else
                {
                    Debug.Log("Login Failed...");
                }
            },
            onAuthError: exception =>
            {
                Debug.Log("AuthError");
            });
    }

    public async void GetStreamKeyAsync()
    {
        try
        {
            currentStreamKeys = await Authentication.Instance.Okami.GetStreamKeysAsync();
        }
        catch (ApiRequestException ex)
        {
            Debug.LogError(apiRequestErrorMessages[ex.ErrorType]);
        }
        if (currentStreamKeys == null || currentStreamKeys.Count == 0)
        {
            Debug.Log("No stream key found. Please set key on Connect web site.");
        }
        currentStreamKeys?.ForEach(keys => Debug.Log(keys.url));
        dropdown.ClearOptions();

        var list = new List<Dropdown.OptionData>();
        list.Add(new StreamKeyOptionData("Choose URL"));
        list.AddRange(currentStreamKeys.Select(key => new StreamKeyOptionData(key.name, key)));
        dropdown.AddOptions(list);
        dropdown.RefreshShownValue();
    }

    public class StreamKeyOptionData : Dropdown.OptionData
    {
        public StreamKeyOptionData(string text) : base(text) { }
        public StreamKeyOptionData(string text, StreamKeyModel model) : base(text) { Model = model; }

        public StreamKeyModel Model { get; set; }
    }
}
