using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DailyStateRepository : IDailyStateRepository
{
    private readonly string _filePath;

    public DailyStateRepository()
    {
        // 안드로이드 내부 저장소 경로
        _filePath = Path.Combine(Application.persistentDataPath, "save_game_state_data.json");
    }

    // 메모리에 가지고 있는 데이터
    private DailtyStateData _cachedData;

    // 현재 날짜 가져오기
    public int GetCurrentDay => _cachedData.CurrentDay;

    // 행동력 횟수 소모
    public void ConsumeActionSlot(string charId, int slotIndex)
    {
        if(_cachedData.CharacterActionMap.ContainsKey(charId))
        {
            _cachedData.CharacterActionMap[charId][slotIndex] = false;
        }

        // 주의: 여기서 매번 SaveDataAsync()를 호출하면 느려집니다.
        // 보통은 AutoSave 매니저가 따로 있거나, 턴 종료 시점에 모아서 저장합니다.
    }

    // 행동력 횟수 가져오기
    public bool[] GetActionSlot(string charId)
    {
        return _cachedData.CharacterActionMap[charId];
    }

    // 데이터 불러오기
    public async UniTask LoadDataAsync()
    {
        string json = await File.ReadAllTextAsync(_filePath);
        _cachedData = JsonUtility.FromJson<DailtyStateData>(json);
    }

    // 데이터 저장하기
    public async UniTask SaveDataAsync()
    {
        string json = JsonUtility.ToJson(_cachedData);
        await File.WriteAllTextAsync(_filePath, json);
    }
}