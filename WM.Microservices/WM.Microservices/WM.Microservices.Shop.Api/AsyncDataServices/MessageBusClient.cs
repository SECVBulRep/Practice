using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using WM.Microservices.Shop.Api.Dtos;

namespace WM.Microservices.Shop.Api.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _chanel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMqHost"],
            Port = Convert.ToInt32(_configuration["RabbitMqPort"])
        };

        try
        {
            _connection = factory.CreateConnection();
            _chanel = _connection.CreateModel();
            _chanel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            Console.WriteLine("--> Connected to RbtMq ");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RbtMq shutdown");
    }

    public void PublishNewProduct(ProductPublishedDto productPublishedDto)
    {
        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RbtMq connection opened, sending message");
            var message = JsonSerializer.Serialize(productPublishedDto);
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RbtMq connection is  closed");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _chanel.BasicPublish(exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body
        );
        
        Console.WriteLine("--> Message is sent");
    }

    public void Dispose()
    {
        Console.WriteLine("--> Message bus disposed");

        if (_chanel.IsOpen)
        {
            _chanel.Close();
            _connection.Close();
        }
    }
}