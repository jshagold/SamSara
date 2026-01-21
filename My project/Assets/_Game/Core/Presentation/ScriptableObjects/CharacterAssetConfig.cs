using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName ="CharacterAssetConfig", menuName = "SamSara/UI/CharacterAssetConfig")]
public class CharacterAssetConfig : ScriptableObject
{
    // inspecter에서 입력받을 데이터 구조
    [System.Serializable]
    public class PortraitData
    {
        public int id;
        public Sprite sprite;
    }

    // 데이터 리스트
    [SerializeField] private List<PortraitData> portraits;
    [SerializeField] private Sprite defaultSprite;

    public Sprite GetPortrait(int id)
    {
        var data = portraits.FirstOrDefault(x => x.id == id);

        return data != null ? data.sprite : defaultSprite;
    }
}