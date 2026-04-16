using System;
using Samsara.Features.Character.Domain;
using Samsara.Features.Ending.MasterData;
using UnityEngine;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 현재 플레이어 상태에 따라 EndingType에 해당하는 EndingSO를 결정한다.
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
        // Resolve
        // ──────────────────────────────────────────────

        /// <summary>
        /// 주어진 EndingType 중 현재 플레이어 상태와 일치하는 최고 우선순위 EndingSO의 Id를 반환.
        /// 일치하는 항목 없음 → InvalidOperationException (Fail Fast).
        /// 폴백 EndingSO(Conditions 빈 배열, Priority=0)가 항상 존재해야 한다.
        /// </summary>
        public int Resolve(EndingType type)
        {
            var candidates = _endingMasterDataRepo.GetEndingByType(type);

            EndingSO best         = null;
            int      bestPriority = int.MinValue;

            // LINQ 금지 — foreach + 직접 비교 (CLAUDE.md §8)
            foreach (var so in candidates)
            {
                if (!EvaluateAll(so.Conditions)) continue;

                if (so.Priority > bestPriority)
                {
                    best         = so;
                    bestPriority = so.Priority;
                }
            }

            if (best == null)
                throw new InvalidOperationException(
                    $"{_logClass} type={type}에 매칭되는 EndingSO가 없습니다. " +
                    $"Conditions=빈 배열, Priority=0 인 폴백 EndingSO가 반드시 존재해야 합니다.");

            Debug.Log($"{_logClass} Resolve 완료: type={type} → id={best.Id}, title={best.Title}, priority={best.Priority}");
            return best.Id;
        }

        // ──────────────────────────────────────────────
        // Condition Evaluation
        // ──────────────────────────────────────────────

        /// <summary>EndingSO의 Conditions 전체를 AND 평가한다.</summary>
        private bool EvaluateAll(EndingCondition[] conditions)
        {
            if (conditions == null || conditions.Length == 0) return true;  // 폴백 후보

            foreach (var condition in conditions)
            {
                if (!EvaluateOne(condition)) return false;
            }
            return true;
        }

        private bool EvaluateOne(EndingCondition condition)
        {
            switch (condition.Type)
            {
                case EndingConditionType.None:
                    return true;

                case EndingConditionType.EvolutionId:
                    // CharacterRunData.EvolutionNodeId는 string → StringValue와 비교
                    return _characterRunRepo.RunData.EvolutionNodeId == condition.StringValue;

                default:
                    Debug.LogWarning($"{_logClass} 미처리 EndingConditionType: {condition.Type} — true 반환 (안전 기본값)");
                    return true;
            }
        }
    }
}
