using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MaintenanceScene.Presentation.Main
{
    public class HpBarView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(HpBarView)}]";

        [SerializeField] private Image _hpBarFill;
        [SerializeField] private TMP_Text _hpText;

        public void SetHp(int current, int max)
        {
            _hpBarFill.fillAmount = max > 0 ? (float)current / max : 0f;
            _hpText.text = $"{current}/{max}";
        }

        private void Reset()
        {
            _hpBarFill = GetComponentInChildren<Image>();
            _hpText    = GetComponentInChildren<TMP_Text>();
        }
    }
}
