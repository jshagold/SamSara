namespace Samsara.Core.Tree
{
    public enum ReplayNodeState
    {
        Selectable, // unlocked node — selectable as starting evolution for the new run
        Locked,     // not unlocked + IsHidden == false
        Hidden,     // not unlocked + IsHidden == true
    }
}
