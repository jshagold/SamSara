using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.Ending.Domain;
using UnityEngine;

namespace Samsara.Features.Ending.Presentation
{
    /// <summary>
    /// EndingScene 진행 로직을 담당하는 Presenter. MonoBehaviour 금지.
    /// EndingSceneBootstrapper에서 new로 생성된다.
    /// </summary>
    public class EndingPresenter
    {
        private readonly string _logClass = $"[{nameof(EndingPresenter)}]";

        private readonly EndingUseCase        _useCase;
        private readonly EndingView           _view;
        private readonly PendingEndingContext _context;
        private readonly ISpriteLoader        _spriteLoader;
        private readonly ISceneNavigator      _sceneNavigator;
        private readonly IPopupManager        _popupManager;
        private readonly GameContext          _gameContext;

        public EndingPresenter(
            EndingUseCase        useCase,
            EndingView           view,
            PendingEndingContext  context,
            ISpriteLoader        spriteLoader,
            ISceneNavigator      sceneNavigator,
            IPopupManager        popupManager,
            GameContext          gameContext)
        {
            _useCase        = useCase;
            _view           = view;
            _context        = context;
            _spriteLoader   = spriteLoader;
            _sceneNavigator = sceneNavigator;
            _popupManager   = popupManager;
            _gameContext    = gameContext;
        }

        // ──────────────────────────────────────────────
        // Entry Point
        // ──────────────────────────────────────────────

        public async UniTask InitializeAsync()
        {
            _useCase.LoadEnding(_context.EndingId);

            // 초기 배경 로드
            var bgKeys = _useCase.CurrentEnding.BackgroundSpriteKeys;
            if (bgKeys != null && bgKeys.Length > 0)
            {
                var initialBg = await _spriteLoader.LoadSpriteAsync(bgKeys[0]);
                _view.BackgroundView.SetBackground(initialBg);
            }

            Debug.Log($"{_logClass} InitializeAsync 완료. endingId={_context.EndingId}");
            await ShowTitlePhaseAsync();
        }

        // ──────────────────────────────────────────────
        // Phase: Title
        // ──────────────────────────────────────────────

        private async UniTask ShowTitlePhaseAsync()
        {
            await _view.TitleView.ShowTitle(
                _useCase.CurrentEnding.Title,
                fadeInDuration:  1.0f,
                holdDuration:    2.0f,
                fadeOutDuration: 1.0f);

            await ShowDialoguePhaseAsync();
        }

        // ──────────────────────────────────────────────
        // Phase: Dialogue
        // ──────────────────────────────────────────────

        private async UniTask ShowDialoguePhaseAsync()
        {
            while (true)
            {
                var dialogue = _useCase.GetCurrentDialogue();
                if (dialogue == null) break;

                // 배경 전환 (BackgroundIndex >= 0 인 경우)
                var bgKey = _useCase.GetBackgroundKeyForCurrentDialogue();
                if (bgKey != null)
                {
                    var bgSprite = await _spriteLoader.LoadSpriteAsync(bgKey);
                    await _view.BackgroundView.FadeToBackground(bgSprite, 0.6f);
                }

                // 나레이션 / 대화 판단
                if (string.IsNullOrEmpty(dialogue.SpeakerName))
                {
                    _view.DialogueView.ShowNarration(dialogue.Text);
                }
                else
                {
                    Sprite portrait = null;
                    if (!string.IsNullOrEmpty(dialogue.SpeakerPortraitKey))
                        portrait = await _spriteLoader.LoadSpriteAsync(dialogue.SpeakerPortraitKey);
                    _view.DialogueView.ShowDialogue(dialogue.SpeakerName, portrait, dialogue.Text);
                }

                // 화면 탭 대기
                await _view.WaitForTapAsync();

                // 다음 대사로 진행
                if (!_useCase.AdvanceDialogue())
                    break;
            }

            await ShowResultPhaseAsync();
        }

        // ──────────────────────────────────────────────
        // Phase: Result
        // ──────────────────────────────────────────────

        private async UniTask ShowResultPhaseAsync()
        {
            _useCase.CompleteEnding();
            _view.DialogueView.HideDialogue();

            await _view.ResultPopupView.Show(
                _useCase.CurrentEnding.Title,
                _context.RunSummary,
                _useCase.CurrentEnding.ResultText);

            HandleRestart();
        }

        // ──────────────────────────────────────────────
        // Restart
        // ──────────────────────────────────────────────

        private void HandleRestart()
        {
            // PendingEndingContext 소비
            _gameContext.PendingEndingContext = null;

            _sceneNavigator.NavigateToAsync(SceneKey.Replay).Forget();

            Debug.Log($"{_logClass} HandleRestart → SceneKey.Replay 이동.");
        }

        // ──────────────────────────────────────────────
        // Cleanup
        // ──────────────────────────────────────────────

        public void Dispose()
        {
            Debug.Log($"{_logClass} Dispose.");
        }
    }
}
