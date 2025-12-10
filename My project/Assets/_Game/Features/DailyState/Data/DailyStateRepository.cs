using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class DailyStateRepository : IDailyStateRepository
{
    private readonly string _filePath;

    public DailyStateRepository()
    {
        // 안드로이드 내부 저장소 경로
        _filePath = Path.Combine(Application.persistentDataPath, "save_game_state_data.json");
    }

    // 메모리에 가지고 있는 데이터
    private DailyStateData _cachedData;

    // 현재 날짜 가져오기
    public async UniTask<int> GetCurrentDay()
    {
        // 1. 메모리 값 리턴
        if (_cachedData != null)
        {
            return _cachedData.CurrentDay;
        }

        // 2. 저장 파일 없을때 새로운 객체 리턴
        if (!File.Exists(_filePath))
        {
            _cachedData = new DailyStateData(currentDay: 0, characterActionMap: new Dictionary<string, bool[]>());
        }
        // 3. 저장 파일 데이터 캐싱하고 리턴
        else
        {
            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_filePath));
            var dto = JsonUtility.FromJson<DailyStateData>(json);
            _cachedData = dto;
        }

        return _cachedData.CurrentDay;
    }

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
    public async UniTask<DailyStateData> LoadDataAsync()
    {
        //string json = await File.ReadAllTextAsync(_filePath);
        //_cachedData = JsonUtility.FromJson<DailyStateData>(json);

        if (!File.Exists(_filePath))
        {
            return new DailyStateData(currentDay: 0, characterActionMap: new Dictionary<string, bool[]>());
        }

        // 파일 읽기 (I/O는 Thread Pool 에서)
        string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_filePath));

        // JSON -> DTO
        var dto = JsonUtility.FromJson<DailyStateData>(json);
        _cachedData = dto;

        // DTO -> Entity
        return dto.ToDomain();
    }

    // 데이터 저장하기
    public async UniTask SaveDataAsync()
    {
        string json = JsonUtility.ToJson(_cachedData);
        await File.WriteAllTextAsync(_filePath, json);
    }
}