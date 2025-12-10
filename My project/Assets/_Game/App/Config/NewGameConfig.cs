using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGameConfig", menuName = "SamSara/App/NewGameConfig")]
public class NewGameConfig : ScriptableObject
{
    [Header("Inventory Settings")]
    public int InitialMoney = 0; // 기본 소지금

    [Header("Daily State Settings")]
    public int StartDay = 1; // 시작 날짜
    public int DefaultActionSlots = 3; // 기본 행동력 슬롯 개수

    [Header("Character Settings")]
    public List<string> StartingCharacterIds = new List<string> { "character_main" };

}