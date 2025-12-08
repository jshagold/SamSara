using UnityEngine;

public class GlobalBootstrapper : MonoBehaviour
{
    public static GameContext Context { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        var userInventoryRepo = new UserInventoryRepository();

        Context = new GameContext(inventory: userInventoryRepo);
    }
}