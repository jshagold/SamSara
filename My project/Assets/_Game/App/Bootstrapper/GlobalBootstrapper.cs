using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GlobalBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(GlobalBootstrapper)}]";

    [SerializeField] private AutoSaveManager _autoSaveManager;
    [SerializeField] private NewGameConfig _newGameConfig;
    [SerializeField] private PopupManager _popupManager;

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

        // MasterData: Bootstrapper creates repos (only place allowed to use new – Constitution §2)
        IItemMasterRepository itemMasterRepo = new ItemMasterRepository();
        IQtePatternMasterRepository qtePatternMasterRepo = new QtePatternMasterRepository();
        ISkillMasterRepository skillMasterRepo = new SkillMasterRepository();
        ICharacterMasterRepository characterMasterRepo = new CharacterMasterRepository();
        MasterDataManager masterDataManager = new MasterDataManager(
            itemRepo: itemMasterRepo,
            qtePatternRepo: qtePatternMasterRepo,
            skillRepo: skillMasterRepo,
            characterRepo: characterMasterRepo);
        masterDataManager.Initialize();

        // Runtime Repository 생성
        ICharacterRepository characterRepo = new CharacterRepository();
        IInventoryRepository userInventoryRepo = new InventoryRepository();
        IDailyStateRepository gameStateRepo = new DailyStateRepository();

        // Required [SerializeField] refs: fail fast if missing (Constitution §7)
        if (_autoSaveManager == null)
            throw new System.InvalidOperationException($"{_logClass} _autoSaveManager must be assigned in Inspector.");
        if (_newGameConfig == null)
            throw new System.InvalidOperationException($"{_logClass} _newGameConfig must be assigned in Inspector.");
        if (_popupManager == null)
            throw new System.InvalidOperationException($"{_logClass} _popupManager must be assigned in Inspector.");

        // GameContext 조립
        GameContext = new GameContext(
            inventoryRepo: userInventoryRepo,
            characterRepo: characterRepo,
            dailyStateRepo: gameStateRepo,
            masterDataManager: masterDataManager,
            popupManager: _popupManager
        );

        _autoSaveManager.Initialize(GameContext);
        GameContext.InventoryRepo.OnInventoryChanged += () => _autoSaveManager.MakeDirty();
        GameContext.DailyStateRepo.OnDailyStateChanged += () => _autoSaveManager.MakeDirty();
        GameContext.CharacterRepo.OnCharacterUpdated += () => _autoSaveManager.MakeDirty();
        // TODO Repo 추가

        // 초기화 실행
        RetryInitialization();

        Debug.Log($"{_logClass} Initialized");
    }

    // 데이터 불러오기 재시도
    public void RetryInitialization()
    {
        InitializationTask = InitializeGameFlowAsync().Preserve();
    }

    private async UniTask InitializeGameFlowAsync()
    {
        try
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
        catch (System.Exception e)
        {
            Debug.LogError($"{_logClass} InitializeGameFlowAsync failed: {e}");
            throw;
        }
    }
}