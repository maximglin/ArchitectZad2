using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<CarReply> AddCar(CarUpdateRequest car)
        {
            return await client.AddCarAsync(car);
        }
        public async Task<CarReply> UpdateCar(CarUpdateRequest car)
        {
            return await client.UpdateCarAsync(car);
        }
        public async Task<CarReply> RemoveCar(CarUpdateRequest car)
        {
            return await client.RemoveCarAsync(car);
        }



        public IAsyncEnumerable<ColorReply> GetColors()
        {
            return client.GetColors(new Empty()).ResponseStream.ToAsyncEnumerable();
        }

        public IAsyncEnumerable<ManufacturerReply> GetManufacturers()
        {
            return client.GetManufacturers(new Empty()).ResponseStream.ToAsyncEnumerable();
        }
    }
}
