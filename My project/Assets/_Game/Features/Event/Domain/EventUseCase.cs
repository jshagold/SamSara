using System;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Event.MasterData;
using Samsara.Features.Stage.Domain;
using UnityEngine;

namespace Samsara.Features.Event.Domain
{
    public class EventUseCase
    {
        private readonly string _logClass = $"[{nameof(EventUseCase)}]";

        private readonly IEventMasterDataRepository _eventMasterDataRepo;
        private readonly ICharacterRunRepository    _characterRunRepo;
        private readonly IStageRepository           _stageRepo;

        private EventSO     _currentEvent;
        private int         _dialogueIndex;
        private bool        _isDialoguePhase;
        private EventResult _appliedResult;  // Preserved across LoadEvent for IsReturningFromBattle

        public EventUseCase(
            IEventMasterDataRepository eventMasterDataRepo,
            ICharacterRunRepository    characterRunRepo,
            IStageRepository           stageRepo)
        {
            _eventMasterDataRepo = eventMasterDataRepo;
            _characterRunRepo    = characterRunRepo;
            _stageRepo           = stageRepo;
        }

        // ──────────────────────────────────────────────
        // Load
        // ──────────────────────────────────────────────

        /// <summary>
        /// 이벤트를 로드하고 상태를 초기화한다.
        /// _appliedResult는 IsReturningFromBattle 지원을 위해 초기화하지 않는다.
        /// </summary>
        public void LoadEvent(int eventId)
        {
            _currentEvent    = _eventMasterDataRepo.GetEvent(eventId);
            _dialogueIndex   = 0;
            _isDialoguePhase = _currentEvent.Dialogues is { Length: > 0 };
            Debug.Log($"{_logClass} LoadEvent: {_currentEvent.EventName} (id={eventId})");
        }

        // ──────────────────────────────────────────────
        // Event Info
        // ──────────────────────────────────────────────

        public string GetBackgroundSpriteKey()
            => _currentEvent?.BackgroundSpriteKey;

        // ──────────────────────────────────────────────
        // Dialogue
        // ──────────────────────────────────────────────

        public EventDialogue GetCurrentDialogue()
        {
            if (_currentEvent?.Dialogues == null) return null;
            if (_dialogueIndex >= _currentEvent.Dialogues.Length) return null;
            return _currentEvent.Dialogues[_dialogueIndex];
        }

        /// <returns>true = 다음 대사 있음, false = 대사 종료</returns>
        public bool AdvanceDialogue()
        {
            _dialogueIndex++;
            bool hasMore = _currentEvent?.Dialogues != null
                           && _dialogueIndex < _currentEvent.Dialogues.Length;
            if (!hasMore) _isDialoguePhase = false;
            return hasMore;
        }

        // ──────────────────────────────────────────────
        // Choice / Result
        // ──────────────────────────────────────────────

        public bool HasChoices()
            => _currentEvent?.Choices is { Length: > 0 };

        public EventChoice[] GetChoices()
            => _currentEvent?.Choices ?? Array.Empty<EventChoice>();

        /// <summary>
        /// 선택지를 적용하고 결과를 반환한다.
        /// 데이터 변경은 동기, 저장은 fire-and-forget (Save-on-Action).
        /// </summary>
        public EventResult ApplyChoice(int choiceIndex)
        {
            var choice = _currentEvent.Choices[choiceIndex];
            ApplyResultInternal(choice.Result);

            if (choice.NextEventId >= 0)
                SaveChainedEventId(choice.NextEventId);
            else
                ClearChainedEventId();

            _appliedResult = choice.Result;
            return choice.Result;
        }

        /// <summary>
        /// 선택지 없는 이벤트의 결과를 적용한다.
        /// </summary>
        public EventResult ApplyDirectResult()
        {
            var result = _currentEvent.Result;
            ApplyResultInternal(result);
            _appliedResult = result;
            return result;
        }

        public EventResult GetAppliedResult() => _appliedResult;

        // ──────────────────────────────────────────────
        // Chain
        // ──────────────────────────────────────────────

        /// <returns>체인 이벤트면 (이름, 현재 단계, 총 단계), 아니면 null</returns>
        public (string eventName, int step, int total)? GetChainInfo()
        {
            if (_currentEvent == null || _currentEvent.EventType != MasterData.EventType.Chained)
                return null;
            return (_currentEvent.EventName, _currentEvent.ChainStep, _currentEvent.ChainTotalSteps);
        }

        public void SaveChainedEventId(int nextEventId)
        {
            _stageRepo.SetPendingChainedEventId(nextEventId);
            _stageRepo.SaveAsync().Forget();
        }

        public void ClearChainedEventId()
        {
            _stageRepo.SetPendingChainedEventId(-1);
            _stageRepo.SaveAsync().Forget();
        }

        // ──────────────────────────────────────────────
        // Internal — Result Application
        // ──────────────────────────────────────────────

        private void ApplyResultInternal(EventResult result)
        {
            switch (result.ResultType)
            {
                case EventResultType.HpChange:
                    _characterRunRepo.RunData.Hp = Mathf.Clamp(
                        _characterRunRepo.RunData.Hp + (int)result.Value,
                        0,
                        _characterRunRepo.RunData.MaxHp);
                    _characterRunRepo.MarkDirty();
                    _characterRunRepo.SaveDataAsync().Forget();
                    break;

                case EventResultType.StatChange:
                    if (result.StatType.HasValue)
                    {
                        ApplyStatChange(result.StatType.Value, (int)result.Value);
                        _characterRunRepo.MarkDirty();
                        _characterRunRepo.SaveDataAsync().Forget();
                    }
                    break;

                case EventResultType.None:
                case EventResultType.ShopEncounter:
                case EventResultType.Battle:
                    // 데이터 변경 없음 — Presenter가 씬 전환 처리
                    break;
            }
        }

        private void ApplyStatChange(StatType statType, int delta)
        {
            var d = _characterRunRepo.RunData;
            switch (statType)
            {
                case StatType.Hp:
                    d.MaxHp += delta;
                    d.Hp     = Mathf.Min(d.Hp, d.MaxHp);
                    break;
                case StatType.Strength:  d.Strength  += delta; break;
                case StatType.Toughness: d.Toughness += delta; break;
                case StatType.Agility:   d.Agility   += delta; break;
            }
        }
    }
}
