using System;
using System.Collections.Generic;

public class MainSceneCharacterSummaryPresenter : IDisposable
{
    private readonly HUDView _hudView;
    private readonly GetCharacterSummaryUseCase _characterSummaryUseCase;

    public MainSceneCharacterSummaryPresenter(
        HUDView hudView,
        GetCharacterSummaryUseCase charSummaryUseCase)
    {
        _hudView = hudView;
        _characterSummaryUseCase = charSummaryUseCase;
    }

    public void Initialize()
    {
        // 1. 초기 데이터 렌더링
        RefreshList();

        // 2. 데이터 변경 감지 (예시: 옵저버 패턴이나 이벤트가 있다면 연결)
        // _charRepo.OnCharacterDataChanged += RefreshView;
    }

    private void RefreshList()
    {
        List<MainSceneCharacterSummaryInfo> characterData = _characterSummaryUseCase.GetCharacterSummaryList();

        _hudView.UpdateCharacterList(characterData);
    }

    public void Dispose()
    {
        if (_characterSummaryUseCase != null)
        {
            _characterSummaryUseCase.OnCharacterUpdated -= RefreshList;
        }
    }
}