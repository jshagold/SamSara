# BattleScene — Tasks

**Version:** 2.0.0 | **Date:** 2026-04-08 | **Status:** ✅ Confirmed
**Feature:** BattleScene
**Phase:** 3 — Battle System
**Constitution Ref:** §2, §3, §4, §5, §6, §7, §8, §9, §10, §11

---

## 1. Overview

Implementation instruction set for BattleScene v2.0.0. This is a **modification/extension of existing v1.0.0 implementation**, NOT a from-scratch creation. Each task reads existing files and modifies/extends them.

**v2.0.0 Key Changes:**
- Turn loop presentation timing + attack motion (coordinate movement)
- Battle start/end presentation
- Skill → target → confirm button UX with cancel/reselect
- Skill UI ↔ QTE panel slide-in/out transition
- QTE closing ring animation + per-hit damage display + success/failure effects
- "Wait" action (skill slots + action slots structure)
- Skill icon 3-state dim
- Long-press info pattern
- Action order UI overhaul (portrait+name, 5 slots, queue touch→field highlight)
- Gauge-value-based simultaneous action sorting
- 4 battle event hook points

---

## 2. Prerequisites

- Read CLAUDE.md before any implementation
- Read the following **existing BattleScene implementation files** (modification targets):
  - Assets/_Game/Features/BattleScene/Domain/BattleUseCase.cs
  - Assets/_Game/Features/BattleScene/Domain/BattleRuntimeData.cs
  - Assets/_Game/Features/BattleScene/Domain/BattleParticipant.cs
  - Assets/_Game/Features/BattleScene/Domain/PendingBattleContext.cs
  - Assets/_Game/Features/BattleScene/Domain/BattleResult.cs
  - Assets/_Game/Features/BattleScene/Presentation/BattleSceneBootstrapper.cs
  - Assets/_Game/Features/BattleScene/Presentation/BattlePresenter.cs
  - Assets/_Game/Features/BattleScene/Presentation/BattleView.cs
  - Assets/_Game/Features/BattleScene/Presentation/TopBar/TurnNumberView.cs
  - Assets/_Game/Features/BattleScene/Presentation/TopBar/OptionButtonView.cs
  - Assets/_Game/Features/BattleScene/Presentation/ActionOrder/ActionOrderView.cs
  - Assets/_Game/Features/BattleScene/Presentation/Field/CharacterUnitView.cs
  - Assets/_Game/Features/BattleScene/Presentation/Field/AllyFieldView.cs
  - Assets/_Game/Features/BattleScene/Presentation/Field/EnemyFieldView.cs
  - Assets/_Game/Features/BattleScene/Presentation/Skill/SkillSelectionView.cs
  - Assets/_Game/Features/BattleScene/Presentation/QTE/BattleQTEView.cs
  - Assets/_Game/Features/BattleScene/Presentation/Damage/DamagePopupView.cs
  - Assets/_Game/Features/BattleScene/Presentation/Result/BattleResultPopupView.cs
- Also read **reference files**:
  - Assets/_Game/App/GameContext.cs
  - Assets/_Game/App/GlobalBootstrapper.cs
  - Assets/_Game/Core/MasterData/QTEPatternSO.cs
  - Assets/_Game/Core/MasterData/SkillSO.cs
  - Assets/_Game/Core/MasterData/EnemySO.cs
  - Assets/_Game/Features/Skill/Domain/ISkillMasterDataRepository.cs
  - Assets/_Game/Features/Stage/MasterData/BattleNodeDataSO.cs

---

## 3. Files to Create/Modify

All paths relative to Assets/_Game/. Create folders if they don't exist.

| # | File | Path | Action |
|---|---|---|---|
| 1 | BattleHookType.cs | Features/BattleScene/Domain/ | New |
| 2 | BattleEventData.cs | Features/BattleScene/Domain/ | New |
| 3 | BattleEventHookRunner.cs | Features/BattleScene/Domain/ | New |
| 4 | PendingBattleContext.cs | Features/BattleScene/Domain/ | Modify |
| 5 | BattleParticipant.cs | Features/BattleScene/Domain/ | Modify |
| 6 | BattleUseCase.cs | Features/BattleScene/Domain/ | Modify |
| 7 | ActionOrderSlotView.cs | Features/BattleScene/Presentation/ActionOrder/ | New |
| 8 | ActionSlotView.cs | Features/BattleScene/Presentation/Skill/ | New |
| 9 | ConfirmButtonView.cs | Features/BattleScene/Presentation/Skill/ | New |
| 10 | QTERingView.cs | Features/BattleScene/Presentation/QTE/ | New |
| 11 | BattleStartView.cs | Features/BattleScene/Presentation/Result/ | New |
| 12 | InfoTooltipView.cs | Features/BattleScene/Presentation/Info/ | New |
| 13 | ActionOrderView.cs | Features/BattleScene/Presentation/ActionOrder/ | Modify |
| 14 | CharacterUnitView.cs | Features/BattleScene/Presentation/Field/ | Modify |
| 15 | SkillSelectionView.cs | Features/BattleScene/Presentation/Skill/ | Modify |
| 16 | BattleQTEView.cs | Features/BattleScene/Presentation/QTE/ | Modify |
| 17 | DamagePopupView.cs | Features/BattleScene/Presentation/Damage/ | Modify |
| 18 | EnemyFieldView.cs | Features/BattleScene/Presentation/Field/ | Modify |
| 19 | BattleResultPopupView.cs | Features/BattleScene/Presentation/Result/ | Modify |
| 20 | BattleView.cs | Features/BattleScene/Presentation/ | Modify |
| 21 | BattlePresenter.cs | Features/BattleScene/Presentation/ | Modify |
| 22 | BattleSceneBootstrapper.cs | Features/BattleScene/Presentation/ | Modify |
| 23 | decisions.md | .claude/specs/features/battle-scene/ | Reset |

---

## 4. Implementation Order and Instructions

### Task 1 — New BattleHookType.cs
- enum: PreBattle, PerTick, PostDamage, PostBattle
- Namespace: Samsara.Features.BattleScene.Domain
- Constitution §8: enum, no _logClass.

### Task 2 — New BattleEventData.cs
- Pure C# class.
- Fields: BattleHookType hookType, string triggerCondition (future use), string eventReference (future event data ref)
- Constructor injection.
- Namespace: Samsara.Features.BattleScene.Domain. Constitution §8: _logClass.

### Task 3 — New BattleEventHookRunner.cs
- Pure C# class.
- Constructor: BattleEventData[] events (nullable)
- async UniTask CheckHook(BattleHookType hookType, BattleRuntimeData data): If events null or no matching hookType, return immediately. If match exists, execute (Phase 1: always returns immediately).
- Constitution §5: UniTask. §8: _logClass.

### Task 4 — Modify PendingBattleContext.cs
- Read existing file, add: BattleEventData[] battleEvents (nullable, default null in constructor)
- Keep existing battleNodeData field/constructor.

### Task 5 — Modify BattleParticipant.cs
- Read existing file, add fields: string portraitSpriteKey, string displayName
- Keep all existing fields.

### Task 6 — Modify BattleUseCase.cs
- Read existing file and apply:
- **Modify ProcessTick():** Sort gauge >= 100 participants by **actionGauge descending** (was: agility descending). Random on equal gauge.
- **Add ExecuteWait(BattleParticipant actor):** No action. Call ReduceCooldowns(actor) + ConsumeGauge(actor). IncrementTurn().
- **Add CalculatePerHitDamage(int totalDamage, int hitCount) -> int[]:** perHitDamage = floor(totalDamage / hitCount). Last hit = totalDamage - (perHitDamage * (hitCount-1)). Return int[]. Empty array if hitCount 0.
- **Add GetPredictedActionOrder(int lookAhead = 4) -> BattleParticipant[]:** Copy all surviving participants' actionGauge, simulate ticks, return next lookAhead actors. Same-tick: actionGauge descending. Constitution §8 GC: consider pre-allocated simulation arrays.
- Keep all existing methods. In InitializeBattle, also set portraitSpriteKey and displayName on BattleParticipant (from EnemySO spriteKey, name).

### Task 7 — New ActionOrderSlotView.cs
- MonoBehaviour. Individual action order queue slot.
- [SerializeField] private: Image _portraitImage, TMP_Text _nameText, Image _borderImage, GameObject _highlightEffect
- ParticipantId property (int).
- Setup(int id, string portraitKey, string name, bool isAlly): Initial setup. Ally=blue border, enemy=red.
- SetHighlight(bool on): Current actor highlight.
- OnSlotTouched event (for field highlight linkage).
- Button or EventTrigger for touch detection.
- Constitution §7: Reset(). §8: _logClass.

### Task 8 — New ActionSlotView.cs
- MonoBehaviour. Non-skill action buttons.
- [SerializeField] private: Button _waitButton, TMP_Text _waitButtonText ("Wait")
- OnWaitSelected event. SetActive(bool active).
- Constitution §7: Reset().

### Task 9 — New ConfirmButtonView.cs
- MonoBehaviour. Confirm button.
- [SerializeField] private: Button _confirmButton, TMP_Text _buttonText ("Confirm")
- OnConfirm event. SetInteractable(bool interactable).
- Constitution §7: Reset().

### Task 10 — New QTERingView.cs
- MonoBehaviour. Individual QTE input — closing ring animation.
- [SerializeField] private: Image _buttonImage, Image _ringImage, RectTransform _ringRect
- async UniTask<bool> RunRing(QTEData qteData):
  1. Position button+ring at coordinate.
  2. Ring scale from large (e.g. 3.0) to 1.0 over duration via DOTween.
  3. During shrink, detect touch: if ring scale within success range (e.g. 1.0~1.3) on touch = success.
  4. Timeout or out-of-range touch = failure.
  5. Return bool.
- Success range defined as constant (future: split for grade levels).
- Constitution §5: UniTask. §11: No Coroutines. DOTween. §8: _logClass.

### Task 11 — New BattleStartView.cs
- MonoBehaviour. Battle start presentation.
- [SerializeField] private: TMP_Text _battleStartText, CanvasGroup _canvasGroup
- async UniTask PlayStartPresentation():
  1. Show "Battle Start!" text.
  2. DOTween scale up + fade in (~0.5s).
  3. Hold (~0.5s).
  4. Fade out (~0.3s).
  5. Deactivate on complete.
- Enemy slide-in: handled by EnemyFieldView.PlaySlideIn() separately (can be omitted if costly, record in decisions.md).
- Constitution §5: UniTask. §11: DOTween.

### Task 12 — New InfoTooltipView.cs
- MonoBehaviour. Long-press info tooltip (enemy/ally/skill shared).
- [SerializeField] private: GameObject _root, TMP_Text _titleText, TMP_Text _detailText, RectTransform _tooltipRect
- Show(string title, string detail, Vector2 screenPosition): Display tooltip near target. Clamp to screen bounds.
- Hide(): Hide tooltip.
- Constitution §7: Reset(). §8: _logClass.

### Task 13 — Modify ActionOrderView.cs
- Read existing file, full overhaul:
- Replace Image-based icon rendering with **5 ActionOrderSlotView** based system.
- [SerializeField] private ActionOrderSlotView[] _slots (5 slots from Inspector) or _slotPrefab + object pool.
- SetOrder(BattleParticipant[] predicted): Setup 5 slots with portrait+name+border. predicted[0] = current actor.
- HighlightCurrent(int participantId): Highlight matching slot.
- OnSlotTouched(int participantId) event → Presenter → field highlight.
- Remove old icon rendering code.
- Ensure prediction queue displays in correct order (existing bug fix point).
- Constitution §5: ObjectPool if applicable. §8: GC optimization.

### Task 14 — Modify CharacterUnitView.cs
- Read existing file, add:
- **Active highlight:** [SerializeField] private GameObject _activeHighlight (separate from target selection highlight). SetActiveHighlight(bool on): Turn start to action end.
- **Attack motion:** async UniTask PlayAttackMotion(Vector3 targetPosition): DOTween move toward target (~0.3s, ~70% of distance). async UniTask PlayReturnMotion(): Return to _originalPosition (~0.2s). Store _originalPosition (Vector3) on Setup().
- **Long-press:** EventTrigger or IPointerDownHandler/IPointerUpHandler. Threshold: 0.5s. OnLongPress(int participantId) event. Short touch retains existing target selection.
- Constitution §11: DOTween. §5: UniTask.

### Task 15 — Modify SkillSelectionView.cs
- Read existing file, add:
- **3-state dim:** Add SkillState enum: Usable, OnCooldown, HpInsufficient. Usable=normal, OnCooldown(dimA)=dark overlay+remaining turns, HpInsufficient(dimB)=red overlay/border. Extend SkillDisplayData with SkillState. Replace single dim with 3-state.
- **Long-press:** Add long-press detection per skill button. OnSkillLongPress(int skillId) event. Short touch retains OnSkillSelected.
- **ConfirmButtonView:** [SerializeField] private ConfirmButtonView _confirmButton (or BattleView manages separately).

### Task 16 — Modify BattleQTEView.cs
- Read existing file, full overhaul:
- **Closing ring:** Remove existing touch judgment. Delegate to QTERingView per input. RunQTE internally: for each qteData → await QTERingView.RunRing(qteData) → collect bool[]. Or Presenter iterates QTERingView directly (Plan RQ-P01).
- **Slide-in/out:** async UniTask SlideIn(bool isDefense): Slide from bottom. isDefense=warning color border. async UniTask SlideOut(): Slide out.
- **Panel visuals:** [SerializeField] private Image _panelBackground, Image _panelBorder. Attack QTE=default color. Defense QTE=warning (red) border.
- **QTERingView ref:** [SerializeField] private QTERingView _ringView.
- Constitution §5: UniTask. §11: DOTween, no Coroutines.

### Task 17 — Modify DamagePopupView.cs
- Read existing file, add:
- ShowHitDamage(int damage, bool success, Vector3 worldPosition): success=large bright font + scale punch effect. failure="Miss" text + small dim font. Font size/color as constants for future extension.
- Keep existing ShowDamage method.
- Constitution §5: ObjectPool. §11: DOTween.

### Task 18 — Modify EnemyFieldView.cs
- Read existing file, add:
- **Same-enemy numbering:** In RenderEnemies, assign numbers to enemies with same EnemySO. 2+ same name: "Slime①", "Slime②". 1 only: original name. Set displayName on BattleParticipant.
- **Slide-in (optional):** async UniTask PlaySlideIn(): Enemy characters slide in from off-screen. Can be omitted if costly (record in decisions.md).

### Task 19 — Modify BattleResultPopupView.cs
- Read existing file, add:
- async UniTask ShowEndPresentation(BattleResult result):
  1. Display "Victory" or "Defeat" large center text.
  2. DOTween scale up + fade in.
  3. Hold (~1s).
  4. Text disappears.
  5. Show existing result popup.
  6. await user confirm (existing OnConfirm event).
- Refactor existing Show() to be called internally.

### Task 20 — Modify BattleView.cs
- Read existing file, add:
- **New View refs:** [SerializeField] private ActionSlotView, ConfirmButtonView, BattleStartView, InfoTooltipView. Keep all existing refs.
- **UI transition:** async UniTask TransitionToQTE(bool isDefense): SkillSelectionView+ActionSlotView+ConfirmButtonView slide out → BattleQTEView.SlideIn(isDefense). async UniTask TransitionToSkillUI(): BattleQTEView.SlideOut() → skill area slide in. ShowSkillUI()/HideSkillUI(): immediate show/hide.
- **Enemy turn labels:** [SerializeField] private TMP_Text _turnLabel. async UniTask ShowEnemyTurnLabel(string displayName): Show + delay + disappear. async UniTask ShowSkillNameLabel(string skillName): Show + delay + disappear.
- Constitution §7: Update Reset() for new fields.

### Task 21 — Modify BattlePresenter.cs
- Read existing file, **full turn loop overhaul**. Implement Plan §6-2 flow directly.
- **Add to constructor:** BattleEventHookRunner.
- **Modify Initialize():** Add same-enemy numbering (displayName), await hookRunner.CheckHook(PreBattle), await battleStartView.PlayStartPresentation().
- **Modify RunBattleLoop():** Full revision per Plan §6-2. Key: per-tick hook check, gauge-desc sorting, ally turn (skill UI + action slots + confirm + TransitionToQTE + attack motion + per-hit QTE/damage + TransitionToSkillUI + return motion), enemy turn (turn label + skill label + attack motion + defense QTE + return motion), active highlight on/off, action order queue refresh.
- **Add long-press handling:** CharacterUnitView.OnLongPress → InfoTooltipView. SkillSelectionView.OnSkillLongPress → InfoTooltipView.
- **Add queue touch handling:** ActionOrderView.OnSlotTouched → CharacterUnitView highlight.
- **Modify HandleBattleEnd():** Add hookRunner.CheckHook(PostBattle). Call ShowEndPresentation instead of Show.
- Constitution §2: No direct UI manipulation. §5: UniTask, .Forget(). §8: Safe Cleanup.

### Task 22 — Modify BattleSceneBootstrapper.cs
- Read existing file, add:
- Get battleEvents from PendingBattleContext.
- Create BattleEventHookRunner instance (Constitution §3: new only in Bootstrapper).
- Inject BattleEventHookRunner into BattlePresenter constructor.
- Keep all existing code.

### Task 23 — Reset decisions.md
- Clear .claude/specs/features/battle-scene/decisions.md and write v2.0.0 header:
  # BattleScene v2.0.0 Decisions
  (Record any decisions made during implementation here)

---

## 5. Validation

| # | Item | Method |
|---|---|---|
| V-01 | No compile errors in Unity console | Console |
| V-02 | "Battle Start!" presentation displays on battle entry | Editor Play |
| V-03 | Action order queue shows 5 slots with portrait+name | Editor Play |
| V-04 | Queue order is correct (gauge descending) | Editor Play |
| V-05 | Queue slot touch highlights corresponding field character | Editor Play |
| V-06 | Active character has highlight border during their turn | Editor Play |
| V-07 | Skill → target → confirm button flow works | Editor Play |
| V-08 | Touching different skill during target selection changes skill | Editor Play |
| V-09 | "Wait" button skips turn and reduces cooldowns | Editor Play |
| V-10 | Skill icons show cooldown (dim A) and HP insufficient (dim B) distinctly | Editor Play |
| V-11 | After confirm, skill UI slides out and QTE panel slides in | Editor Play |
| V-12 | QTE ring shrinks and touch judgment works | Editor Play |
| V-13 | Per-hit damage numbers display (success: bright, failure: Miss) | Editor Play |
| V-14 | Characters move toward target on attack and return | Editor Play |
| V-15 | Enemy turn shows "[name]'s turn" + skill name | Editor Play |
| V-16 | Defense QTE panel has warning (red) border | Editor Play |
| V-17 | Long-press on enemy/ally/skill shows info tooltip | Editor Play |
| V-18 | Same-type enemies get numbered names (Slime①, Slime②) | Editor Play |
| V-19 | Victory/Defeat text presentation then result popup on battle end | Editor Play |
| V-20 | Scene returns to StageScene after result popup confirm | Editor Play |

---

## 6. Manual Tasks (Hak performs after Claude Code implementation)

Additions to existing M-01~M-11:

| Order | Task |
|---|---|
| M-12 | Create ActionOrderSlotView prefab (portrait+name+border) + connect to ActionOrderView |
| M-13 | Place ActionSlotView GameObject + "Wait" button + Inspector connections |
| M-14 | Place ConfirmButtonView GameObject + "Confirm" button + Inspector connections |
| M-15 | Create QTERingView prefab (ring image + button image) + connect to BattleQTEView |
| M-16 | Place BattleStartView GameObject + "Battle Start!" text + Inspector connections |
| M-17 | Create InfoTooltipView prefab/GameObject + connect to BattleView |
| M-18 | Connect BattleView new SerializeField refs (_actionSlotView, _confirmButtonView, _battleStartView, _infoTooltipView, _turnLabel) |
| M-19 | Prepare per-character portrait sprites + place in Art/Sprites/ |
| M-20 | Prepare QTE panel background/border sprites |
| M-21 | Complete M-01~M-11 if not yet done |

---

## 7. Claude Code Delivery Guide

- Run claude from project root
- CLAUDE.md loads automatically
- Deliver .claude/specs/features/battle-scene/tasks.md for sequential implementation
- **IMPORTANT:** v2.0.0 modifies existing files — each task MUST read existing file first as instructed
- Record any judgment calls in .claude/specs/features/battle-scene/decisions.md with [DECISION], [BACKLOG], or [SPEC-GAP] tags
- DO NOT create files outside Assets/_Game/ (except decisions.md)