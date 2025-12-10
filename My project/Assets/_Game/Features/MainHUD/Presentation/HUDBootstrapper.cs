using UnityEngine;

public class HUDBootstrapper : MonoBehaviour
{
    [SerializeField] private TopHUDView _topView;
    
    private TopHUDPresenter _topHUDpresenter;

    public void Initialize(GameContext gameContext) // MainBootstrapper가 호출해 줌
    {
        _topHUDpresenter = new TopHUDPresenter(
            view: _topView, 
            moneyUseCase: gameContext.GetMoneyUseCase,
            dailyStateUseCase: gameContext.DailyStateUseCase
        );
        _topHUDpresenter.Initialize();

        Debug.Log("HUD 조립 완료");
    }
}