using System;
using UnityEngine;

public class MainSceneBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"{nameof(MainSceneBootstrapper)}";

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
        {
            Debug.LogError("GlobalBootstrapper 선언되지 않음");
            return;
        }

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



            Debug.Log(">>> MainScene Bootstrapping Start");
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
