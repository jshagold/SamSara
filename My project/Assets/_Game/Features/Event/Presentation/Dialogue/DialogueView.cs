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

        private void Awake()
        {
            _leftPortrait.Hide();
            _rightPortrait.Hide();
        }

        /// <summary>
        /// 대화를 표시한다. portrait는 사전 로드된 Sprite (없으면 null).
        /// </summary>
        public void ShowDialogue(EventDialogue dialogue, Sprite portrait)
        {
            _dialoguePanel.SetActive(true);
            _speakerNameText.text = dialogue.SpeakerName;
            _dialogueText.text    = dialogue.DialogueText;

            bool hasPortrait = portrait != null;

            if (dialogue.SpeakerPosition == SpeakerPosition.Left)
            {
                if (hasPortrait)
                {
                    _leftPortrait.SetPortrait(portrait);
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
                    _rightPortrait.SetPortrait(portrait);
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
            _leftPortrait.Hide();
            _rightPortrait.Hide();
        }

        // _leftPortrait, _rightPortrait: Inspector에서 수동 연결 필요
        private void Reset()
        {
            _speakerNameText = GetComponentInChildren<TMP_Text>();
            _dialogueText    = GetComponentInChildren<TMP_Text>();
        }
    }
}
