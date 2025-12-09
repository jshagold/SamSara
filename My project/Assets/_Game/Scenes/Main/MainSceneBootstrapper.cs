using UnityEngine;

public class MainSceneBootstrapper : MonoBehaviour
{
    [Header("UI Views")]
    [SerializeField] private HUDBootstrapper _hudBootstrapper;
    [SerializeField] private BackgroundBootstrapper _backgroundBootstrapper;

    private void Start() // Global이 Awake에서 초기화될 시간을 주기 위해 Start 권장
    {
        if(GlobalBootstrapper.Instance == null)
        {
            Debug.LogError("GlobalBootstrapper 선언되지 않음");
            return;
        }

        var gameContext = GlobalBootstrapper.Instance.GameContext;



        // 1. Background Presenter 조립
        _backgroundBootstrapper.Initialize();

        // 2. HUD Bootstrapper
        _hudBootstrapper.Initialize(gameContext);


        Debug.Log(">>> MainScene Bootstrapping Start");
    }

    private void OnDestroy()
    {

    }
}
