using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QtePatternData", menuName = "Samsara/Features/Qte/Qte Pattern Data")]
public class QtePatternData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string patternName;

    [Header("Sequence")]
    public List<QteNoteData> notes = new List<QteNoteData>();
}