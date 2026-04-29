using System;
using Samsara.Core.Tree;
using Samsara.Features.ReplayScene.Presentation.Popup;
using UnityEngine;

namespace Samsara.Features.ReplayScene.Presentation
{
    public class ReplayView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplayView)}]";

        [SerializeField] private TreeScrollView _treeScrollView;
        [SerializeField] private ReplayGuidanceView _guidanceView;
        [SerializeField] private OptionButtonView _optionButton;
        [SerializeField] private ReplayNodeDescriptionPopupView _nodeDescriptionPopupView;
        [SerializeField] private ReplaySkillDescriptionPopupView _skillDescriptionPopupView;

        public TreeScrollView TreeScrollView => _treeScrollView;
        public ReplayGuidanceView GuidanceView => _guidanceView;
        public ReplayNodeDescriptionPopupView NodeDescriptionPopupView => _nodeDescriptionPopupView;
        public ReplaySkillDescriptionPopupView SkillDescriptionPopupView => _skillDescriptionPopupView;

        // OptionButtonView 이벤트는 ReplayView가 relay (TreeScrollView는 자체 이벤트 없음 — Presenter가 BuildTree 후 NodeView 단위로 직접 구독)
        public event Action OnOptionButtonTapped;

        private void Awake()
        {
            _optionButton.OnTapped += HandleOptionButtonTapped;
        }

        private void OnDestroy()
        {
            if (_optionButton != null)
                _optionButton.OnTapped -= HandleOptionButtonTapped;
        }

        private void HandleOptionButtonTapped()
        {
            OnOptionButtonTapped?.Invoke();
        }

        private void Reset()
        {
            _treeScrollView = GetComponentInChildren<TreeScrollView>();
            _guidanceView = GetComponentInChildren<ReplayGuidanceView>();
            _optionButton = GetComponentInChildren<OptionButtonView>();
            _nodeDescriptionPopupView = GetComponentInChildren<ReplayNodeDescriptionPopupView>();
            _skillDescriptionPopupView = GetComponentInChildren<ReplaySkillDescriptionPopupView>();
        }
    }
}
