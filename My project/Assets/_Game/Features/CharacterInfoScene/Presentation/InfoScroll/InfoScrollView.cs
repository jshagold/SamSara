using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.CharacterInfoScene.Presentation.InfoScroll
{
    public class InfoScrollView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(InfoScrollView)}]";

        [SerializeField] private ScrollRect        _scrollRect;
        [SerializeField] private CharacterNameView _characterNameView;
        [SerializeField] private StatListView      _statListView;
        [SerializeField] private SkillListView     _skillListView;
        [SerializeField] private InventoryView     _inventoryView;

        // ── Child View 프로퍼티 (Presenter 접근용) ──
        public CharacterNameView CharacterName => _characterNameView;
        public StatListView      StatList      => _statListView;
        public SkillListView     SkillList     => _skillListView;
        public InventoryView     Inventory     => _inventoryView;

        private void Reset()
        {
            _scrollRect        = GetComponentInChildren<ScrollRect>();
            _characterNameView = GetComponentInChildren<CharacterNameView>();
            _statListView      = GetComponentInChildren<StatListView>();
            _skillListView     = GetComponentInChildren<SkillListView>();
            _inventoryView     = GetComponentInChildren<InventoryView>();
        }
    }
}
