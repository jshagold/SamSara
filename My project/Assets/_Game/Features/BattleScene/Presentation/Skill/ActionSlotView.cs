using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Skill
{
    /// <summary>
    /// 스킬 외 액션 슬롯 (Wait 등). 현재는 Wait 버튼만 포함.
    /// </summary>
    public class ActionSlotView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ActionSlotView)}]";

        [SerializeField] private Button _waitButton;
        [SerializeField] private TMP_Text _waitButtonText;

        public event Action OnWaitSelected;

        private void Awake()
        {
            _waitButton.onClick.AddListener(() => OnWaitSelected?.Invoke());
            _waitButtonText.text = "Wait";
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        private void OnDestroy()
        {
            _waitButton?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _waitButton = GetComponentInChildren<Button>();
            _waitButtonText = GetComponentInChildren<TMP_Text>();
        }
    }
}
