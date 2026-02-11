using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System;

public class NavigationButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _label;

    public void SetOnClickAction(Action action)
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => action.Invoke());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetVisual(Sprite sprite, string text)
    {
        gameObject.SetActive(true);
        _iconImage.sprite = sprite;
        _label.text = text;
    }
}