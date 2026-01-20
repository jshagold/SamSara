using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionMenuPopupView : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _accountButton;
    // TODO 추후 다른 버튼 추가

    [Header("Button Texts")]
    [SerializeField] private TextMeshProUGUI _settingText;
    [SerializeField] private TextMeshProUGUI _accountText;

    [Header("Close Area")]
    [SerializeField] private Button _dimmedBackground;  // 팝업 바깥을 눌렀을때 팝업 닫기 효과용

    private void Reset()
    {
        Button[] allButtons = GetComponentsInChildren<Button>(true);

        foreach (Button button in allButtons)
        {
            string objName = button.gameObject.name.ToLower();

            if(_settingButton == null && objName.Contains("setting")) _settingButton = button;
            if(_accountButton == null && objName.Contains("account")) _accountButton = button;
            if(_dimmedBackground == null && objName.Contains("dimmed")) _dimmedBackground = button;
        }

        if(_settingButton != null && _settingText == null) _settingText = _settingButton.GetComponentInChildren<TextMeshProUGUI>();
        if (_accountButton != null && _accountButton == null) _accountText = _accountButton.GetComponentInChildren<TextMeshProUGUI>();        
        
    }

    private void OnDestroy()
    {
        // 1. 유니티 버튼 리스너 정리
        if (_settingButton != null) _settingButton.onClick.RemoveAllListeners();
        if (_dimmedBackground != null) _dimmedBackground.onClick.RemoveAllListeners();
    }

    public void ShowPopup() => gameObject.SetActive(true);
    public void HidePopup() => gameObject.SetActive(false);

    public void SetActions(Action onSettingsClicked, Action onAccountClicked, Action onBackgroundClicked)
    {
        _settingButton.onClick.RemoveAllListeners();
        _settingButton.onClick.AddListener(() => onSettingsClicked.Invoke());

        _accountButton.onClick.RemoveAllListeners();
        _accountButton.onClick.AddListener(() => onAccountClicked.Invoke());

        // 팝업 밖 영역 누르면 메뉴 닫기 기능 연결
        if(_dimmedBackground != null)
        {
            _dimmedBackground.onClick.RemoveAllListeners();
            _dimmedBackground.onClick.AddListener(() => onBackgroundClicked.Invoke());
        }
    }

    public void UpdateTexts(string settingText, string accountText)
    {
        if(_settingText != null) _settingText.text = settingText;
        if(_accountText != null) _accountText.text = accountText;
    }
}
