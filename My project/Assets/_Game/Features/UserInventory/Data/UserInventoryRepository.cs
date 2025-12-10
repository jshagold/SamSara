using System;
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

    // 데이터 변경 알림 이벤트
    public event Action OnInventoryChanged;

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

    // 정보 저장 - 비동기
    public async UniTask SaveDataAsync()
    {
        CheckDataIntegrity();

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

    // 긴급 저장 (OnApplicationPause 용) - 동기
    public void SaveDataSync()
    {
        CheckDataIntegrity();

        try
        {
            // 메인 스레드에서 즉시 씀
            string json = JsonConvert.SerializeObject(_cachedData);
            File.WriteAllText(_filePath, json);
            Debug.Log($"{_logClass} 동기 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass} 동기 저장 실패: {e.Message}");
        }
    }

    public int GetMoney()
    {
        CheckDataIntegrity();
        return _cachedData.money;
    }

    public void AddMoney(int amount)
    {
        CheckDataIntegrity();
        if (amount < 0) throw new ArgumentException("음수는 추가할 수 없습니다.");

        _cachedData.money += amount;

        NotifyChanged();
    }


    private void NotifyChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    // 데이터 무결성 체크 (Fail Fast)
    private void CheckDataIntegrity()
    {
        if (_cachedData == null)
        {
            throw new InvalidOperationException($"{_logClass} 데이터가 로드되지 않았습니다! (Bootstrapper 확인 필요)");
        }
    }
}
