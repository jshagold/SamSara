using System;
using Samsara.Features.EvolutionTreeScene.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.EvolutionTreeScene.Presentation.TreeArea
{
    public class EvolutionNodeView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EvolutionNodeView)}]";

        [SerializeField] private Button _button;
        [SerializeField] private Image _nodeIcon;
        [SerializeField] private Image _frameImage;

        private Sprite _currentFrameSprite;
        private Sprite _evolvableFrameSprite;
        private Sprite _reachableFrameSprite;
        private Sprite _lockedFrameSprite;
        private Sprite _hiddenFrameSprite;

        private Sprite _questionMarkSprite;

        private string _nodeId;
        private Sprite _originalIcon;

        public string NodeId => _nodeId;

        public event Action<string> OnNodeClicked;

        /// <summary>UI 스프라이트 외부 주입. EvolutionTreeSceneBootstrapper에서 호출.</summary>
        public void SetUISprites(
            Sprite current, Sprite evolvable, Sprite reachable,
            Sprite locked, Sprite hidden, Sprite questionMark)
        {
            _currentFrameSprite   = current;
            _evolvableFrameSprite = evolvable;
            _reachableFrameSprite = reachable;
            _lockedFrameSprite    = locked;
            _hiddenFrameSprite    = hidden;
            _questionMarkSprite   = questionMark;
        }

        public void Setup(string nodeId, Sprite icon)
        {
            _nodeId = nodeId;
            _originalIcon = icon;
            _nodeIcon.sprite = icon;
            _nodeIcon.enabled = icon != null;
        }

        public void SetNodeState(NodeState state)
        {
            _nodeIcon.sprite = _originalIcon;
            _nodeIcon.enabled = _originalIcon != null;

            switch (state)
            {
                case NodeState.Current:
                    _frameImage.sprite = _currentFrameSprite;
                    break;
                case NodeState.Evolvable:
                    _frameImage.sprite = _evolvableFrameSprite;
                    break;
                case NodeState.Reachable:
                    _frameImage.sprite = _reachableFrameSprite;
                    break;
                case NodeState.Locked:
                    _frameImage.sprite = _lockedFrameSprite;
                    break;
                case NodeState.Hidden:
                    _frameImage.sprite = _hiddenFrameSprite;
                    if (_questionMarkSprite != null)
                    {
                        _nodeIcon.sprite = _questionMarkSprite;
                        _nodeIcon.enabled = true;
                    }
                    break;
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleNodeClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleNodeClicked);
        }

        private void HandleNodeClicked()
        {
            OnNodeClicked?.Invoke(_nodeId);
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
            _nodeIcon = GetComponentInChildren<Image>();
        }
    }
}
