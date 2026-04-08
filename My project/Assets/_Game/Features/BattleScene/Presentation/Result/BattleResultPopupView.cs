using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Samsara.Features.BattleScene.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Result
{
    public class BattleResultPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BattleResultPopupView)}]";

        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private TMP_Text _statChangesText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private GameObject _root;

        [Header("End Presentation")]
        [SerializeField] private TMP_Text _endAnnouncementText;
        [SerializeField] private CanvasGroup _endAnnouncementGroup;

        public event Action OnConfirm;

        private void Awake()
        {
            _confirmButton.onClick.AddListener(() => OnConfirm?.Invoke());
            Hide();
            if (_endAnnouncementGroup != null)
                _endAnnouncementGroup.alpha = 0f;
            if (_endAnnouncementText != null)
                _endAnnouncementText.gameObject.SetActive(false);
        }

        /// <summary>
        /// 전투 종료 연출 재생 후 결과 팝업 표시.
        /// Victory/Defeat 텍스트 → Scale up + Fade in → Hold 1s → Fade out → 결과 팝업 표시.
        /// </summary>
        public async UniTask ShowEndPresentation(BattleResult result)
        {
            // ── 1. 중앙 대형 텍스트 연출 ──
            if (_endAnnouncementText != null && _endAnnouncementGroup != null)
            {
                _endAnnouncementText.text = result == BattleResult.Victory ? "Victory!" : "Defeat...";
                _endAnnouncementText.transform.localScale = Vector3.one * 0.5f;
                _endAnnouncementGroup.alpha = 0f;
                _endAnnouncementText.gameObject.SetActive(true);

                // Fade in + Scale up
                var seq = DOTween.Sequence();
                seq.Append(_endAnnouncementGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad));
                seq.Join(_endAnnouncementText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
                await seq.AsyncWaitForCompletion();

                // Hold
                await UniTask.Delay(1000);

                // Fade out
                await _endAnnouncementGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad).AsyncWaitForCompletion();
                _endAnnouncementText.gameObject.SetActive(false);
            }

            // ── 2. 결과 팝업 표시 ──
            Show(result);
        }

        public void Show(BattleResult result)
        {
            _resultText.text = result == BattleResult.Victory ? "Victory!" : "Defeat...";
            _statChangesText.text = "";
            _root.SetActive(true);
        }

        public void Hide()
        {
            _root.SetActive(false);
        }

        private void OnDestroy()
        {
            _confirmButton?.onClick.RemoveAllListeners();
            _endAnnouncementGroup?.DOKill();
            _endAnnouncementText?.transform.DOKill();
        }

        private void Reset()
        {
            _resultText = GetComponentInChildren<TMP_Text>();
            _confirmButton = GetComponentInChildren<Button>();
        }
    }
}
