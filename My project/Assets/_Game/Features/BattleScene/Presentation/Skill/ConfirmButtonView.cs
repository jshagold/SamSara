using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Skill
{
    /// <summary>
    /// 스킬 → 타겟 선택 후 행동 확정 버튼.
    /// </summary>
    public class ConfirmButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ConfirmButtonView)}]";

        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _buttonText;

        public event Action OnConfirm;

        private void Awake()
        {
            _confirmButton.onClick.AddListener(() => OnConfirm?.Invoke());
            _buttonText.text = "Confirm";
        }

        /// <summary>버튼 인터랙션 가능 여부 설정. 타겟 미선택 시 비활성화에 사용.</summary>
        public void SetInteractable(bool interactable)
        {
            _confirmButton.interactable = interactable;
        }

        private void OnDestroy()
        {
            _confirmButton?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _confirmButton = GetComponentInChildren<Button>();
            _buttonText = GetComponentInChildren<TMP_Text>();
        }
    }
}
