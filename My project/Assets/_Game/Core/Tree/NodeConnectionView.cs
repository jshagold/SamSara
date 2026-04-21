using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Core.Tree
{
    public class NodeConnectionView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(NodeConnectionView)}]";

        [SerializeField] private Image _lineImage;

        public void SetConnection(Vector2 startPos, Vector2 endPos)
        {
            var direction = endPos - startPos;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // NodeConnectionView 자체를 Content 좌표계에서 startPos에 배치
            var selfRt = GetComponent<RectTransform>();
            selfRt.anchorMin = new Vector2(0.5f, 1f);
            selfRt.anchorMax = new Vector2(0.5f, 1f);
            selfRt.pivot = new Vector2(0f, 0.5f);
            selfRt.anchoredPosition = startPos;
            selfRt.sizeDelta = new Vector2(distance, 6f);
            selfRt.localRotation = Quaternion.Euler(0, 0, angle);

            // _lineImage를 NodeConnectionView에 꽉 채움
            var lineRt = _lineImage.rectTransform;
            lineRt.anchorMin = Vector2.zero;
            lineRt.anchorMax = Vector2.one;
            lineRt.sizeDelta = Vector2.zero;
            lineRt.anchoredPosition = Vector2.zero;
            lineRt.localRotation = Quaternion.identity;

            Debug.Log($"{_logClass} SetConnection start={startPos} end={endPos} dist={distance:F1} angle={angle:F1}");
        }

        private void Reset()
        {
            _lineImage = GetComponentInChildren<Image>();
        }
    }
}
