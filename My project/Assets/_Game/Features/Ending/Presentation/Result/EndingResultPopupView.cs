using Cysharp.Threading.Tasks;
using Samsara.Features.Ending.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Ending.Presentation
{
    public class EndingResultPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EndingResultPopupView)}]";

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _summaryText;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private Button   _restartButton;

        /// <summary>
        /// 결과 팝업을 표시하고 재시작 버튼 클릭을 기다린다.
        /// </summary>
        public async UniTask Show(string title, RunSummaryData summary, string resultText)
        {
            _titleText.text   = title;
            _summaryText.text = $"Day: {summary.TotalDays}\n"
                              + $"진화: {summary.FinalEvolutionName}\n"
                              + $"클리어 스테이지: {summary.StagesCleared}\n"
                              + $"최종 골드: {summary.FinalGold}";
            _resultText.text  = resultText;

            gameObject.SetActive(true);

            var tcs = new UniTaskCompletionSource();
            void OnRestart() => tcs.TrySetResult();
            _restartButton.onClick.AddListener(OnRestart);

            await tcs.Task;

            _restartButton.onClick.RemoveListener(OnRestart);
        }

        private void Reset()
        {
            _restartButton = GetComponentInChildren<Button>();
        }

        private void OnDestroy()
        {
            _restartButton?.onClick.RemoveAllListeners();
        }
    }
}
