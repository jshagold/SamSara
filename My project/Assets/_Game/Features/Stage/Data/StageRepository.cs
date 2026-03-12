using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// [STUB] 스테이지 저장 데이터 Repository. 구체 구현은 Stage Feature 스펙에서 진행.
/// </summary>
public class StageRepository
{
    private readonly string _logClass = $"[{nameof(StageRepository)}]";

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
