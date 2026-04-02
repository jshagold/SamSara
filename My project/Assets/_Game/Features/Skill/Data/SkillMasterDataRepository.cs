using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Core.MasterData;
using Samsara.Features.Skill.Domain;
using UnityEngine;

namespace Samsara.Features.Skill.Data
{
    public class SkillMasterDataRepository : ISkillMasterDataRepository
    {
        private readonly string _logClass = $"[{nameof(SkillMasterDataRepository)}]";

        private Dictionary<int, SkillSO> _skillCache;
        private Dictionary<int, QTEPatternSO> _qtePatternCache;

        public UniTask LoadAsync()
        {
            _skillCache = new Dictionary<int, SkillSO>();
            _qtePatternCache = new Dictionary<int, QTEPatternSO>();

            var skills = Resources.LoadAll<SkillSO>("MasterData");
            foreach (var skill in skills)
                _skillCache[skill.SkillId] = skill;

            var patterns = Resources.LoadAll<QTEPatternSO>("MasterData");
            foreach (var pattern in patterns)
                _qtePatternCache[pattern.PatternId] = pattern;

            Debug.Log($"{_logClass} LoadAsync 완료 — Skill:{_skillCache.Count}, QTEPattern:{_qtePatternCache.Count}");

            return UniTask.CompletedTask;
        }

        public SkillSO GetSkill(int skillId)
        {
            if (_skillCache.TryGetValue(skillId, out var skill)) return skill;
            throw new InvalidOperationException($"{_logClass} SkillSO not found: {skillId}");
        }

        public QTEPatternSO GetQTEPattern(int patternId)
        {
            if (_qtePatternCache.TryGetValue(patternId, out var pattern)) return pattern;
            throw new InvalidOperationException($"{_logClass} QTEPatternSO not found: {patternId}");
        }

        public SkillSO[] GetAllSkills()
        {
            var result = new SkillSO[_skillCache.Count];
            _skillCache.Values.CopyTo(result, 0);
            return result;
        }

        public SkillSO[] GetSkillsByIds(int[] skillIds)
        {
            var result = new SkillSO[skillIds.Length];
            for (var i = 0; i < skillIds.Length; i++)
                result[i] = GetSkill(skillIds[i]);
            return result;
        }
    }
}
