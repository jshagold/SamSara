using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Field
{
    public class CharacterUnitView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private readonly string _logClass = $"[{nameof(CharacterUnitView)}]";

        [SerializeField] private Image _characterSprite;
        [SerializeField] private Image _hpBarFill;
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _highlightEffect;
        [SerializeField] private GameObject _activeHighlight;    // 현재 액터 하이라이트 (타겟 선택과 별도)
        [SerializeField] private Transform _statusIconContainer; // Phase 1: reserved only

        private int _participantId;
        private int _maxHp;
        private Vector3 _originalPosition;
        private CancellationTokenSource _longPressCts;

        private const float LongPressThreshold = 0.5f;

        public int ParticipantId => _participantId;

        /// <summary>롱프레스 발생. int = ParticipantId</summary>
        public event Action<int> OnLongPress;

        public void Setup(int id, string spriteKey, int maxHp)
        {
            _participantId = id;
            _maxHp = maxHp;
            _originalPosition = transform.position;

            LoadSprite(spriteKey);

            SetHp(maxHp, maxHp);
            SetHighlight(false);
            SetActiveHighlight(false);
            SetDim(false);
        }

        private void LoadSprite(string spriteKey)
        {
            if (string.IsNullOrEmpty(spriteKey)) return;

#if UNITY_EDITOR
            var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(spriteKey);
            if (sprite != null)
                _characterSprite.sprite = sprite;
#endif
        }

        public void SetHp(int current, int max)
        {
            _maxHp = max;
            float ratio = max > 0 ? (float)current / max : 0f;
            _hpBarFill.fillAmount = ratio;
            _hpText.text = $"{current}/{max}";

            if (ratio > 0.5f)
                _characterSprite.color = Color.white;
            else if (ratio > 0f)
                _characterSprite.color = new Color(1f, 0.8f, 0.8f);
            else
                _characterSprite.color = new Color(0.5f, 0.5f, 0.5f);
        }

        /// <summary>타겟 선택 하이라이트 (파란색 글로우 등).</summary>
        public void SetHighlight(bool on)
        {
            _highlightEffect.SetActive(on);
        }

        /// <summary>현재 턴 액터 하이라이트. 턴 시작 ~ 행동 종료까지 ON.</summary>
        public void SetActiveHighlight(bool on)
        {
            if (_activeHighlight != null)
                _activeHighlight.SetActive(on);
        }

        public void SetDim(bool dim)
        {
            _canvasGroup.alpha = dim ? 0.4f : 1f;
        }

        public void SetDead()
        {
            SetDim(true);
            SetHighlight(false);
            SetActiveHighlight(false);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        // ──────────────────────────────────────────────
        // Attack / Return Motion (DOTween)
        // ──────────────────────────────────────────────

        /// <summary>타겟 위치 방향으로 거리의 70%만큼 이동. (~0.3s)</summary>
        public async UniTask PlayAttackMotion(Vector3 targetPosition)
        {
            Vector3 dir = (targetPosition - _originalPosition).normalized;
            float dist = Vector3.Distance(targetPosition, _originalPosition) * 0.7f;
            Vector3 attackPos = _originalPosition + dir * dist;

            await transform.DOMove(attackPos, 0.3f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
        }

        /// <summary>원래 위치로 복귀. (~0.2s)</summary>
        public async UniTask PlayReturnMotion()
        {
            await transform.DOMove(_originalPosition, 0.2f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
        }

        // ──────────────────────────────────────────────
        // Long-Press Detection
        // ──────────────────────────────────────────────

        public void OnPointerDown(PointerEventData eventData)
        {
            _longPressCts?.Cancel();
            _longPressCts?.Dispose();
            _longPressCts = new CancellationTokenSource();
            StartLongPressTimer(_longPressCts.Token).Forget();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _longPressCts?.Cancel();
            _longPressCts?.Dispose();
            _longPressCts = null;
        }

        private async UniTaskVoid StartLongPressTimer(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(LongPressThreshold), cancellationToken: token);
            if (!token.IsCancellationRequested)
                OnLongPress?.Invoke(_participantId);
        }

        private void OnDestroy()
        {
            transform?.DOKill();
            _longPressCts?.Cancel();
            _longPressCts?.Dispose();
        }

        private void Reset()
        {
            _characterSprite = GetComponentInChildren<Image>();
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _hpText = GetComponentInChildren<TMP_Text>();
        }
    }
}
