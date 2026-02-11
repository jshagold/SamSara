using System;
using UnityEngine;

public class MainSceneBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(MainSceneBootstrapper)}]";

    [Header("UI Bootstrappers")]
    [SerializeField] private HUDBootstrapper _hudBootstrapper;
    [SerializeField] private BackgroundBootstrapper _backgroundBootstrapper;
    [SerializeField] private LobbyBootstrapper _lobbyBootstrapper;

    [Header("Popup Components")]
    [SerializeField] private OptionMenuPopupView _menuPopup;
    [SerializeField] private SettingsPopupView _settingsPopup;

    private async void Start() // Global이 Awake에서 초기화될 시간을 주기 위해 Start 권장
    {
        if (GlobalBootstrapper.Instance == null)
            throw new InvalidOperationException($"{_logClass} GlobalBootstrapper must exist in scene.");

        try
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext = GlobalBootstrapper.Instance.GameContext;

            // Background Presenter 조립
            _backgroundBootstrapper.Initialize();

            // 메인화면 버튼 Bootstrapper
            _lobbyBootstrapper.Initialize();

            // HUD Bootstrapper
            _hudBootstrapper.Initialize(gameContext: gameContext, menuPopup: _menuPopup, settingsPopup: _settingsPopup);



            Debug.Log($"{_logClass} Bootstrapping complete");
        }
        catch (Exception e)
        {
            // TODO 에러팝업띄우기 or 타이틀화면으로 이동
            throw new InvalidOperationException($"{_logClass} 초기화 오류 {e}");
        }
    }

    private void OnDestroy()
    {

    }
}
