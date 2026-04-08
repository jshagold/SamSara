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
        public event Action<int> OnLongPress;

        public void RenderEnemies(BattleParticipant[] enemies)
        {
            ClearEnemies();

            // ── 동일 이름 적 번호 부여 ──
            AssignNumberedDisplayNames(enemies);

            foreach (var enemy in enemies)
            {
                var unit = Instantiate(_unitPrefab, _enemyContainer);
                unit.Setup(enemy.Id, enemy.SpriteKey, enemy.MaxHp);
                _units.Add(unit);
                _unitById[enemy.Id] = unit;

                // 타겟 선택 버튼
                var button = unit.GetComponent<Button>();
                if (button == null)
                    button = unit.gameObject.AddComponent<Button>();

                int capturedId = enemy.Id;
                button.onClick.AddListener(() => HandleUnitClicked(capturedId));
                _unitButtons[enemy.Id] = button;

                // 롱프레스 이벤트 릴레이
                unit.OnLongPress += HandleUnitLongPress;
            }

            // ── 적 유닛 가로 균등 배치 ──
            DistributeUnits();

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
            {
                if (unit != null)
                    unit.OnLongPress -= HandleUnitLongPress;
                if (unit != null)
                    Destroy(unit.gameObject);
            }

            _units.Clear();
            _unitById.Clear();
            _unitButtons.Clear();
        }

        // ──────────────────────────────────────────────
        // Unit Distribution
        // ──────────────────────────────────────────────

        /// <summary>
        /// _enemyContainer 내에서 적 유닛들을 가로 균등 배치.
        /// 컨테이너 폭을 적 수로 나눠 각 유닛의 anchoredPosition을 설정.
        /// </summary>
        private void DistributeUnits()
        {
            if (_units.Count == 0) return;

            var containerRect = _enemyContainer as RectTransform;
            if (containerRect == null) return;

            float containerWidth = containerRect.rect.width;
            int count = _units.Count;

            // 컨테이너를 count등분, 각 구간의 중앙에 배치
            float slotWidth = containerWidth / count;

            for (int i = 0; i < count; i++)
            {
                var unitRect = _units[i].GetComponent<RectTransform>();
                if (unitRect == null) continue;

                // 좌측 기준: -containerWidth/2 + slotWidth * (i + 0.5)
                float x = -containerWidth / 2f + slotWidth * (i + 0.5f);
                unitRect.anchoredPosition = new Vector2(x, 0f);

                _units[i].SetOriginalPosition();
            }
        }

        // ──────────────────────────────────────────────
        // Same-enemy Numbering
        // ──────────────────────────────────────────────

        /// <summary>
        /// 동일 displayName을 가진 적에게 ①②③… 번호를 부여.
        /// 단독이면 원래 이름 유지.
        /// </summary>
        private static void AssignNumberedDisplayNames(BattleParticipant[] enemies)
        {
            // displayName별 등장 횟수 집계
            var nameCount = new Dictionary<string, int>();
            foreach (var e in enemies)
            {
                string name = e.DisplayName ?? e.SpriteKey ?? $"Enemy{e.Id}";
                if (!nameCount.ContainsKey(name)) nameCount[name] = 0;
                nameCount[name]++;
            }

            // 중복 이름에만 번호 부여
            var nameIndex = new Dictionary<string, int>();
            foreach (var e in enemies)
            {
                string baseName = e.DisplayName ?? e.SpriteKey ?? $"Enemy{e.Id}";
                if (nameCount[baseName] >= 2)
                {
                    if (!nameIndex.ContainsKey(baseName)) nameIndex[baseName] = 0;
                    int idx = nameIndex[baseName];
                    nameIndex[baseName]++;

                    // ①②③④⑤⑥⑦⑧⑨ (Unicode circled numbers)
                    string circle = idx < CircledNumbers.Length ? CircledNumbers[idx] : $"({idx + 1})";
                    e.DisplayName = $"{baseName}{circle}";
                }
                // 단독이면 DisplayName 유지 (이미 InitializeBattle에서 설정됨)
            }
        }

        private static readonly string[] CircledNumbers = { "①", "②", "③", "④", "⑤", "⑥", "⑦", "⑧", "⑨" };

        // ──────────────────────────────────────────────
        // Event Handlers
        // ──────────────────────────────────────────────

        private void HandleUnitClicked(int participantId)
        {
            if (!_targetSelectionEnabled) return;
            OnTargetSelected?.Invoke(participantId);
        }

        private void HandleUnitLongPress(int participantId)
        {
            OnLongPress?.Invoke(participantId);
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
