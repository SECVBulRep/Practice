using System.Net;
using System.Text;
using MT.SampleContracts.DTO;
using Newtonsoft.Json;

namespace MT.Client.Console;

internal class Program
{
    static HttpClient _client;

    static async Task Main(string[] args)
    {
        _client = new HttpClient { Timeout = TimeSpan.FromMinutes(1) };

        while (true)
        {
            System.Console.Write("Enter # of orders to send, or empty to quit: ");

            var line = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
                break;

            int limit;
            int loops = 1;
            var segments = line.Split(',');
            if (segments.Length == 2)
            {
                loops = int.TryParse(segments[1], out int result) ? result : 1;
                limit = int.TryParse(segments[0], out result) ? result : 1;
            }
            else if (!int.TryParse(line, out limit))
                limit = 1;

            for (var pass = 0; pass < loops; pass++)
            {
                var tasks = new List<Task>();

                for (var i = 0; i < limit; i++)
                {
                    var order = new OrderModel
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = $"CUSTOMER{i}",
                        PaymentCardNumber = i % 4 == 0 ? "5999" : "4000-1234",
                    };

                    tasks.Add(Execute(order));
                }

                await Task.WhenAll(tasks.ToArray());

                System.Console.WriteLine();
                System.Console.WriteLine("Results {0}/{1}", pass + 1, loops);

                foreach (Task<string> task in tasks.Cast<Task<string>>())
                    System.Console.WriteLine(task.Result);
            }
        }
    }

    static readonly Random _random = new Random();

    static async Task<string> Execute(OrderModel order)
    {
        try
        {
            var json = JsonConvert.SerializeObject(order);
            var data = new StringContent(json, Encoding.UTF8, "application/json");


            string host = "https://localhost:7206";
            
            var responseMessage = await _client.PostAsync($"{host}/Order", data);

            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                await Task.Delay(2000);
                await Task.Delay(_random.Next(6000));

                var orderAddress = $"{host}/Order?id={order.Id:D}";

                var patchResponse = await _client.PatchAsync(orderAddress, data);

                patchResponse.EnsureSuccessStatusCode();
                var patchResult = await patchResponse.Content.ReadAsStringAsync();
                do
                {
                    await Task.Delay(5000);

                    var getResponse = await _client.GetAsync(orderAddress);
                    getResponse.EnsureSuccessStatusCode();
                    var getResult = await getResponse.Content.ReadAsAsync<OrderStatusModel>();
                    if (getResult.State == "Completed" || getResult.State == "Faulted")
                        return $"ORDER: {order.Id:D} STATUS: {getResult.State}";
                    System.Console.Write(".");
                } while (true);
            }

            return result;
        }
        catch (Exception exception)
        {
            System.Console.WriteLine(exception);

            return exception.Message;
        }
    }
}

