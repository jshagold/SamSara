using TMPro;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation.InfoScroll
{
    public class CharacterNameView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterNameView)}]";

        [SerializeField] private TMP_Text _nameText;

        public void SetName(string characterName)
        {
            _nameText.text = characterName;
        }

        private void Reset()
        {
            _nameText = GetComponentInChildren<TMP_Text>();
        }
    }
}
