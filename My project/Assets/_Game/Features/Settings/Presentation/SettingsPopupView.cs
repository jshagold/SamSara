using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsPopupView : MonoBehaviour
{
    [Header("Window Control")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _closeButton;

    [Header("Volume Controls")]
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Game Options")]
    [SerializeField] private Toggle _autoBattleToggle;
    [SerializeField] private Toggle _qteToggle;

    private void Reset()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (_closeButton == null) _closeButton = GetComponentInChildren<Button>();
        if (_bgmSlider == null) _bgmSlider = GetComponentInChildren<Slider>();
    }

    private void OnDestroy()
    {
        if (_closeButton) _closeButton.onClick.RemoveAllListeners();
        if (_bgmSlider) _bgmSlider.onValueChanged.RemoveAllListeners();
        if (_sfxSlider) _sfxSlider.onValueChanged.RemoveAllListeners();
        if (_autoBattleToggle) _autoBattleToggle.onValueChanged.RemoveAllListeners();
        if (_qteToggle) _qteToggle.onValueChanged.RemoveAllListeners();
    }

    public void SetEvents(
        UnityAction onClose,
        UnityAction<float> onBgmChanged,
        UnityAction<float> onSfxChanged,
        UnityAction<bool> onAutoBattleChanged,
        UnityAction<bool> onQteChanged
    )
    {
        _closeButton.onClick.RemoveAllListeners();
        _bgmSlider.onValueChanged.RemoveAllListeners();
        _sfxSlider.onValueChanged.RemoveAllListeners();
        _autoBattleToggle.onValueChanged.RemoveAllListeners();
        _qteToggle.onValueChanged.RemoveAllListeners();

        _closeButton.onClick.AddListener(onClose);
        _bgmSlider.onValueChanged.AddListener(onBgmChanged);
        _sfxSlider.onValueChanged.AddListener(onSfxChanged);
        _autoBattleToggle.onValueChanged.AddListener(onAutoBattleChanged);
        _qteToggle.onValueChanged.AddListener(onQteChanged);
    }

    public void InitView(SettingsData data)
    {
        if (_bgmSlider) _bgmSlider.value = data.bgmVolume;
        if (_sfxSlider) _sfxSlider.value = data.sfxVolume;
        if (_autoBattleToggle) _autoBattleToggle.isOn = data.autoBattle;
        if (_qteToggle) _qteToggle.isOn = data.qteEnabled;
    }

    public void OpenPopup()
    {
        gameObject.SetActive(true);
        if (_canvasGroup)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    public void ClosePopup() => gameObject.SetActive(false);
}