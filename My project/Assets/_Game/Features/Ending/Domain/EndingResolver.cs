using Samsara.Features.Character.Domain;
using Samsara.Features.Ending.MasterData;
using UnityEngine;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// TriggerKind 필터 + EndingContext 기반 Condition 평가로 최고 우선순위 EndingSO Id를 결정한다.
    /// Pure C# class. Constructor DI.
    /// </summary>
    public class EndingResolver : IEndingResolver
    {
        private readonly string _logClass = $"[{nameof(EndingResolver)}]";

        private readonly IEndingMasterDataRepository  _endingMasterDataRepo;
        private readonly ICharacterRunRepository      _characterRunRepo;
        private readonly ICharacterAccountRepository  _characterAccountRepo;

        public EndingResolver(
            IEndingMasterDataRepository endingMasterDataRepo,
            ICharacterRunRepository     characterRunRepo,
            ICharacterAccountRepository characterAccountRepo)
        {
            _endingMasterDataRepo = endingMasterDataRepo;
            _characterRunRepo     = characterRunRepo;
            _characterAccountRepo = characterAccountRepo;
        }

        // ──────────────────────────────────────────────
        // TryResolve — TriggerKind 전역 탐색
        // ──────────────────────────────────────────────

        /// <summary>
        /// 주어진 TriggerKind의 EndingSO 중 EndingContext 조건을 만족하는
        /// 최고 우선순위 Id를 반환. 매칭 없으면 null.
        /// </summary>
        public int? TryResolve(EndingTriggerKind trigger, EndingContext context)
        {
            var candidates = _endingMasterDataRepo.GetEndingsByTriggerKind(trigger);

            EndingSO best         = null;
            int      bestPriority = int.MinValue;

            // LINQ 금지 — foreach + 직접 비교 (CLAUDE.md §8)
            foreach (var so in candidates)
            {
                if (!EvaluateAll(so.Conditions, context)) continue;

                if (so.Priority > bestPriority)
                {
                    best         = so;
                    bestPriority = so.Priority;
                }
            }

            if (best == null)
            {
                Debug.Log($"{_logClass} TryResolve: trigger={trigger} — 매칭 EndingSO 없음. 런 계속.");
                return null;
            }

            Debug.Log($"{_logClass} TryResolve: trigger={trigger} → id={best.Id}, title={best.Title}, priority={best.Priority}");
            return best.Id;
        }

        // ──────────────────────────────────────────────
        // TryResolve — 슬롯 후보 배열 탐색
        // ──────────────────────────────────────────────

        /// <summary>
        /// BattleNodeDataSO 슬롯 내 후보 배열만 평가. 매칭 없으면 null.
        /// </summary>
        public int? TryResolve(EndingSO[] candidates, EndingContext context)
        {
            if (candidates == null || candidates.Length == 0) return null;

            EndingSO best         = null;
            int      bestPriority = int.MinValue;

            foreach (var so in candidates)
            {
                if (!EvaluateAll(so.Conditions, context)) continue;

                if (so.Priority > bestPriority)
                {
                    best         = so;
                    bestPriority = so.Priority;
                }
            }

            if (best == null)
            {
                Debug.Log($"{_logClass} TryResolve(slot): 매칭 EndingSO 없음. 런 계속.");
                return null;
            }

            Debug.Log($"{_logClass} TryResolve(slot): id={best.Id}, title={best.Title}, priority={best.Priority}");
            return best.Id;
        }

        // ──────────────────────────────────────────────
        // Condition Evaluation
        // ──────────────────────────────────────────────

        private bool EvaluateAll(EndingCondition[] conditions, in EndingContext context)
        {
            if (conditions == null || conditions.Length == 0) return true;  // 폴백 후보

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

                case EndingConditionType.StageCompleteFlag:
                    return context.IsStageEndNode;

                default:
                    Debug.LogWarning($"{_logClass} 미처리 EndingConditionType: {condition.Type} — true 반환 (안전 기본값)");
                    return true;
            }
        }
    }
}
