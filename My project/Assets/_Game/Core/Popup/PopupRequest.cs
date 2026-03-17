namespace Samsara.Core.Popup
{
    /// <summary>
    /// 팝업 표시에 필요한 데이터를 담는 불변 요청 객체.
    /// Title이 null이면 제목 영역을 숨긴다.
    /// </summary>
    public sealed class PopupRequest
    {
        public string Title       { get; }
        public string Message     { get; }
        public string ConfirmText { get; }
        public string CancelText  { get; }

        /// <summary>제목 없는 팝업. Title은 null로 설정된다.</summary>
        public PopupRequest(string message, string confirmText = "Confirm", string cancelText = "Cancel")
        {
            Title       = null;
            Message     = message;
            ConfirmText = confirmText;
            CancelText  = cancelText;
        }

        /// <summary>제목 있는 팝업.</summary>
        public PopupRequest(string title, string message, string confirmText = "Confirm", string cancelText = "Cancel")
        {
            Title       = title;
            Message     = message;
            ConfirmText = confirmText;
            CancelText  = cancelText;
        }
    }
}
