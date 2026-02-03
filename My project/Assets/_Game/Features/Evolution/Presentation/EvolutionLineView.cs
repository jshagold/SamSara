using UnityEngine;
using UnityEngine.UI;

public class EvolutionLineView : MonoBehaviour
{
    [Header("Line Segment")]
    [Tooltip("시작에서 중간까지의 세로선")]
    [SerializeField] private Image _startVerticalLine;

    [Tooltip("중간에서 좌/우로 꺾이는 가로선")]
    [SerializeField] private Image _horizontalLine;

    [Tooltip("중간에서 끝까지의 세로선")]
    [SerializeField] private Image _endVerticalLine;

    private void Reset()
    {
        Image[] children = GetComponentsInChildren<Image>(true);
        foreach (var img in children)
        {
            if (img.gameObject == this.gameObject) continue;
            string objName = img.gameObject.name.ToLower();

            if (_startVerticalLine == null && objName.Contains("start")) _startVerticalLine = img;
            if (_horizontalLine == null && objName.Contains("horizontal")) _horizontalLine = img;
            if (_endVerticalLine == null && objName.Contains("end")) _endVerticalLine = img;
        }
    }

    public void DrawLine(Vector2 startPos, Vector2 endPos, float thickness, Color color)
    {
        SetLineColor(color);

        float midY = (startPos.y + endPos.y) / 2f;

        SetSegment(
            img: _startVerticalLine,
            centerX: startPos.x,
            centerY: (startPos.y + midY) / 2f,
            width: thickness,
            height: Mathf.Abs(startPos.y - midY) + thickness);

        SetSegment(
            img: _horizontalLine,
            centerX: (startPos.x + endPos.x) / 2f,
            centerY: midY,
            width: Mathf.Abs(startPos.x - endPos.x) + thickness,
            height: thickness);

        SetSegment(
            img: _startVerticalLine,
            centerX: endPos.x,
            centerY: (midY + endPos.y) / 2f,
            width: thickness,
            height: Mathf.Abs(midY - endPos.y) + thickness);
    }

    private void SetSegment(Image img, float centerX, float centerY, float width, float height)
    {
        img.rectTransform.anchoredPosition = new Vector2(centerX, centerY);
        img.rectTransform.sizeDelta = new Vector2(width, height);
    }

    private void SetLineColor(Color color)
    {
        _startVerticalLine.color = color;
        _horizontalLine.color = color;
        _endVerticalLine.color = color;
    }
}