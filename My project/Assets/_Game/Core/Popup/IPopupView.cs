using System;
using UnityEngine;

namespace Samsara.Core.Popup
{
    /// <summary>
    /// 팝업 UI 컴포넌트의 인터페이스.
    /// Core 레이어이지만 풀링을 위해 UnityEngine.GameObject를 노출한다 (decision.md 참조).
    /// </summary>
    public interface IPopupView
    {
        /// <summary>팝업 내용 설정 및 버튼 활성화 여부 지정.</summary>
        void Setup(PopupRequest request, bool showCancelButton);

        /// <summary>버튼 결과 이벤트. true = 확인, false = 취소.</summary>
        event Action<bool> OnResult;

        /// <summary>풀링/SetActive 처리를 위한 GameObject 참조.</summary>
        GameObject GameObject { get; }

        /// <summary>풀 반환 시 호출. 리스너 및 이벤트 구독 해제.</summary>
        void ResetView();
    }
}
