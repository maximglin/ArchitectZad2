using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcClient
{
    public class ManufacturersRepository : IRepository<ManufacturerMessage, ManufacturerMessage>, IDisposable
    {
        GrpcChannel channel;
        Cars.CarsClient client;
        public ManufacturersRepository()
        {
            channel = GrpcChannel.ForAddress("https://localhost:5001");
            client = new Cars.CarsClient(channel);
        }

        public IAsyncEnumerable<ManufacturerMessage> GetAll()
        {
            return client.GetManufacturers(new Empty()).ResponseStream.ToAsyncEnumerable();
        }


        public async Task<ManufacturerMessage> AddEntity(ManufacturerMessage manuf)
        {
            return await client.AddManufacturerAsync(manuf);
        }
        public async Task<ManufacturerMessage> UpdateEntity(ManufacturerMessage manuf)
        {
            return await client.UpdateManufacturerAsync(manuf);
        }
        public async Task<ManufacturerMessage> RemoveEntity(ManufacturerMessage manuf)
        {
            return await client.RemoveManufacturerAsync(manuf);
        }

        public void Dispose()
        {
            channel.Dispose();
        }
    }
}
