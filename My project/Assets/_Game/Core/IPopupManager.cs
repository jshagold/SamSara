using Cysharp.Threading.Tasks;

public interface IPopupManager
{
    UniTask<bool> ShowCommonPopupAsync(string title, string desc, string confirmText, string cancelText);
}
