using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GlobalBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"{nameof(GlobalBootstrapper)}";

    [SerializeField] private AutoSaveManager _autoSaveManager;
    [SerializeField] private NewGameConfig _newGameConfig;

    // SingleTon 패턴
    public static GlobalBootstrapper Instance { get; private set; }
    public GameContext GameContext { get; private set; }

    // 다른 씬들이 초기화 완료를 기다릴 수 있게 하는 Task
    public UniTask InitializationTask { get; private set; }


    // 게임 초기 데이터 UseCase
    private CreateNewCharacterUseCase _newCharacterUseCase;

    
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

        // MasterData 초기화
        MasterDataManager masterDataManager = new MasterDataManager();
        masterDataManager.Initialize();

        // Repository 생성
        ICharacterRepository characterRepo = new CharacterRepository();
        IInventoryRepository userInventoryRepo = new InventoryRepository();
        IDailyStateRepository gameStateRepo = new DailyStateRepository();

        // GameContext 조립
        GameContext = new GameContext(
            inventoryRepo: userInventoryRepo,
            characterRepo: characterRepo,
            dailyStateRepo: gameStateRepo,
            masterDataManager: masterDataManager
        );

        // 자동저장 연동
        if (_autoSaveManager != null)
        {
            _autoSaveManager.Initialize(GameContext);

            GameContext.InventoryRepo.OnInventoryChanged += () => _autoSaveManager.MakeDirty();
            GameContext.DailyStateRepo.OnDailyStateChanged += () => _autoSaveManager.MakeDirty();
            GameContext.CharacterRepo.OnCharacterUpdated += () => _autoSaveManager.MakeDirty();
            // TODO Repo 추가
        }

        // 초기화 실행
        RetryInitialization();

        Debug.Log("Global Bootstrapper Initialized");
    }

    // 데이터 불러오기 재시도
    public void RetryInitialization()
    {
        InitializationTask = InitializeGameFlowAsync().Preserve();
    }

    private async UniTask InitializeGameFlowAsync()
    {
        // Localization
        await LocalizationSettings.InitializationOperation;
        await LocalizationSettings.StringDatabase.GetTableAsync(LocalizationUtils.TextTableName);

        var initUserDataUC = new InitializeUserDataUseCase(
            gameContext: GameContext,
            newGameConfig: _newGameConfig);
        initUserDataUC.Execute();

        await GameContext.LoadAllDataAsync();
    }
}