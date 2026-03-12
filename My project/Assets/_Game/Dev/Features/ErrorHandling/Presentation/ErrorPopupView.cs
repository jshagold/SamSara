using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.ErrorHandling.Presentation
{
    /// <summary>
    /// Full-screen modal content for error recovery (UR-01–UR-05). PopupManager hosts this via IPopupService.
    /// </summary>
    public class ErrorPopupView : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private CanvasGroup _dimmer;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private TextMeshProUGUI _debugText;

        [Header("Buttons")]
        [SerializeField] private Button _reconnectButton;
        [SerializeField] private Button _toTitleButton;
        [SerializeField] private TextMeshProUGUI _reconnectButtonText;
        [SerializeField] private TextMeshProUGUI _toTitleButtonText;

        private void Reset()
        {
            if (_panelRoot == null) _panelRoot = gameObject;
            if (_dimmer == null) _dimmer = GetComponent<CanvasGroup>();
            if (_dimmer == null && _panelRoot != null) _dimmer = _panelRoot.GetComponent<CanvasGroup>();

            var allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
            var allButtons = GetComponentsInChildren<Button>(true);

            foreach (var b in allButtons)
            {
                string n = b.name.ToLower();
                if (_reconnectButton == null && n.Contains("reconnect")) _reconnectButton = b;
                if (_toTitleButton == null && n.Contains("totitle")) _toTitleButton = b;
            }
            foreach (var t in allTexts)
            {
                string n = t.name.ToLower();
                if (_titleText == null && n.Contains("title") && !n.Contains("button")) _titleText = t;
                if (_messageText == null && (n.Contains("message") || n.Contains("desc"))) _messageText = t;
                if (_debugText == null && n.Contains("debug")) _debugText = t;
                if (_reconnectButtonText == null && n.Contains("reconnect")) _reconnectButtonText = t;
                if (_toTitleButtonText == null && n.Contains("totitle")) _toTitleButtonText = t;
            }
        }

        public void SetButtonsInteractable(bool interactable)
        {
            if (_reconnectButton != null) _reconnectButton.interactable = interactable;
            if (_toTitleButton != null) _toTitleButton.interactable = interactable;
        }

        /// <summary>
        /// Shows the popup. firstIsReconnect: true = Reconnect, false = To Title. Returns true if first button (Reconnect) was chosen.
        /// </summary>
        public async UniTask<bool> ShowAsync(
            string title,
            string message,
            string debugInfo,
            bool showReconnect)
        {
            var token = this.GetCancellationTokenOnDestroy();

            if (_titleText != null) _titleText.text = title ?? "";
            if (_messageText != null) _messageText.text = message ?? "";
            if (_debugText != null) _debugText.text = debugInfo ?? "";
            if (_reconnectButton != null) _reconnectButton.gameObject.SetActive(showReconnect);
            if (_toTitleButtonText != null) _toTitleButtonText.text = "To Title";
            if (_reconnectButtonText != null) _reconnectButtonText.text = "Reconnect";

            if (_panelRoot != null) _panelRoot.SetActive(true);
            if (_dimmer != null) _dimmer.interactable = true;

            UniTask reconnectTask = _reconnectButton != null && showReconnect
                ? _reconnectButton.OnClickAsync(token)
                : UniTask.Never(token);
            UniTask toTitleTask = _toTitleButton != null
                ? _toTitleButton.OnClickAsync(token)
                : UniTask.Never(token);

            int which = await UniTask.WhenAny(reconnectTask, toTitleTask);
            if (_dimmer != null) _dimmer.interactable = false;
            if (_panelRoot != null) _panelRoot.SetActive(false);

            return which == 0 && showReconnect;
        }
    }
}
