namespace MT.SampleComponents.CourierAcitivities;

public interface IAllocateInventoryArguments
{
    Guid OrderId { get; set; }
    string ItemNumber { get; set; }
    string Quantity { get; set; }
}