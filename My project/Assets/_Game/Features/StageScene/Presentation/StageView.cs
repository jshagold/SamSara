using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.MasterData;
using Samsara.Features.StageScene.Presentation.Background;
using Samsara.Features.StageScene.Presentation.Map;
using Samsara.Features.StageScene.Presentation.Popup;
using Samsara.Features.StageScene.Presentation.TopBar;
using UnityEngine;

namespace Samsara.Features.StageScene.Presentation
{
    public class StageView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(StageView)}]";

        [SerializeField] private DayView _dayView;
        [SerializeField] private BackButtonView _backButtonView;
        [SerializeField] private OptionButtonView _optionButtonView;
        [SerializeField] private BackgroundView _backgroundView;
        [SerializeField] private NodeMapView _nodeMapView;
        [SerializeField] private CharacterMarkerView _characterMarkerView;
        [SerializeField] private StageCompletePopupView _stageCompletePopupView;

        // ── Events ─────────────────────────────────────────────────────
        public event Action<int> OnNodeClicked;
        public event Action OnBackClicked;
        public event Action OnOptionClicked;
        public event Action<string> OnStageSelected;
        public event Action OnReturnToMainClicked;

        private void Reset()
        {
            _dayView                  = GetComponentInChildren<DayView>();
            _backButtonView           = GetComponentInChildren<BackButtonView>();
            _optionButtonView         = GetComponentInChildren<OptionButtonView>();
            _backgroundView           = GetComponentInChildren<BackgroundView>();
            _nodeMapView              = GetComponentInChildren<NodeMapView>();
            _characterMarkerView      = GetComponentInChildren<CharacterMarkerView>();
            _stageCompletePopupView   = GetComponentInChildren<StageCompletePopupView>();
        }

        private void Awake()
        {
            _nodeMapView.OnNodeClicked             += HandleNodeClicked;
            _backButtonView.OnBackClicked          += HandleBackClicked;
            _optionButtonView.OnOptionClicked      += HandleOptionClicked;
            _stageCompletePopupView.OnStageSelected     += HandleStageSelected;
            _stageCompletePopupView.OnReturnToMainClicked += HandleReturnToMain;
        }

        // ── TopBar ──────────────────────────────────────────────────────
        public void SetDay(int day) => _dayView.SetDay(day);

        public void SetBackButtonInteractable(bool interactable) =>
            _backButtonView.SetInteractable(interactable);

        // ── Background ──────────────────────────────────────────────────
        public void SetBackground(Sprite sprite) => _backgroundView.SetBackground(sprite);

        public void FadeToBackground(Sprite sprite, float duration) =>
            _backgroundView.FadeToBackground(sprite, duration);

        // ── NodeMap ─────────────────────────────────────────────────────
        public string[] GetNodeTypeIconKeys() => _nodeMapView.NodeTypeIconKeys;
        public void SetNodeTypeIcons(Sprite[] icons) => _nodeMapView.SetNodeTypeIcons(icons);

        public void RenderNodes(StageNodeSO[] nodes) => _nodeMapView.RenderNodes(nodes);

        public void HighlightNode(int index) => _nodeMapView.HighlightNode(index);

        public void MarkNodeCompleted(int index) => _nodeMapView.MarkCompleted(index);

        public void FocusOnNode(int index) => _nodeMapView.FocusOnNode(index);

        public void ClearNodes() => _nodeMapView.ClearNodes();

        public Vector3 GetNodeWorldPosition(int index) => _nodeMapView.GetNodeWorldPosition(index);

        // ── Character ───────────────────────────────────────────────────
        public void SetCharacterPosition(Vector3 worldPosition) =>
            _characterMarkerView.SetPosition(worldPosition);

        public UniTask MoveCharacterTo(Vector3 worldPosition, float duration) =>
            _characterMarkerView.MoveTo(worldPosition, duration);

        // ── Popup ────────────────────────────────────────────────────────
        public void ShowStageCompletePopup(string message, List<StageOptionData> options) =>
            _stageCompletePopupView.Show(message, options);

        public void HideStageCompletePopup() => _stageCompletePopupView.Hide();

        // ── Event relay helpers ──────────────────────────────────────────
        private void HandleNodeClicked(int index)  => OnNodeClicked?.Invoke(index);
        private void HandleBackClicked()           => OnBackClicked?.Invoke();
        private void HandleOptionClicked()         => OnOptionClicked?.Invoke();
        private void HandleStageSelected(string id) => OnStageSelected?.Invoke(id);
        private void HandleReturnToMain()          => OnReturnToMainClicked?.Invoke();

        private void OnDestroy()
        {
            _nodeMapView.OnNodeClicked                    -= HandleNodeClicked;
            _backButtonView.OnBackClicked                 -= HandleBackClicked;
            _optionButtonView.OnOptionClicked             -= HandleOptionClicked;
            _stageCompletePopupView.OnStageSelected       -= HandleStageSelected;
            _stageCompletePopupView.OnReturnToMainClicked -= HandleReturnToMain;
        }
    }
}
