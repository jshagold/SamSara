using Cysharp.Threading.Tasks;
using Samsara.Features.MiniGame.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MiniGame.Presentation.Result
{
    public class MiniGameResultPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MiniGameResultPopupView)}]";

        [SerializeField] private TMP_Text _verdictText;
        [SerializeField] private TMP_Text _statNameText;
        [SerializeField] private TMP_Text _beforeValueText;
        [SerializeField] private TMP_Text _afterValueText;
        [SerializeField] private Button   _returnButton;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>결과 팝업을 표시하고 귀환 버튼 탭까지 대기한다.</summary>
        public async UniTask Show(MiniGameVerdict verdict, string statName, int beforeValue, int afterValue)
        {
            _verdictText.text     = GetVerdictText(verdict);
            _statNameText.text    = statName;
            _beforeValueText.text = beforeValue.ToString();
            _afterValueText.text  = afterValue.ToString();

            gameObject.SetActive(true);

            var tcs = new UniTaskCompletionSource();
            void OnReturn() => tcs.TrySetResult();
            _returnButton.onClick.AddListener(OnReturn);

            await tcs.Task;

            _returnButton.onClick.RemoveListener(OnReturn);
            Hide();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private static string GetVerdictText(MiniGameVerdict verdict)
        {
            return verdict switch
            {
                MiniGameVerdict.Success  => "성공!",
                MiniGameVerdict.Maintain => "유지",
                MiniGameVerdict.Fail     => "실패",
                _                        => verdict.ToString()
            };
        }

        private void Reset()
        {
            _returnButton = GetComponentInChildren<Button>();
        }
    }
}
