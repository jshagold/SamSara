# 🛡️ Samsara Code Review

**Review Date:** 2026-02-11  
**Scope:** All files in `Assets/_Game/` directory  
**Total Files Reviewed:** 126 C# files

**Summary:** **PASS** ✅ (After Fixes Applied)

The codebase demonstrates strong adherence to the Samsara Constitution in most areas, particularly in:
- ✅ Pure DI pattern (Constructor Injection)
- ✅ Bootstrapper hierarchy compliance
- ✅ Repository pattern implementation
- ✅ Async/UniTask usage
- ✅ No hot path allocations (no Update/FixedUpdate violations found)
- ✅ **Safe Cleanup** (Fixed - all OnDestroy methods now use null-conditional operators)
- ✅ **Fail Fast** (Fixed - MainBackgroundView now throws instead of defensive null checking)

**Status:** All critical violations have been resolved. ✅

---

## 🔴 Critical Violations (FIXED ✅)

### 1. **Safe Cleanup Violation** (Constitution §8 - Safe Cleanup)
**Rule:** Do NOT throw exceptions inside `OnDestroy()` or `Dispose()` if a dependency is null. Use null-conditional operator (`?.`) for cleanup calls.

**Violations Found:**

#### `BackgroundBootstrapper.cs` (Line 22-24)
```csharp
if (_backgroundPresenter == null)
    throw new InvalidOperationException($"{_logClass} OnDestroy without Initialize - _backgroundPresenter is null.");
_backgroundPresenter.Dispose();
```
**Fix Required:**
```csharp
_backgroundPresenter?.Dispose();
```

#### `LobbyBootstrapper.cs` (Line 29-31)
```csharp
if (_lobbyBtnPresenter == null)
    throw new InvalidOperationException($"{_logClass} OnDestroy without Initialize - _lobbyBtnPresenter is null.");
_lobbyBtnPresenter.Dispose();
```
**Fix Required:**
```csharp
_lobbyBtnPresenter?.Dispose();
```

#### `IntroBootstrapper.cs` (Line 27-29)
```csharp
if (_introPresenter == null)
    throw new InvalidOperationException($"{_logClass} OnDestroy without Initialize - _introPresenter is null.");
_introPresenter.Dispose();
```
**Fix Required:**
```csharp
_introPresenter?.Dispose();
```

#### `CharacterDetailBootstrapper.cs` (Lines 61-74)
Multiple violations:
```csharp
if (_characterDetailPresenter == null)
    throw new InvalidOperationException(...);
_characterDetailPresenter.Dispose();
// ... repeated for _skillListPresenter and _inventoryPresenter
```
**Fix Required:**
```csharp
_characterDetailPresenter?.Dispose();
_skillListPresenter?.Dispose();
_inventoryPresenter?.Dispose();
```

#### `EvolutionStageSceneBootstrapper.cs` (Lines 72-78)
```csharp
if (_evolutionPresenter == null)
    throw new InvalidOperationException(...);
_evolutionPresenter.Dispose();
// ... repeated for _optionPresenter
```
**Fix Required:**
```csharp
_evolutionPresenter?.Dispose();
_optionPresenter?.Dispose();
```

#### `CharacterInfoSceneBootstrapper.cs` (Line 60-62)
```csharp
if (_mainOptionPresenter == null)
    throw new InvalidOperationException(...);
_mainOptionPresenter.Dispose();
```
**Fix Required:**
```csharp
_mainOptionPresenter?.Dispose();
```

#### `HUDBootstrapper.cs` (Lines 45-58)
Multiple violations:
```csharp
if (_hudPresenter == null)
    throw new InvalidOperationException(...);
_hudPresenter.Dispose();
// ... repeated for _mainOptionPresenter and _characterSummaryPresenter
```
**Fix Required:**
```csharp
_hudPresenter?.Dispose();
_mainOptionPresenter?.Dispose();
_characterSummaryPresenter?.Dispose();
```

**Reason:** Cleanup logic must be "Fail Safe", not "Fail Fast". If an object failed to initialize (causing the dependency to be null), throwing another error during cleanup hides the original, critical error.

---

### 2. **Fail Fast Violation** (Constitution §7 - Fail Fast)
**Rule:** DO NOT use `if(component != null)` for `[SerializeField]` UI elements. Missing references must cause a `NullReferenceException` immediately.

**Violation Found:**

#### `MainBackgroundView.cs` (Line 19)
```csharp
if (data != null && data.Sprite != null)
{
    _backgroundImage.sprite = data.Sprite;
}
else
{
    Debug.LogWarning($"{_logClass} Background not found: {type}");
}
```
**Issue:** Defensive null checking hides missing `[SerializeField]` assignment errors.

**Fix Required:**
```csharp
// Remove null check - let it fail fast if _backgroundImage is not assigned
_backgroundImage.sprite = data.Sprite;
```

**Note:** The `_backgrounds?.Find(...)` check is acceptable as it's checking runtime data, not a `[SerializeField]` field.

---

## 🟡 Warnings (Optimization & Style)

### 1. **LINQ Usage in Non-Hot Paths** (Performance Consideration)
**Status:** ⚠️ Acceptable but monitor

Several files use LINQ operations (`Select`, `Find`, `FirstOrDefault`, `ToList`) which can cause GC allocations. However, these are **NOT** in `Update()` or `FixedUpdate()` loops, so they're acceptable per Constitution §8.

**Files with LINQ:**
- `EvolutionNodeMapper.cs` (Line 25-26): `.Select(...).ToList()`
- `GetSkillListUseCase.cs` (Line 40): `.Select(...).ToList()`
- `InventoryRepository.cs` (Lines 55, 64, 89): `.FirstOrDefault(...)`
- Various Master Repositories: `.ToList()` calls

**Recommendation:** Monitor performance. If GC spikes occur, consider pre-allocated collections or manual loops for frequently-called methods.

---

### 2. **Missing Null Checks in Dispose Methods** (Style)
**Status:** ⚠️ Minor

Some Presenters have empty `Dispose()` methods that should unsubscribe from events using null-conditional operators:

**Examples:**
- `MainBackgroundPresenter.cs` (Line 35-38): Empty Dispose
- `IntroPresenter.cs` (Line 104-107): Empty Dispose  
- `OptionPresenter.cs` (Line 76-79): Empty Dispose

**Note:** These are acceptable if there are no event subscriptions to clean up, but ensure consistency.

---

### 3. **String Interpolation Usage** (Performance)
**Status:** ✅ Acceptable

The codebase uses string interpolation (`$"{...}"`) extensively, which is fine. No string concatenation (`+`) violations found in hot paths.

---

## ✅ Refactoring Plan

### Priority 1: Fix Safe Cleanup Violations

**File:** `BackgroundBootstrapper.cs`
```csharp
private void OnDestroy()
{
    _backgroundPresenter?.Dispose();
}
```

**File:** `LobbyBootstrapper.cs`
```csharp
private void OnDestroy()
{
    _lobbyBtnPresenter?.Dispose();
}
```

**File:** `IntroBootstrapper.cs`
```csharp
private void OnDestroy()
{
    _introPresenter?.Dispose();
}
```

**File:** `CharacterDetailBootstrapper.cs`
```csharp
private void OnDestroy()
{
    _characterDetailPresenter?.Dispose();
    _skillListPresenter?.Dispose();
    _inventoryPresenter?.Dispose();
    
    _characterResourceProvider = null;
    _skillResourceProvider = null;
    _itemResourceProvider = null;
}
```

**File:** `EvolutionStageSceneBootstrapper.cs`
```csharp
private void OnDestroy()
{
    _evolutionPresenter?.Dispose();
    _optionPresenter?.Dispose();
}
```

**File:** `CharacterInfoSceneBootstrapper.cs`
```csharp
private void OnDestroy()
{
    _mainOptionPresenter?.Dispose();
}
```

**File:** `HUDBootstrapper.cs`
```csharp
private void OnDestroy()
{
    _hudPresenter?.Dispose();
    _mainOptionPresenter?.Dispose();
    _characterSummaryPresenter?.Dispose();
    
    Debug.Log($"{_logClass} Destroyed, Presenters Disposed");
}
```

### Priority 2: Fix Fail Fast Violation

**File:** `MainBackgroundView.cs`
```csharp
public void SetBackground(BackgroundType type)
{
    var data = _backgrounds?.Find(x => x.Type == type);
    
    // Fail fast if data not found or sprite missing
    if (data == null)
        throw new InvalidOperationException($"{_logClass} Background not found: {type}");
    
    _backgroundImage.sprite = data.Sprite;
}
```

**Note:** The `_backgrounds?.Find(...)` is acceptable as it checks runtime data. The issue is the defensive null check that hides missing sprite assignment.

---

## 📊 Compliance Summary

| Category | Status | Notes |
|----------|--------|-------|
| Pure DI (Constructor Injection) | ✅ Pass | All logic classes use constructor injection |
| Bootstrapper Hierarchy | ✅ Pass | Correct initialization flow |
| Repository Pattern | ✅ Pass | Cached repository pattern correctly implemented |
| Async Safety | ✅ Pass | Proper UniTask usage with cancellation tokens |
| Hot Path Allocations | ✅ Pass | No Update/FixedUpdate violations found |
| Safe Cleanup | ❌ Fail | Multiple OnDestroy violations |
| Fail Fast (UI) | ⚠️ Warning | One violation in MainBackgroundView |
| Singleton Usage | ✅ Pass | Only GlobalBootstrapper.Instance used (allowed) |

---

## 🎯 Next Steps

1. ✅ **COMPLETED:** Fixed all Safe Cleanup violations (Priority 1)
2. ✅ **COMPLETED:** Fixed Fail Fast violation in MainBackgroundView (Priority 2)
3. **Monitor:** Track GC allocations from LINQ usage during profiling
4. **Future:** Consider adding unit tests for cleanup scenarios

---

## ✅ Fixes Applied (2026-02-11)

All critical violations have been resolved:

1. **Safe Cleanup Fixes:** Updated 7 Bootstrapper files to use null-conditional operators (`?.`) in `OnDestroy()` methods:
   - `BackgroundBootstrapper.cs`
   - `LobbyBootstrapper.cs`
   - `IntroBootstrapper.cs`
   - `CharacterDetailBootstrapper.cs`
   - `EvolutionStageSceneBootstrapper.cs`
   - `CharacterInfoSceneBootstrapper.cs`
   - `HUDBootstrapper.cs`

2. **Fail Fast Fix:** Updated `MainBackgroundView.cs` to throw `InvalidOperationException` instead of defensive null checking.

All changes verified with no linter errors.

---

**Review Completed:** 2026-02-11  
**Reviewed By:** Samsara Code Review Protocol
