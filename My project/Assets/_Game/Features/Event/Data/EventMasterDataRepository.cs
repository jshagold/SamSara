using System;
using System.Collections.Generic;
using Samsara.Features.Event.Domain;
using Samsara.Features.Event.MasterData;
using UnityEngine;

namespace Samsara.Features.Event.Data
{
    public class EventMasterDataRepository : IEventMasterDataRepository
    {
        private readonly string _logClass = $"[{nameof(EventMasterDataRepository)}]";

        private readonly Dictionary<int, EventSO> _cache = new();
        private EventSO[] _all;

        public EventMasterDataRepository()
        {
            _all = Resources.LoadAll<EventSO>("MasterData/Event");
            foreach (var so in _all)
                _cache[so.EventId] = so;

            Debug.Log($"{_logClass} {_all.Length}개 EventSO 로드 완료.");
        }

        public EventSO GetEvent(int eventId)
        {
            if (!_cache.TryGetValue(eventId, out var so))
                throw new InvalidOperationException($"{_logClass} EventId {eventId} 를 찾을 수 없음.");
            return so;
        }

        public EventSO[] GetAllEvents() => _all;

        public EventSO[] GetMaintenanceEvents()
        {
            var result = new List<EventSO>();
            foreach (var so in _all)
                if (so.EventSource == EventSource.Maintenance)
                    result.Add(so);
            return result.ToArray();
        }
    }
}
