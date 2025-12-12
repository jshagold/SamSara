using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class DailyStateRepository : IDailyStateRepository
{
    private readonly string _logClass = "[DailyStateRepository]";

    private readonly string _filePath;
    private readonly NewGameConfig _newGameConfig;
    // 메모리에 가지고 있는 데이터
    private DailyStateData _cachedData;

    // 데이터 변경 알림 이벤트
    public event Action OnDailyStateChanged;

    public DailyStateRepository(NewGameConfig newGameConfig)
    {
        _newGameConfig = newGameConfig;
        // 안드로이드/IOS/PC 공용 경로
        _filePath = Path.Combine(Application.persistentDataPath, "save_game_state_data.json");
        
    }

    // -------
    // 데이터 불러오기
    // -------
    public async UniTask<DailyState> LoadDataAsync()
    {
        if(_cachedData != null)
        {
            return _cachedData.ToDomain();
        }

        if (!File.Exists(_filePath))
        {
            Debug.Log($"{_logClass}[LoadDataAsync] 세이브 파일 없어서 새로 생성.");

            var initialMap = new Dictionary<string, bool[]>();
            foreach(var charId in _newGameConfig.StartingCharacterIds)
            {
                bool[] slots = new bool[_newGameConfig.DefaultActionSlots];
                for (int i = 0; i < slots.Length; i++) slots[i] = true;

                initialMap.Add(charId, slots);
            }

            _cachedData = new DailyStateData(currentDay: _newGameConfig.StartDay, characterActionMap: initialMap);
            return _cachedData.ToDomain();
        }

        try
        {
            // 파일 읽기 (I/O는 Thread Pool 에서)
            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_filePath));

            // JsonConvert 사용
            var loadedDto = JsonConvert.DeserializeObject<DailyStateData>(json);

            if (loadedDto == null)
            {
                throw new InvalidOperationException("[LoadDataAsync] 데이터가 null입니다. 파일 손상 의심.");
            }

            _cachedData = loadedDto;

            Debug.Log($"{_logClass} 로드 완료");

            return _cachedData.ToDomain();
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass}[CRITICAL] 로드 실패: {e.Message}");
            throw;
        }
    }

    // -------
    // 데이터 저장 - 비동기
    // -------
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
        catch (Exception e)
        {
            Debug.LogError($"{_logClass}[IO Error] 파일 쓰기 실패!!: {e.Message}");
        }

        Debug.Log("[DailyStateRepository] 저장 완료");
    }

    // 데이터 저장 - 동기 (긴급 저장)
    public void SaveDataSync()
    {
        if (_cachedData == null)
        {
            Debug.LogWarning($"{_logClass}[Save Skip] 로드된 데이터가 없어서 강제 저장 스킵");
            return;
        }

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

    // 현재 날짜 가져오기
    public int GetCurrentDay()
    {
        CheckDataIntegrity();
        return _cachedData.currentDay;
    }

    // 행동력 횟수 가져오기
    public bool[] GetActionSlot(string charId)
    {
        CheckDataIntegrity();

        if (_cachedData.characterActionMap == null)
        {
            throw new InvalidOperationException("[CRITICAL] characterActionMap is NULL!");
        }

        if (_cachedData.characterActionMap.TryGetValue(charId, out bool[] slots))
        {
            return slots;
        }

        throw new InvalidOperationException("[CRITICAL] charId 가 등록되지 않음!");
    }

    // 행동력 횟수 소모
    public void ConsumeActionSlot(string charId, int slotIndex)
    {
        CheckDataIntegrity();

        bool[] slots = _cachedData.characterActionMap[charId];
        if (slotIndex < 0 || slotIndex >= slots.Length)
        {
            throw new IndexOutOfRangeException($"[CRITICAL] 잘못된 슬롯 인덱스입니다. CharID: {charId}, Request: {slotIndex}, Max: {slots.Length - 1}");
        }
        slots[slotIndex] = false;

        _cachedData.characterActionMap[charId] = slots;

        NotifyChanged();
    }



    private void NotifyChanged()
    {
        OnDailyStateChanged?.Invoke();
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