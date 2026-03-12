namespace Core.ErrorHandling
{
    /// <summary>
    /// Save-block contract (FR-07). Consumed by AutoSaveManager before any save.
    /// </summary>
    public interface IStabilityFlag
    {
        bool IsSaveAllowed { get; }
        void Block();
    }
}
