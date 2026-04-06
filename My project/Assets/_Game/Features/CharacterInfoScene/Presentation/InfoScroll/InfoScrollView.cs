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

        public CharacterNameView CharacterNameView => _characterNameView;
        public StatListView      StatListView      => _statListView;
        public SkillListView     SkillListView     => _skillListView;
        public InventoryView     InventoryView     => _inventoryView;

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
