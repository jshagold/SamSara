using UnityEngine;

public class DashboardBootstrapper : MonoBehaviour
{
    // 소지금 View
    [SerializeField] private CurrencyView _currencyView;

    public void Initialize(DailyStateUseCase dailyStateUseCase, GetMoneyUseCase getMoneyUseCase) // MainBootstrapper가 호출해 줌
    {
        var contentPresenter = new DashboardPresenter(view: _currencyView, dailyStateUseCase: dailyStateUseCase, getMoneyUseCase: getMoneyUseCase);
        contentPresenter.Initialize();

        Debug.Log("Main Contents 조립 완료");
    }
}