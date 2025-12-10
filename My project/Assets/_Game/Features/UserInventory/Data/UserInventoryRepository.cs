using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;


public class UserInventoryRepository : IUserInventoryRepository
{
    private readonly string _logClass = "[UserInventoryRepository]";

    private readonly string _filePath;
    private readonly NewGameConfig _newGameConfig;
    private UserInventoryData _cachedData;

    public UserInventoryRepository(NewGameConfig config)
    {
        _newGameConfig = config;
        // 안드로이드/IOS/PC 공용 경로
        _filePath = Path.Combine(Application.persistentDataPath, "save_user_inventory_data.json");
    }

    // 정보 가져오기
    public async UniTask<UserInventory> LoadDataAsync()
    {
        if(_cachedData == null)
        {
            return _cachedData.ToDomain();
        }

        if (!File.Exists(_filePath))
        {
            Debug.Log($"{_logClass}[LoadDataAsync] 세이브 파일 없어서 새로 생성.");

            _cachedData = new UserInventoryData(money: _newGameConfig.InitialMoney);

            return _cachedData.ToDomain();
        }

        try
        {
            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_filePath));

            var loadDto = JsonConvert.DeserializeObject<UserInventoryData>(json);
            if (loadDto == null)
            {
                throw new System.InvalidOperationException("[LoadDataAsync] 데이터가 null입니다. 파일 손상 의심.");
            }

            _cachedData = loadDto;

            Debug.Log($"{_logClass} 로드 완료");

            return _cachedData.ToDomain();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{_logClass}[CRITICAL] 로드 실패: {e.Message}");
            throw;
        }
    }

    // 정보 저장
    public async UniTask SaveDataAsync()
    {
        if (_cachedData == null)
        {
            throw new System.InvalidOperationException("[CRITICAL] 저장 실패! 메모리 데이터가 증발했습니다. 이 세션은 오염되었습니다.");
        }

        // 스레드 풀에서 저장. (게임 멈춤 방지)
        try
        {
            await UniTask.RunOnThreadPool(() =>
            {
                string json = JsonConvert.SerializeObject(_cachedData, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{_logClass}[IO Error] 파일 쓰기 실패!!: {e.Message}");
        }

        Debug.Log("[DailyStateRepository] 저장 완료");
    }


    public int GetMoneyAsync()
    {
        // 1. 메모리 값 리턴
        if(_cachedData == null)
        {
            throw new System.InvalidOperationException("[CRITICAL] UserInventoryData is NULL! 데이터가 로드되지 않은 상태에서 접근했습니다.");
        }

        return _cachedData.money;
    }

}
