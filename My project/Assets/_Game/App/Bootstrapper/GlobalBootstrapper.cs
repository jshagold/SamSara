using Cysharp.Threading.Tasks;
using UnityEngine;

public class GlobalBootstrapper : MonoBehaviour
{
    [SerializeField] private AutoSaveManager _autoSaveManager;
    [SerializeField] private NewGameConfig _newGameConfig;

    // SingleTon 패턴
    public static GlobalBootstrapper Instance { get; private set; }

    public GameContext GameContext { get; private set; }

    // 다른 씬들이 초기화 완료를 기다릴 수 있게 하는 Task
    public UniTask InitializationTask { get; private set; }

    private void Awake()
    {
        // 중복 생성 방지 (싱글톤 보장)
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Scene이 바뀌어도 파괴되지 않게하는 코드

        MasterDataManager masterDataManager = new MasterDataManager();
        masterDataManager.Initialize();

        IUserInventoryRepository userInventoryRepo = new InventoryRepository(config: _newGameConfig);
        IDailyStateRepository gameStateRepo = new DailyStateRepository(newGameConfig: _newGameConfig);

        GameContext = new GameContext(
            inventory: userInventoryRepo,
            dailyStateRepo: gameStateRepo,
            masterDataManager: masterDataManager
        );

        RetryInitialization();

        if (_autoSaveManager != null)
        {
            _autoSaveManager.Initialize(GameContext);

            GameContext.InventoryRepo.OnInventoryChanged += () => _autoSaveManager.MakeDirty();
            GameContext.DailyStateRepo.OnDailyStateChanged += () => _autoSaveManager.MakeDirty();
            // TODO Repo 추가
        }

        Debug.Log("Global Bootstrapper Initialized");
    }

    // 데이터 불러오기 재시도
    public void RetryInitialization()
    {
        InitializationTask = GameContext.LoadAllDataAsync().Preserve();
    }
}