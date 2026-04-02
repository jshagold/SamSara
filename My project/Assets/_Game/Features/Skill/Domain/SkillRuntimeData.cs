using System;
using System.Collections.Generic;

namespace Samsara.Features.Skill.Domain
{
    public class SkillRuntimeData
    {
        private readonly string _logClass = $"[{nameof(SkillRuntimeData)}]";

        private Dictionary<int, int> _cooldowns;

        public void Initialize(int[] skillIds)
        {
            _cooldowns = new Dictionary<int, int>(skillIds.Length);
            foreach (var id in skillIds)
                _cooldowns[id] = 0;
        }

        public int GetCooldown(int skillId)
        {
            if (_cooldowns.TryGetValue(skillId, out var value)) return value;
            throw new InvalidOperationException($"{_logClass} SkillId not found: {skillId}");
        }

        public void SetCooldown(int skillId, int value)
        {
            if (!_cooldowns.ContainsKey(skillId))
                throw new InvalidOperationException($"{_logClass} SkillId not found: {skillId}");
            _cooldowns[skillId] = value;
        }

        public void TickAll()
        {
            var keys = new List<int>(_cooldowns.Keys);
            foreach (var key in keys)
            {
                if (_cooldowns[key] > 0)
                    _cooldowns[key]--;
            }
        }

        public void Clear()
        {
            _cooldowns?.Clear();
        }

        public int[] GetAllSkillIds()
        {
            if (_cooldowns == null) return Array.Empty<int>();
            var ids = new int[_cooldowns.Count];
            _cooldowns.Keys.CopyTo(ids, 0);
            return ids;
        }
    }
}
