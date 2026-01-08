using System;
using Cysharp.Threading.Tasks;

public interface ICharacterRepository
{
    CharacterInfo GetCharacterInfo();

    void ModifyStat(StatGroup stat);

    event Action OnCharacterUpdated;

    UniTask LoadDataAsync();
    UniTask SaveDataAsync();    // 일반 저장
    void SaveDataSync();        // [긴급] 앱 일시정지(Pause) 시 저장
}