using Samsara.Features.Character.Domain;
using Samsara.Features.Ending.MasterData;
using UnityEngine;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// EndingCandidateSlot 기반 Condition 평가로 최고 우선순위 EndingSO Id를 결정한다.
    /// Pure C# class. Constructor DI.
    /// </summary>
    public class EndingResolver : IEndingResolver
    {
        private readonly string _logClass = $"[{nameof(EndingResolver)}]";

        private readonly ICharacterRunRepository     _characterRunRepo;
        private readonly ICharacterAccountRepository _characterAccountRepo;

        public EndingResolver(
            ICharacterRunRepository     characterRunRepo,
            ICharacterAccountRepository characterAccountRepo)
        {
            _characterRunRepo     = characterRunRepo;
            _characterAccountRepo = characterAccountRepo;
        }

        // ──────────────────────────────────────────────
        // IEndingResolver
        // ──────────────────────────────────────────────

        public int? TryResolve(EndingCandidateSlot slot, EndingContext context)
        {
            if (slot == null || slot.IsEmpty)
            {
                Debug.Log($"{_logClass} TryResolve: 슬롯 null 또는 비어있음 — 매칭 스킵.");
                return null;
            }

            EndingSO matchedEnding   = null;
            int      matchedPriority = int.MinValue;

            // 단일 foreach 패스, LINQ 금지 (§8 GC 최적화)
            foreach (var ending in slot.Candidates)
            {
                if (ending == null) continue;
                if (!EvaluateAll(ending.Conditions, context)) continue;

                if (ending.Priority > matchedPriority)
                {
                    matchedEnding   = ending;
                    matchedPriority = ending.Priority;
                }
            }

            if (matchedEnding != null)
            {
                Debug.Log($"{_logClass} TryResolve: 매칭 성공 — id={matchedEnding.Id}, title={matchedEnding.Title}, priority={matchedEnding.Priority}");
                return matchedEnding.Id;
            }

            if (slot.Fallback != null)
            {
                Debug.Log($"{_logClass} TryResolve: 조건 매칭 없음 → Fallback 사용 — id={slot.Fallback.Id}");
                return slot.Fallback.Id;
            }

            Debug.Log($"{_logClass} TryResolve: 매칭 없음, Fallback 없음 — 런 계속.");
            return null;
        }

        // ──────────────────────────────────────────────
        // Condition Evaluation
        // ──────────────────────────────────────────────

        private bool EvaluateAll(EndingCondition[] conditions, in EndingContext context)
        {
            if (conditions == null || conditions.Length == 0) return true;  // 빈 조건 = 무조건 통과

            foreach (var condition in conditions)
            {
                if (!EvaluateOne(condition, in context)) return false;
            }
            return true;
        }

        private bool EvaluateOne(EndingCondition condition, in EndingContext context)
        {
            switch (condition.Type)
            {
                case EndingConditionType.None:
                    return true;

                case EndingConditionType.EvolutionId:
                    return _characterRunRepo.RunData.EvolutionNodeId == condition.StringValue;

                case EndingConditionType.EventId:
                    return context.EventId.HasValue && context.EventId.Value == condition.IntValue;

                case EndingConditionType.EventResultType:
                    return context.EventResultType.HasValue
                        && (int)context.EventResultType.Value == condition.IntValue;

                default:
                    Debug.LogWarning($"{_logClass} 미처리 EndingConditionType: {condition.Type} — true 반환 (안전 기본값)");
                    return true;
            }
        }
    }
}
