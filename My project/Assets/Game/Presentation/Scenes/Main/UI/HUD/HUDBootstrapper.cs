using UnityEngine;

public class HUDBootstrapper : MonoBehaviour
{
    [SerializeField] private TopHUDView _topView;

    public void Initialize() // MainBootstrapper가 호출해 줌
    {
        var topPresenter = new TopHUDPresenter(_topView);
        topPresenter.Initialize();

        Debug.Log("HUD 조립 완료");
    }
}