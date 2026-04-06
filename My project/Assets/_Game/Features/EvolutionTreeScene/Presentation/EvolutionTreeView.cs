using Samsara.Features.EvolutionTreeScene.Presentation.Popup;
using Samsara.Features.EvolutionTreeScene.Presentation.TopBar;
using Samsara.Features.EvolutionTreeScene.Presentation.TreeArea;
using UnityEngine;

namespace Samsara.Features.EvolutionTreeScene.Presentation
{
    public class EvolutionTreeView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EvolutionTreeView)}]";

        [SerializeField] private BackButtonView _backButtonView;
        [SerializeField] private OptionButtonView _optionButtonView;
        [SerializeField] private TreeScrollView _treeScrollView;
        [SerializeField] private NodeDescriptionPopupView _nodeDescriptionPopupView;
        [SerializeField] private ReincarnationButtonView _reincarnationButtonView;

        public BackButtonView BackButtonView => _backButtonView;
        public OptionButtonView OptionButtonView => _optionButtonView;
        public TreeScrollView TreeScrollView => _treeScrollView;
        public NodeDescriptionPopupView NodeDescriptionPopupView => _nodeDescriptionPopupView;
        public ReincarnationButtonView ReincarnationButtonView => _reincarnationButtonView;

        private void Reset()
        {
            _backButtonView = GetComponentInChildren<BackButtonView>();
            _optionButtonView = GetComponentInChildren<OptionButtonView>();
            _treeScrollView = GetComponentInChildren<TreeScrollView>();
            _nodeDescriptionPopupView = GetComponentInChildren<NodeDescriptionPopupView>();
            _reincarnationButtonView = GetComponentInChildren<ReincarnationButtonView>();
        }
    }
}
