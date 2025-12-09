using UnityEngine;

public class HUDBootstrapper : MonoBehaviour
{
    [SerializeField] private TopHUDView _topView;
    
    private TopHUDPresenter _topHUDpresenter;

    public void Initialize(GameContext gameContext) // MainBootstrapper가 호출해 줌
    {
        _topHUDpresenter = new TopHUDPresenter(_topView, gameContext.GetMoneyUseCase);
        _topHUDpresenter.Initialize();

        Debug.Log("HUD 조립 완료");
    }
}