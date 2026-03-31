using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.StageScene.Presentation.Popup
{
    public class StageOptionData
    {
        public string StageId;
        public string StageName;
    }

    public class StageCompletePopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(StageCompletePopupView)}]";

        [SerializeField] private TMP_Text _clearMessageText;
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private Button _buttonPrefab;
        [SerializeField] private Button _returnToMainButton;

        private readonly List<Button> _optionButtons = new();

        public event Action<string> OnStageSelected;
        public event Action OnReturnToMainClicked;

        private void Reset()
        {
            _clearMessageText    = GetComponentInChildren<TMP_Text>();
            _returnToMainButton  = GetComponentInChildren<Button>();
            // _buttonContainer: Transform — Constitution §7 예외, 자동 할당 제외
            // _buttonPrefab: 프리팹 참조 — GetComponentInChildren 할당 불가, Inspector에서 수동 연결
        }

        private void Awake()
        {
            _returnToMainButton.onClick.AddListener(() => OnReturnToMainClicked?.Invoke());
        }

        public void Show(string clearMessage, List<StageOptionData> options)
        {
            gameObject.SetActive(true);
            _clearMessageText.text = clearMessage;
            ClearOptionButtons();

            foreach (var option in options)
            {
                var btn = Instantiate(_buttonPrefab, _buttonContainer);
                var capturedId = option.StageId;
                btn.GetComponentInChildren<TMP_Text>().text = option.StageName;
                btn.onClick.AddListener(() => OnStageSelected?.Invoke(capturedId));
                _optionButtons.Add(btn);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ClearOptionButtons();
        }

        private void ClearOptionButtons()
        {
            foreach (var btn in _optionButtons)
            {
                if (btn != null) Destroy(btn.gameObject);
            }
            _optionButtons.Clear();
        }

        private void OnDestroy()
        {
            _returnToMainButton?.onClick.RemoveAllListeners();
        }
    }
}
