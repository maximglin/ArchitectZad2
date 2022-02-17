using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service;
using Microsoft.EntityFrameworkCore;

namespace ArchitectZad2
{
    public class CarsService : Cars.CarsBase
    {
        private readonly ILogger<CarsService> _logger;
        public CarsService(ILogger<CarsService> logger)
        {
            _logger = logger;
            cars = new CarsDbContext();
        }


        CarsDbContext cars;

        public override async Task GetCars(Empty request, IServerStreamWriter<CarReply> responseStream, ServerCallContext context)
        {

            var response = cars.Cars
                .Join(cars.Colors, car => car.Color, color => color.Id,
                (car, color) => new { car.Id, car.Model, car.Manufacturer, car.Price, color.Name})
                .Join(cars.Manufacturers, car => car.Manufacturer, m => m.Id,
                (car, m) => new CarReply
                {
                    Id = car.Id,
                    Manufacturer = m.Name,
                    Model = car.Model,
                    Color = car.Name,
                    Price = (float)car.Price
                });

            await foreach(var car in response.AsAsyncEnumerable())
            {
                await responseStream.WriteAsync(car);
            }
        }
    }
}
