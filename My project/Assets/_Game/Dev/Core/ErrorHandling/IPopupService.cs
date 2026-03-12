using Cysharp.Threading.Tasks;

namespace Core.ErrorHandling
{
    /// <summary>
    /// Popup UI contract for error recovery (UR-06). Domain must not depend on concrete PopupManager.
    /// </summary>
    public interface IPopupService
    {
        /// <summary>
        /// Shows a common popup. Returns true if user chose the first button (e.g. Reconnect).
        /// </summary>
        UniTask<bool> ShowCommonPopup(string title, string desc, string firstText, string secondText);
    }
}
