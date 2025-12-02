using UnityEngine;

public class MainSceneBootstrapper : MonoBehaviour
{
    [Header("UI Views")]
    [SerializeField] private TopHUDView _topHudView;
    [SerializeField] private MainContentView _mainContentView;
    [SerializeField] private MainBackgroundView _backgroundView;

    // Presenter 보관
    private TopHUDPresenter _hudPresenter;
    private MainContentPresenter _contentPresenter;
    private MainBackgroundPresenter _backgroundPresenter;

    private void Awake()
    {
        Debug.Log(">>> MainScene Bootstrapping Start");
        
        // 1. Background Presenter 조립
        _backgroundPresenter = new MainBackgroundPresenter(_backgroundView);
        _backgroundPresenter.Initialize();

        // 2. HUD Presenter 조립
        _hudPresenter = new TopHUDPresenter(_topHudView);
        _hudPresenter.Initialize();

        // 3. Content Presenter 조립
        _contentPresenter = new MainContentPresenter(_mainContentView);
        _contentPresenter.Initialize();

    }

    private void OnDestroy()
    {

    }
}
