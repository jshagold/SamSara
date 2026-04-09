using System;
using System.Collections.Generic;
using Samsara.Features.BattleScene.Domain;
using UnityEngine;

namespace Samsara.Features.BattleScene.Presentation.Field
{
    public class AllyFieldView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(AllyFieldView)}]";

        [SerializeField] private Transform _allyContainer;
        [SerializeField] private CharacterUnitView _unitPrefab;

        private readonly List<CharacterUnitView> _units = new List<CharacterUnitView>();
        private readonly Dictionary<int, CharacterUnitView> _unitById = new Dictionary<int, CharacterUnitView>();

        public event Action<int> OnLongPress;

        public void RenderAllies(BattleParticipant[] allies)
        {
            ClearAllies();

            foreach (var ally in allies)
            {
                var unit = Instantiate(_unitPrefab, _allyContainer);
                unit.Setup(ally.Id, ally.SpriteKey, ally.MaxHp);
                _units.Add(unit);
                _unitById[ally.Id] = unit;

                unit.OnLongPress += HandleUnitLongPress;
            }

            // ── 아군 유닛 가로 균등 배치 ──
            DistributeUnits();
        }

        private void DistributeUnits()
        {
            if (_units.Count == 0) return;

            var containerRect = _allyContainer as RectTransform;
            if (containerRect == null) return;

            float containerWidth = containerRect.rect.width;
            int count = _units.Count;

            float slotWidth = containerWidth / count;

            for (int i = 0; i < count; i++)
            {
                var unitRect = _units[i].GetComponent<RectTransform>();
                if (unitRect == null) continue;

                float x = -containerWidth / 2f + slotWidth * (i + 0.5f);
                unitRect.anchoredPosition = new Vector2(x, 0f);

                _units[i].SetOriginalPosition();
            }
        }

        public CharacterUnitView GetUnit(int participantId)
        {
            return _unitById[participantId];
        }

        public void ClearAllies()
        {
            foreach (var unit in _units)
            {
                if (unit != null)
                    unit.OnLongPress -= HandleUnitLongPress;
                if (unit != null)
                    Destroy(unit.gameObject);
            }

            _units.Clear();
            _unitById.Clear();
        }

        private void HandleUnitLongPress(int participantId)
        {
            Debug.Log($"{_logClass} HandleUnitLongPress id={participantId} → relay OnLongPress (subscribers={OnLongPress?.GetInvocationList()?.Length ?? 0})");
            OnLongPress?.Invoke(participantId);
        }

        private void OnDestroy()
        {
            ClearAllies();
        }

        private void Reset()
        {
            _allyContainer = transform;
        }
    }
}
