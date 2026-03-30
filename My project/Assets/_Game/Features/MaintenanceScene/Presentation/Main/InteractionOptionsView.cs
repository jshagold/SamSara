using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MaintenanceScene.Presentation.Main
{
    public class InteractionOptionsView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(InteractionOptionsView)}]";

        [SerializeField] private Button _trainingButton;
        [SerializeField] private Button _explorationButton;
        [SerializeField] private Button _shopButton;

        public event Action OnTrainingClicked;
        public event Action OnExplorationClicked;
        public event Action OnShopClicked;

        private void Awake()
        {
            _trainingButton.onClick.AddListener(()    => OnTrainingClicked?.Invoke());
            _explorationButton.onClick.AddListener(() => OnExplorationClicked?.Invoke());
            _shopButton.onClick.AddListener(()        => OnShopClicked?.Invoke());
        }

        public void SetShopButtonVisible(bool visible)
        {
            _shopButton.gameObject.SetActive(visible);
        }

        /// <summary>
        /// Training/Exploration 버튼을 시각적으로 흐리게 처리한다. Shop 버튼은 영향받지 않는다.
        /// button.interactable은 변경하지 않음 — AP 부족 시에도 클릭 이벤트가 발생해야 팝업을 표시할 수 있다.
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            var color = interactable ? Color.white : new Color(1f, 1f, 1f, 0.4f);
            _trainingButton.image.color    = color;
            _explorationButton.image.color = color;
        }

        private void OnDestroy()
        {
            _trainingButton?.onClick.RemoveAllListeners();
            _explorationButton?.onClick.RemoveAllListeners();
            _shopButton?.onClick.RemoveAllListeners();
        }
    }
}
