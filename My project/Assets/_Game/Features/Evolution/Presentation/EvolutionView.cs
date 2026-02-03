using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionView : MonoBehaviour
{
    [Header("Top Area")]
    [SerializeField] private Button _backButton;

    [Header("Scroll Area")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _contentRoot;

    [Header("Layer Containers (BG -> Line -> Node 순서)")]
    [SerializeField] private RectTransform _bgContainer;
    [SerializeField] private RectTransform _lineContainer;
    [SerializeField] private RectTransform _nodeContainer;

    [Header("Prefabs")]
    [SerializeField] private EvolutionNodeSlotView _nodePrefab;
    [SerializeField] private EvolutionLineView _linePrefab;
    [SerializeField] private EvolutionBackgroundView _bgPrefab;

    [Header("Settings")]
    [SerializeField] private List<Color> _levelColorList;
    [SerializeField] private Color _lineColor = Color.black;
    [SerializeField] private float _lineThickness = 8f;
    [SerializeField] private float _nodePadding = 200f;
    [SerializeField] private Vector2 _mapPadding = new Vector2(300f, 300f);

    private List<EvolutionNodeSlotView> _spawnedNodeList = new List<EvolutionNodeSlotView>();
    private List<EvolutionLineView> _spawnedLineList = new List<EvolutionLineView>();
    private List<EvolutionBackgroundView> _spawnedBgList = new List<EvolutionBackgroundView>();

    public RectTransform ContentRoot => _contentRoot;
    public Color LineColor => _lineColor;
    public float LineThickness => _lineThickness;
    public float NodePadding => _nodePadding;
    public Vector2 MapPadding => _mapPadding;

    private void Reset()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            string objName = button.gameObject.name.ToLower();
            if (_backButton == null && objName.Contains("back")) _backButton = button;
        }

        if (_scrollRect == null) _scrollRect = GetComponentInChildren<ScrollRect>(true);
    }

    private void Awake()
    {
        // 2D 스크롤(상하좌우)을 위해 스크롤 설정 강제
        if (_scrollRect != null)
        {
            _scrollRect.horizontal = true;
            _scrollRect.vertical = true;
            _scrollRect.movementType = ScrollRect.MovementType.Elastic; // 혹은 Clamped
        }
    }

    public void OnClickBackButton(Action action)
    {
        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() => action?.Invoke());
    }

    public void ClearAll()
    {
        foreach(var node in _spawnedNodeList)
        {
            if(node != null) Destroy(node.gameObject);
        }

        foreach(var bg in _spawnedBgList)
        { 
            if(bg != null) Destroy(bg.gameObject); 
        }


        _spawnedNodeList.Clear();
        _spawnedBgList.Clear();
    }

    public EvolutionNodeSlotView CreateNode()
    {
        EvolutionNodeSlotView slot = Instantiate(_nodePrefab, _nodeContainer);

        // UI 생성 시 Scale/Position 꼬임 방지
        slot.transform.localScale = Vector3.one;
        // 위치는 Presenter에서 SetPosition으로 잡을 것이므로 여기선 초기화만
        slot.transform.localPosition = Vector3.zero;

        _spawnedNodeList.Add(slot);
        return slot;
    }

    public EvolutionLineView CreateLine()
    {
        EvolutionLineView lineView = Instantiate(_linePrefab, _lineContainer);
        lineView.transform.localScale = Vector3.one;
        lineView.transform.localPosition = Vector3.zero;
        _spawnedLineList.Add(lineView);
        return lineView;
    }

    public EvolutionBackgroundView CreateBackground(int level, float yCenter, float height, float contentWidth)
    {
        EvolutionBackgroundView bgView = Instantiate(_bgPrefab, _bgContainer);
        bgView.transform.localScale = Vector3.one;
        bgView.transform.localPosition = Vector3.zero;

        // 레벨에 맞는 색 가져오기
        Color bgColor = Color.gray;
        if(_levelColorList != null && level >= 0 && level < _levelColorList.Count)
        {
            bgColor = _levelColorList[level];
        }

        bgView.SetArea(yPosition: yCenter, height: height, width: contentWidth, color: bgColor);
        _spawnedBgList.Add(bgView);

        return bgView;
    }

    /// <summary>
    /// 노드 배치가 끝난 후, 스크롤 가능한 영역(Content Size)을 설정
    /// </summary>
    public void SetContentSize(Vector2 size)
    {
        _nodeContainer.sizeDelta = size;
    }

    /// <summary>
    /// 스크롤 위치를 초기화
    /// </summary>
    public void ResetScrollPosition()
    {
        _scrollRect.normalizedPosition = new Vector2(0.5f, 0.5f);   // 중앙 시작
    }
}