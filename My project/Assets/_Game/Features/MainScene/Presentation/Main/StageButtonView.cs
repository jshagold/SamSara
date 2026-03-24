using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MainScene.Presentation.Main
{
    public class StageButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(StageButtonView)}]";

        [SerializeField] private Button _button;
        [SerializeField] private RectTransform _characterTransform;
        [SerializeField] private float _animDuration = 0.3f;

        public event Action OnButtonClicked;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _button.interactable = false;

            _characterTransform.DOPunchScale(Vector3.one * 0.1f, _animDuration)
                .OnComplete(() =>
                {
                    _button.interactable = true;
                    OnButtonClicked?.Invoke();
                });
        }

        private void OnDestroy()
        {
            _characterTransform?.DOKill();
            _button?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
        }
    }
}
