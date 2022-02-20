using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcClient
{
    public class CarsRepository : IDisposable, IRepository<CarReply, CarUpdateRequest>
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


        public IAsyncEnumerable<CarReply> GetAll()
        {
            return client.GetCars(new Empty()).ResponseStream.ToAsyncEnumerable();
        }

        public async Task<CarReply> AddEntity(CarUpdateRequest car)
        {
            return await client.AddCarAsync(car);
        }
        public async Task<CarReply> UpdateEntity(CarUpdateRequest car)
        {
            return await client.UpdateCarAsync(car);
        }
        public async Task<CarReply> RemoveEntity(CarUpdateRequest car)
        {
            return await client.RemoveCarAsync(car);
        }


    }
}
