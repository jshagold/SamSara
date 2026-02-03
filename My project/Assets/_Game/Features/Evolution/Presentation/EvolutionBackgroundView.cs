using UnityEngine;
using UnityEngine.UI;

public class EvolutionBackgroundView : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _bgImage;

    private void Reset()
    {
        _bgImage = GetComponent<Image>();
    }

    public void SetArea(float yPosition, float height, float width, Color color)
    {
        _bgImage.color = color;

        // Content의 중심 기준 배치를 위해 설정
        // 피벗 (0.5, 1) -> 상단 중앙 기준 등 기획에 따라 조절 가능. 여기선 중앙(0.5, 0.5) 기준
        _rectTransform.pivot = new Vector2(0.5f, 0.5f);
        _rectTransform.sizeDelta = new Vector2(width, height);
        _rectTransform.anchoredPosition = new Vector2(0f, yPosition); // X는 중앙, Y는 해당 레벨의 중심
    }
}