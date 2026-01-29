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
    [SerializeField] private RectTransform _nodeContainer;

    [Header("Node")]
    [SerializeField] private EvolutionNodeSlotView _nodePrefab;

    private List<EvolutionNodeSlotView> _spawnedNodeList = new List<EvolutionNodeSlotView>();

    public RectTransform NodeContainer => _nodeContainer;

    private void Reset()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            string objName = button.gameObject.name.ToLower();
            if (_backButton == null && objName.Contains("back")) _backButton = button;
        }

        // 2. Scroll Area 자동 할당
        if (_scrollRect == null) _scrollRect = GetComponentInChildren<ScrollRect>(true);

        if (_scrollRect != null && _nodeContainer == null)
        {
            _nodeContainer = _scrollRect.content;
        }
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

    public void ClearNodeList()
    {
        foreach(var node in _spawnedNodeList)
        {
            if(node != null) Destroy(node.gameObject);
        }
        _spawnedNodeList.Clear();
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