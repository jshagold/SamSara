using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Field
{
    public class CharacterUnitView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterUnitView)}]";

        [SerializeField] private Image _characterSprite;
        [SerializeField] private Image _hpBarFill;
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _highlightEffect;
        [SerializeField] private Transform _statusIconContainer; // Phase 1: reserved only

        private int _participantId;
        private int _maxHp;

        public int ParticipantId => _participantId;

        public void Setup(int id, string spriteKey, int maxHp)
        {
            _participantId = id;
            _maxHp = maxHp;

            // TODO: [BACKLOG] Phase 2에서 Addressables 로드로 교체
            LoadSprite(spriteKey);

            SetHp(maxHp, maxHp);
            SetHighlight(false);
            SetDim(false);
        }

        private void LoadSprite(string spriteKey)
        {
            if (string.IsNullOrEmpty(spriteKey)) return;

#if UNITY_EDITOR
            var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(spriteKey);
            if (sprite != null)
                _characterSprite.sprite = sprite;
#endif
        }

        public void SetHp(int current, int max)
        {
            _maxHp = max;
            float ratio = max > 0 ? (float)current / max : 0f;
            _hpBarFill.fillAmount = ratio;
            _hpText.text = $"{current}/{max}";

            // 3-stage sprite switch based on HP ratio
            if (ratio > 0.5f)
                _characterSprite.color = Color.white;
            else if (ratio > 0f)
                _characterSprite.color = new Color(1f, 0.8f, 0.8f);
            else
                _characterSprite.color = new Color(0.5f, 0.5f, 0.5f);
        }

        public void SetHighlight(bool on)
        {
            _highlightEffect.SetActive(on);
        }

        public void SetDim(bool dim)
        {
            _canvasGroup.alpha = dim ? 0.4f : 1f;
        }

        public void SetDead()
        {
            SetDim(true);
            SetHighlight(false);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void Reset()
        {
            _characterSprite = GetComponentInChildren<Image>();
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _hpText = GetComponentInChildren<TMP_Text>();
        }
    }
}
