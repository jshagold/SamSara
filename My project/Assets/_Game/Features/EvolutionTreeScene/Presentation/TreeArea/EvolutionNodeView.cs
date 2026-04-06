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
        [SerializeField] private Image _highlightBorder;
        [SerializeField] private Image _dimOverlay;
        [SerializeField] private GameObject _evolvableIndicator;

        private string _nodeId;
        private Sprite _questionMarkSprite;
        private Sprite _originalIcon;

        public string NodeId => _nodeId;

        public event Action<string> OnNodeClicked;

        public void Setup(string nodeId, Sprite icon)
        {
            _nodeId = nodeId;
            _originalIcon = icon;
            _nodeIcon.sprite = icon;
        }

        public void SetNodeState(NodeState state)
        {
            _highlightBorder.gameObject.SetActive(false);
            _dimOverlay.gameObject.SetActive(false);
            _evolvableIndicator.SetActive(false);
            _nodeIcon.sprite = _originalIcon;

            switch (state)
            {
                case NodeState.Current:
                    _highlightBorder.gameObject.SetActive(true);
                    break;
                case NodeState.Evolvable:
                    _evolvableIndicator.SetActive(true);
                    break;
                case NodeState.Reachable:
                    break;
                case NodeState.Locked:
                    _dimOverlay.gameObject.SetActive(true);
                    break;
                case NodeState.Hidden:
                    if (_questionMarkSprite != null)
                        _nodeIcon.sprite = _questionMarkSprite;
                    _dimOverlay.gameObject.SetActive(true);
                    break;
            }
        }

        public void SetQuestionMarkSprite(Sprite questionMark)
        {
            _questionMarkSprite = questionMark;
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
