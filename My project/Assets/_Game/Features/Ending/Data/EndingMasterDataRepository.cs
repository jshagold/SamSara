using System.Collections.Generic;
using System.Linq;
using Samsara.Features.Ending.Domain;
using Samsara.Features.Ending.MasterData;
using UnityEngine;

namespace Samsara.Features.Ending.Data
{
    public class EndingMasterDataRepository : IEndingMasterDataRepository
    {
        private readonly string _logClass = $"[{nameof(EndingMasterDataRepository)}]";

        private readonly Dictionary<int, EndingSO> _cache;

        public EndingMasterDataRepository()
        {
            var loaded = Resources.LoadAll<EndingSO>("MasterData/Ending");
            _cache = new Dictionary<int, EndingSO>(loaded.Length);
            foreach (var so in loaded)
                _cache[so.Id] = so;

            Debug.Log($"{_logClass} {_cache.Count}개 EndingSO 로드 완료.");
        }

        public EndingSO GetEnding(int endingId)
        {
            if (_cache.TryGetValue(endingId, out var so)) return so;
            throw new System.InvalidOperationException($"{_logClass} EndingSO not found: id={endingId}");
        }

        public EndingSO[] GetEndingByType(EndingType type)
        {
            var result = new List<EndingSO>();
            foreach (var so in _cache.Values)
                if (so.EndingType == type) result.Add(so);
            return result.ToArray();
        }

        public EndingSO[] GetAllEndings()
        {
            var result = new EndingSO[_cache.Count];
            _cache.Values.CopyTo(result, 0);
            return result;
        }
    }
}
