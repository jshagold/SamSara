using System;
using Samsara.Core.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.App.Popup
{
    /// <summary>
    /// 범용 팝업 View. IPopupView 구현체.
    /// Reset() 메서드로 Inspector 자동 할당 지원 (Constitution §7).
    /// </summary>
    public class CommonPopupView : MonoBehaviour, IPopupView
    {
        private readonly string _logClass = $"[{nameof(CommonPopupView)}]";

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button   _confirmButton;
        [SerializeField] private Button   _cancelButton;
        [SerializeField] private TMP_Text _confirmButtonText;
        [SerializeField] private TMP_Text _cancelButtonText;

        public event Action<bool> OnResult;
        public GameObject GameObject => gameObject;

        public void Setup(PopupRequest request, bool showCancelButton)
        {
            if (request.Title == null)
            {
                _titleText.gameObject.SetActive(false);
            }
            else
            {
                _titleText.gameObject.SetActive(true);
                _titleText.text = request.Title;
            }

            _messageText.text       = request.Message;
            _confirmButtonText.text = request.ConfirmText;
            _cancelButtonText.text  = request.CancelText;

            _cancelButton.gameObject.SetActive(showCancelButton);

            _confirmButton.onClick.AddListener(() => OnResult?.Invoke(true));
            _cancelButton.onClick.AddListener(() => OnResult?.Invoke(false));
        }

        public void ResetView()
        {
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            OnResult = null;
        }

        /// <summary>
        /// Unity Editor 자동 할당 (Reset 버튼 또는 컴포넌트 최초 추가 시 호출).
        /// 프리팹 계층 구조 순서 기준으로 할당: TMP_Text[0~3], Button[0~1].
        /// </summary>
        private void Reset()
        {
            var texts   = GetComponentsInChildren<TMP_Text>(true);
            var buttons = GetComponentsInChildren<Button>(true);

            _titleText         = texts[0];
            _messageText       = texts[1];
            _confirmButtonText = texts[2];
            _cancelButtonText  = texts[3];

            _confirmButton = buttons[0];
            _cancelButton  = buttons[1];
        }
    }
}
