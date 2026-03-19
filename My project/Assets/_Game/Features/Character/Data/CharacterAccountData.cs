using System.Collections.Generic;

namespace Samsara.Features.Character.Data
{
    public class CharacterAccountData
    {
        public List<string> UnlockedEvolutionNodeIds = new();
        public List<string> CompletedCodexIds = new();
        public int Gems;
    }
}
