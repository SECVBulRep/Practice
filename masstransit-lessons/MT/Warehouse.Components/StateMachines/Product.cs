using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines;

public class Product :
    SagaStateMachineInstance
{
    public int CurrentState { get; set; }
    public DateTime DateAdded { get; set; }
    public string Name { get; set; }
    public string ManufacturerId { get; set; }
    public Guid CorrelationId { get; set; }
}

