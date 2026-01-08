using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QtePatternInfo
{
    public int Id;
    public string Name;

    public List<QteNoteData> NoteList = new List<QteNoteData>();

    public QtePatternInfo() { }
}