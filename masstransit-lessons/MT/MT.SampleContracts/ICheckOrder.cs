using System.Net.Mail;

namespace MT.SampleContracts;

public interface ICheckOrder
{
    Guid OrderId { get; set; }
}