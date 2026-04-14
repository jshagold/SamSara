# ShopSystem — Patch-001

**Version:** 1.0.0 | **Date:** 2026-04-14
**Feature:** ShopSystem
**Patch ID:** Patch-001
**Type:** bugfix
**Related:** UI bugs found during testing

---

## Background

Two UI bugs found during ShopSystem testing:
1. Potion purchase failure (insufficient gold, out of stock) has no UI feedback. Log outputs "purchase failed" but player only sees the purchase popup closing.
2. After successful potion purchase, Maintenance scene top bar gold does not refresh immediately. Requires scene transition to reflect change.

Additionally, purchase-blocked state UI handling is unified:
- Before: stock 0 -> button disabled (pre-block), insufficient gold -> button active (post-block). Inconsistent.
- After: All purchase-blocked states use post-block popup approach (Plan B confirmed).

## Changes

### Change 1: Show reason popup on purchase failure

Target: Assets/_Game/Features/Maintenance/Presentation/MaintenancePresenter.cs

In OnPurchaseRequested, after PurchasePotion result:
- InsufficientGold -> show "Insufficient gold" popup via IPopupManager
- OutOfStock -> show "Sold out" popup via IPopupManager

### Change 2: Remove stock 0 button disable (unify approach)

Target: Assets/_Game/Features/Shop/Presentation/ShopPanelView.cs and/or ShopItemSlotView.cs

- Remove logic that disables purchase button when stock is 0
- Stock 0 items keep button active; tapping shows "Sold out" popup (unified with Change 1)

### Change 3: Refresh Maintenance top bar gold after purchase

Target: Assets/_Game/Features/Maintenance/Presentation/MaintenancePresenter.cs

In OnPurchaseRequested, after successful PurchasePotion:
- Refresh View top bar gold display with latest CharacterRunData gold value
- Also refresh ShopPanelView gold display

## Reference

- ShopSystem Specify v1.0.0
- ShopSystem Plan v1.0.0 RQ-P02

## Verification

- Insufficient gold + tap purchase -> "Insufficient gold" popup shown
- Stock 0 + tap purchase -> "Sold out" popup shown (button NOT disabled)
- After successful purchase, Maintenance top bar gold refreshes immediately
- After successful purchase, ShopPanelView gold display refreshes immediately

## Claude Code Implementation Guide

- Read CLAUDE.md first before any implementation
- Files to modify: MaintenancePresenter.cs, ShopPanelView.cs and/or ShopItemSlotView.cs
- Use IPopupManager for failure popups (same pattern as other popup usages in the project)
- If you make any judgment calls not covered by this Patch, record them in .claude/specs/features/shop-system/decisions.md with appropriate tags: [DECISION], [BACKLOG], or [SPEC-GAP]