using System;
using UnityEngine;

namespace Samsara.Core.MasterData
{
    [CreateAssetMenu(fileName = "SkillSO", menuName = "Samsara/Core/Skill")]
    public class SkillSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(SkillSO)}]";

        [SerializeField] private int _skillId;
        [SerializeField] private string _skillName;
        [SerializeField] private string _description;
        [SerializeField] private float _damage;
        [SerializeField] private int _qtePatternId;
        [SerializeField] private SkillCost[] _costs;
        [SerializeField] private SkillEffect[] _effects;
        [SerializeField] private string _iconSpriteKey;
        [SerializeField] private string _effectSpriteKey;

        public int SkillId => _skillId;
        public string SkillName => _skillName;
        public string Description => _description;
        public float Damage => _damage;
        public int QtePatternId => _qtePatternId;
        public SkillCost[] Costs => _costs;
        public SkillEffect[] Effects => _effects;
        public string IconSpriteKey => _iconSpriteKey;
        public string EffectSpriteKey => _effectSpriteKey;
    }

    [Serializable]
    public class SkillCost
    {
        [SerializeField] private CostType _costType;
        [SerializeField] private float _value;

        public CostType CostType => _costType;
        public float Value => _value;
    }

    [Serializable]
    public class SkillEffect
    {
        [SerializeField] private EffectType _effectType;
        [SerializeField] private float _duration;

        public EffectType EffectType => _effectType;
        public float Duration => _duration;
    }
}
