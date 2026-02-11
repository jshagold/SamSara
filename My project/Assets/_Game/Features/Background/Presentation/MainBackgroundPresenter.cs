using System;
using UnityEngine;

public class MainBackgroundPresenter : IDisposable
{
    private readonly string _logClass = $"[{nameof(MainBackgroundPresenter)}]";
    private readonly MainBackgroundView _view;

    public MainBackgroundPresenter(MainBackgroundView view)
    {
        _view = view;
    }

    public void Initialize()
    {
        ChangeToDay();
    }

    public void ChangeToDay()
    {
        Debug.Log($"{_logClass} 배경: 낮");
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

    public void Dispose()
    {

    }
}
