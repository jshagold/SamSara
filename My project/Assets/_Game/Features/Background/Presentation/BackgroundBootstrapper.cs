using System;
using UnityEngine;

public class BackgroundBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(BackgroundBootstrapper)}]";

    [SerializeField] private MainBackgroundView _mainBackgroundView;

    private MainBackgroundPresenter _backgroundPresenter;

    public void Initialize() // MainSceneBootstrapper가 호출
    {
        _backgroundPresenter = new MainBackgroundPresenter(_mainBackgroundView);
        _backgroundPresenter.Initialize();

        Debug.Log($"{_logClass} 조립 완료");
    }

    private void OnDestroy()
    {
        if (_backgroundPresenter == null)
            throw new InvalidOperationException($"{_logClass} OnDestroy without Initialize - _backgroundPresenter is null.");
        _backgroundPresenter.Dispose();
    }
}