namespace Samsara.Core.Tree
{
    public enum EvolutionNodeState
    {
        Current,    // EvolutionTreeScene-only — current evolution
        Evolvable,  // EvolutionTreeScene-only — unlock conditions met, can evolve
        Reachable,  // EvolutionTreeScene-only — next-stage candidate (conditions unmet)
        Locked,     // EvolutionTreeScene-only — outside nextNodes
        Hidden,     // EvolutionTreeScene-only — hidden + not unlocked
    }
}
