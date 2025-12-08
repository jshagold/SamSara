using Cysharp.Threading.Tasks;

public interface IGameStateRepository
{
    // =================================================
    // 런타임 로직 (게임 플레이 중 빈번하게 호출)
    // =================================================

    // 현재 Day를 return 
    int GetCurrentDay { get; }

    // 행동력 소모
    void ConsumeActionSlot(int charId, int slotIndex);

    // 행동력 상태 조회
    bool[] GetActionSlot(int charId);

    // =================================================
    // 데이터 영속성 (저장/로딩)
    // =================================================

    // 게임 실행할 때 데이터 읽어와서 메모리 변수에 세팅.
    UniTask LoadDataAsync();

    UniTask SaveDataAsync();
}