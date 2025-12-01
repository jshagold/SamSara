using UnityEngine;

public class MainSceneBootstrapper : MonoBehaviour
{
    [Header("UI Views")]
    [SerializeField] private TopHUDView _topHudView;
    [SerializeField] private MainContentView _mainContentView;

    // Presenter 보관
    private TopHUDPresenter _hudPresenter;
    private MainContentPresenter _contentPresenter;

    private void Awake()
    {
        Debug.Log(">>> MainScene Bootstrapping Start");

        // 1. HUD Presenter 조립
        _hudPresenter = new TopHUDPresenter(_topHudView);
        _hudPresenter.Initialize();

        // 2. Content Presenter 조립
        _contentPresenter = new MainContentPresenter(_mainContentView);
        _contentPresenter.Initialize();

        // 3. (선택) 배경 이미지 등은 로직이 없다면 굳이 Presenter를 안 만들어도 됨
        // 그냥 Unity Hierarchy에 이미지로 깔아두면 끝.
    }
}
