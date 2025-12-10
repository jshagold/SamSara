using Cysharp.Threading.Tasks;
using UnityEngine;

public class GlobalBootstrapper : MonoBehaviour
{
    // SingleTon 패턴
    public static GlobalBootstrapper Instance { get; private set; }

    public GameContext GameContext { get; private set; }

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

        IUserInventoryRepository userInventoryRepo = new UserInventoryRepository();
        IDailyStateRepository gameStateRepo = new DailyStateRepository();

        GameContext = new GameContext(
            inventory: userInventoryRepo,
            dailyStateRepo: gameStateRepo
        );

        Debug.Log("Global Bootstrapper Initialized");
    }

    public async UniTask LoadAllGameDataAsync()
    {
        Debug.Log("게임 데이터 로딩 시작...");

        // 각 Repository의 로딩 함수들을 호출합니다.
        // 이때 await를 바로 걸지 않고 Task(일감)만 받아옵니다.
        var task1 = GameContext.Inverntory.LoadDataAsync();
        var task2 = GameContext.DailyStateRepo.LoadDataAsync();
        // TODO 추가

        // UniTask.WhenAll: 두 작업이 '모두' 끝날 때까지 병렬로 기다립니다.
        // (하나가 1초, 다른 하나가 2초 걸리면 총 2초만 기다림)
        await UniTask.WhenAll(task1, task2);

        Debug.Log("모든 게임 데이터 로딩 완료! (캐시 생성됨)");
    }
}