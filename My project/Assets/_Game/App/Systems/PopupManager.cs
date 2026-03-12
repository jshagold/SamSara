using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// IPopupManager 구체 구현 (Stub).
/// 팝업 UI 연결은 Popup Feature 스펙에서 구체화. GlobalBootstrapper가 new로 생성하므로 MonoBehaviour 불가.
/// </summary>
public class PopupManager : IPopupManager
{
    private readonly string _logClass = $"[{nameof(PopupManager)}]";

    public async UniTask<bool> ShowCommonPopupAsync(string title, string desc, string confirmText, string cancelText)
    {
        Debug.LogWarning($"{_logClass} ShowCommonPopupAsync (stub) — title: {title}");
        await UniTask.CompletedTask;
        return false;
    }
}
