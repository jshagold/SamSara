using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Domain;
using Samsara.Features.Ending.MasterData;
using UnityEngine;

namespace Samsara.Features.Ending.Domain
{
    public class EndingUseCase
    {
        private readonly string _logClass = $"[{nameof(EndingUseCase)}]";

        private readonly IEndingMasterDataRepository  _endingMasterDataRepo;
        private readonly ICharacterAccountRepository  _characterAccountRepo;

        private EndingSO _currentEnding;
        private int      _dialogueIndex;

        public EndingUseCase(
            IEndingMasterDataRepository endingMasterDataRepo,
            ICharacterAccountRepository characterAccountRepo)
        {
            _endingMasterDataRepo = endingMasterDataRepo;
            _characterAccountRepo = characterAccountRepo;
        }

        // ──────────────────────────────────────────────
        // Load
        // ──────────────────────────────────────────────

        public void LoadEnding(int endingId)
        {
            _currentEnding = _endingMasterDataRepo.GetEnding(endingId);
            _dialogueIndex = 0;
            Debug.Log($"{_logClass} LoadEnding id={endingId}, title={_currentEnding.Title}");
        }

        // ──────────────────────────────────────────────
        // Dialogue
        // ──────────────────────────────────────────────

        public EndingDialogue GetCurrentDialogue()
        {
            if (_currentEnding?.Dialogues == null) return null;
            if (_dialogueIndex >= _currentEnding.Dialogues.Length) return null;
            return _currentEnding.Dialogues[_dialogueIndex];
        }

        /// <returns>true = 다음 대사 있음, false = 대사 종료</returns>
        public bool AdvanceDialogue()
        {
            _dialogueIndex++;
            return _currentEnding?.Dialogues != null
                   && _dialogueIndex < _currentEnding.Dialogues.Length;
        }

        public bool IsDialogueComplete()
        {
            if (_currentEnding?.Dialogues == null) return true;
            return _dialogueIndex >= _currentEnding.Dialogues.Length;
        }

        /// <summary>
        /// 현재 대사의 BackgroundIndex를 기반으로 배경 전환이 필요한 경우 SpriteKey를 반환.
        /// BackgroundIndex == -1이거나 유효하지 않으면 null 반환.
        /// </summary>
        public string GetBackgroundKeyForCurrentDialogue()
        {
            var dialogue = GetCurrentDialogue();
            if (dialogue == null) return null;

            int bgIndex = dialogue.BackgroundIndex;
            if (bgIndex < 0) return null;
            if (_currentEnding.BackgroundSpriteKeys == null
                || bgIndex >= _currentEnding.BackgroundSpriteKeys.Length) return null;

            return _currentEnding.BackgroundSpriteKeys[bgIndex];
        }

        // ──────────────────────────────────────────────
        // Completion
        // ──────────────────────────────────────────────

        /// <summary>
        /// 엔딩을 완료 처리한다. Save-on-Action (§9).
        /// IsGameOver=false인 경우에만 UnlockedEndingIds에 추가.
        /// </summary>
        public void CompleteEnding()
        {
            bool dirty = false;

            if (!_currentEnding.IsGameOver)
            {
                if (!_characterAccountRepo.AccountData.UnlockedEndingIds.Contains(_currentEnding.Id))
                {
                    _characterAccountRepo.AccountData.UnlockedEndingIds.Add(_currentEnding.Id);
                    dirty = true;
                }
            }

            if (!string.IsNullOrEmpty(_currentEnding.UnlocksMainBgKey))
            {
                _characterAccountRepo.AccountData.MainSceneBgSpriteKey = _currentEnding.UnlocksMainBgKey;
                dirty = true;
            }

            if (!string.IsNullOrEmpty(_currentEnding.UnlocksMainBgmKey))
            {
                _characterAccountRepo.AccountData.MainSceneBgmKey = _currentEnding.UnlocksMainBgmKey;
                dirty = true;
            }

            if (dirty)
            {
                _characterAccountRepo.MarkDirty();
                _characterAccountRepo.SaveDataAsync().Forget();
            }

            Debug.Log($"{_logClass} CompleteEnding id={_currentEnding.Id}, isGameOver={_currentEnding.IsGameOver}, dirty={dirty}");
        }

        public bool IsEndingAlreadyUnlocked(int endingId)
            => _characterAccountRepo.AccountData.UnlockedEndingIds.Contains(endingId);

        // ──────────────────────────────────────────────
        // Accessors (Presenter용)
        // ──────────────────────────────────────────────

        public EndingSO CurrentEnding => _currentEnding;
    }
}
