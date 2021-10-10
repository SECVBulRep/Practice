using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogWarning(request.Name);
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<HelloReply> SayHello2(HelloRequest request, ServerCallContext context)
        {
            _logger.LogWarning(request.Name);
            
            return Task.FromResult(new HelloReply
            {
                Message = "Hello2 " + request.Name
            });
        }
        
        public override Task<ExampleResponse> UnaryCall(ExampleRequest request,
            ServerCallContext context)
        {
            _logger.LogWarning("UnaryCall "+ request.RequestId);
            
            var response = new ExampleResponse();
            response.RequestId = request.RequestId + " " + DateTime.Now.ToString();
            return Task.FromResult(response);
        }
        
        public override async Task StreamingFromServer(ExampleRequest request,
            IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            
            _logger.LogWarning("StreamingFromServer "+ request.RequestId);
            
            for (var i = 0; i < 3; i++)
            {
                await responseStream.WriteAsync(new ExampleResponse{RequestId = "StreamingFromServer response  "+i+" "+DateTime.Now+ request.RequestId});
                _logger.LogWarning("StreamingFromServer response  "+i+" "+DateTime.Now+ request.RequestId);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        
        public override async Task<ExampleResponse> StreamingFromClient(
            IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            string allReqs = "";
            
            while (await requestStream.MoveNext())
            {
                var message = requestStream.Current;
                allReqs = allReqs +" "+ message.RequestId+" "+DateTime.Now+" "+Environment.NewLine;
            }
            
            _logger.LogWarning("StreamingFromClient "+ allReqs);
            return new ExampleResponse{RequestId = allReqs};
        }
        
        
        ConcurrentBag<string> cb = new ConcurrentBag<string>();
        
        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream,
            IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            // Read requests in a background task.
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    _logger.LogWarning("StreamingBothWays MessageFromClient "+ message.RequestId);
                    cb.Add(message.RequestId);
                    // Process request.
                }
            });
    
            // Send responses until the client signals that it is complete.
            while (!readTask.IsCompleted)
            {
                string res;
                if(cb.TryTake(out res)){
                    await responseStream.WriteAsync(new ExampleResponse{RequestId = res});
                }
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }
        
    }
}