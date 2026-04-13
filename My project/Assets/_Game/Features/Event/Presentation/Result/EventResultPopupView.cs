using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Event.Presentation
{
    /// <summary>
    /// 씬 전용 결과 팝업. IPopupManager를 거치지 않고 씬 내부에서 직접 관리.
    /// (Constitution §5 Scene-specific popup bypass 허용)
    /// </summary>
    public class EventResultPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EventResultPopupView)}]";

        [SerializeField] private GameObject _popupRoot;
        [SerializeField] private TMP_Text   _resultText;
        [SerializeField] private Button     _confirmButton;

        public async UniTask ShowAsync(string resultText)
        {
            _resultText.text = resultText;
            _popupRoot.SetActive(true);

            var tcs = new UniTaskCompletionSource();
            _confirmButton.onClick.AddListener(() => tcs.TrySetResult());
            await tcs.Task;
            _confirmButton.onClick.RemoveAllListeners();

            Hide();
        }

        public void Hide()
        {
            _popupRoot.SetActive(false);
        }

        private void Reset()
        {
            _popupRoot     = gameObject;
            _resultText    = GetComponentInChildren<TMP_Text>();
            _confirmButton = GetComponentInChildren<Button>();
        }
    }
}
