using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcClient
{
    public class ColorsRepository : IRepository<DataMessage, DataMessage>, IDisposable
    {

        GrpcChannel channel;
        Cars.CarsClient client;
        public ColorsRepository()
        {
            channel = GrpcChannel.ForAddress("https://localhost:5001");
            client = new Cars.CarsClient(channel);
        }

        public IAsyncEnumerable<DataMessage> GetAll()
        {
            return client.GetColors(new Empty()).ResponseStream.ToAsyncEnumerable();
        }

        public async Task<DataMessage> AddEntity(DataMessage color)
        {
            return await client.AddColorAsync(color);
        }
        public async Task<DataMessage> UpdateEntity(DataMessage color)
        {
            return await client.UpdateColorAsync(color);
        }
        public async Task<DataMessage> RemoveEntity(DataMessage color)
        {
            return await client.RemoveColorAsync(color);
        }

        public void Dispose()
        {
            channel.Dispose();
        }
    }
}
