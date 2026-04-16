using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Domain;
using UnityEngine;

namespace Samsara.Features.Stage.Domain
{
    /// <summary>
    /// 노드 완료 처리 공통 서비스.
    /// StagePresenter (전투 복귀, 이벤트 복귀)에서 공통 호출된다.
    /// Pure C# class. Constructor DI.
    /// </summary>
    public class StageProgressService : IStageProgressService
    {
        private readonly string _logClass = $"[{nameof(StageProgressService)}]";

        private readonly IStageRepository        _stageRepo;
        private readonly ICharacterRunRepository _characterRunRepo;

        public StageProgressService(
            IStageRepository        stageRepo,
            ICharacterRunRepository characterRunRepo)
        {
            _stageRepo        = stageRepo;
            _characterRunRepo = characterRunRepo;
        }

        // ──────────────────────────────────────────────
        // IStageProgressService
        // ──────────────────────────────────────────────

        public async UniTask CompleteNodeAsync(NodeCompletionContext context)
        {
            if (context.IsStageEndNode)
            {
                // 스테이지 클리어 — ClearedStageCount 증가 + 저장.
                // CurrentNodeIndex 갱신은 불필요 (TransitionToStage에서 0으로 리셋됨).
                _stageRepo.IncrementClearedStageCount();
                await _stageRepo.SaveAsync();
                Debug.Log($"{_logClass} CompleteNodeAsync: 끝 노드 {context.NodeIndex} — ClearedStageCount 증가.");
            }
            else
            {
                // 일반 노드 완료 — Day++, AP 리셋, CurrentNodeIndex++ + 저장.
                _characterRunRepo.RunData.Day           += 1;
                _characterRunRepo.RunData.ActionPoints   = _characterRunRepo.RunData.MaxActionPoints;
                _stageRepo.CompleteNode(_stageRepo.RunData.CurrentNodeIndex);
                _characterRunRepo.MarkDirty();
                await UniTask.WhenAll(_stageRepo.SaveAsync(), _characterRunRepo.SaveDataAsync());
                Debug.Log($"{_logClass} CompleteNodeAsync: 노드 {context.NodeIndex} 완료 처리.");
            }
        }
    }
}
