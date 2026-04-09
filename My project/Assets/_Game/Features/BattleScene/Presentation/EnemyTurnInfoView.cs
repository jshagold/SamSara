using TMPro;
using UnityEngine;

namespace Samsara.Features.BattleScene.Presentation
{
    /// <summary>
    /// 적 턴 중 하단 스킬 영역에 표시되는 정보 패널.
    /// SkillSelection/ActionSlot/Confirm 영역과 같은 위치를 공유하며,
    /// BattleView.ShowEnemyTurnPanel() 호출 시 스킬 UI 대신 표시된다.
    /// </summary>
    public class EnemyTurnInfoView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EnemyTurnInfoView)}]";

        [SerializeField] private TMP_Text _infoText;

        public void Show(string text)
        {
            _infoText.text = text;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Reset()
        {
            _infoText = GetComponentInChildren<TMP_Text>();
        }
    }
}
