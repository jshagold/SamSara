using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HUDPresenter : IDisposable
{
    private readonly string _logClass = $"[{nameof(HUDPresenter)}]";

    private readonly HUDView _view;
    private readonly GetMoneyUseCase _moneyUseCase;
    private readonly DailyStateUseCase _dailyStateUseCase;

    // --- 상태 변수 ---
    private bool _isShow = true;    // 현재 HUD가 보이는 상태인지
    private bool _isAnimating = false;  // 애니메이션 중복 실행 방지 플래그

    public HUDPresenter(HUDView view, GetMoneyUseCase moneyUseCase, DailyStateUseCase dailyStateUseCase)
    {
        _view = view;
        _moneyUseCase = moneyUseCase;
        _dailyStateUseCase = dailyStateUseCase;
    }

    public void Initialize()
    {
        _view.SetOnClickHUDOnOffBtnAction(HandleOptionClick);

        RefreshUI();

        _moneyUseCase.OnInventoryChanged += RefreshUI;
        _dailyStateUseCase.OnCharacterUpdated += RefreshUI;

        // 이미지 초기 설정
        _view.OnOffButton.SetState(true);
        _view.PlaySlideIn().Forget();
    }

    private void RefreshUI()
    {
        int money = _moneyUseCase.GetInventoryMoneyAsync();
        int date = _dailyStateUseCase.GetCurrentDay();

        _view.UpdateDate(date: date);
        _view.UpdateCurrency(amount: money);
    }
    
    public async void ToggleHUD()
    {
        // 애니메이션 도중에는 입력 무시
        if (_isAnimating) return;
        _isAnimating = true;

        if(_isShow)
        {
            // 보이고 있을때
            _isShow = false;
            _view.OnOffButton.SetState(false);
            await _view.PlaySlideOut();
        }
        else
        {
            // 숨겨져 있을때
            _isShow = true;
            _view.OnOffButton.SetState(true);
            await _view.PlaySlideIn();
        }

        _isAnimating = false;
    }

    private void HandleOptionClick()
    {
        Debug.Log($"{_logClass} 옵션 버튼 클릭 -> 토글");
        ToggleHUD();
    }

    // [Dispose 패턴]
    // Boostrapper의 OnDestroy에서 호출됨
    public void Dispose()
    {
        _moneyUseCase.OnInventoryChanged -= RefreshUI;
        _dailyStateUseCase.OnCharacterUpdated -= RefreshUI;
    }
}
