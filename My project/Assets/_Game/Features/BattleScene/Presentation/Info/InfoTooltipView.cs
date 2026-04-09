using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Info
{
    /// <summary>
    /// 롱프레스 정보 툴팁. 적/아군/스킬 공용.
    ///
    /// 계층 구조 (Inspector에서 구성):
    ///   InfoTooltipView  — RectTransform full-stretch, CanvasGroup, 이 스크립트
    ///   ├── Blocker      — full-stretch 투명 Image + Button (_closeBlocker)
    ///   └── TooltipPanel — 중앙 고정 패널, Background Image 포함
    ///       ├── TitleText  (TMP_Text → _titleText)
    ///       └── DetailText (TMP_Text → _detailText)
    ///
    /// 닫기: Blocker 터치 → Hide(). CanvasGroup.blocksRaycasts가 툴팁 표시 중
    /// 하위 UI 입력을 전부 차단하므로 블로커 외 다른 탭은 불가.
    /// </summary>
    public class InfoTooltipView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(InfoTooltipView)}]";

        // 전체화면 투명 블로커 버튼 — 터치 시 Hide() 호출
        [SerializeField] private Button _closeBlocker;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _detailText;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _closeBlocker.onClick.AddListener(Hide);
            Hide();
        }

        /// <summary>
        /// 툴팁을 표시한다. 전체화면 블로커가 함께 활성화되어
        /// 사용자가 아무 곳이나 탭하면 닫힌다.
        /// screenPosition 파라미터는 향후 위치 기반 배치 확장 시 사용.
        /// </summary>
        public void Show(string title, string detail, Vector2 screenPosition)
        {
            Debug.Log($"{_logClass} Show({title}) — activeInHierarchy={gameObject.activeInHierarchy}");
            _titleText.text = title;
            _detailText.text = detail;

            // CanvasGroup ON → InfoTooltipView 전체(블로커+패널) 표시 및 입력 차단
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        private void OnDestroy()
        {
            _closeBlocker?.onClick.RemoveListener(Hide);
        }

        private void Reset()
        {
            // _closeBlocker: Inspector에서 Blocker GameObject의 Button 컴포넌트 수동 연결
            var texts = GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 1) _titleText = texts[0];
            if (texts.Length >= 2) _detailText = texts[1];
        }
    }
}
