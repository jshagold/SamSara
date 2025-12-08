using UnityEngine;

public class HUDBootstrapper : MonoBehaviour
{
    [SerializeField] private TopHUDView _topView;
    

    public void Initialize(GetMoneyUseCase getMoneyUseCase) // MainBootstrapper가 호출해 줌
    {
        var topPresenter = new TopHUDPresenter(_topView, getMoneyUseCase);
        topPresenter.Initialize();

        Debug.Log("HUD 조립 완료");
    }
}