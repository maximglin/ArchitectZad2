using System;
using System.Collections.Generic;
using Grpc.Net.Client;

namespace GrpcClient
{
    public class CarsRepository : IDisposable
    {
        GrpcChannel channel;
        Cars.CarsClient client;
        public CarsRepository()
        {
            channel = GrpcChannel.ForAddress("https://localhost:5001");
            client = new Cars.CarsClient(channel);
        }

        public void Dispose()
        {
            channel.Dispose();
        }


        public IAsyncEnumerable<CarReply> GetCars()
        {
            return client.GetCars(new Empty()).ResponseStream.ToAsyncEnumerable();
        }


    }
}
