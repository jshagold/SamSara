using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.EvolutionTreeScene.Presentation.TreeArea
{
    public class NodeConnectionView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(NodeConnectionView)}]";

        [SerializeField] private Image _lineImage;

        public void SetConnection(Vector2 startPos, Vector2 endPos)
        {
            var rectTransform = _lineImage.rectTransform;
            var direction = endPos - startPos;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            rectTransform.anchoredPosition = startPos;
            rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
            rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        private void Reset()
        {
            _lineImage = GetComponentInChildren<Image>();
        }
    }
}
