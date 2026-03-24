using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MainScene.Presentation.Hud
{
    public class CharacterStatusView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterStatusView)}]";

        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private Image _portraitImage;
        [SerializeField] private List<Image> _actionPointCircles;

        private const int MaxActionPointCircles = 5;

        public void SetHp(int current, int max)
        {
            _hpText.text = $"{current}/{max}";
        }

        public void SetActionPoints(int current, int max)
        {
            for (int i = 0; i < _actionPointCircles.Count; i++)
            {
                if (i < max)
                {
                    _actionPointCircles[i].gameObject.SetActive(true);
                    _actionPointCircles[i].color = i < current ? Color.white : Color.gray;
                }
                else
                {
                    _actionPointCircles[i].gameObject.SetActive(false);
                }
            }
        }

        public void SetPortrait(Sprite sprite)
        {
            _portraitImage.sprite = sprite;
        }

        private void Reset()
        {
            _hpText = GetComponentInChildren<TMP_Text>();
            _portraitImage = GetComponentInChildren<Image>();
        }
    }
}
