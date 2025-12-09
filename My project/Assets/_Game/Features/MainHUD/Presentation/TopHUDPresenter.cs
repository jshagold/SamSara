using UnityEngine;

public class TopHUDPresenter
{
    private readonly TopHUDView _view;
    

    // 로직
    private readonly GetMoneyUseCase _moneyUseCase;

    public TopHUDPresenter(TopHUDView view, GetMoneyUseCase moneyUseCase)
    {
        _view = view;
        _moneyUseCase = moneyUseCase;
    }

    public async void Initialize()
    {
        int money = await _moneyUseCase.GetInventoryMoneyAsync();

        _view.OnOptionClicked += HandleOptionClick;
        _view.UpdateStatus("Ready");
        _view.UpdateMoney(money.ToString());
    }


    private void HandleOptionClick()
    {
        Debug.Log("[HUD] 옵션 버튼 클릭됨 -> 팝업을 띄우거나 씬 이동");
        _view.UpdateStatus("Paused");
    }

}
