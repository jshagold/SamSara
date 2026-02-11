using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsPopupView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private CanvasGroup _canvasGroup; // 팝업 전체 투명도/제어용
    [SerializeField] private Button _closeButton;
    [SerializeField] private Toggle _soundToggle;

    private void Reset()
    {
        if( _canvasGroup == null ) _canvasGroup = GetComponent<CanvasGroup>();

        if( _closeButton == null ) _closeButton = GetComponentInChildren<Button>();
        if( _soundToggle == null ) _soundToggle = GetComponentInChildren<Toggle>();
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveAllListeners();
        _soundToggle.onValueChanged.RemoveAllListeners();
    }

    // 현재 소리상태 반영해서 Popup열기
    public void OpenPopup(bool isSoundOn)
    {
        gameObject.SetActive(true);
        _soundToggle.isOn = isSoundOn; // 현재 상태로 UI 갱신

        // 애니메이션 효과
        // TODO
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void SetEvents(Action onClosePopup, Action<bool> onSoundToggle)
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(() => onClosePopup.Invoke());

        _soundToggle.onValueChanged.RemoveAllListeners();
        _soundToggle.onValueChanged.AddListener((isOn) => onSoundToggle.Invoke(isOn));
    }
}