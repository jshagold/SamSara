# CharacterInfoScene — Patch-002

**Type:** spec-change
**Related:** EvolutionTreeScene Plan v1.0.0 RQ-P06
**Source:** EvolutionTreeScene implementation complete, replacing stub

---

## Background

CharacterInfoScene's evolution stage button tap was implemented as a stub showing a "Coming soon" CommonPopup because EvolutionTreeScene was not yet implemented. Now that EvolutionTreeScene implementation is complete, replace the stub with actual scene transition.

---

## Changes

### CharacterInfoPresenter.cs

- Change EvolutionStageButton tap event handler

**Before:** Show "Coming soon" CommonPopup via IPopupManager
**After:** Call ISceneNavigator.LoadScene(SceneKey.EvolutionTree)

---

## Validation

- Verify evolution stage button tap transitions to EvolutionTreeScene
- Verify "Coming soon" popup no longer appears
- No compile errors

---

## Claude Code Implementation Guide

- Read CLAUDE.md first
- Read Assets/_Game/Features/CharacterInfoScene/Presentation/CharacterInfoPresenter.cs
- Find the EvolutionStageButton tap event handler
- Replace the IPopupManager "Coming soon" popup call with ISceneNavigator.LoadScene(SceneKey.EvolutionTree)
- DO NOT create files outside Assets/_Game/
- Record any judgment calls in .claude/specs/features/character-info-scene/decisions.md