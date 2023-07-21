using DVRSDK.Auth.Okami.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarListViewManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private GameObject itemPrefab;

    public Action<AvatarModel> SelectModelAction;

    public void UpdateList(List<AvatarModel> models)
    {
        if (content == null) return;
        foreach(Transform child in content.transform)
        {
            Destroy(child);
        }
        foreach(var model in models)
        {
            var item = Instantiate(itemPrefab);
            item.transform.SetParent(content, false);
            var avatarSelectListItem = item.GetComponent<AvatarSelectListItem>();
            avatarSelectListItem.UseButtonAction += UseButton;
            avatarSelectListItem.SetAvatarModel(model);
        }
    }

    private void UseButton(AvatarModel model) => SelectModelAction?.Invoke(model);
}
