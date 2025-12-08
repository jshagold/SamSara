using UnityEngine;

public class MainContentPresenter
{
    private readonly MainContentView _view;

    public MainContentPresenter(MainContentView view)
    {
        _view = view;
    }

    public void Initialize()
    {
        _view.AddStartListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        Debug.Log("[Content] 게임 시작 버튼 클릭됨");
        // 도메인 로직 호출: GameStartUseCase.Execute();
    }
}
