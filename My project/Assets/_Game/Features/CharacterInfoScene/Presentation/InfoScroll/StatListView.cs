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
            _strengthText.text  = $"힘: {strength}";
            _toughnessText.text = $"강인함: {toughness}";
            _agilityText.text   = $"민첩: {agility}";
        }

        // Reset() 미구현 — TMP_Text 4개의 GetComponentsInChildren 순서가 계층 구성에 의존하여
        // 잘못된 스탯 표시 버그가 발생할 수 있음. Inspector에서 수동 배정 필요. (decisions.md D-02 참조)
    }
}
