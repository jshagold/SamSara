using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class UserInventoryRepository : IUserInventoryRepository
{
    private readonly string _filePath;

    public UserInventoryRepository()
    {
        // 안드로이드 내부 저장소 경로
        _filePath = Path.Combine(Application.persistentDataPath, "save_user_inventory_data.json");
    }


    // 메모리에 가지고 있는 데이터
    private UserInventoryData _cachedData;

    public async UniTask<int> GetMoneyAsync()
    {
        // 1. 메모리 값 리턴
        if(_cachedData != null)
        {
            return _cachedData.money;
        }

        // 2. 저장 파일 없을때 새로운 객체 리턴
        if (!File.Exists(_filePath))
        {
            _cachedData = new UserInventoryData(money: 0);
        }
        // 3. 저장 파일 데이터 캐싱하고 리턴
        else
        {
            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_filePath));
            var dto = JsonUtility.FromJson<UserInventoryData>(json);
            _cachedData = dto;
        }

        return _cachedData.money;
    }


    // 인벤토리 정보 가져오기
    public async UniTask<UserInventory> LoadInventoryAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new UserInventory(money: 0);
        }

        // 파일 읽기 (I/O는 Thread Pool 에서)
        string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_filePath));

        // JSON -> DTO
        var dto = JsonUtility.FromJson<UserInventoryData>(json);
        _cachedData = dto;

        // DTO -> Entity
        return dto.ToDomain();
    }

    // 인벤토리 정보 저장
    public async UniTask SaveUserInventoryAsync(UserInventory inventory)
    {
        // Entity -> DTO
        var dto = inventory.ToData();

        // DTO -> JSON
        string json = JsonUtility.ToJson(dto, true);

        // 파일 쓰기
        await UniTask.RunOnThreadPool(() => File.WriteAllText(_filePath, json));

        Debug.Log($"로컬 파일 저장완료 : {_filePath}");
    }
}
