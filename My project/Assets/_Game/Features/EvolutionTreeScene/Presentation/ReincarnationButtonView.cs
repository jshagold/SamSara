using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.EvolutionTreeScene.Presentation
{
    public class ReincarnationButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReincarnationButtonView)}]";

        [SerializeField] private Button _button;

        public event Action OnReincarnationClicked;

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleReincarnationClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleReincarnationClicked);
        }

        private void HandleReincarnationClicked()
        {
            OnReincarnationClicked?.Invoke();
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
        }
    }
}
