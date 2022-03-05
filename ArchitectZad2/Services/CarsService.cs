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


            await cars.Entry(car).Reference(c => c.ManufacturerNavigation).LoadAsync();
            await cars.Entry(car).Reference(c => c.ColorNavigation).LoadAsync();
            //var color = await cars.Colors.FindAsync(car.Color);
            //var manuf = await cars.Manufacturers.FindAsync(car.Manufacturer);
            return await Task.FromResult(new CarReply
            {
                Id = car.Id,
                Manufacturer = car.ManufacturerNavigation.Name,
                Model = car.Model,
                Color = car.ColorNavigation.Name,
                Price = (float)car.Price,
                ManufacturerId = car.Manufacturer.GetValueOrDefault(),
                ColorId = car.Color.GetValueOrDefault()
            });
        }

        public override async Task GetColors(Empty request, IServerStreamWriter<ColorMessage> responseStream, ServerCallContext context)
        {
            var colors = cars.Colors.AsAsyncEnumerable();
            await foreach (var col in colors)
                await responseStream.WriteAsync(new ColorMessage
                {
                    Id = col.Id,
                    Name = col.Name,
                    Description = col.Code
                });
        }

        public override async Task GetManufacturers(Empty request, IServerStreamWriter<ManufacturerMessage> responseStream, ServerCallContext context)
        {
            var manufs = cars.Manufacturers.AsAsyncEnumerable();
            await foreach (var m in manufs)
                await responseStream.WriteAsync(new ManufacturerMessage
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Country
                });
        }

        public override async Task<CarReply> RemoveCar(CarUpdateRequest request, ServerCallContext context)
        {
            var car = await cars.Cars.FindAsync(request.Id);
            cars.Cars.Remove(car);
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


        public override async Task<ColorMessage> AddColor(ColorMessage request, ServerCallContext context)
        {
            var color = new Color()
            {
                Name = request.Name,
                Code = request.Description
            };

            await cars.Colors.AddAsync(color);
            await cars.SaveChangesAsync();

            request.Id = color.Id;

            return await Task.FromResult(request);
        }
        public override async Task<ColorMessage> UpdateColor(ColorMessage request, ServerCallContext context)
        {
            var color = await cars.Colors.FindAsync(request.Id);
            color.Name = request.Name;
            color.Code = request.Description;

            cars.Colors.Update(color);
            await cars.SaveChangesAsync();


            return await Task.FromResult(request);
        }

        public override async Task<ManufacturerMessage> AddManufacturer(ManufacturerMessage request, ServerCallContext context)
        {
            var manuf = new Manufacturer()
            {
                Name = request.Name,
                Country = request.Description
            };

            await cars.Manufacturers.AddAsync(manuf);
            await cars.SaveChangesAsync();

            request.Id = manuf.Id;

            return await Task.FromResult(request);
        }

        public override async Task<ManufacturerMessage> UpdateManufacturer(ManufacturerMessage request, ServerCallContext context)
        {
            var manuf = await cars.Manufacturers.FindAsync(request.Id);
            manuf.Name = request.Name;
            manuf.Country = request.Description;

            cars.Manufacturers.Update(manuf);
            await cars.SaveChangesAsync();


            return await Task.FromResult(request);
        }


        public override async Task<ColorMessage> RemoveColor(ColorMessage request, ServerCallContext context)
        {
            var color = await cars.Colors.FindAsync(request.Id);
            cars.Cars.RemoveRange(cars.Cars.Where(c => c.Color == color.Id));
            cars.Colors.Remove(color);
            await cars.SaveChangesAsync();

            return await Task.FromResult(request);
        }

        public override async Task<ManufacturerMessage> RemoveManufacturer(ManufacturerMessage request, ServerCallContext context)
        {
            var manuf = await cars.Manufacturers.FindAsync(request.Id);
            cars.Cars.RemoveRange(cars.Cars.Where(c => c.Manufacturer == manuf.Id));
            cars.Manufacturers.Remove(manuf);
            await cars.SaveChangesAsync();

            return await Task.FromResult(request);
        }
    }
}
