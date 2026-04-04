using System;
using System.Collections.Generic;
using Samsara.Features.BattleScene.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Field
{
    public class EnemyFieldView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EnemyFieldView)}]";

        [SerializeField] private Transform _enemyContainer;
        [SerializeField] private CharacterUnitView _unitPrefab;

        private readonly List<CharacterUnitView> _units = new List<CharacterUnitView>();
        private readonly Dictionary<int, CharacterUnitView> _unitById = new Dictionary<int, CharacterUnitView>();
        private readonly Dictionary<int, Button> _unitButtons = new Dictionary<int, Button>();

        private bool _targetSelectionEnabled;

        public event Action<int> OnTargetSelected;

        public void RenderEnemies(BattleParticipant[] enemies)
        {
            ClearEnemies();

            foreach (var enemy in enemies)
            {
                var unit = Instantiate(_unitPrefab, _enemyContainer);
                unit.Setup(enemy.Id, enemy.SpriteKey, enemy.MaxHp);
                _units.Add(unit);
                _unitById[enemy.Id] = unit;

                // Connect touch event per unit
                var button = unit.GetComponent<Button>();
                if (button == null)
                    button = unit.gameObject.AddComponent<Button>();

                int capturedId = enemy.Id;
                button.onClick.AddListener(() => HandleUnitClicked(capturedId));
                _unitButtons[enemy.Id] = button;
            }

            EnableTargetSelection(false);
        }

        public CharacterUnitView GetUnit(int participantId)
        {
            return _unitById[participantId];
        }

        public void EnableTargetSelection(bool enable)
        {
            _targetSelectionEnabled = enable;
            foreach (var kvp in _unitButtons)
                kvp.Value.interactable = enable;
        }

        public void ClearEnemies()
        {
            foreach (var kvp in _unitButtons)
                kvp.Value?.onClick.RemoveAllListeners();

            foreach (var unit in _units)
                Destroy(unit.gameObject);

            _units.Clear();
            _unitById.Clear();
            _unitButtons.Clear();
        }

        private void HandleUnitClicked(int participantId)
        {
            if (!_targetSelectionEnabled) return;
            OnTargetSelected?.Invoke(participantId);
        }

        private void OnDestroy()
        {
            ClearEnemies();
        }

        private void Reset()
        {
            _enemyContainer = transform;
        }
    }
}
