using Samsara.Features.Ending.MasterData;

namespace Samsara.Features.Ending.Domain
{
    public interface IEndingMasterDataRepository
    {
        EndingSO   GetEnding(int endingId);
        EndingSO[] GetEndingByType(EndingType type);
        EndingSO[] GetAllEndings();
    }
}
