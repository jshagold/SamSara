using System;
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

        public event Action OnConfirm;

        private void Awake()
        {
            _confirmButton.onClick.AddListener(() => OnConfirm?.Invoke());
            Hide();
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
        }

        private void Reset()
        {
            _resultText = GetComponentInChildren<TMP_Text>();
            _confirmButton = GetComponentInChildren<Button>();
        }
    }
}
