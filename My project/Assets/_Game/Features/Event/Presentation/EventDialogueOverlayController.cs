using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using Samsara.Features.Event.Domain;
using Samsara.Features.Event.MasterData;
using UnityEngine;

namespace Samsara.Features.Event.Presentation
{
    /// <summary>
    /// 씬 전환 없이 오버레이 형태로 이벤트를 실행하는 컨트롤러 (Prefab 루트).
    /// 현재 Phase에서는 구조와 코드만 작성. 외부 씬 연동은 추후 Patch에서 진행.
    /// </summary>
    public class EventDialogueOverlayController : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EventDialogueOverlayController)}]";

        [SerializeField] private DialogueView  _dialogueView;
        [SerializeField] private ChoiceListView _choiceListView;
        [SerializeField] private GameObject    _dimBackground;

        private EventUseCase              _useCase;
        private ISpriteLoader             _spriteLoader;
        private UniTaskCompletionSource   _tapTcs;

        public void Initialize(EventUseCase useCase, ISpriteLoader spriteLoader = null)
        {
            _useCase      = useCase;
            if (spriteLoader != null) _spriteLoader = spriteLoader;
        }

        /// <summary>EventUseCase 없이 오버레이만 사용할 때 SpriteLoader를 주입한다 (상인 대화 등).</summary>
        public void SetSpriteLoader(ISpriteLoader spriteLoader)
        {
            _spriteLoader = spriteLoader;
        }

        /// <summary>
        /// 이벤트를 오버레이로 실행하고 최종 결과를 반환한다.
        /// </summary>
        public async UniTask<EventResult> RunEventAsync(int eventId)
        {
            _useCase.LoadEvent(eventId);
            gameObject.SetActive(true);
            _dimBackground.SetActive(true);

            // 대사 루프
            if (_useCase.GetCurrentDialogue() != null)
                await RunDialogueLoopAsync();

            // 선택지 또는 직접 결과
            EventResult result;
            if (_useCase.HasChoices())
            {
                var selectionTcs = new UniTaskCompletionSource<int>();
                _choiceListView.ShowChoices(
                    _useCase.GetChoices(),
                    index => selectionTcs.TrySetResult(index));

                int chosen = await selectionTcs.Task;
                _choiceListView.HideChoices();
                result = _useCase.ApplyChoice(chosen);
            }
            else
            {
                result = _useCase.ApplyDirectResult();
            }

            _dimBackground.SetActive(false);
            gameObject.SetActive(false);
            return result;
        }

        /// <summary>
        /// EventUseCase 없이 원시 대사 배열과 선택지 텍스트로 오버레이를 실행한다.
        /// 상점 대사 등 ScriptableObject 이벤트가 없는 상황에서 재사용할 때 사용.
        /// 선택된 선택지 인덱스를 반환한다.
        /// </summary>
        public async UniTask<int> RunDialoguesWithChoicesAsync(EventDialogue[] dialogues, string[] choiceTexts)
        {
            gameObject.SetActive(true);
            _dimBackground.SetActive(true);

            // 대사 루프
            if (dialogues != null && dialogues.Length > 0)
                await RunRawDialogueLoopAsync(dialogues);

            // 선택지
            var selectionTcs = new UniTaskCompletionSource<int>();
            _choiceListView.ShowChoices(choiceTexts, index => selectionTcs.TrySetResult(index));

            int chosen = await selectionTcs.Task;
            _choiceListView.HideChoices();

            _dimBackground.SetActive(false);
            gameObject.SetActive(false);
            return chosen;
        }

        /// <summary>
        /// Inspector의 Button.onClick 또는 외부 코드에서 탭 이벤트를 주입한다.
        /// </summary>
        public void OnDialogueTapped()
        {
            _tapTcs?.TrySetResult();
        }

        // ──────────────────────────────────────────────
        // Internal
        // ──────────────────────────────────────────────

        private async UniTask RunRawDialogueLoopAsync(EventDialogue[] dialogues)
        {
            int index = 0;
            var portrait = await LoadPortraitAsync(dialogues[index].PortraitSpriteKey);
            _dialogueView.ShowDialogue(dialogues[index], portrait);

            while (true)
            {
                _tapTcs = new UniTaskCompletionSource();
                await _tapTcs.Task;

                index++;
                if (index >= dialogues.Length) break;
                portrait = await LoadPortraitAsync(dialogues[index].PortraitSpriteKey);
                _dialogueView.ShowDialogue(dialogues[index], portrait);
            }

            _dialogueView.HideDialogue();
        }

        private async UniTask RunDialogueLoopAsync()
        {
            await ShowCurrentDialogueAsync();

            while (true)
            {
                _tapTcs = new UniTaskCompletionSource();
                await _tapTcs.Task;

                bool hasNext = _useCase.AdvanceDialogue();
                if (!hasNext) break;
                await ShowCurrentDialogueAsync();
            }

            _dialogueView.HideDialogue();
        }

        private async UniTask ShowCurrentDialogueAsync()
        {
            var dialogue = _useCase.GetCurrentDialogue();
            if (dialogue == null) return;
            var portrait = await LoadPortraitAsync(dialogue.PortraitSpriteKey);
            _dialogueView.ShowDialogue(dialogue, portrait);
        }

        private async UniTask<Sprite> LoadPortraitAsync(string key)
        {
            if (_spriteLoader == null || string.IsNullOrEmpty(key)) return null;
            return await _spriteLoader.LoadSpriteAsync(key);
        }

        private void OnDestroy()
        {
            _tapTcs?.TrySetCanceled();
        }
    }
}
