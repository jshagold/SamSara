using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MiniGame.Presentation.Play
{
    public class RoundIndicatorView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(RoundIndicatorView)}]";

        [SerializeField] private TMP_Text _roundText;
        [SerializeField] private Image[]  _roundIcons;

        /// <summary>현재 라운드 텍스트를 표시한다. 예: "2/3"</summary>
        public void SetRound(int current, int total)
        {
            _roundText.text = $"{current}/{total}";
        }

        /// <summary>지정 인덱스의 라운드 결과 아이콘을 표시한다.</summary>
        public void SetRoundResult(int index, bool success)
        {
            if (index < 0 || index >= _roundIcons.Length) return;
            _roundIcons[index].color = success ? Color.green : Color.red;
        }

        /// <summary>모든 결과 아이콘을 초기 상태로 되돌린다.</summary>
        public void ResetAll()
        {
            foreach (var icon in _roundIcons)
                icon.color = Color.white;
        }

        private void Reset()
        {
            _roundText  = GetComponentInChildren<TMP_Text>();
            _roundIcons = GetComponentsInChildren<Image>();
        }
    }
}
