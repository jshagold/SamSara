using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameStateRepository : IGameStateRepository
{
    private readonly string _filePath;

    public GameStateRepository()
    {
        // 안드로이드 내부 저장소 경로
        _filePath = Path.Combine(Application.persistentDataPath, "save_game_state_data.json");
    }

    // 메모리에 가지고 있는 데이터
    private GameStateData _cachedData;

    public int GetCurrentDay => _cachedData.CurrentDay;

    public void ConsumeActionSlot(int charId, int slotIndex)
    {
        if(_cachedData.CharacterActionMap.ContainsKey(charId))
        {
            _cachedData.CharacterActionMap[charId][slotIndex] = false;
        }

        // 주의: 여기서 매번 SaveDataAsync()를 호출하면 느려집니다.
        // 보통은 AutoSave 매니저가 따로 있거나, 턴 종료 시점에 모아서 저장합니다.
    }

    public bool[] GetActionSlot(int charId)
    {
        return _cachedData.CharacterActionMap[charId];
    }

    public async UniTask LoadDataAsync()
    {
        string json = await File.ReadAllTextAsync(_filePath);
        _cachedData = JsonUtility.FromJson<GameStateData>(json);
    }

    public async UniTask SaveDataAsync()
    {
        string json = JsonUtility.ToJson(_cachedData);
        await File.WriteAllTextAsync(_filePath, json);
    }
}