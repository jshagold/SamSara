using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// [STUB] 캐릭터 저장 데이터 Repository. 구체 구현은 Character Feature 스펙에서 진행.
/// </summary>
public class CharacterRepository
{
    private readonly string _logClass = $"[{nameof(CharacterRepository)}]";

    public async UniTask LoadDataAsync()
    {
        await UniTask.CompletedTask;
        Debug.Log($"{_logClass} LoadDataAsync (stub)");
    }

    public void SaveDataSync()
    {
        Debug.Log($"{_logClass} SaveDataSync (stub)");
    }
}
