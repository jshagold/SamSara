using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatListView : MonoBehaviour
{
    [Header("Title Section")]
    [SerializeField] private Image _boxLabelImage;
    [SerializeField] private TextMeshProUGUI _boxLabelText;

    [Header("Stat Section")]
    [SerializeField] private Transform _prefabContainer;
    [SerializeField] private StatRowView _statPrefab;

    private void Reset()
    {
        if (_boxLabelImage == null) _boxLabelImage = GetComponentInChildren<Image>(true);
        if (_boxLabelText == null) _boxLabelText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void SetData(List<StatDisplayInfo> statList, string boxLabel = null)
    {
        _boxLabelText.text = boxLabel;

        foreach(Transform child in _prefabContainer)
        { 
            Destroy(child.gameObject);
        }

        foreach(var statDisplayInfo in statList)
        {
            StatRowView statRow = Instantiate(_statPrefab, _prefabContainer);
            statRow.SetData(statDisplayInfo);
        }
    }
}