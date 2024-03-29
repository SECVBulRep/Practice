﻿namespace Warehouse.Contracts;

public interface IInventoryAllocated
{
    Guid AllocationId { get; set; }
    string ItemNumber { get; set; }
    int Quantity { get; set; } 
}