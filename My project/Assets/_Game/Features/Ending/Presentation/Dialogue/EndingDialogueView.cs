using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Ending.Presentation
{
    public class EndingDialogueView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EndingDialogueView)}]";

        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private Image      _portraitImage;
        [SerializeField] private TMP_Text   _speakerNameText;
        [SerializeField] private TMP_Text   _dialogueText;

        /// <summary>대화 모드 — 초상화·화자명·텍스트 표시.</summary>
        public void ShowDialogue(string speakerName, Sprite portrait, string text)
        {
            _dialoguePanel.SetActive(true);
            _portraitImage.gameObject.SetActive(true);
            _speakerNameText.gameObject.SetActive(true);
            _dialogueText.gameObject.SetActive(true);

            _speakerNameText.text  = speakerName;
            _portraitImage.sprite  = portrait;
            _dialogueText.text     = text;
        }

        /// <summary>나레이션 모드 — 초상화·화자명 숨김, 텍스트만 표시.</summary>
        public void ShowNarration(string text)
        {
            _dialoguePanel.SetActive(true);
            _portraitImage.gameObject.SetActive(false);
            _speakerNameText.gameObject.SetActive(false);
            _dialogueText.gameObject.SetActive(true);

            _dialogueText.text = text;
        }

        public void HideDialogue()
        {
            _dialoguePanel.SetActive(false);
        }

        private void Reset()
        {
            _dialoguePanel = gameObject;
        }

        private void OnDestroy()
        {
            // 참조 해제 (Exception Masking 방지)
            _portraitImage    = null;
            _speakerNameText  = null;
            _dialogueText     = null;
        }
    }
}
