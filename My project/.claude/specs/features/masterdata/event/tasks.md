# Event MasterData — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-17 | **Status:** Ready for Claude Code
**Based on:** Specify v1.0.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- [ ] Read `CLAUDE.md` first
- [ ] Create `Assets/_Game/Features/Event/MasterData/` folder if missing
- [ ] Create `Assets/Resources/MasterData/Event/` folder if missing
- [ ] DO NOT modify any files not explicitly listed below

---

## TASK-01 — EventType.cs

**Path:** `Assets/_Game/Features/Event/MasterData/EventType.cs`
**Type:** Enum
**Priority:** First (no dependencies)

### Implementation

    namespace Samsara.Features.Event.MasterData
    {
        public enum EventType
        {
            OneShot,
            Chained
        }
    }

---

## TASK-02 — EventResultType.cs

**Path:** `Assets/_Game/Features/Event/MasterData/EventResultType.cs`
**Type:** Enum
**Priority:** After TASK-01

### Implementation

    namespace Samsara.Features.Event.MasterData
    {
        public enum EventResultType
        {
            None,
            HpChange,
            StatChange,
            ShopEncounter,
            Battle,
            Death
        }
    }

---

## TASK-03 — EventSO.cs

**Path:** `Assets/_Game/Features/Event/MasterData/EventSO.cs`
**Type:** ScriptableObject
**Priority:** After TASK-01, TASK-02

### Implementation

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

---

## TASK-04 — Validation

**Priority:** Final — after all tasks complete.

| # | Check | Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | `Assets > Create` menu shows `Samsara/Event/Event` | Editor check |
| V-03 | `EventSO` .asset shows EventId, EventName, EventType enum, BackgroundSpriteKey fields in Inspector | Inspector check |
| V-04 | `EventSO` Dialogues array entries show PortraitSpriteKey and DialogueText fields | Inspector check |
| V-05 | `EventSO` Result field shows ResultType enum (None/HpChange/StatChange/ShopEncounter/Battle/Death) and Value fields | Inspector check |
| V-06 | `EventSO` Choices array entries show ChoiceText, Result (nested), and NextEventId fields | Inspector check |
| V-07 | EventType enum shows OneShot and Chained options | Inspector check |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/Features/Event/MasterData/EventType.cs`
  - `Assets/_Game/Features/Event/MasterData/EventResultType.cs`
  - `Assets/_Game/Features/Event/MasterData/EventSO.cs`
- **Implementation order:** TASK-01 → TASK-02 → TASK-03 → TASK-04
- **DO NOT** create files outside `Assets/_Game/`.
- **DO NOT** reference or copy patterns from `Assets/_Game/Dev/`.
- If you make any judgment calls not covered by this Spec,
  record them in `.claude/specs/features/masterdata/event/decisions.md`