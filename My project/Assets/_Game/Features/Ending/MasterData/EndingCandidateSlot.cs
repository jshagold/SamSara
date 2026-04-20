using System;
using UnityEngine;

namespace Samsara.Features.Ending.MasterData
{
    [Serializable]
    public class EndingCandidateSlot
    {
        [SerializeField] private EndingSO[] _candidates;
        [SerializeField] private EndingSO   _fallback;

        public EndingSO[] Candidates => _candidates;
        public EndingSO   Fallback   => _fallback;

        public bool IsEmpty =>
            (_candidates == null || _candidates.Length == 0) && _fallback == null;
    }
}
