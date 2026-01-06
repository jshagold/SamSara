using System;
using Cysharp.Threading.Tasks;

public interface ICharacterRepository
{
    EvolutionNodeInfo GetCurrentNode();

    StatGroup GetCurrentStat();

    void UpdateStat(StatGroup stat);

    event Action OnCharacterStatChanged;

    UniTask LoadDataAsync();
    UniTask SaveDataAsync();    // 일반 저장
    void SaveDataSync();        // [긴급] 앱 일시정지(Pause) 시 저장
}