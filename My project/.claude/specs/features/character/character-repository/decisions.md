## D-01 — InitializeNewRun() does not call SaveDataAsync() internally

**Date:** 2026-03-19
**Context:** Spec (Tasks) specified that InitializeNewRun() should call SaveDataAsync().Forget() internally.
**Problem:** Calling SaveDataAsync().Forget() inside InitializeNewRun() causes IOException: Sharing violation
when the caller also invokes SaveDataAsync() immediately after, resulting in concurrent writes to the same file.
**Decision:** Removed SaveDataAsync().Forget() from InitializeNewRun(). Only sets _isDirty = true.
Save responsibility belongs to the caller (UseCase).
