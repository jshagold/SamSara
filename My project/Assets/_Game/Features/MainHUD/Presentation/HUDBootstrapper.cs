using UnityEngine;

public class HUDBootstrapper : MonoBehaviour
{
    [SerializeField] private HUDView _hudView;
    
    private HUDPresenter _hudPresenter;

    public void Initialize(GameContext gameContext)
    {
        _hudPresenter = new HUDPresenter(
            view: _hudView, 
            moneyUseCase: gameContext.GetMoneyUseCase,
            dailyStateUseCase: gameContext.DailyStateUseCase
        );

        _hudPresenter.Initialize();

        Debug.Log("HUD 조립 완료");
    }
}