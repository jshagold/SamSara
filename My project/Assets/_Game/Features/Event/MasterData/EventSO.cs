using System;
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

        public int EventId => _eventId;
        public string EventName => _eventName;
        public EventType EventType => _eventType;
        public string BackgroundSpriteKey => _backgroundSpriteKey;
        public EventDialogue[] Dialogues => _dialogues;
        public EventResult Result => _result;
        public EventChoice[] Choices => _choices;
    }

    [Serializable]
    public class EventDialogue
    {
        [SerializeField] private string _portraitSpriteKey;  // Speaker portrait (player or NPC)
        [SerializeField] private string _dialogueText;

        public string PortraitSpriteKey => _portraitSpriteKey;
        public string DialogueText => _dialogueText;
    }

    [Serializable]
    public class EventResult
    {
        [SerializeField] private EventResultType _resultType;
        [SerializeField] private float _value;  // Used for HpChange, StatChange

        public EventResultType ResultType => _resultType;
        public float Value => _value;
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
