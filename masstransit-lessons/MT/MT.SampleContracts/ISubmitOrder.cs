﻿using MassTransit;

namespace MT.SampleContracts;

public interface ISubmitOrder
{
    Guid OrderId { get; set; }
    DateTime TimeStamp { get; set; }
    string CustomerNumber { get; set; }
    string PaymentCardNumber { get; set; }
    MessageData<string> Notes { get; set; }
}