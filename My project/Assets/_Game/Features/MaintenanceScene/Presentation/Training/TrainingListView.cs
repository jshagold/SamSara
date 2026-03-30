using System;
using Samsara.Features.Character.MasterData;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MaintenanceScene.Presentation.Training
{
    public struct TrainingItemData
    {
        public StatType StatType;
        public string MiniGameName;
        public int CurrentValue;
    }

    public class TrainingListView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(TrainingListView)}]";

        [SerializeField] private Button _hpStatButton;
        [SerializeField] private Button _strengthStatButton;
        [SerializeField] private Button _toughnessStatButton;
        [SerializeField] private Button _agilityStatButton;
        [SerializeField] private Button _closeButton;

        public event Action<StatType> OnStatSelected;
        public event Action OnCloseClicked;

        private void Awake()
        {
            _hpStatButton.onClick.AddListener(()        => OnStatSelected?.Invoke(StatType.Hp));
            _strengthStatButton.onClick.AddListener(()  => OnStatSelected?.Invoke(StatType.Strength));
            _toughnessStatButton.onClick.AddListener(() => OnStatSelected?.Invoke(StatType.Toughness));
            _agilityStatButton.onClick.AddListener(()   => OnStatSelected?.Invoke(StatType.Agility));
            _closeButton.onClick.AddListener(()      => OnCloseClicked?.Invoke());
        }

        public void Show(TrainingItemData[] items)
        {
            // TODO: Populate per-item labels (name, currentValue) using items array.
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _hpStatButton?.onClick.RemoveAllListeners();
            _strengthStatButton?.onClick.RemoveAllListeners();
            _toughnessStatButton?.onClick.RemoveAllListeners();
            _agilityStatButton?.onClick.RemoveAllListeners();
            _closeButton?.onClick.RemoveAllListeners();
        }
    }
}
