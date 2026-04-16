using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Ending.Presentation
{
    public class SkipButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(SkipButtonView)}]";

        [SerializeField] private Button _skipButton;

        private void Start()
        {
            // 1st dev: 스킵 버튼 비활성화
            _skipButton.interactable = false;
        }

        private void Reset()
        {
            _skipButton = GetComponentInChildren<Button>();
        }

        private void OnDestroy()
        {
            _skipButton?.onClick.RemoveAllListeners();
        }
    }
}
