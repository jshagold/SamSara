using TMPro;
using UnityEngine;

namespace Samsara.Features.ReplayScene.Presentation
{
    public class ReplayGuidanceView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplayGuidanceView)}]";

        [SerializeField] private TMP_Text _guidanceText;

        public void SetGuidanceText(string text)
        {
            _guidanceText.text = text;
        }

        private void Reset()
        {
            _guidanceText = GetComponentInChildren<TMP_Text>();
        }
    }
}
