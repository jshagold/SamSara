using System;
using Samsara.Features.Event.MasterData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Event.Presentation
{
    public class ChoiceListView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ChoiceListView)}]";

        [SerializeField] private Transform  _choiceContainer;
        [SerializeField] private GameObject _choiceButtonPrefab;

        public void ShowChoices(EventChoice[] choices, Action<int> onSelect)
        {
            HideChoices();

            for (int i = 0; i < choices.Length; i++)
            {
                int index = i;
                var go     = Instantiate(_choiceButtonPrefab, _choiceContainer);
                var text   = go.GetComponentInChildren<TMP_Text>();
                var button = go.GetComponent<Button>();

                text.text = choices[i].ChoiceText;
                button.onClick.AddListener(() => onSelect(index));
            }
        }

        /// <summary>텍스트 배열로 선택지를 표시한다. EventChoice 없이 원시 문자열만 필요할 때 사용.</summary>
        public void ShowChoices(string[] texts, Action<int> onSelect)
        {
            HideChoices();

            for (int i = 0; i < texts.Length; i++)
            {
                int index = i;
                var go     = Instantiate(_choiceButtonPrefab, _choiceContainer);
                var text   = go.GetComponentInChildren<TMP_Text>();
                var button = go.GetComponent<Button>();

                text.text = texts[i];
                button.onClick.AddListener(() => onSelect(index));
            }
        }

        public void HideChoices()
        {
            for (int i = _choiceContainer.childCount - 1; i >= 0; i--)
                Destroy(_choiceContainer.GetChild(i).gameObject);
        }

        private void Reset()
        {
            _choiceContainer = transform;
        }
    }
}
