using DG.Tweening;
using UnityEngine;

namespace Samsara.Features.MainScene.Presentation.Hud
{
    public class HudView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(HudView)}]";

        [SerializeField] private RectTransform _topRightGroup;
        [SerializeField] private RectTransform _characterStatusGroup;
        [SerializeField] private float _slideDuration = 0.3f;

        private Vector2 _topRightOriginalPos;
        private Vector2 _characterStatusOriginalPos;
        private bool _isInitialized;

        private void Awake()
        {
            _topRightOriginalPos = _topRightGroup.anchoredPosition;
            _characterStatusOriginalPos = _characterStatusGroup.anchoredPosition;
            _isInitialized = true;
        }

        public void ShowHud()
        {
            if (!_isInitialized) return;

            _topRightGroup.DOKill();
            _characterStatusGroup.DOKill();

            _topRightGroup.DOAnchorPos(_topRightOriginalPos, _slideDuration)
                .SetEase(Ease.OutCubic);
            _characterStatusGroup.DOAnchorPos(_characterStatusOriginalPos, _slideDuration)
                .SetEase(Ease.OutCubic);
        }

        public void HideHud()
        {
            if (!_isInitialized) return;

            _topRightGroup.DOKill();
            _characterStatusGroup.DOKill();

            var topRightHidePos = _topRightOriginalPos + new Vector2(0, 300f);
            _topRightGroup.DOAnchorPos(topRightHidePos, _slideDuration)
                .SetEase(Ease.InCubic);

            var statusHidePos = _characterStatusOriginalPos + new Vector2(-300f, 0);
            _characterStatusGroup.DOAnchorPos(statusHidePos, _slideDuration)
                .SetEase(Ease.InCubic);
        }

        private void OnDestroy()
        {
            _topRightGroup?.DOKill();
            _characterStatusGroup?.DOKill();
        }
    }
}
