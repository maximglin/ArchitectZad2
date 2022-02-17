﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrpcClient;

namespace Client
{
    class CarVM : BaseVM
    {
        public CarsRepository repo;
        public CarVM() { }
        public CarVM(CarsRepository repository, int id, int manufid, int colid, string model, float price)
        {
            repo = repository;
            this.id = id;
            this.manufacturerId = manufid;
            this.colorId = colid;
            this.model = model;
            this.price = price;
        }


        SemaphoreSlim sem = new SemaphoreSlim(1, 1);

        void UpdateCar()
        {
            Task.Run(async () =>
            {
                await sem.WaitAsync();
                if (manufacturerId != null && colorId != null)
                {
                    if (id == null)
                    {
                        var car = await repo.AddCar(new CarUpdateRequest()
                        {
                            ManufacturerId = manufacturerId.Value,
                            ColorId = colorId.Value,
                            Model = model,
                            Price = price
                        });
                        this.Id = car.Id;
                        Color = car.Color;
                        Manufacturer = car.Manufacturer;
                    }
                    else
                    {
                        var car = await repo.UpdateCar(new CarUpdateRequest()
                        {
                            Id = id.Value,
                            ManufacturerId = manufacturerId.Value,
                            ColorId = colorId.Value,
                            Model = model,
                            Price = price
                        });
                        Color = car.Color;
                        Manufacturer = car.Manufacturer;
                    }

                }
                sem.Release();
            });
        }


        int? id = null;
        int? manufacturerId = null, colorId = null;
        string model = "", manufacturer = "", color = "";
        float price = 0f;
        public int? Id { get => id; set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Model { get => model; set { model = value; OnPropertyChanged(nameof(Model)); UpdateCar(); } }


        public string Manufacturer { get => manufacturer; set { manufacturer = value; OnPropertyChanged(nameof(Manufacturer)); } }
        public int? ManufacturerId { get => manufacturerId; set { manufacturerId = value; OnPropertyChanged(nameof(ManufacturerId)); UpdateCar(); } }


        public string Color { get => color; set { color = value; OnPropertyChanged(nameof(Color)); } }
        public int? ColorId { get => colorId; set { colorId = value; OnPropertyChanged(nameof(ColorId)); UpdateCar(); } }


        public float Price { get => price; set { price = value; OnPropertyChanged(nameof(Price)); UpdateCar(); } }
    }
}
