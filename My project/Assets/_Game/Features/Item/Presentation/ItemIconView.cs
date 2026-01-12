using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemIconView : MonoBehaviour
{
    [Header("Frame")]
    [SerializeField] private GameObject _contentsRoot;
    [SerializeField] private Image _iconFrame;

    [Header("Icon/CountText")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _countText;

    [Header("Interaction")]
    [SerializeField] private Button _iconButton;

    private void Reset()
    {
        Image[] allImages = GetComponentsInChildren<Image>(true);

        foreach (Image image in allImages)
        {
            string objName = image.gameObject.name.ToLower();

            if(_iconFrame == null && objName.Contains("frame")) _iconFrame = image;
            if(_iconImage == null && objName.Contains("image")) _iconImage = image;
        }

        if(_countText == null) _countText = GetComponentInChildren<TextMeshProUGUI>(true);
        if(_iconButton == null) _iconButton = GetComponentInChildren<Button>(true);
    }

    public void SetData(ItemInfo info, Sprite iconImage)
    {
        if(iconImage == null)
        {
            _iconFrame.enabled = true;
            _contentsRoot.SetActive(false);
            return;
        }

        _contentsRoot.SetActive(true);
        _iconFrame.enabled = true;

        _iconImage.sprite = iconImage;
        // TODO ItemType에 따른 Item Frame 나중에 설정해야함.

        bool showCount = info.Count > 0;
        _countText.gameObject.SetActive(showCount);
        if(showCount)  _countText.text = info.Count.ToString();
    }

    public void SetOnClickIcon(Action action)
    {
        _iconButton.onClick.RemoveAllListeners();

        if(action != null)
        {
            _iconButton.onClick.AddListener(() => action.Invoke());
            _iconButton.interactable = true;
        }
        else
        {
            _iconButton.interactable = false;
        }
    }
}