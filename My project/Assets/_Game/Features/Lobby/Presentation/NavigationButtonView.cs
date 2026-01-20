using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System;

public class NavigationButtonView : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI label;

    public void SetOnClickAction(Action action)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => action.Invoke());
    }

    // --- 화면 갱신용 함수들 ---
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
        iconImg.sprite = sprite;
        label.text = text;
    }
}