﻿using Cysharp.Threading.Tasks;
using UnityEngine;

public class HUDPresenter
{
    private readonly HUDView _view;

    // 로직
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

    public async void Initialize()
    {
        _view.SetOnClickOptionBtnAction(HandleOptionClick);

        int money = await _moneyUseCase.GetInventoryMoneyAsync();
        int date = _dailyStateUseCase.GetCurrentDay();

        _view.UpdateDate(date: date);
        _view.UpdateCurreny(amount: money);

        _view.PlaySlideIn().Forget();
    }

    
    public async void ToggleHUD()
    {
        // 애니메이션 도중에는 입력 무시
        if (_isAnimating) return;
        _isAnimating = true;

        if(_isShow)
        {
            // 보이고 있을때
            await _view.PlaySlideOut();
            _isShow = false;
        }
        else
        {
            // 숨겨져 있을때
            await _view.PlaySlideIn();
            _isShow = true;
        }

        _isAnimating = false;
    }


    private void HandleOptionClick()
    {
        Debug.Log("[HUD] 옵션 버튼 클릭됨 -> 팝업을 띄우거나 씬 이동");
        
        // TODO 토글 테스트
        ToggleHUD();
    }

}
