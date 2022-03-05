using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcClient
{
    public class ColorsRepository : IRepository<ColorMessage, ColorMessage>, IDisposable
    {

        GrpcChannel channel;
        Cars.CarsClient client;
        public ColorsRepository()
        {
            channel = GrpcChannel.ForAddress("https://localhost:5001");
            client = new Cars.CarsClient(channel);
        }

        public IAsyncEnumerable<ColorMessage> GetAll()
        {
            return client.GetColors(new Empty()).ResponseStream.ToAsyncEnumerable();
        }

        public async Task<ColorMessage> AddEntity(ColorMessage color)
        {
            return await client.AddColorAsync(color);
        }
        public async Task<ColorMessage> UpdateEntity(ColorMessage color)
        {
            return await client.UpdateColorAsync(color);
        }
        public async Task<ColorMessage> RemoveEntity(ColorMessage color)
        {
            return await client.RemoveColorAsync(color);
        }

        public void Dispose()
        {
            channel.Dispose();
        }
    }
}
