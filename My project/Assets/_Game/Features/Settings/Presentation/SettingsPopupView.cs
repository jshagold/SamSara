using UnityEngine;
using UnityEngine.UI;
using System;

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

    public event Action OnCloseClicked;
    public event Action<float> OnBgmChanged;
    public event Action<float> OnSfxChanged;
    public event Action<bool> OnAutoBattleChanged;
    public event Action<bool> OnQteChanged;

    private void Reset()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (_closeButton == null) _closeButton = GetComponentInChildren<Button>();
        if (_bgmSlider == null) _bgmSlider = GetComponentInChildren<Slider>();
    }

    private void Awake()
    {
        if (_closeButton) _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

        if (_bgmSlider) _bgmSlider.onValueChanged.AddListener(val => OnBgmChanged?.Invoke(val));
        if (_sfxSlider) _sfxSlider.onValueChanged.AddListener(val => OnSfxChanged?.Invoke(val));

        if (_autoBattleToggle) _autoBattleToggle.onValueChanged.AddListener(val => OnAutoBattleChanged?.Invoke(val));
        if (_qteToggle) _qteToggle.onValueChanged.AddListener(val => OnQteChanged?.Invoke(val));
    }

    private void OnDestroy()
    {
        if (_closeButton) _closeButton.onClick.RemoveAllListeners();
        if (_bgmSlider) _bgmSlider.onValueChanged.RemoveAllListeners();
        if (_sfxSlider) _sfxSlider.onValueChanged.RemoveAllListeners();
        if (_autoBattleToggle) _autoBattleToggle.onValueChanged.RemoveAllListeners();
        if (_qteToggle) _qteToggle.onValueChanged.RemoveAllListeners();
    }

    public void InitView(SettingsData data)
    {
        if (_bgmSlider) _bgmSlider.value = data.bgmVolume;
        if (_sfxSlider) _sfxSlider.value = data.sfxVolume;
        if (_autoBattleToggle) _autoBattleToggle.isOn = data.autoBattle;
        if (_qteToggle) _qteToggle.isOn = data.qteEnabled;
    }

    public void Open()
    {
        gameObject.SetActive(true);

        if (_canvasGroup)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}