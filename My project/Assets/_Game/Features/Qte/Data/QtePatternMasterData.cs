using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QtePatternData", menuName = "Samsara/Features/Qte/Qte Pattern Data")]
public class QtePatternMasterData : ScriptableObject
{
    [Header("Identity")]
    public int Id;
    public string Name;

    [Header("Sequence")]
    public List<QteNoteData> NoteList = new List<QteNoteData>();
}