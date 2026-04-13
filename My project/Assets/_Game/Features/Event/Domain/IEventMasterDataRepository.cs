using Samsara.Features.Event.MasterData;

namespace Samsara.Features.Event.Domain
{
    public interface IEventMasterDataRepository
    {
        EventSO GetEvent(int eventId);
        EventSO[] GetAllEvents();
        EventSO[] GetMaintenanceEvents();
    }
}
