using UnityEngine;

public class TopHUDPresenter
{
    private readonly TopHUDView _view;

    // 현재 HUD가 보이는 상태인지
    private bool _isShow = true;
    // 애니메이션 중복 실행 방지 플래그
    private bool _isAnimating = false;

    // 로직
    private readonly GetMoneyUseCase _moneyUseCase;
    private readonly DailyStateUseCase _dailyStateUseCase;

    public TopHUDPresenter(TopHUDView view, GetMoneyUseCase moneyUseCase, DailyStateUseCase dailyStateUseCase)
    {
        _view = view;
        _moneyUseCase = moneyUseCase;
        _dailyStateUseCase = dailyStateUseCase;
    }

    public async void Initialize()
    {
        int money = await _moneyUseCase.GetInventoryMoneyAsync();
        int date = await _dailyStateUseCase.GetCurrentDay();

        _view.OnOptionClicked += HandleOptionClick;
        _view.UpdateDate(date: "Ready");
        _view.UpdateCurreny(amount: money);
    }


    private void HandleOptionClick()
    {
        Debug.Log("[HUD] 옵션 버튼 클릭됨 -> 팝업을 띄우거나 씬 이동");
        _view.UpdateStatus("Paused");
    }

}
