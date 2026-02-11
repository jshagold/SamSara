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
        _backgroundPresenter?.Dispose();
    }
}