using System.Collections.Generic;
using System.Linq;

public static class QtePatternMapper
{
    // MasterData -> Domain
    public static QtePatternInfo ToDomain(this QtePatternMasterData masterData)
    {
        if(masterData == null) return null;

        return new QtePatternInfo
        {
            Id = masterData.Id,
            Name = masterData.Name,
            NoteList = masterData.NoteList != null
                ? masterData.NoteList.ToList() 
                : new List<QteNoteData>(),
        };
    }
}