using UnityEngine;

public class MainSceneBootstrapper : MonoBehaviour
{
    [Header("UI Views")]
    [SerializeField] private HUDBootstrapper _hudBootstrapper;
    [SerializeField] private ContentBootstrapper _contentBootstrapper;
    [SerializeField] private BackgroundBootstrapper _backgroundBootstrapper;

    private void Awake()
    {
        Debug.Log(">>> MainScene Bootstrapping Start");

        IUserInventroyRepository userInventoryRepository = new UserInventoryRepository();
        var getMoneyUseCase = new GetMoneyUseCase(userInventoryRepository);


        // 1. Background Presenter 조립
        _backgroundBootstrapper.Initialize();

        // 2. HUD Bootstrapper
        _hudBootstrapper.Initialize(getMoneyUseCase);

        // 3. Content Presenter 조립
        _contentBootstrapper.Initialize();

    }

    private void OnDestroy()
    {

    }
}
