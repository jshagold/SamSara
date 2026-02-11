using UnityEngine;

[CreateAssetMenu(fileName = "NewGameConfig", menuName = "SamSara/App/NewGameConfig")]
public class NewGameConfig : ScriptableObject
{
    [Header("Inventory Settings")]
    [SerializeField] private int _initialMoney = 0;

    [Header("Daily State Settings")]
    [SerializeField] private int _startDay = 1;
    [SerializeField] private int _defaultActionSlots = 3;

    [Header("Character Settings")]
    [SerializeField] private int _startingCharacterId = 1001;
    [SerializeField] private int _startingCharacterNodeId = 101;

    public int InitialMoney => _initialMoney;
    public int StartDay => _startDay;
    public int DefaultActionSlots => _defaultActionSlots;
    public int StartingCharacterId => _startingCharacterId;
    public int StartingCharacterNodeId => _startingCharacterNodeId;
}