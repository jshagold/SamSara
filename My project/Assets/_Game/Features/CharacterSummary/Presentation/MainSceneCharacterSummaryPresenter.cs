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

    public void Dispose()
    {

    }

    private void RefreshList()
    {
        // DTO 생성 (Repository에서 가져온 데이터로 조립)
        // 예시 데이터입니다. 실제 로직에 맞게 연결하세요.
        List<MainSceneCharacterSummaryDto> characterData = _characterSummaryUseCase.GetCharacterSummaryList();

        _hudView.UpdateCharacterList(characterData);
    }
}