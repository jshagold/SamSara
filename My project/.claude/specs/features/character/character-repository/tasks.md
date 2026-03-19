# CharacterRepository — Tasks

**Version:** 1.0.0 | **Date:** 2026-03-19 | **Status:** Ready for Claude Code
**Based on:** Specify v1.0.0 / Plan v1.0.0

---

## Pre-Implementation Checklist

> Claude Code MUST verify the following before starting any task.

- [ ] Read `CLAUDE.md` first
- [ ] Verify `Assets/_Game/Features/Character/Domain/` folder exists (create if missing)
- [ ] Verify `Assets/_Game/Features/Character/Data/` folder exists (create if missing)
- [ ] Read existing `GameContext.cs` before modifying
- [ ] DO NOT modify any files not explicitly listed below

---

## TASK-01 — ICharacterRunRepository.cs

**Path:** `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`
**Type:** Interface
**Priority:** First (no dependencies)

### Implementation

    using Cysharp.Threading.Tasks;
    using Samsara.Features.Character.Data;
    using Samsara.Features.Character.MasterData;
    
    namespace Samsara.Features.Character.Domain
    {
        public interface ICharacterRunRepository
        {
            CharacterRunData RunData { get; }
            void InitializeNewRun(string startingEvolutionNodeId, CharacterStatsSO baseStats);
            UniTask SaveDataAsync();
            void SaveDataSync();
            UniTask LoadDataAsync();
        }
    }

---

## TASK-02 — ICharacterAccountRepository.cs

**Path:** `Assets/_Game/Features/Character/Domain/ICharacterAccountRepository.cs`
**Type:** Interface
**Priority:** First (no dependencies)

### Implementation

    using Cysharp.Threading.Tasks;
    using Samsara.Features.Character.Data;
    
    namespace Samsara.Features.Character.Domain
    {
        public interface ICharacterAccountRepository
        {
            CharacterAccountData AccountData { get; }
            void UnlockEvolutionNode(string nodeId);
            void RegisterCodex(string nodeId);
            UniTask SaveDataAsync();
            void SaveDataSync();
            UniTask LoadDataAsync();
        }
    }

---

## TASK-03 — CharacterRunData.cs

**Path:** `Assets/_Game/Features/Character/Data/CharacterRunData.cs`
**Type:** Serialization model (pure data class, no logic)
**Priority:** First (no dependencies)

### Implementation

    namespace Samsara.Features.Character.Data
    {
        public class CharacterRunData
        {
            public int Hp;
            public int Strength;
            public int Toughness;
            public int Speed;
            public string EvolutionNodeId;
            public int Day;
            public int Gold;
        }
    }

---

## TASK-04 — CharacterAccountData.cs

**Path:** `Assets/_Game/Features/Character/Data/CharacterAccountData.cs`
**Type:** Serialization model (pure data class, no logic)
**Priority:** First (no dependencies)

### Implementation

    using System.Collections.Generic;
    
    namespace Samsara.Features.Character.Data
    {
        public class CharacterAccountData
        {
            public List<string> UnlockedEvolutionNodeIds = new();
            public List<string> CompletedCodexIds = new();
            public int Gems;
        }
    }

---

## TASK-05 — CharacterRunRepository.cs

**Path:** `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs`
**Type:** Repository implementation
**Priority:** After TASK-01, TASK-03

### Implementation

    using System.IO;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Samsara.Features.Character.Domain;
    using Samsara.Features.Character.MasterData;
    using UnityEngine;
    
    namespace Samsara.Features.Character.Data
    {
        public class CharacterRunRepository : ICharacterRunRepository
        {
            private readonly string _logClass = $"[{nameof(CharacterRunRepository)}]";
            private readonly string _savePath =
                Application.persistentDataPath + "/run_save.json";
    
            private CharacterRunData _runData;
            private bool _isDirty;
    
            public CharacterRunData RunData => _runData;
    
            public async UniTask LoadDataAsync()
            {
                await UniTask.RunOnThreadPool(() =>
                {
                    if (File.Exists(_savePath))
                    {
                        var json = File.ReadAllText(_savePath);
                        _runData = JsonConvert.DeserializeObject<CharacterRunData>(json);
                    }
                    else
                    {
                        _runData = new CharacterRunData();
                    }
                });
            }
    
            public void InitializeNewRun(string startingEvolutionNodeId, CharacterStatsSO baseStats)
            {
                _runData = new CharacterRunData
                {
                    Hp = baseStats.Hp,
                    Strength = baseStats.Strength,
                    Toughness = baseStats.Toughness,
                    Speed = baseStats.Speed,
                    EvolutionNodeId = startingEvolutionNodeId,
                    Day = 1,
                    Gold = 0
                };
                _isDirty = true;
                SaveDataAsync().Forget();
            }
    
            public async UniTask SaveDataAsync()
            {
                if (!_isDirty) return;
                await UniTask.RunOnThreadPool(() =>
                {
                    var json = JsonConvert.SerializeObject(_runData);
                    File.WriteAllText(_savePath, json);
                });
                _isDirty = false;
            }
    
            public void SaveDataSync()
            {
                if (!_isDirty) return;
                var json = JsonConvert.SerializeObject(_runData);
                File.WriteAllText(_savePath, json);
                _isDirty = false;
            }
        }
    }

---

## TASK-06 — CharacterAccountRepository.cs

**Path:** `Assets/_Game/Features/Character/Data/CharacterAccountRepository.cs`
**Type:** Repository implementation
**Priority:** After TASK-02, TASK-04

### Implementation

    using System.IO;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Samsara.Features.Character.Domain;
    using UnityEngine;
    
    namespace Samsara.Features.Character.Data
    {
        public class CharacterAccountRepository : ICharacterAccountRepository
        {
            private readonly string _logClass = $"[{nameof(CharacterAccountRepository)}]";
            private readonly string _savePath =
                Application.persistentDataPath + "/account_save.json";
    
            private CharacterAccountData _accountData;
            private bool _isDirty;
    
            public CharacterAccountData AccountData => _accountData;
    
            public async UniTask LoadDataAsync()
            {
                await UniTask.RunOnThreadPool(() =>
                {
                    if (File.Exists(_savePath))
                    {
                        var json = File.ReadAllText(_savePath);
                        _accountData = JsonConvert.DeserializeObject<CharacterAccountData>(json);
                    }
                    else
                    {
                        _accountData = new CharacterAccountData();
                    }
                });
            }
    
            public void UnlockEvolutionNode(string nodeId)
            {
                if (_accountData.UnlockedEvolutionNodeIds.Contains(nodeId)) return;
                _accountData.UnlockedEvolutionNodeIds.Add(nodeId);
                _isDirty = true;
            }
    
            public void RegisterCodex(string nodeId)
            {
                if (_accountData.CompletedCodexIds.Contains(nodeId)) return;
                _accountData.CompletedCodexIds.Add(nodeId);
                _isDirty = true;
            }
    
            public async UniTask SaveDataAsync()
            {
                if (!_isDirty) return;
                await UniTask.RunOnThreadPool(() =>
                {
                    var json = JsonConvert.SerializeObject(_accountData);
                    File.WriteAllText(_savePath, json);
                });
                _isDirty = false;
            }
    
            public void SaveDataSync()
            {
                if (!_isDirty) return;
                var json = JsonConvert.SerializeObject(_accountData);
                File.WriteAllText(_savePath, json);
                _isDirty = false;
            }
        }
    }

---

## TASK-07 — GameContext.cs (Modify)

**Path:** `Assets/_Game/App/GameContext.cs`
**Type:** Modification
**Priority:** After TASK-01 ~ TASK-06

Read the existing file first, then add the following:

1. Add fields and public properties for `ICharacterRunRepository` and `ICharacterAccountRepository`
2. In constructor: instantiate `new CharacterRunRepository()` and `new CharacterAccountRepository()`
3. In `LoadAllDataAsync()`: add both `LoadDataAsync()` calls to `UniTask.WhenAll`
4. In `SaveAllDataSync()`: add both `SaveDataSync()` calls

---

## TASK-08 — Validation

**Priority:** Final — after all tasks complete.

| # | Check | Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | CharacterRunRepository and CharacterAccountRepository are registered in GameContext | Runtime debug log |
| V-03 | `LoadDataAsync()` initializes with default values when no save file exists | Play mode |
| V-04 | `run_save.json` and `account_save.json` are created in `persistentDataPath` after `SaveDataAsync()` | File system check |
| V-05 | Values are correctly restored on next run after `SaveDataAsync()` | Play mode |
| V-06 | `SaveDataAsync()` does not write file when Dirty Flag is false | Log check |
| V-07 | `UnlockEvolutionNode()` does not produce duplicate entries on repeated calls | Play mode |
| V-08 | `InitializeNewRun()` sets RunData to correct initial values | Play mode |

---

## Claude Code Implementation Guide

- Read `CLAUDE.md` first before any implementation.
- **Files to create:**
  - `Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs`
  - `Assets/_Game/Features/Character/Domain/ICharacterAccountRepository.cs`
  - `Assets/_Game/Features/Character/Data/CharacterRunData.cs`
  - `Assets/_Game/Features/Character/Data/CharacterAccountData.cs`
  - `Assets/_Game/Features/Character/Data/CharacterRunRepository.cs`
  - `Assets/_Game/Features/Character/Data/CharacterAccountRepository.cs`
- **Files to modify:**
  - `Assets/_Game/App/GameContext.cs`
- **Implementation order:** TASK-01 → TASK-02 → TASK-03 → TASK-04 → TASK-05 → TASK-06 → TASK-07 → TASK-08
- **DO NOT** create files outside `Assets/_Game/`.
- If you make any judgment calls not covered by this Spec,
  record them in `.claude/specs/features/character/character-repository/decisions.md`