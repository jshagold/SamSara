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
