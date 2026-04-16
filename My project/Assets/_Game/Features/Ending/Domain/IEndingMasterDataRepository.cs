using System.Collections.Generic;
using Samsara.Features.Ending.MasterData;

namespace Samsara.Features.Ending.Domain
{
    public interface IEndingMasterDataRepository
    {
        EndingSO                 GetEnding(int endingId);
        IReadOnlyList<EndingSO>  GetEndingsByTriggerKind(EndingTriggerKind triggerKind);
        EndingSO[]               GetAllEndings();
    }
}
