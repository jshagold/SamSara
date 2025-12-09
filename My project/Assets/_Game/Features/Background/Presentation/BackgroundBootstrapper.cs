using UnityEngine;

public class BackgroundBootstrapper : MonoBehaviour
{
    [SerializeField] private MainBackgroundView _mainBackgroundView;

    private MainBackgroundPresenter _backgroundPresenter;

    public void Initialize() // MainBootstrapper가 호출해 줌
    {
        _backgroundPresenter = new MainBackgroundPresenter(_mainBackgroundView);
        _backgroundPresenter.Initialize();

        Debug.Log("Background 조립 완료");
    }
}