using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
        

            // унарный вызов
            var unarReply = await client.UnaryCallAsync(new ExampleRequest { RequestId = Guid.NewGuid().ToString() });
            Console.WriteLine("unarReply: " + unarReply.RequestId);

            // передача данных их сервера 
            using (var streamingFromServerReplays = client.StreamingFromServer(new ExampleRequest { RequestId = Guid.NewGuid().ToString() }))
            {
                var i = 0;
                while (await streamingFromServerReplays.ResponseStream.MoveNext(CancellationToken.None))
                {
                    Console.WriteLine("StreamingFromServer: " + streamingFromServerReplays.ResponseStream.Current.RequestId);
                    i++;
                }
            }

            //пеоедача данных их клиента
            AsyncClientStreamingCall<ExampleRequest, ExampleResponse> call =  client.StreamingFromClient();
            for (var i = 0; i < 3; i++)
            {
                await call.RequestStream.WriteAsync(new ExampleRequest { RequestId = Guid.NewGuid().ToString() });
            }
            await call.RequestStream.CompleteAsync();
            var streamingFromClientreplay = await call;
            
            Console.WriteLine("StreamingFromClient: " + streamingFromClientreplay.RequestId);
            
   
            // двухсторонняя передача данных
            using var StreamingBothWayscall = client.StreamingBothWays();

            Console.WriteLine("Starting background task to receive messages");
            var readTask = Task.Run(async () =>
            {
                await foreach (var response in StreamingBothWayscall.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(response.RequestId);
                    // Echo messages sent to the service
                }
            });

            Console.WriteLine("Starting to send messages");
            Console.WriteLine("Type a message to echo then press enter.");
            while (true)
            {
                var result = Console.ReadLine();
                if (string.IsNullOrEmpty(result))
                {
                    break;
                }

                await StreamingBothWayscall.RequestStream.WriteAsync(new ExampleRequest{RequestId = result});
            }

            Console.WriteLine("Disconnecting");
            await StreamingBothWayscall.RequestStream.CompleteAsync();
            await readTask;

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

      

    }
}