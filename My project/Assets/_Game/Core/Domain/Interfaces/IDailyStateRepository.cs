using Cysharp.Threading.Tasks;

public interface IDailyStateRepository
{
    // =================================================
    // 런타임 로직 (게임 플레이 중 빈번하게 호출)
    // =================================================

    /// <summary>
    /// 현재 Day를 return
    /// </summary> 
    /// <returns>현재 날짜 return - int</returns>
    int GetCurrentDay { get; }

    /// <summary>
    /// 행동력 상태 조회
    /// </summary> 
    /// <param name="charId">캐릭터 Id 값 - int</param>
    /// <returns>캐릭터의 행동 횟수 return - bool[]</returns>
    bool[] GetActionSlot(string charId);

    // =================================================
    // 데이터 영속성 (저장/로딩)
    // =================================================

    // 게임 실행할 때 데이터 읽어와서 메모리 변수에 세팅.
    UniTask LoadDataAsync();
}