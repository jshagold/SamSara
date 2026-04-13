# Event MasterData — Patch-003

**Version:** 1.0.0 | **Date:** 2026-04-13
**Feature:** Event MasterData
**Patch ID:** Patch-003
**Type:** spec-change
**Related:** Specify v1.3.0 — Add MerchantId field to EventResult

---

## Background

ShopSystem (Phase 5-B) requires a MerchantId field in EventResult to specify which merchant to activate when a ShopEncounter event result occurs.

## Changes

### Target: EventResult structure in EventSO.cs

File: Assets/_Game/Features/Event/MasterData/EventSO.cs

- Add [SerializeField] private int? _merchantId field to EventResult
- Add public int? MerchantId accessor
- Value is null for non-ShopEncounter results

## Reference

- Event MasterData Specify v1.3.0

## Verification

- Confirm MerchantId field appears in EventSO Inspector when setting ShopEncounter result
- Confirm existing EventSO .asset files load correctly (null default for backward compatibility)

## Claude Code Implementation Guide

- Read CLAUDE.md first before any implementation
- File to modify: Assets/_Game/Features/Event/MasterData/EventSO.cs
- Add _merchantId field to EventResult struct/class
- Ensure backward compatibility with existing .asset files (nullable int defaults to null)
- If you make any judgment calls not covered by this Patch, record them in .claude/specs/features/masterdata/event/decisions.md with appropriate tags: [DECISION], [BACKLOG], or [SPEC-GAP]