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
            NoteList = masterData.NoteList,
        };
    }

    // Domain -> MasterData
    public static QtePatternMasterData ToMasterData(this QtePatternInfo domain)
    {
        if(domain == null) return null;

        return new QtePatternMasterData
        {
            Id = domain.Id,
            Name = domain.Name,
            NoteList = domain.NoteList,
        };
    }
}