# CharacterInfoScene v2.0.0 — Tasks

**Feature:** CharacterInfoScene v2.0.0
**Phase:** 5-C — Inventory & Shop v2
**Version:** 2.0.0
**Date:** 2026-04-15
**Status:** Confirmed
**Constitution Reference:** §2, §3, §4, §5, §7, §8, §9, §10, §11

---

## 1. Overview

Implementation guide for Claude Code to add inventory functionality to CharacterInfoScene v2.0.0. Modifies/adds Presentation layer files on top of existing v1.0.0 code.

**Scope:** Presentation layer changes only. Domain/Data layer (InventorySystem) is already implemented.

---

## 2. Prerequisites

- Read CLAUDE.md first
- Read the following existing files before making changes:
    - Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoSceneBootstrapper.cs
    - Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoPresenter.cs
    - Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoView.cs
    - Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/InventoryView.cs
    - Assets/_Game/Features/Inventory/Domain/InventoryUseCase.cs
    - Assets/_Game/Features/Inventory/Domain/InventorySlotDisplayData.cs
    - Assets/_Game/Features/Inventory/Domain/ItemDetailData.cs
    - Assets/_Game/Features/Inventory/Domain/IItemData.cs
    - Assets/_Game/App/GameContext.cs
    - Assets/_Game/Core/Popup/IPopupManager.cs
    - Assets/_Game/Core/SpriteLoading/ISpriteLoader.cs
- .claude/specs/features/character-info-scene/ folder already exists

---

## 3. Files to Create/Modify

| Order | File | Path | Action |
|---|---|---|---|
| 1 | ItemInfoPopupResult.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/Popup/ | Create |
| 2 | DiscardPopupResult.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/Popup/ | Create |
| 3 | InventorySlotView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/ | Create |
| 4 | ItemInfoPopupView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/Popup/ | Create |
| 5 | DiscardItemPopupView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/Popup/ | Create |
| 6 | InventoryView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/InfoScroll/ | Modify |
| 7 | CharacterInfoView.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/ | Modify |
| 8 | CharacterInfoPresenter.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/ | Modify |
| 9 | CharacterInfoSceneBootstrapper.cs | Assets/_Game/Features/CharacterInfoScene/Presentation/ | Modify |
| 10 | decisions.md | .claude/specs/features/character-info-scene/ | Modify (add v2.0.0 section) |

---

## 4. Implementation Order and Instructions

### Task 1 — Create ItemInfoPopupResult.cs

- namespace: Samsara.Features.CharacterInfoScene.Presentation
- Define ItemInfoPopupResult enum: Use, Discard, Close

---

### Task 2 — Create DiscardPopupResult.cs

- namespace: Samsara.Features.CharacterInfoScene.Presentation
- Define DiscardPopupResult struct:
    - bool Confirmed
    - int Quantity

---

### Task 3 — Create InventorySlotView.cs

- namespace: Samsara.Features.CharacterInfoScene.Presentation
- MonoBehaviour. Individual slot UI.
- [SerializeField] private Image _iconImage.
- [SerializeField] private TMP_Text _quantityText.
- [SerializeField] private GameObject _emptyState.
- [SerializeField] private GameObject _filledState.
- [SerializeField] private Button _button.
- Action OnTapped event: Register on _button.onClick. Do not fire when slot is empty (_isEmpty internal flag).
- SetSlot(Sprite icon, int quantity):
    - _filledState.SetActive(true), _emptyState.SetActive(false).
    - _iconImage.sprite = icon.
    - _quantityText.text = quantity > 99 ? "99+" : quantity.ToString().
    - _isEmpty = false.
- SetEmpty():
    - _emptyState.SetActive(true), _filledState.SetActive(false).
    - _isEmpty = true.
- Reset(): _button = GetComponentInChildren<Button>(), _iconImage = GetComponentInChildren<Image>() etc. (S7).
- _logClass required (S8).
- OnDestroy(): _button?.onClick.RemoveAllListeners() (S8 Safe Cleanup).

---

### Task 4 — Create ItemInfoPopupView.cs

- namespace: Samsara.Features.CharacterInfoScene.Presentation
- MonoBehaviour. UniTask-based async popup (S5).
- [SerializeField] private GameObject _popupRoot.
- [SerializeField] private Image _itemIcon.
- [SerializeField] private TMP_Text _itemNameText.
- [SerializeField] private TMP_Text _descriptionText.
- [SerializeField] private TMP_Text _effectText.
- [SerializeField] private Button _useButton.
- [SerializeField] private Button _discardButton.
- [SerializeField] private Button _closeButton.
- _result (ItemInfoPopupResult?): Internal result storage.
- Show(Sprite icon, string itemName, string description, string effectText) -> UniTask<ItemInfoPopupResult>:
    - Set popup data -> _popupRoot.SetActive(true).
    - _result = null.
    - Register button onClick: Use -> _result = Use, Discard -> _result = Discard, Close -> _result = Close.
    - await UniTask.WaitUntil(() => _result.HasValue).
    - _popupRoot.SetActive(false).
    - Return _result.Value.
- Hide(): _popupRoot.SetActive(false).
- Reset(): Auto-assign child components (S7).
- _logClass required (S8).
- OnDestroy(): Remove button listeners (S8 Safe Cleanup).

---

### Task 5 — Create DiscardItemPopupView.cs

- namespace: Samsara.Features.CharacterInfoScene.Presentation
- MonoBehaviour. UniTask-based async popup (S5).
- [SerializeField] private GameObject _popupRoot.
- [SerializeField] private Image _itemIcon.
- [SerializeField] private TMP_Text _itemNameText.
- [SerializeField] private TMP_Text _quantityText.
- [SerializeField] private Button _plusButton.
- [SerializeField] private Button _minusButton.
- [SerializeField] private Button _confirmButton.
- [SerializeField] private Button _cancelButton.
- _selectedQuantity (int), _maxQuantity (int), _isConfirmed (bool?): Internal state.
- Show(Sprite icon, string itemName, int maxQuantity) -> UniTask<DiscardPopupResult>:
    - _maxQuantity = maxQuantity, _selectedQuantity = 1.
    - Set data -> _popupRoot.SetActive(true).
    - _quantityText.text = _selectedQuantity.ToString().
    - _isConfirmed = null.
    - + button: _selectedQuantity = Mathf.Min(_selectedQuantity + 1, _maxQuantity) -> update text.
    - - button: _selectedQuantity = Mathf.Max(_selectedQuantity - 1, 1) -> update text.
    - Confirm: _isConfirmed = true.
    - Cancel: _isConfirmed = false.
    - await UniTask.WaitUntil(() => _isConfirmed.HasValue).
    - _popupRoot.SetActive(false).
    - Return new DiscardPopupResult { Confirmed = _isConfirmed.Value, Quantity = _selectedQuantity }.
- Reset(): Auto-assign child components (S7).
- _logClass required (S8).
- OnDestroy(): Remove button listeners (S8 Safe Cleanup).

---

### Task 6 — Modify InventoryView.cs

**Read existing file first.** Understand current stub implementation before modifying.

Changes:
- Replace existing [SerializeField] private GameObject[] _inventorySlots with [SerializeField] private InventorySlotView[] _slotViews (3 items).
- Add Action<int> OnSlotTapped event: Subscribe to each InventorySlotView.OnTapped and emit with slot index.
- Add SetSlots(InventorySlotDisplayData[] displayData) method:
    - For each slot: if displayData.IsEmpty then _slotViews[i].SetEmpty(), else _slotViews[i].SetSlot(displayData.IconSprite, displayData.Quantity).
- Add Initialize() method: Subscribe to each InventorySlotView.OnTapped event (capture index).
- Reset(): _slotViews = GetComponentsInChildren<InventorySlotView>() (S7).
- Keep existing _logClass (S8).
- OnDestroy(): Unsubscribe events (S8 Safe Cleanup).

---

### Task 7 — Modify CharacterInfoView.cs

**Read existing file first.**

Changes:
- Add [SerializeField] private ItemInfoPopupView _itemInfoPopupView.
- Add [SerializeField] private DiscardItemPopupView _discardItemPopupView.
- Expose both popups as public properties (Presenter access).
- Update Reset(): Add auto-assignment for new popups (S7).

---

### Task 8 — Modify CharacterInfoPresenter.cs

**Read existing file first.** Follow existing implementation patterns.

Changes:

**Additional constructor parameters:**
- InventoryUseCase _inventoryUseCase
- ISpriteLoader _spriteLoader

**Add to Initialize() (after existing logic):**
- Call RefreshInventoryUI() for initial inventory rendering.
- Subscribe _view.InfoScrollView.InventoryView.OnSlotTapped += HandleInventorySlotTapped.

**New methods:**

HandleInventorySlotTapped(int slotIndex):
1. Call _inventoryUseCase.GetItemDetail(slotIndex).
2. Load icon via _spriteLoader.LoadSpriteAsync(). (Check existing ISpriteLoader usage pattern in codebase. If ItemDetailData already has Sprite field, use directly.)
3. Compose effect text: $"{itemDetail.TargetStat} +{itemDetail.EffectValue}".
4. var result = await _view.ItemInfoPopupView.Show(icon, name, description, effectText).
5. Based on result:
    - Use -> call HandleUseItem(slotIndex).
    - Discard -> call HandleDiscardItem(slotIndex).
    - Close -> do nothing.

HandleUseItem(int slotIndex):
1. var useResult = await _inventoryUseCase.UseItem(slotIndex).
2. On success:
    - Compose result text: $"{useResult.AffectedStat} +{useResult.EffectValue}!".
    - Show CommonPopupView via _popupManager (follow existing CommonPopupView call pattern).
    - await popup dismiss.
    - RefreshInventoryUI().
    - RefreshStatUI().

HandleDiscardItem(int slotIndex):
1. Call _inventoryUseCase.GetItemDetail(slotIndex) to get icon/name/quantity.
2. var discardResult = await _view.DiscardItemPopupView.Show(icon, name, quantity).
3. If discardResult.Confirmed:
    - Call _inventoryUseCase.DiscardItem(slotIndex, discardResult.Quantity).
    - RefreshInventoryUI().

RefreshInventoryUI():
1. var displayData = _inventoryUseCase.GetSlotDisplayData().
2. Load icon Sprites for non-empty slots (use ISpriteLoader or use Sprite from displayData if already present).
3. _view.InfoScrollView.InventoryView.SetSlots(displayData).

RefreshStatUI():
1. var stats = _useCase.GetCurrentStats().
2. _view.InfoScrollView.StatListView.SetStats(stats.Hp, stats.Strength, stats.Toughness, stats.Agility).

**Add to Dispose():**
- _view.InfoScrollView.InventoryView.OnSlotTapped -= HandleInventorySlotTapped (S8 Safe Cleanup, use ?.).

---

### Task 9 — Modify CharacterInfoSceneBootstrapper.cs

**Read existing file first.**

Changes:
- Get additional dependencies from GameContext:
    - GameContext.InventoryUseCase
    - GameContext.SpriteLoader (ISpriteLoader)
- Pass additional parameters to CharacterInfoPresenter constructor: inventoryUseCase, spriteLoader.
- Follow existing dependency acquisition/passing patterns.

---

### Task 10 — Modify decisions.md

Add v2.0.0 header to .claude/specs/features/character-info-scene/decisions.md. Keep existing content, add v2.0.0 section at bottom for recording decisions made during this implementation.

---

## 5. Validation

| # | Item | Verification |
|---|---|---|
| V-01 | No compile errors in Unity console | Check console |
| V-02 | Inventory slots show icon + quantity when items present | Editor Play (requires test inventory data) |
| V-03 | Empty slot tap does nothing | Editor Play |
| V-04 | Slot with item tap shows item info popup | Editor Play |
| V-05 | "Use" on info popup -> effect applied -> result popup -> slot + stat UI refresh | Editor Play |
| V-06 | "Discard" on info popup -> discard popup -> +/- quantity -> confirm refreshes slot UI | Editor Play |
| V-07 | "Cancel" on discard popup -> no changes | Editor Play |
| V-08 | Quantity "99+" display works correctly | Code review |
| V-09 | Slot becomes empty when item quantity reaches 0 | Editor Play |
| V-10 | Existing features (stats, skill popup, evolution tree nav, back) work normally | Editor Play |

---

## 6. Manual Tasks (Hak performs after Claude Code implementation)

| Order | Task |
|---|---|
| M-01 | Place InventorySlotView x3 objects in CharacterInfo.unity scene (under InventoryView): each slot with _emptyState, _filledState (Image + TMP_Text), Button |
| M-02 | Place ItemInfoPopupView object under Canvas: _popupRoot, _itemIcon, 3 texts, 3 buttons Inspector wiring |
| M-03 | Place DiscardItemPopupView object under Canvas: _popupRoot, _itemIcon, _itemNameText, _quantityText, +/-/confirm/cancel buttons Inspector wiring |
| M-04 | Wire CharacterInfoView Inspector: _itemInfoPopupView, _discardItemPopupView references |
| M-05 | Wire InventoryView Inspector: _slotViews array with 3 InventorySlotView references |
| M-06 | Prepare test inventory data: purchase potions from shop to have items in inventory for testing |

---

## 7. Claude Code Handoff Guide

- Run `claude` from project root
- CLAUDE.md is auto-loaded
- Pass .claude/specs/features/character-info-scene/tasks.md for sequential implementation
- **When modifying existing files, always read current code first before making changes**
- Record any judgment calls in .claude/specs/features/character-info-scene/decisions.md with [DECISION], [BACKLOG], or [SPEC-GAP] tags
- Do NOT create files outside Assets/_Game/ (except decisions.md)