using System;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Ending.MasterData;
using UnityEngine;

namespace Samsara.Features.Event.MasterData
{
    [CreateAssetMenu(fileName = "EventSO", menuName = "Samsara/Event/Event")]
    public class EventSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(EventSO)}]";

        [SerializeField] private int _eventId;
        [SerializeField] private string _eventName;
        [SerializeField] private EventType _eventType;
        [SerializeField] private string _backgroundSpriteKey;
        [SerializeField] private EventDialogue[] _dialogues;
        [SerializeField] private EventResult _result;
        [SerializeField] private EventChoice[] _choices;
        [SerializeField] private int _chainTotalSteps;  // Total steps in chained sequence. 0 for one-shot.
        [SerializeField] private int _chainStep;        // This event's step in the chain. 0 for one-shot.
        [SerializeField] private EventSource _eventSource;

        public int EventId => _eventId;
        public string EventName => _eventName;
        public EventType EventType => _eventType;
        public string BackgroundSpriteKey => _backgroundSpriteKey;
        public EventDialogue[] Dialogues => _dialogues;
        public EventResult Result => _result;
        public EventChoice[] Choices => _choices;
        public int ChainTotalSteps => _chainTotalSteps;
        public int ChainStep => _chainStep;
        public EventSource EventSource => _eventSource;
    }

    [Serializable]
    public class EventDialogue
    {
        [SerializeField] private string _portraitSpriteKey;  // Speaker portrait (player or NPC)
        [SerializeField] private string _dialogueText;
        [SerializeField] private string _speakerName;
        [SerializeField] private SpeakerPosition _speakerPosition;

        public string PortraitSpriteKey => _portraitSpriteKey;
        public string DialogueText => _dialogueText;
        public string SpeakerName => _speakerName;
        public SpeakerPosition SpeakerPosition => _speakerPosition;

        /// <summary>코드에서 직접 생성할 때 사용하는 생성자.</summary>
        public EventDialogue(string portraitSpriteKey, string dialogueText, string speakerName, SpeakerPosition speakerPosition)
        {
            _portraitSpriteKey = portraitSpriteKey;
            _dialogueText      = dialogueText;
            _speakerName       = speakerName;
            _speakerPosition   = speakerPosition;
        }
    }

    [Serializable]
    public class EventResult
    {
        [SerializeField] private EventResultType    _resultType;
        [SerializeField] private float              _value;          // Used for HpChange, StatChange
        [SerializeField] private bool               _useStatType;    // Enable to specify which stat to change
        [SerializeField] private StatType           _statType;       // Which stat to change. Only used when _useStatType is true.
        [SerializeField] private bool               _hasMerchantId;  // Enable to specify which merchant to activate
        [SerializeField] private int                _merchantId;     // Merchant ID. Only used when _hasMerchantId is true.
        [SerializeField] private EndingCandidateSlot _endingSlot;   // 이 결과 적용 시 시도할 엔딩 후보 슬롯. IsEmpty면 매칭 스킵.

        public EventResultType     ResultType  => _resultType;
        public float               Value       => _value;
        public StatType?           StatType    => _useStatType ? _statType : (StatType?)null;
        public int?                MerchantId  => _hasMerchantId ? _merchantId : (int?)null;
        public EndingCandidateSlot EndingSlot  => _endingSlot;
    }

    [Serializable]
    public class EventChoice
    {
        [SerializeField] private string _choiceText;
        [SerializeField] private EventResult _result;
        [SerializeField] private int _nextEventId;  // Next event ID for chained events. -1 if not chained.

        public string ChoiceText => _choiceText;
        public EventResult Result => _result;
        public int NextEventId => _nextEventId;
    }
}
