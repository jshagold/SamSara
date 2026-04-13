using Samsara.Features.Event.MasterData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Event.Presentation
{
    public class DialogueView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(DialogueView)}]";

        [SerializeField] private PortraitView _leftPortrait;
        [SerializeField] private PortraitView _rightPortrait;
        [SerializeField] private TMP_Text     _speakerNameText;
        [SerializeField] private TMP_Text     _dialogueText;
        [SerializeField] private GameObject   _dialoguePanel;

        public void ShowDialogue(EventDialogue dialogue)
        {
            _dialoguePanel.SetActive(true);
            _speakerNameText.text = dialogue.SpeakerName;
            _dialogueText.text    = dialogue.DialogueText;

            bool hasPortrait = !string.IsNullOrEmpty(dialogue.PortraitSpriteKey);

            if (dialogue.SpeakerPosition == SpeakerPosition.Left)
            {
                if (hasPortrait)
                {
                    var sprite = UnityEngine.Resources.Load<Sprite>(dialogue.PortraitSpriteKey);
                    _leftPortrait.SetPortrait(sprite);
                    _leftPortrait.Show();
                    _leftPortrait.SetHighlight(true);
                }
                else
                {
                    _leftPortrait.Hide();
                }
                _rightPortrait.SetHighlight(false);
            }
            else  // Right
            {
                if (hasPortrait)
                {
                    var sprite = UnityEngine.Resources.Load<Sprite>(dialogue.PortraitSpriteKey);
                    _rightPortrait.SetPortrait(sprite);
                    _rightPortrait.Show();
                    _rightPortrait.SetHighlight(true);
                }
                else
                {
                    _rightPortrait.Hide();
                }
                _leftPortrait.SetHighlight(false);
            }
        }

        public void HideDialogue()
        {
            _dialoguePanel.SetActive(false);
        }

        // _leftPortrait, _rightPortrait: Inspector에서 수동 연결 필요
        private void Reset()
        {
            _speakerNameText = GetComponentInChildren<TMP_Text>();
            _dialogueText    = GetComponentInChildren<TMP_Text>();
        }
    }
}
