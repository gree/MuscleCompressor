using DVRSDK.Auth;
using DVRSDK.Auth.Okami.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelectListItem : MonoBehaviour
{
    [SerializeField]
    private Image thumbnailImage;
    [SerializeField]
    private Text apText;
    [SerializeField]
    private Text titleText;

    public Action<AvatarModel> UseButtonAction;

    private AvatarModel avatarModel;

    public async void SetAvatarModel(AvatarModel model)
    {
        avatarModel = model;
        apText.text = $"AP: {model.ap}";
        titleText.text = model.name;
        await GetAvatarThumbnailAsync(model);
    }

    private async Task GetAvatarThumbnailAsync(AvatarModel model)
    {
        if (thumbnailImage == null) return;
        if (model == null) return;
        var imageBinary = await Authentication.Instance.Okami.GetAvatarThumbnailAsync(model);
        if (imageBinary != null)
        {
            var texture = new Texture2D(1, 1);
            texture.LoadImage(imageBinary);
            thumbnailImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }

    public void UseButton_Click()
    {
        UseButtonAction?.Invoke(avatarModel);
    }
}
