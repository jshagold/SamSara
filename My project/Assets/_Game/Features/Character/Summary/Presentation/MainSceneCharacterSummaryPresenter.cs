using System;
using System.Collections.Generic;

public class MainSceneCharacterSummaryPresenter : IDisposable
{
    private readonly MainSceneCharacterListView _characterListView;
    private readonly GetCharacterSummaryUseCase _characterSummaryUseCase;

    public MainSceneCharacterSummaryPresenter(
        MainSceneCharacterListView characterListView,
        GetCharacterSummaryUseCase charSummaryUseCase)
    {
        _characterListView = characterListView ?? throw new ArgumentNullException(nameof(characterListView));
        _characterSummaryUseCase = charSummaryUseCase ?? throw new ArgumentNullException(nameof(charSummaryUseCase));
    }

    public void Initialize()
    {
        // 1. 초기 데이터 렌더링
        RefreshList();

        // 2. 데이터 변경 감지 (예시: 옵저버 패턴이나 이벤트가 있다면 연결)
        _characterSummaryUseCase.OnCharacterUpdated += RefreshList;
    }

    private void RefreshList()
    {
        List<MainSceneCharacterSummaryInfo> characterData = _characterSummaryUseCase.GetCharacterSummaryList();

        _characterListView.UpdateList(characterData);
    }

    public void Dispose()
    {
        _characterSummaryUseCase.OnCharacterUpdated -= RefreshList;
    }
}