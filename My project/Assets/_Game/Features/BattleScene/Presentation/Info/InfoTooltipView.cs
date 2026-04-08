using TMPro;
using UnityEngine;

namespace Samsara.Features.BattleScene.Presentation.Info
{
    /// <summary>
    /// 롱프레스 시 표시되는 정보 툴팁. 적/아군/스킬 공용.
    /// 화면 경계를 벗어나지 않도록 클램프 처리.
    /// </summary>
    public class InfoTooltipView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(InfoTooltipView)}]";

        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _detailText;
        [SerializeField] private RectTransform _tooltipRect;

        // 화면 가장자리 여백 (px)
        private const float EdgePadding = 16f;

        private void Awake()
        {
            Hide();
        }

        /// <summary>
        /// 툴팁을 표시한다. screenPosition 근처에 배치하되 화면 밖으로 나가지 않도록 클램프.
        /// </summary>
        public void Show(string title, string detail, Vector2 screenPosition)
        {
            _titleText.text = title;
            _detailText.text = detail;
            _root.SetActive(true);

            // Canvas 갱신 후 크기를 읽기 위해 LayoutRebuilder 강제 갱신
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipRect);

            var tooltipSize = _tooltipRect.rect.size;
            var clampedPos = ClampToScreen(screenPosition, tooltipSize);
            _tooltipRect.position = clampedPos;
        }

        public void Hide()
        {
            _root.SetActive(false);
        }

        private static Vector2 ClampToScreen(Vector2 screenPos, Vector2 tooltipSize)
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            // 터치 위치 약간 위에 표시
            screenPos.y += tooltipSize.y * 0.5f + EdgePadding;

            float clampedX = Mathf.Clamp(screenPos.x, EdgePadding + tooltipSize.x * 0.5f,
                screenW - EdgePadding - tooltipSize.x * 0.5f);
            float clampedY = Mathf.Clamp(screenPos.y, EdgePadding + tooltipSize.y * 0.5f,
                screenH - EdgePadding - tooltipSize.y * 0.5f);

            return new Vector2(clampedX, clampedY);
        }

        private void Reset()
        {
            _root = gameObject;
            _tooltipRect = GetComponent<RectTransform>();

            var texts = GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 1) _titleText = texts[0];
            if (texts.Length >= 2) _detailText = texts[1];
        }
    }
}
