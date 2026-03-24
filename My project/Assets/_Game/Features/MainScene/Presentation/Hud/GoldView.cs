using TMPro;
using UnityEngine;

namespace Samsara.Features.MainScene.Presentation.Hud
{
    public class GoldView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(GoldView)}]";

        [SerializeField] private TMP_Text _goldText;

        public void SetGold(int gold)
        {
            _goldText.text = gold.ToString();
        }

        private void Reset()
        {
            _goldText = GetComponentInChildren<TMP_Text>();
        }
    }
}
