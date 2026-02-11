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

        _introPresenter = new IntroPresenter(
            introView: _introView,
            popupManager: gameContext.PopupManager);

        _introPresenter.Initialize();
    }

    private void OnDestroy()
    {
        if (_introPresenter == null)
            throw new InvalidOperationException($"{_logClass} OnDestroy without Initialize - _introPresenter is null.");
        _introPresenter.Dispose();
    }
}