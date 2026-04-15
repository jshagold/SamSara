using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.CharacterInfoScene.Presentation.CharacterArea
{
    public class EvolutionStageButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EvolutionStageButtonView)}]";

        [SerializeField] private Button   _button;
        [SerializeField] private Image    _evolutionIcon;
        [SerializeField] private TMP_Text _evolutionNameText;

        public event Action OnEvolutionStageClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnEvolutionStageClicked?.Invoke());
        }

        public void SetEvolutionInfo(Sprite icon, string name)
        {
            _evolutionIcon.sprite  = icon;
            _evolutionNameText.text = name;
        }

        private void Reset()
        {
            _button            = GetComponentInChildren<Button>();
            _evolutionIcon     = GetComponentInChildren<Image>();
            _evolutionNameText = GetComponentInChildren<TMP_Text>();
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
        }
    }
}
