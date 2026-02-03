using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class UIVerticalEdgeFade : BaseMeshEffect
{
    [Header("Settings")]
    [Tooltip("위아래 페이드 영역의 비율 (0.0 ~ 0.5) \n예: 0.2 = 상하단 각 20%씩 페이드")]
    [Range(0f, 0.5f)]
    public float _fadeRatio = 0.1f;

    // UI 메쉬가 다시 그려질 때마다 호출됨
    public override void ModifyMesh(VertexHelper vh)
    {
        if(!IsActive() || _fadeRatio <= 0f) return;

        Rect rect = GetComponent<RectTransform>().rect;
        UIVertex vertex = new UIVertex();

        // 메쉬의 모든 정점(Vertex)을 순회하며 Y 위치에 따라 알파(Alpha)값 조정
        for(int i = 0; i< vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);

            // 현재 정점의 Y 위치를 0.0(바닥) ~ 1.0(천장) 비율로 변환
            float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, vertex.position.y);

            float alphaMult = 1f;

            // 1. 하단 페이드 (Bottom Fade)
            if(normalizedY < _fadeRatio)
            {
                alphaMult = normalizedY / _fadeRatio;
            }
            // 2. 상단 페이드 (Top Fade)
            else if(normalizedY > 1f - _fadeRatio)
            {
                alphaMult = (1f - normalizedY) / _fadeRatio;
            }

            // 기존 색상에 계산된 알파 비율을 곱함
            vertex.color.a = (byte)(vertex.color.a * alphaMult);
            vh.SetUIVertex(vertex, i);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        // 에디터에서 값 수정 시 즉시 화면에 반영되도록 그래픽 갱신
        if (GetComponent<Graphic>() != null) GetComponent<Graphic>().SetVerticesDirty();
    }
#endif
}