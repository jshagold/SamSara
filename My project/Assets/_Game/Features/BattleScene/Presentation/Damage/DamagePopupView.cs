using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Samsara.Features.BattleScene.Presentation.Damage
{
    public class DamagePopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(DamagePopupView)}]";

        [SerializeField] private TMP_Text _damageTextPrefab;

        // 히트 데미지 표시 상수
        private const float HitSuccessFontSize = 72f;
        private const float HitFailFontSize    = 40f;
        private static readonly Color HitSuccessColor = new Color(1f, 0.95f, 0.2f, 1f);  // 밝은 황색
        private static readonly Color HitFailColor    = new Color(0.6f, 0.6f, 0.6f, 1f); // 회색

        private ObjectPool<TMP_Text> _textPool;

        private void Awake()
        {
            _textPool = new ObjectPool<TMP_Text>(
                createFunc: () => Instantiate(_damageTextPrefab, transform),
                actionOnGet: text =>
                {
                    text.gameObject.SetActive(true);
                    text.alpha = 1f;
                },
                actionOnRelease: text => text.gameObject.SetActive(false),
                actionOnDestroy: text => Destroy(text.gameObject),
                defaultCapacity: 4,
                maxSize: 12
            );
        }

        /// <summary>기존 메서드. 단순 데미지 팝업 (플로팅 텍스트).</summary>
        public void ShowDamage(int damage, Vector3 worldPosition)
        {
            var text = _textPool.Get();
            text.text = damage.ToString();
            text.fontSize = HitSuccessFontSize;
            text.color = HitSuccessColor;
            text.transform.position = worldPosition;

            var seq = DOTween.Sequence();
            seq.Append(text.transform.DOMoveY(worldPosition.y + 1f, 0.8f).SetEase(Ease.OutQuad));
            seq.Join(text.DOFade(0f, 0.8f).SetEase(Ease.InQuad));
            seq.OnComplete(() => _textPool.Release(text));
        }

        /// <summary>
        /// QTE 히트별 데미지 표시. success=큰 황색 폰트 + 펀치 스케일. failure="Miss" + 작은 회색 폰트.
        /// </summary>
        public void ShowHitDamage(int damage, bool success, Vector3 worldPosition)
        {
            var text = _textPool.Get();
            text.transform.position = worldPosition;
            text.transform.localScale = Vector3.one;

            if (success)
            {
                text.text = damage.ToString();
                text.fontSize = HitSuccessFontSize;
                text.color = HitSuccessColor;
                text.alpha = 1f;

                var seq = DOTween.Sequence();
                seq.Append(text.transform.DOPunchScale(Vector3.one * 0.4f, 0.2f, 5, 0.5f));
                seq.Append(text.transform.DOMoveY(worldPosition.y + 1.2f, 0.7f).SetEase(Ease.OutQuad));
                seq.Join(text.DOFade(0f, 0.7f).SetDelay(0.2f).SetEase(Ease.InQuad));
                seq.OnComplete(() => _textPool.Release(text));
            }
            else
            {
                text.text = "Miss";
                text.fontSize = HitFailFontSize;
                text.color = HitFailColor;
                text.alpha = 1f;

                var seq = DOTween.Sequence();
                seq.Append(text.transform.DOMoveY(worldPosition.y + 0.5f, 0.6f).SetEase(Ease.OutQuad));
                seq.Join(text.DOFade(0f, 0.6f).SetEase(Ease.InQuad));
                seq.OnComplete(() => _textPool.Release(text));
            }
        }

        private void OnDestroy()
        {
            _textPool?.Dispose();
        }
    }
}
