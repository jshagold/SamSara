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
                maxSize: 10
            );
        }

        public void ShowDamage(int damage, Vector3 worldPosition)
        {
            var text = _textPool.Get();
            text.text = damage.ToString();
            text.transform.position = worldPosition;

            var seq = DOTween.Sequence();
            seq.Append(text.transform.DOMoveY(worldPosition.y + 1f, 0.8f).SetEase(Ease.OutQuad));
            seq.Join(text.DOFade(0f, 0.8f).SetEase(Ease.InQuad));
            seq.OnComplete(() => _textPool.Release(text));
        }

        private void OnDestroy()
        {
            _textPool?.Dispose();
        }
    }
}
