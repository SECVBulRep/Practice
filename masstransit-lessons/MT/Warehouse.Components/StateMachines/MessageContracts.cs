using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines;

public static  class MessageContracts
{
    private static bool _initialized;

    public static void Initialize()
    {
        if(_initialized)
            return;
        
        GlobalTopology.Send.UseCorrelationId<IProductAdded>(x => x.ProductId);
        GlobalTopology.Send.UseCorrelationId<IReservationRequested>(x=>x.ReservationId);
        GlobalTopology.Send.UseCorrelationId<IReservationExpired>(x=>x.ReservationId);
    }
}