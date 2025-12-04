using UnityEngine;

public class ContentBootstrapper : MonoBehaviour
{
    [SerializeField] private MainContentView _mainContentView;

    public void Initialize() // MainBootstrapper가 호출해 줌
    {
        var contentPresenter = new MainContentPresenter(_mainContentView);
        contentPresenter.Initialize();

        Debug.Log("Main Contents 조립 완료");
    }
}