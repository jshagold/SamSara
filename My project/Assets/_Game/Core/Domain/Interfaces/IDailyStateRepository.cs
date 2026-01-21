using Cysharp.Threading.Tasks;
using System;

public interface IDailyStateRepository
{
    // =================================================
    // Getter (동기)
    // =================================================

    void InitializeData(DailyStateSaveData initData);
    bool HasSaveData();

    /// <summary>
    /// 현재 Day를 return
    /// </summary> 
    /// <returns>현재 날짜 return - UniTask<int></returns>
    int GetCurrentDay();

    /// <summary>
    /// 행동력 상태 조회
    /// </summary> 
    /// <param name="charId">캐릭터 Id 값 - int</param>
    /// <returns>캐릭터의 행동 횟수 return - bool[]</returns>
    bool[] GetActionSlot(int charId);

    // =================================================
    // Command (동기)
    // =================================================
    void ConsumeActionSlot(int charId, int slotIndex);

    event Action OnDailyStateChanged;

    // =================================================
    // I/O (비동기)
    // =================================================

    // 게임 실행할 때 데이터 읽어와서 메모리 변수에 세팅.
    UniTask<DailyStateInfo> LoadDataAsync();

    UniTask SaveDataAsync();
    void SaveDataSync();
}