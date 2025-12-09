using UnityEngine;

public class DashboardPresenter
{
    private readonly CurrencyView _view;

    // 로직
    private readonly DailyStateUseCase _dailyStateUseCase;
    private readonly GetMoneyUseCase _moneyUseCase;

    public DashboardPresenter(CurrencyView view, DailyStateUseCase dailyStateUseCase, GetMoneyUseCase getMoneyUseCase)
    {
        _view = view;
        _dailyStateUseCase = dailyStateUseCase;
        _moneyUseCase = getMoneyUseCase;
    }

    public async void Initialize()
    {
        int currentMoney = await _moneyUseCase.GetInventoryMoneyAsync();
        _view.SetMoneyText(currentMoney);

        Debug.Log("DashboardPresenter Initialized");
    }
}
