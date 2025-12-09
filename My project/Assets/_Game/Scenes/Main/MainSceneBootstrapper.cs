using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class MainSceneBootstrapper : MonoBehaviour
{
    [Header("UI Bootstrappers")]
    [SerializeField] private HUDBootstrapper _hudBootstrapper;
    [SerializeField] private BackgroundBootstrapper _backgroundBootstrapper;
    [SerializeField] private LobbyBootstrapper _lobbyBootstrapper;

    private void Start() // Global이 Awake에서 초기화될 시간을 주기 위해 Start 권장
    {
        if(GlobalBootstrapper.Instance == null)
        {
            Debug.LogError("GlobalBootstrapper 선언되지 않음");
            return;
        }

        var gameContext = GlobalBootstrapper.Instance.GameContext;

        // Background Presenter 조립
        _backgroundBootstrapper.Initialize();

        // 메인화면 버튼 Bootstrapper
        _lobbyBootstrapper.Initialize();

        // HUD Bootstrapper
        _hudBootstrapper.Initialize(gameContext);



        Debug.Log(">>> MainScene Bootstrapping Start");
    }

    private void OnDestroy()
    {

    }
}
