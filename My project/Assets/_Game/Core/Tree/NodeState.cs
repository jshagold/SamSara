namespace Samsara.Core.Tree
{
    public enum NodeState
    {
        Current,      // EvolutionTreeScene-only — current evolution
        Evolvable,    // EvolutionTreeScene-only — unlock conditions met, can evolve
        Reachable,    // EvolutionTreeScene-only — next-stage candidate
        Locked,       // shared — not unlocked
        Hidden,       // shared — hidden route + not unlocked
        Selectable    // ReplayScene-only — unlocked, selectable as starting node
    }
}
