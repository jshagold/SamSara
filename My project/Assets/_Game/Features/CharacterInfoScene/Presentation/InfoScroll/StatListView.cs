using TMPro;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation.InfoScroll
{
    public class StatListView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(StatListView)}]";

        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _strengthText;
        [SerializeField] private TMP_Text _toughnessText;
        [SerializeField] private TMP_Text _agilityText;

        public void SetStats(int hp, int strength, int toughness, int agility)
        {
            _hpText.text        = $"HP: {hp}";
            _strengthText.text  = $"Strength: {strength}";
            _toughnessText.text = $"Toughness: {toughness}";
            _agilityText.text   = $"Agility: {agility}";
        }

        // Reset() intentionally omitted: 4 TMP_Text children share the same type,
        // so auto-assignment via GetComponentsInChildren<TMP_Text>() cannot guarantee
        // HP/Strength/Toughness/Agility order. Assign each field manually in Inspector.
        // See decisions.md D-03.
    }
}
