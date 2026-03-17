using System.Threading;
using Cysharp.Threading.Tasks;

namespace Samsara.Core.Popup
{
    /// <summary>
    /// 팝업 시스템의 공개 인터페이스.
    /// UnityEngine import 금지 — 순수 C# 인터페이스.
    /// </summary>
    public interface IPopupManager
    {
        /// <summary>확인 버튼 하나짜리 팝업. 확인 시 true 반환.</summary>
        UniTask<bool> ShowConfirmAsync(PopupRequest request, CancellationToken ct = default);

        /// <summary>예/아니오 버튼 팝업. 예 = true, 아니오 = false.</summary>
        UniTask<bool> ShowYesNoAsync(PopupRequest request, CancellationToken ct = default);

        /// <summary>현재 열린 팝업을 모두 닫는다.</summary>
        void DismissAll();
    }
}
