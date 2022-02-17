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
                (car, color) => new { car.Id, car.Model, car.Manufacturer, car.Price, color.Name, ColorId = color.Id})
                .Join(cars.Manufacturers, car => car.Manufacturer, m => m.Id,
                (car, m) => new CarReply
                {
                    Id = car.Id,
                    Manufacturer = m.Name,
                    Model = car.Model,
                    Color = car.Name,
                    Price = (float)car.Price,
                    ManufacturerId = m.Id,
                    ColorId = car.ColorId
                });

            await foreach(var car in response.AsAsyncEnumerable())
            {
                await responseStream.WriteAsync(car);
            }
        }

        public override async Task<CarReply> AddCar(CarUpdateRequest request, ServerCallContext context)
        {
            var car = new Car()
            {
                Manufacturer = request.ManufacturerId,
                Color = request.ColorId,
                Model = request.Model,
                Price = (decimal)request.Price
            };
            await cars.Cars.AddAsync(car);
            await cars.SaveChangesAsync();

            request.Id = car.Id;
            var color = await cars.Colors.FindAsync(car.Color);
            var manuf = await cars.Manufacturers.FindAsync(car.Manufacturer);
            return await Task.FromResult(new CarReply
            {
                Id = car.Id,
                Manufacturer = manuf.Name,
                Model = car.Model,
                Color = color.Name,
                Price = (float)car.Price,
                ManufacturerId = car.Manufacturer.GetValueOrDefault(),
                ColorId = car.Color.GetValueOrDefault()
            });
        }

        public override async Task<CarReply> UpdateCar(CarUpdateRequest request, ServerCallContext context)
        {
            var car = await cars.Cars.FindAsync(request.Id);
            car.Color = request.ColorId;
            car.Manufacturer = request.ManufacturerId;
            car.Model = request.Model;
            car.Price = (decimal)request.Price;

            cars.Cars.Update(car);
            await cars.SaveChangesAsync();

            var color = await cars.Colors.FindAsync(car.Color);
            var manuf = await cars.Manufacturers.FindAsync(car.Manufacturer);
            return await Task.FromResult(new CarReply
            {
                Id = car.Id,
                Manufacturer = manuf.Name,
                Model = car.Model,
                Color = color.Name,
                Price = (float)car.Price,
                ManufacturerId = car.Manufacturer.GetValueOrDefault(),
                ColorId = car.Color.GetValueOrDefault()
            });
        }

        public override async Task GetColors(Empty request, IServerStreamWriter<ColorReply> responseStream, ServerCallContext context)
        {
            var colors = cars.Colors.AsAsyncEnumerable();
            await foreach (var col in colors)
                await responseStream.WriteAsync(new ColorReply
                {
                    Id = col.Id,
                    Name = col.Name,
                    Code = col.Code
                });
        }

        public override async Task GetManufacturers(Empty request, IServerStreamWriter<ManufacturerReply> responseStream, ServerCallContext context)
        {
            var manufs = cars.Manufacturers.AsAsyncEnumerable();
            await foreach (var m in manufs)
                await responseStream.WriteAsync(new ManufacturerReply
                {
                    Id = m.Id,
                    Name = m.Name,
                    Country = m.Country
                });
        }
    }
}
