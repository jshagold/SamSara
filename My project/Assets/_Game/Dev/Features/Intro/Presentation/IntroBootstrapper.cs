using System;
using UnityEngine;

public class IntroBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(IntroBootstrapper)}]";

    [SerializeField] private IntroView _introView;

    private IntroPresenter _introPresenter;

    private void Start()
    {
        if (GlobalBootstrapper.Instance == null)
            throw new InvalidOperationException($"{_logClass} GlobalBootstrapper must exist in scene.");
        var gameContext = GlobalBootstrapper.Instance.GameContext;
        var appLifecycle = GlobalBootstrapper.Instance.AppLifecycleService;

        _introPresenter = new IntroPresenter(
            introView: _introView,
            popupManager: gameContext.PopupManager,
            appLifecycle: appLifecycle);

        _introPresenter.Initialize();
    }

    private void OnDestroy()
    {
        _introPresenter?.Dispose();
    }
}