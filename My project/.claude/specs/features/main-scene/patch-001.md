# MainScene — Patch-001

## Patch ID: Patch-001
## Type: spec-change
## Related: Specify v1.2.0

---

## Context

In Specify v1.2.0, the Merchant NPC was moved from MainScene to MaintenanceScene.
The Specify/Plan/Tasks documents have already been updated to v1.2.0 with all merchant-related
items removed. However, the actual code still contains merchant-related files and logic that
need to be removed.

---

## Changes Required

### What was removed in Specify v1.2.0 (find and remove corresponding code)

**1. Merchant NPC dedicated View**
- Was in Specify v1.1.0: "Merchant NPC Button (conditional): appears in empty screen space
  on event trigger. Disappears when merchant leaves. Taps to enter merchant NPC interaction."
- Removed in v1.2.0
- Code action: Delete the merchant-dedicated View file if it exists

**2. Merchant-related data/condition branching**
- Was in Specify v1.1.0: Data table had a "merchant NPC activation status" row,
  used for merchant button show/hide
- Removed in v1.2.0
- Code action: Remove any logic that queries merchant activation status or shows/hides
  a merchant button

**3. Merchant-related scene transitions**
- Was in Specify v1.1.0: Scene transition table included "merchant NPC button tap -> merchant
  interaction" row
- Removed in v1.2.0
- Code action: Remove merchant button tap event subscriptions, merchant-related scene
  transitions or popup calls

**4. Merchant-related responsibility definitions**
- Was in Specify v1.1.0: "Responsibilities" included "conditional merchant NPC button display
  and entry", "Out of Scope" included "merchant NPC trade logic"
- Removed in v1.2.0
- Code action: Remove any merchant-related references remaining in comments or code

### Summary

Search the entire MainScene Feature code and the full project for the keyword "Merchant".
Remove all code corresponding to the items above without omission.

---

## Files to Reference
- Specify v1.2.0 (Notion page)

---

## Verification
- [ ] No MainScene-related references to "Merchant" remain in the entire project
- [ ] No compile errors in Unity console
- [ ] MainScene entry works correctly with existing features (HUD, 3 navigation buttons)

---

## Claude Code Implementation Guide
- Read CLAUDE.md first before any modification
- Fetch the MainScene Specify-MD page from Notion (page ID: 32c52975d2df8103981af18b84c27e94)
  to confirm the current v1.2.0 spec with no merchant items
- Search all files under Assets/_Game/Features/MainScene/ for "Merchant" keyword
- Search the entire project for "MerchantButtonView" to find cross-references
- Delete merchant-dedicated View file(s)
- Remove all merchant-related fields, events, subscriptions, and logic from remaining files
- DO NOT create files outside Assets/_Game/
- If you make any judgment calls not covered by this Patch,
  record them in .claude/specs/features/main-scene/decisions.md
  with [DECISION] tag