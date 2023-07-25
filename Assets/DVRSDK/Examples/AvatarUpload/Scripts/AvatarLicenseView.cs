using DVRSDK.Avatar;
using DVRSDK.Test;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class AvatarLicenseView : MonoBehaviour
{
    [SerializeField]
    private RawImage ThumbnailTexture;

    [SerializeField]
    private Text TitleText;
    [SerializeField]
    private Text VersionText;
    [SerializeField]
    private Text AuthorText;
    [SerializeField]
    private Text ReferenceText;
    [SerializeField]
    private Text ContactInformationText;

    [SerializeField]
    private Text AllowedUserMarkText;
    [SerializeField]
    private Text AllowedUserText;
    [SerializeField]
    private Text ViolentUssageMarkText;
    [SerializeField]
    private Text ViolentUssageText;
    [SerializeField]
    private Text SexualUssageMarkText;
    [SerializeField]
    private Text SexualUssageText;
    [SerializeField]
    private Text CommercialUssageMarkText;
    [SerializeField]
    private Text CommercialUssageText;

    [SerializeField]
    private Text OtherPermissionUrlText;
    [SerializeField]
    private Text LicenseTypeText;
    [SerializeField]
    private Text OtherLicenseUrlText;

    [SerializeField]
    private Button AgreeButton;
    [SerializeField]
    private Button BackButton;

    private Action<VRMMetaObject> AgreeAction;
    private Action BackAction;

    private VRMMetaObject currentVRMMetaObject;

    private VRMLoader vrmLoader;

    static string[] AllowedUserTexts = { "only creator", "authorized only", "anyone" };
    static string[] AllowedUserMarks = { "×", "△", "○" };
    static string[] UssageTexts = { "Disallow", "Allow" };
    static string[] UssageMarks = { "×", "○" };
    static string[] LicenseTypeTexts = {
        "Redistribution Prohibited",
        "CC0",
        "CC BY",
        "CC BY-NC",
        "CC BY-SA",
        "CC BY-NC-SA",
        "CC BY-ND",
        "CC BY-NC-ND",
        "Other"
    };

    private void Awake()
    {
        AgreeButton.onClick.RemoveAllListeners();
        AgreeButton.onClick.AddListener(() =>
        {
            AgreeAction?.Invoke(currentVRMMetaObject);
        });

        BackButton.onClick.RemoveAllListeners();
        BackButton.onClick.AddListener(() =>
        {
            BackAction?.Invoke();
        });
    }

    private void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
        if (vrmLoader != null) vrmLoader.Dispose();
    }

    private void Initialize(Action<VRMMetaObject> agreeAction, Action backAction)
    {
        AgreeAction = agreeAction;
        BackAction = backAction;

        vrmLoader?.Dispose();
        vrmLoader = new VRMLoader();
    }

    public void ShowPanelFromPrefab(GameObject prefab, Action<VRMMetaObject> agreeAction, Action backAction)
    {
        var vrmMeta = prefab.GetComponent<VRMMeta>();
        if (vrmMeta == null) throw new InvalidOperationException("prefab is not VRM");

        Initialize(agreeAction, backAction);

        ImportMeta(vrmMeta.Meta);

        ShowPanel();
    }

    public void ShowPanelFromBinary(byte[] vrmByteArray, Action<VRMMetaObject> agreeAction, Action backAction)
    {
        Initialize(agreeAction, backAction);

        ImportMeta(vrmLoader.LoadVrmMetaFromByteArray(vrmByteArray, true));

        ShowPanel();
    }

    public async Task ShowPanelFromBinaryAsync(byte[] vrmByteArray, Action<VRMMetaObject> agreeAction, Action backAction)
    {
        Initialize(agreeAction, backAction);

        ImportMeta(await vrmLoader.LoadVrmMetaFromByteArrayAsync(vrmByteArray, true));

        ShowPanel();
    }

    public void ShowPanelFromFile(string vrmFilePath, Action<VRMMetaObject> agreeAction, Action backAction)
    {
        Initialize(agreeAction, backAction);

        ImportMeta(vrmLoader.LoadVrmMetaFromFile(vrmFilePath, true));

        ShowPanel();
    }

    public async Task ShowPanelFromFileAsync(string vrmFilePath, Action<VRMMetaObject> agreeAction, Action backAction)
    {
        Initialize(agreeAction, backAction);

        ImportMeta(await vrmLoader.LoadVrmMetaFromFileAsync(vrmFilePath, true));

        ShowPanel();
    }

    private void ImportMeta(VRMMetaObject meta)
    {
        currentVRMMetaObject = meta;

        ThumbnailTexture.texture = meta.Thumbnail;
        TitleText.text = meta.Title;
        VersionText.text = meta.Version;
        AuthorText.text = meta.Author;
        ReferenceText.text = meta.Reference;
        ContactInformationText.text = meta.ContactInformation;

        AllowedUserMarkText.text = AllowedUserMarks[(int)meta.AllowedUser];
        AllowedUserText.text = AllowedUserTexts[(int)meta.AllowedUser];
        ViolentUssageMarkText.text = UssageMarks[(int)meta.ViolentUssage];
        ViolentUssageText.text = UssageTexts[(int)meta.ViolentUssage];
        SexualUssageMarkText.text = UssageMarks[(int)meta.SexualUssage];
        SexualUssageText.text = UssageTexts[(int)meta.SexualUssage];
        CommercialUssageMarkText.text = UssageMarks[(int)meta.CommercialUssage];
        CommercialUssageText.text = UssageTexts[(int)meta.CommercialUssage];

        OtherPermissionUrlText.text = meta.OtherPermissionUrl;
        LicenseTypeText.text = LicenseTypeTexts[(int)meta.LicenseType];
        OtherLicenseUrlText.text = meta.OtherLicenseUrl;
    }
}
