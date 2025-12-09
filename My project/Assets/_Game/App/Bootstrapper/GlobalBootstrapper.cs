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
}