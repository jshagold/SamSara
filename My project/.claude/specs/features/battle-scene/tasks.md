# BattleScene — Tasks

**Version:** 1.0.0 | **Date:** 2026-04-03 | **Status:** ✅ Confirmed
**Feature:** BattleScene
**Phase:** 3 — Battle System
**Constitution Ref:** §2, §3, §4, §5, §6, §7, §8, §9, §10, §11

---

## 1. Overview

Implementation instruction set for BattleScene Feature. Based on the design confirmed in Plan. Claude Code executes these tasks in order.

**IMPORTANT:** SkillSystem Patch-001 must be completed before BattleScene implementation (removes combat runtime features from SkillUseCase).

---

## 2. Prerequisites

- Read CLAUDE.md before any implementation
- Read the following existing files first:
  - Assets/_Game/Features/Battle/Domain/BattleUseCase.cs (existing stub — deletion target)
  - Assets/_Game/Features/Character/Data/CharacterRunData.cs
  - Assets/_Game/Features/Character/Domain/ICharacterRunRepository.cs
  - Assets/_Game/Features/Character/MasterData/CharacterStatsSO.cs
  - Assets/_Game/Features/Character/MasterData/EvolutionNodeSO.cs
  - Assets/_Game/Features/Character/MasterData/StatType.cs
  - Assets/_Game/Features/Skill/Domain/SkillUseCase.cs
  - Assets/_Game/Features/Skill/Data/SkillMasterDataRepository.cs
  - Assets/_Game/Features/Skill/Domain/ISkillMasterDataRepository.cs
  - Assets/_Game/Core/MasterData/SkillSO.cs
  - Assets/_Game/Core/MasterData/QTEPatternSO.cs
  - Assets/_Game/Core/MasterData/EnemySO.cs
  - Assets/_Game/Core/MasterData/CostType.cs
  - Assets/_Game/Core/MasterData/EffectType.cs
  - Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs
  - Assets/_Game/App/GameContext.cs
  - Assets/_Game/App/GlobalBootstrapper.cs
  - Assets/_Game/Core/Navigation/ISceneNavigator.cs
  - Assets/_Game/Core/Navigation/SceneKey.cs
  - Assets/_Game/Core/Popup/IPopupManager.cs
  - Assets/_Game/Features/MainScene/Presentation/MainSceneBootstrapper.cs (existing pattern reference)
  - Assets/_Game/Features/MiniGame/Presentation/MiniGameSceneBootstrapper.cs (existing pattern reference)
- Create Assets/_Game/Features/BattleScene/ folder
- Create .claude/specs/features/battle-scene/ folder

---

## 3. Files to Create/Modify

| Order | File | Path | Action |
|---|---|---|---|
| 0 | BattleUseCase.cs (stub) | Assets/_Game/Features/Battle/Domain/ | Delete |
| 1 | SceneKey.cs | Assets/_Game/Core/Navigation/ | Modify (add Battle value, skip if exists) |
| 2 | BattleResult.cs | Assets/_Game/Features/BattleScene/Domain/ | Create |
| 3 | PendingBattleContext.cs | Assets/_Game/Features/BattleScene/Domain/ | Create |
| 4 | BattleParticipant.cs | Assets/_Game/Features/BattleScene/Domain/ | Create |
| 5 | BattleRuntimeData.cs | Assets/_Game/Features/BattleScene/Domain/ | Create |
| 6 | BattleUseCase.cs | Assets/_Game/Features/BattleScene/Domain/ | Create |
| 7 | GameContext.cs | Assets/_Game/App/ | Modify (add PendingBattleContext?, LastBattleResult?) |
| 8 | TurnNumberView.cs | Assets/_Game/Features/BattleScene/Presentation/TopBar/ | Create |
| 9 | OptionButtonView.cs | Assets/_Game/Features/BattleScene/Presentation/TopBar/ | Create |
| 10 | ActionOrderView.cs | Assets/_Game/Features/BattleScene/Presentation/ActionOrder/ | Create |
| 11 | CharacterUnitView.cs | Assets/_Game/Features/BattleScene/Presentation/Field/ | Create |
| 12 | AllyFieldView.cs | Assets/_Game/Features/BattleScene/Presentation/Field/ | Create |
| 13 | EnemyFieldView.cs | Assets/_Game/Features/BattleScene/Presentation/Field/ | Create |
| 14 | SkillSelectionView.cs | Assets/_Game/Features/BattleScene/Presentation/Skill/ | Create |
| 15 | BattleQTEView.cs | Assets/_Game/Features/BattleScene/Presentation/QTE/ | Create |
| 16 | DamagePopupView.cs | Assets/_Game/Features/BattleScene/Presentation/Damage/ | Create |
| 17 | BattleResultPopupView.cs | Assets/_Game/Features/BattleScene/Presentation/Result/ | Create |
| 18 | BattleView.cs | Assets/_Game/Features/BattleScene/Presentation/ | Create |
| 19 | BattlePresenter.cs | Assets/_Game/Features/BattleScene/Presentation/ | Create |
| 20 | BattleSceneBootstrapper.cs | Assets/_Game/Features/BattleScene/Presentation/ | Create |
| 21 | decisions.md | .claude/specs/features/battle-scene/ | Create (empty) |

---

## 4. Implementation Order and Instructions

### Task 0 — Delete existing stub

Delete Assets/_Game/Features/Battle/Domain/BattleUseCase.cs. If Features/Battle/ folder is empty after deletion, delete the folder too.

---

### Task 1 — Modify SceneKey.cs

Add Battle entry to SceneKey enum. Skip if already exists. Constitution §6 SceneKey rules.

---

### Task 2 — Create BattleResult.cs

- BattleResult enum: Victory, Defeat
- Namespace: Samsara.Features.BattleScene.Domain
- Constitution §8: enum, no _logClass needed.

---

### Task 3 — Create PendingBattleContext.cs

- Pure C# class.
- BattleNodeDataSO battleNodeData field (read-only property).
- Constructor injection.
- Namespace: Samsara.Features.BattleScene.Domain

---

### Task 4 — Create BattleParticipant.cs

- Pure C# class (DTO). Must be serializable via Newtonsoft.Json in the future.
- Fields: id (int), isAlly (bool), currentHp (int), maxHp (int), strength (int), toughness (int), agility (int), actionGauge (float), skillIds (int[]), skillCooldowns (Dictionary<int, int>), isDead (bool), spriteKey (string).
- Constitution §8: _camelCase private fields, PascalCase public properties.

---

### Task 5 — Create BattleRuntimeData.cs

- Pure C# class (DTO). Must be serializable via Newtonsoft.Json in the future.
- Fields: allies (List<BattleParticipant>), enemies (List<BattleParticipant>), turnNumber (int), currentPhase (BattlePhase enum).
- Define BattlePhase enum in same file: SkillSelect, QTE, DamageProcess, Result.
- Constitution §2: Data classes have no logic.

---

### Task 6 — Create BattleUseCase.cs

Pure C# class. Core combat logic. Constitution §2 Data/Logic separation, §3 new only in Bootstrapper, §8 _logClass included.

**Constructor injection:**
- ISkillMasterDataRepository — skill/QTE pattern queries
- ICharacterRunRepository — ally HP/stats read

**Internal state:**
- BattleRuntimeData _runtimeData

**Methods (see Plan §3-2):**
- InitializeBattle(PendingBattleContext context): Get EnemySO[] from BattleNodeDataSO, convert each EnemySO to BattleParticipant (copy stats from CharacterStatsSO, copy skill IDs). Convert CharacterRunData + EvolutionNodeSO to ally BattleParticipant. Initialize all participant actionGauge = 0, all skillCooldowns to 0. turnNumber = 1.
- ProcessTick(): All surviving participants gauge += agility. Return participants with gauge >= 100 sorted by agility descending. Random on equal agility. Constitution §8: use pre-allocated list (called every tick, GC caution).
- ExecuteAction(BattleParticipant actor, int skillId, BattleParticipant target, float qteRate): Query SkillSO, baseDamage = actor.strength * skillDamageMultiplier. defense = target.toughness. finalDamage = max(1, floor((baseDamage - defense) * qteRate)). target.currentHp -= finalDamage. If target.currentHp <= 0, target.isDead = true. Set used skill cooldown to SkillSO.coolDown value. If HP cost exists, actor.currentHp -= cost (check actual SkillSO CostType/value fields). Return: finalDamage (int).
- ReduceCooldowns(BattleParticipant actor): All values in actor.skillCooldowns -= 1 (min 0).
- ConsumeGauge(BattleParticipant actor): actor.actionGauge -= 100.
- GetUsableSkills(BattleParticipant actor): Return skill ID list where cooldown is 0 and HP cost payable.
- SelectEnemySkill(BattleParticipant enemy): Random from GetUsableSkills() result. If no usable skills, handle default attack (record in decisions.md if judgment needed).
- SelectEnemyTarget(BattleParticipant enemy): First non-dead ally (Phase 1: 1 ally, auto).
- CalculateQTERate(bool isAttack, bool[] inputResults, int inputCount): successCount = count of true. successRate = successCount / inputCount. isAttack: return 1.0f + (0.5f * successRate). !isAttack: return 1.0f - (0.5f * successRate).
- CheckBattleEnd(): All enemies isDead -> Victory, all allies isDead -> Defeat, otherwise null.
- GetPredictedActionOrder(int lookAhead): Copy current gauge values and simulate, return next lookAhead action participant IDs.
- CleanupBattle(): _runtimeData = null.
- IncrementTurn(): _runtimeData.turnNumber++.

---

### Task 7 — Modify GameContext.cs

- Add PendingBattleContext? field + public accessor.
- Add BattleResult? field (LastBattleResult) + public accessor.
- Constitution §6: GameContext is pure C# class. New fields are nullable.

---

### Task 8 — Create TurnNumberView.cs

- MonoBehaviour. [SerializeField] private TMP_Text reference.
- SetTurn(int turn): Set "Turn {turn}" text.
- Constitution §7: Reset() auto-assignment. §8: _logClass, [SerializeField] private.

---

### Task 9 — Create OptionButtonView.cs

- MonoBehaviour. Button component.
- OnOptionClicked event.
- Reference existing OptionButtonView files for identical pattern.
- Constitution §7: Reset() auto-assignment.

---

### Task 10 — Create ActionOrderView.cs

- MonoBehaviour. Side vertical list.
- [SerializeField] private Transform _iconContainer, Image _iconPrefab.
- SetOrder(ActionOrderEntry[] entries): Render icon list. Define ActionOrderEntry in same file (id, spriteKey, isAlly).
- HighlightCurrent(int id): Highlight currently acting character.
- ClearOrder(): Remove all icons.
- Constitution §8: GC optimization — consider object pooling or pre-allocation. §5: ObjectPool candidate.

---

### Task 11 — Create CharacterUnitView.cs

- MonoBehaviour. Shared ally/enemy character UI.
- [SerializeField] private: Image _characterSprite, Image _hpBarFill, TMP_Text _hpText, CanvasGroup _canvasGroup, GameObject _highlightEffect, Transform _statusIconContainer (Phase 1: reserved only).
- ParticipantId property (int).
- Setup(int id, string spriteKey, int maxHp): Initial setup.
- SetHp(int current, int max): HP bar + text update. 3-stage sprite switch (100%/50%/0%).
- SetHighlight(bool on): Target selection highlight.
- SetDim(bool dim): CanvasGroup.alpha dimming.
- SetDead(): Death visual treatment.
- Constitution §7: Reset() auto-assignment (except Transform/RectTransform). §8: Safe Cleanup.

---

### Task 12 — Create AllyFieldView.cs

- MonoBehaviour. Ally area management.
- [SerializeField] private Transform _allyContainer, CharacterUnitView _unitPrefab.
- RenderAllies(BattleParticipant[] allies): Create CharacterUnitView instances.
- GetUnit(int participantId): Return CharacterUnitView by ID.
- ClearAllies(): Cleanup instances.

---

### Task 13 — Create EnemyFieldView.cs

- MonoBehaviour. Enemy area management.
- [SerializeField] private Transform _enemyContainer, CharacterUnitView _unitPrefab.
- RenderEnemies(BattleParticipant[] enemies): Create CharacterUnitView instances. Connect touch events per unit.
- GetUnit(int participantId): Return CharacterUnitView by ID.
- EnableTargetSelection(bool enable): Enable/disable target selection mode.
- OnTargetSelected(int participantId) event.
- ClearEnemies(): Cleanup instances.

---

### Task 14 — Create SkillSelectionView.cs

- MonoBehaviour. Skill icon list.
- [SerializeField] private Transform _skillContainer, Button _skillButtonPrefab.
- SetSkills(SkillDisplayData[] skills): Render skill icons. Define SkillDisplayData in same file (skillId, spriteKey, cooldownRemaining, isUsable).
- OnSkillSelected(int skillId) event.
- SetActive(bool active): Enable/disable. Dim when inactive.
- RefreshCooldowns(Dictionary<int, int> cooldowns): Update cooldown states.
- Constitution §5: Button prefab ObjectPool candidate.

---

### Task 15 — Create BattleQTEView.cs

- MonoBehaviour. QTE panel.
- Receives QTEPatternSO's QTEData[] array and displays QTE input UI sequentially.
- RunQTE(QTEData[] qteDataList, bool isAttack) -> UniTask<bool[]>. For each QTEData: display touch target at coordinate position, judge success/failure within duration, wait intervalToNext, next input. Return bool[] (per-input success/failure) after all inputs complete.
- Hide(): Hide QTE panel.
- Reference MiniGame TimingBarView pattern, but BattleQTEView is a separate implementation using QTEData coordinate/duration.
- Constitution §5: UniTask-based async. §11: Coroutines forbidden.

---

### Task 16 — Create DamagePopupView.cs

- MonoBehaviour. Damage number popup.
- [SerializeField] private TMP_Text _damageTextPrefab.
- ShowDamage(int damage, Vector3 worldPosition): Instantiate prefab, set text, DOTween (move up + fade out), Destroy on complete.
- Constitution §5: ObjectPool candidate (frequent create/destroy). §8: GC optimization.

---

### Task 17 — Create BattleResultPopupView.cs

- MonoBehaviour. Battle result popup.
- [SerializeField] private: TMP_Text _resultText, TMP_Text _statChangesText, Button _confirmButton, GameObject _root.
- Show(BattleResult result): Set text per result + show popup.
- OnConfirm event.
- Hide(): Hide popup.
- Constitution §7: Reset() auto-assignment.

---

### Task 18 — Create BattleView.cs

- MonoBehaviour. Scene root View.
- [SerializeField] private all child Views: TurnNumberView, OptionButtonView, ActionOrderView, AllyFieldView, EnemyFieldView, SkillSelectionView, BattleQTEView, DamagePopupView, BattleResultPopupView.
- [SerializeField] private Image _backgroundImage: Battle background.
- Expose public events relaying child View events to Presenter.
- SetBackground(Sprite sprite): Set background sprite.
- Constitution §7: Reset() auto-assignment. §8: _logClass.

---

### Task 19 — Create BattlePresenter.cs

Pure C# class. Bridges BattleUseCase and BattleView. Constitution §2: Presenter must NOT directly manipulate UI — delegate to View methods.

**Constructor injection:**
- BattleUseCase, BattleView, ISceneNavigator, IPopupManager, ISkillMasterDataRepository, GameContext

**Initialize(PendingBattleContext context):**
- Call BattleUseCase.InitializeBattle(context)
- AllyFieldView.RenderAllies() + EnemyFieldView.RenderEnemies()
- ActionOrderView.SetOrder() initial display
- TurnNumberView.SetTurn(1)
- BattleView.SetBackground() background setup
- Subscribe to events (SkillSelected, TargetSelected, QTE complete, etc.)
- RunBattleLoop().Forget() — start async turn loop

**async UniTask RunBattleLoop():**
- Implement Plan §6-2 turn loop flow directly.
- Per tick: ProcessTick -> action queue -> ally/enemy turn processing -> death check -> battle end check
- Ally turn: Activate SkillSelectionView -> await skill selection -> await target selection -> QTE (if applicable, await) -> ExecuteAction -> update UI
- Enemy turn: AI skill/target -> defense QTE (if applicable, await) -> ExecuteAction -> update UI
- On battle end call HandleBattleEnd()

**async UniTask HandleBattleEnd(BattleResult result):**
- BattleUseCase.CleanupBattle()
- BattleResultPopupView.Show(result)
- await OnConfirm
- GameContext.LastBattleResult = result
- GameContext.PendingBattleContext = null
- ISceneNavigator.LoadScene(SceneKey.Stage)

**Dispose():** Unsubscribe events. Constitution §8: Safe Cleanup (?. operator).

---

### Task 20 — Create BattleSceneBootstrapper.cs

- MonoBehaviour. Async initialization in Start(). Constitution §3: SceneBootstrapper initializes from Start().
- await GlobalBootstrapper.Instance.InitializationTask.
- Acquire ICharacterRunRepository, ISkillMasterDataRepository, IStageMasterDataRepository, PendingBattleContext from GameContext.
- Acquire ISceneNavigator, IPopupManager from GlobalBootstrapper. (G-06 reference)
- Create BattleUseCase instance (Constitution §3: new only in Bootstrapper).
- BattleView connected via Inspector ([SerializeField] private).
- Create BattlePresenter instance with injected dependencies.
- Call BattlePresenter.Initialize(PendingBattleContext).
- In OnDestroy(): BattlePresenter?.Dispose(). Constitution §8: Safe Cleanup.

---

### Task 21 — Create decisions.md

Create empty file at .claude/specs/features/battle-scene/decisions.md.

---

## 5. Validation

| # | Item | Verification Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console check |
| V-02 | Battle entry exists in SceneKey enum | File check |
| V-03 | Features/Battle/ folder deleted | File system check |
| V-04 | GameContext has PendingBattleContext?, LastBattleResult? fields | File check |
| V-05 | No console errors when placing BattleSceneBootstrapper in scene and pressing Play | Editor check |
| V-06 | Ally/enemy characters render in battle field | Editor Play check |
| V-07 | Action order UI displays on side | Editor Play check |
| V-08 | Skill selection UI activates on player turn and deactivates on enemy turn | Editor Play check |
| V-09 | Enemy sprite touch selects target with highlight | Editor Play check |
| V-10 | QTE panel displays when using skill with QTE pattern | Editor Play check |
| V-11 | Damage number popup shows and HP bar updates on damage | Editor Play check |
| V-12 | Victory result popup shows when all enemies dead | Editor Play check |
| V-13 | Defeat result popup shows when ally HP reaches 0 | Editor Play check |
| V-14 | Scene transitions to StageScene after result popup confirm | Editor Play check |
| V-15 | Turn number increments per action | Editor Play check |

---

## 6. Manual Tasks (Hak performs after Claude Code implementation)

| Order | Task |
|---|---|
| M-01 | Create Assets/_Game/Scenes/Battle.unity scene file |
| M-02 | Place Main Camera + Canvas (Screen Space - Camera) |
| M-03 | Place BattleSceneBootstrapper at scene root, connect BattleView in Inspector |
| M-04 | Connect each View's [SerializeField] fields in Inspector |
| M-05 | Add Battle scene to Build Settings |
| M-06 | Create CharacterUnitView prefab + connect _unitPrefab in AllyFieldView, EnemyFieldView |
| M-07 | Create skill button prefab + connect _skillButtonPrefab in SkillSelectionView |
| M-08 | Create ActionOrderView icon prefab + connect _iconPrefab |
| M-09 | Create DamagePopupView text prefab + connect _damageTextPrefab |
| M-10 | Prepare battle background Sprite and connect to BattleView |
| M-11 | Prepare character/enemy Sprites (HP 3-stage) |

---

## 7. Claude Code Delivery Guide

- Run claude from project root
- CLAUDE.md loads automatically
- **Apply Patch first:** Verify SkillSystem Patch-001 is already applied. If not, deliver .claude/specs/skill-system/patch-001.md first to complete SkillUseCase cleanup before BattleScene Tasks
- **Test data:** Battle test SO asset creation editor script is delivered as separate instruction (Dev/Features/Battle/BattleTestDataCreator.cs). After BattleScene code implementation, run test data creation script for validation
- Deliver .claude/specs/features/battle-scene/tasks.md for sequential implementation
- Record any judgment calls in .claude/specs/features/battle-scene/decisions.md
- DO NOT create files outside Assets/_Game/ (except decisions.md)