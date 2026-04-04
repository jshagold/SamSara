using TMPro;
using UnityEngine;

namespace Samsara.Features.BattleScene.Presentation.TopBar
{
    public class TurnNumberView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(TurnNumberView)}]";

        [SerializeField] private TMP_Text _turnText;

        public void SetTurn(int turn)
        {
            _turnText.text = $"Turn {turn}";
        }

        private void Reset()
        {
            _turnText = GetComponentInChildren<TMP_Text>();
        }
    }
}
