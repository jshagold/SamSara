using UnityEngine;

public class BackgroundBootstrapper : MonoBehaviour
{
    [SerializeField] private MainBackgroundView _mainBackgroundView;

    public void Initialize() // MainBootstrapper가 호출해 줌
    {
        var backgroundPresenter = new MainBackgroundPresenter(_mainBackgroundView);
        backgroundPresenter.Initialize();

        Debug.Log("Background 조립 완료");
    }
}