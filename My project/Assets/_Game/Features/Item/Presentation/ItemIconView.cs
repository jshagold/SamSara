using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemIconView : MonoBehaviour
{
    [SerializeField] private Image _iconFrame;
    [SerializeField] private Image _iconImg;

    [SerializeField] private TextMeshProUGUI _countText;

    private void Reset()
    {
        Image[] allImages = GetComponentsInChildren<Image>(true);

        foreach (Image image in allImages)
        {
            string objName = image.gameObject.name.ToLower();

            if (_iconFrame == null && objName.Contains("frame")) _iconFrame = image;
            if (_iconImg == null && objName.Contains("img")) _iconImg = image;
        }

        if (_countText == null) _countText = GetComponent<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {

    }

    public void SetData(ItemInfo info, ItemMasterData masterData)
    {
        if(masterData != null)
        {
            _iconImg.sprite = masterData.Icon;
            _iconImg.enabled = true;
        }
        else
        {
            _iconImg.sprite = null;
            _iconImg.enabled = false;
        }

        if(info != null)
        {
            _countText.text = info.Count.ToString();
            // TODO 나중에 Item Frame 추가
        }
    }
}