using System.Threading;
using Cysharp.Threading.Tasks;
using Samsara.App.Popup;
using Samsara.Core.Popup;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Samsara.App
{
    /// <summary>
    /// IPopupManager 구체 구현.
    /// MonoBehaviour 금지 — GlobalBootstrapper가 new로 생성.
    /// ObjectPool로 CommonPopupView를 재사용하고 Dim 배경으로 하단 UI 터치를 차단한다.
    /// </summary>
    public class PopupManager : IPopupManager
    {
        private readonly string _logClass = $"[{nameof(PopupManager)}]";

        private readonly ObjectPool<CommonPopupView>  _pool;
        private readonly Transform                    _popupCanvasRoot;
        private readonly CommonPopupView              _prefab;
        private readonly GameObject                   _dimBackground;

        private CommonPopupView               _activePopup;
        private UniTaskCompletionSource<bool> _activeUtcs;

        public PopupManager(Transform popupCanvasRoot, CommonPopupView prefab)
        {
            _popupCanvasRoot = popupCanvasRoot;
            _prefab          = prefab;

            // (1) Object Pool 초기화
            _pool = new ObjectPool<CommonPopupView>(
                createFunc: () =>
                {
                    var view = Object.Instantiate(prefab, popupCanvasRoot);
                    view.gameObject.SetActive(false);
                    return view;
                },
                actionOnGet:     view => { view.gameObject.SetActive(true); view.transform.SetAsLastSibling(); },
                actionOnRelease: view => view.gameObject.SetActive(false),
                actionOnDestroy: view => Object.Destroy(view.gameObject),
                defaultCapacity: 1,
                maxSize: 3
            );

            // (2) Dim 배경 생성 — 팝업 하단 UI 터치 차단
            var dimGO = new GameObject("DimBackground");
            dimGO.transform.SetParent(popupCanvasRoot, worldPositionStays: false);

            var rect = dimGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            var image = dimGO.AddComponent<Image>();
            image.color         = new Color(0f, 0f, 0f, 0.5f);
            image.raycastTarget = true;

            dimGO.SetActive(false);
            _dimBackground = dimGO;
        }

        // ──────────────────────────────────────────────
        // IPopupManager
        // ──────────────────────────────────────────────

        public UniTask<bool> ShowConfirmAsync(PopupRequest request, CancellationToken ct = default)
            => ShowPopupInternalAsync(request, showCancelButton: false, ct);

        public UniTask<bool> ShowYesNoAsync(PopupRequest request, CancellationToken ct = default)
            => ShowPopupInternalAsync(request, showCancelButton: true, ct);

        public void DismissAll()
        {
            DismissActive();
            Debug.Log($"{_logClass} DismissAll 완료.");
        }

        // ──────────────────────────────────────────────
        // Internal
        // ──────────────────────────────────────────────

        private async UniTask<bool> ShowPopupInternalAsync(PopupRequest request, bool showCancelButton, CancellationToken ct)
        {
            DismissActive();

            _dimBackground.SetActive(true);

            var view = _pool.Get();
            view.Setup(request, showCancelButton);

            _activePopup = view;
            _activeUtcs  = new UniTaskCompletionSource<bool>();

            view.OnResult += OnPopupResult;

            if (ct != CancellationToken.None)
                ct.Register(() => DismissActive());

            var result = await _activeUtcs.Task;
            return result;
        }

        private void OnPopupResult(bool result)
        {
            Debug.Log($"{_logClass} 팝업 결과: {result}");
            _activeUtcs?.TrySetResult(result);
            DismissActive();
        }

        private void DismissActive()
        {
            if (_activePopup == null) return;

            var popup = _activePopup;
            var utcs  = _activeUtcs;

            _activePopup = null;
            _activeUtcs  = null;

            popup.ResetView();
            _pool.Release(popup);
            _dimBackground.SetActive(false);

            utcs?.TrySetResult(false);
        }
    }
}
