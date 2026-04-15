using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.ActionOrder
{
    /// <summary>
    /// 행동 순서 큐의 개별 슬롯. 초상화 + 이름 + 팀 컬러 보더 + 현재 액터 하이라이트.
    /// </summary>
    public class ActionOrderSlotView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ActionOrderSlotView)}]";

        [Header("Visuals")]
        [SerializeField] private Image _portraitImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _borderImage;
        [SerializeField] private GameObject _highlightEffect;

        [Header("Border Colors")]
        [SerializeField] private Color _allyBorderColor = new Color(0.2f, 0.5f, 1f, 1f);
        [SerializeField] private Color _enemyBorderColor = new Color(1f, 0.3f, 0.3f, 1f);

        private Button _button;

        public int ParticipantId { get; private set; }

        /// <summary>슬롯 터치 시 발생. int = ParticipantId</summary>
        public event Action<int> OnSlotTouched;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
                _button.onClick.AddListener(HandleSlotTouched);
        }

        /// <summary>
        /// 슬롯 초기 설정.
        /// </summary>
        /// <param name="id">참여자 ID</param>
        /// <param name="portraitKey">초상화 스프라이트 키 (Addressables)</param>
        /// <param name="name">표시 이름</param>
        /// <param name="isAlly">아군 여부 — 보더 색상 결정</param>
        public void Setup(int id, string portraitKey, string name, bool isAlly)
        {
            ParticipantId = id;
            _nameText.text = name;
            _borderImage.color = isAlly ? _allyBorderColor : _enemyBorderColor;

            SetHighlight(false);
        }

        /// <summary>현재 행동 중인 액터 하이라이트 ON/OFF.</summary>
        public void SetHighlight(bool on)
        {
            _highlightEffect.SetActive(on);
        }

        private void HandleSlotTouched()
        {
            OnSlotTouched?.Invoke(ParticipantId);
        }

        public void SetPortrait(Sprite sprite)
        {
            _portraitImage.sprite = sprite;
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _portraitImage = GetComponentInChildren<Image>();
            _nameText = GetComponentInChildren<TMP_Text>();
            _button = GetComponent<Button>();
        }
    }
}
