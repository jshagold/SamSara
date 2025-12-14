using System;
using UnityEngine;

public class MainBackgroundPresenter : IDisposable
{
    private readonly MainBackgroundView _view;

    public MainBackgroundPresenter(MainBackgroundView view)
    {
        _view = view;
    }

    public void Initialize()
    {
        // 초기 상태는 '낮'으로 설정
        ChangeToDay();
    }

    public void Dispose()
    {

    }

    public void ChangeToDay()
    {
        Debug.Log("배경 변경: 낮");
        _view.SetBackground(BackgroundType.Day);
    }


    // 조건에 따라 변경하는 로직 예시
    public void ChangeByCondition(bool isHardMode)
    {
        if (isHardMode)
            _view.SetBackground(BackgroundType.Day);
        else
            _view.SetBackground(BackgroundType.Day);
    }
}
