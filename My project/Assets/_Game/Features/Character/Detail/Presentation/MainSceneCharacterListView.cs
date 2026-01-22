using System.Collections.Generic;
using UnityEngine;

public class MainSceneCharacterListView : MonoBehaviour
{

    [Header("Character List Settings")]
    [SerializeField] private Transform _charListContainer;
    [SerializeField] private MainSceneCharacterSummaryView _characterSummaryPrefab;

    // 캐릭터 뷰 리스트 (오브젝트 풀링, 재사용 목적)
    private List<MainSceneCharacterSummaryView> _spawnedSummaryViews = new List<MainSceneCharacterSummaryView>();

    private void Reset()
    {
        
    }

    public void UpdateList(List<MainSceneCharacterSummaryInfo> dataList)
    {
        if (dataList == null)
        {
            foreach (var view in _spawnedSummaryViews)
            {
                view.gameObject.SetActive(false);
            }
            return;
        }

        // 개수 맞추기 (오브젝트 풀링 개념: 모자르면 더만들고 남으면 끝)
        while (_spawnedSummaryViews.Count < dataList.Count)
        {
            MainSceneCharacterSummaryView newView = Instantiate(_characterSummaryPrefab, _charListContainer);
            _spawnedSummaryViews.Add(newView);
        }

        // 데이터 바인딩
        for (int i = 0; i < _spawnedSummaryViews.Count; i++)
        {
            if (i < dataList.Count)
            {
                var view = _spawnedSummaryViews[i];
                view.gameObject.SetActive(true);
                view.Render(dataList[i]);   // 개별 View에 데이터 주입
            }
            else
            {
                // 데이터보다 뷰가 많으면 남는 View 숨김
                _spawnedSummaryViews[i].gameObject.SetActive(false);
            }
        }
    }
}