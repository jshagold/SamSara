using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LobbyButtonThemeConfig", menuName = "Configs/LobbyButtonTheme")]
public class LobbyButtonThemeConfig : ScriptableObject
{
    [System.Serializable]
    public class LobbyButtonThemeData
    {
        public LobbyButtonType type;
        public Sprite icon;
        public Color backgroundColor;
        public string localizationKey;
    }

    [SerializeField] private List<LobbyButtonThemeData> _themes;

    public LobbyButtonThemeData GetData(LobbyButtonType type)
    {
        return _themes?.FirstOrDefault(t => t.type == type);
    }
}