using Samsara.Core.Tree;
using Samsara.Features.EvolutionTreeScene.Presentation.TopBar;
using Samsara.Features.ReplayScene.Presentation.Popup;
using UnityEngine;

namespace Samsara.Features.ReplayScene.Presentation
{
    public class ReplaySceneView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplaySceneView)}]";

        [SerializeField] private OptionButtonView         _optionButtonView;
        [SerializeField] private TreeScrollView           _treeScrollView;
        [SerializeField] private NodeDescriptionPopupView _popupView;

        public OptionButtonView         OptionButtonView => _optionButtonView;
        public TreeScrollView           TreeScrollView   => _treeScrollView;
        public NodeDescriptionPopupView PopupView        => _popupView;

        private void Reset()
        {
            _optionButtonView = GetComponentInChildren<OptionButtonView>();
            _treeScrollView   = GetComponentInChildren<TreeScrollView>();
            _popupView        = GetComponentInChildren<NodeDescriptionPopupView>();
        }
    }
}
