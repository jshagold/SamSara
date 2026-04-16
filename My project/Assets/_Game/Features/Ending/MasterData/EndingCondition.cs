using System;
using UnityEngine;

namespace Samsara.Features.Ending.MasterData
{
    /// <summary>
    /// 엔딩 하나의 발동 조건. EndingSO.Conditions 배열의 원소 (AND 조건).
    /// Type에 따라 사용하는 Value 필드가 달라진다.
    /// </summary>
    [Serializable]
    public class EndingCondition
    {
        [SerializeField] private EndingConditionType _type;
        /// <summary>정수형 조건값. 미래 정수 조건(StatRange 등)에 재사용.</summary>
        [SerializeField] private int    _intValue;
        /// <summary>문자열 조건값. EvolutionId 조건 시 사용 (CharacterRunData.EvolutionNodeId는 string 타입).</summary>
        [SerializeField] private string _stringValue;

        public EndingConditionType Type        => _type;
        public int                 IntValue    => _intValue;
        public string              StringValue => _stringValue;
    }
}
