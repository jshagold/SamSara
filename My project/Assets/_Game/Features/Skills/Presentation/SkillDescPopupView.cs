using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDescPopupView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private Image _iconFrame;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _desc;
    [SerializeField] private Button _btnClose;
    [SerializeField] private Button _btnBackgroundDim;

    private void Reset()
    {
        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            string objName = image.gameObject.name.ToLower();
            if(_iconFrame == null && objName.Contains("frame")) _iconFrame = image;
            if(_iconImage == null && objName.Contains("icon")) _iconImage = image;
        }

        TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach(TextMeshProUGUI text in allTexts)
        {
            string objName = text.gameObject.name.ToLower();
            if(_name == null && objName.Contains("name")) _name = text;
            if(_desc == null && objName.Contains("desc")) _desc = text;
        }

        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach(Button btn in buttons)
        {
            string objName = btn.gameObject.name.ToLower();
            if (_btnClose == null && objName.Contains("close")) _btnClose = btn;
            if (_btnBackgroundDim == null && objName.Contains("dimmed")) _btnBackgroundDim = btn;
        }
    }

    private void Awake()
    {
        ClosePopup();
    }

    public void SetActions(Action onClickClose)
    {
        _btnClose.onClick.RemoveAllListeners();
        _btnClose.onClick.AddListener(() =>
        {
            ClosePopup();
            onClickClose?.Invoke();
        });

        _btnBackgroundDim.onClick.RemoveAllListeners();
        _btnBackgroundDim.onClick.AddListener(() =>
        {
            ClosePopup();
            onClickClose?.Invoke();
        });
    }

    public void OpenPopup(string name, string desc, Sprite iconSprite, Sprite frameSprite)
    {
        _name.text = name;
        _desc.text = desc;

        _iconImage.sprite = iconSprite;
        _iconFrame.sprite = frameSprite;

        _panelRoot.SetActive(true);
    }

    public void ClosePopup()
    {
        if (_panelRoot != null) _panelRoot.SetActive(false);
        else gameObject.SetActive(false);
    }
}