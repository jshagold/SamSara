using System;
using UnityEngine;

namespace Samsara.Features.Ending.MasterData
{
    /// <summary>
    /// 배틀 노드의 특정 결과(승리/패배)에서 시도할 엔딩 후보 슬롯.
    /// EndingResolver가 Candidates를 평가해 최고 우선순위 엔딩을 선택한다.
    /// </summary>
    [Serializable]
    public class EndingCandidateSlot
    {
        [SerializeField] private EndingSO[] _candidates;

        public EndingSO[] Candidates => _candidates;

        /// <summary>후보가 없으면 true — 엔딩 매칭을 스킵한다.</summary>
        public bool IsEmpty => _candidates == null || _candidates.Length == 0;
    }
}
