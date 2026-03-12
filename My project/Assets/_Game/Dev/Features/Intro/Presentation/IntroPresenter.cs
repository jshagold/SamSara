using System;
using Core.System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroPresenter : IDisposable
{
    private readonly string _logClass = $"[{nameof(IntroPresenter)}]";

    /// <summary>Minimum display time for logo (ms) before showing title.</summary>
    private const int MinLogoDisplayMs = 2000;

    private readonly IntroView _introView;
    private readonly PopupManager _popupManager;
    private readonly IAppLifecycleService _appLifecycle;
    private string _nextSceneName;

    public IntroPresenter(IntroView introView, PopupManager popupManager, IAppLifecycleService appLifecycle)
    {
        _introView = introView ?? throw new ArgumentNullException(nameof(introView));
        _popupManager = popupManager ?? throw new ArgumentNullException(nameof(popupManager));
        _appLifecycle = appLifecycle ?? throw new ArgumentNullException(nameof(appLifecycle));
    }

    public async void Initialize()
    {
        // 1. 초기화면 세팅 (로고 on, 타이틀 off)
        _introView.SetupInitialState();

        // 성공할 때까지 무한 반복
        while (true)
        {
            try
            {
                // 2. 데이터 로딩 (최소 2초 보장)
                var loadTask = GlobalBootstrapper.Instance.InitializationTask;
                var waitTask = UniTask.Delay(MinLogoDisplayMs);

                await UniTask.WhenAll(loadTask, waitTask);

                break;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log($"{_logClass} 데이터 로딩 실패 : {e.Message}");

                bool isRetry = await _popupManager.ShowCommonPopup(
                    title: LocalizationUtils.GetString("common_error_title"),
                    desc: LocalizationUtils.GetString("common_error_network_case1"),
                    firstText: LocalizationUtils.GetString("common_error_retry"),
                    secondText: LocalizationUtils.GetString("common_error_quit_game")
                );

                if (isRetry)
                {
                    // firstButton 입력 (재시도)
                    GlobalBootstrapper.Instance.RetryInitialization();
                    UnityEngine.Debug.Log($"{_logClass} 재시도 시작...");
                    continue;
                }
                else
                {
                    _appLifecycle.Quit();
                    return;
                }
            }
        }

        // 데이터 로딩 성공 이후
        bool isRestartGame = CheckStartOption();

        if (isRestartGame)
        {
            // 재시작
            // TODO
            _nextSceneName = "RestartScene";
        }
        else
        {
            // 일반시작
            // TODO 튜토리얼 넣을지 정해야함.
            _nextSceneName = "MainScene";
        }

        await _introView.TransitionToTitle(isRestart: isRestartGame);

        _introView.SetOnClickStartButton(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene( _nextSceneName );
    }

    private bool CheckStartOption()
    {
        // TODO Repository에서 값 받아와야함

        return false;
    }

    public void Dispose()
    {

    }
}