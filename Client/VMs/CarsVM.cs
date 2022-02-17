using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using GrpcClient;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Linq;

namespace Client
{
    class CarsVM : BaseVM
    {
        CarsRepository repo;

        SemaphoreSlim semCars = new SemaphoreSlim(1, 1);
        SemaphoreSlim semLists = new SemaphoreSlim(1, 1);
        private async Task UpdateCars()
        {
            await semCars.WaitAsync();
            await Application.Current.Dispatcher.InvokeAsync(() => Cars.Clear());
            await foreach (var car in repo.GetCars())
                await Application.Current.Dispatcher.InvokeAsync(() => Cars.Add(new CarVM(
                    repo, 
                    car.Id,
                    car.ManufacturerId,
                    car.ColorId,
                    car.Model,
                    car.Price) 
                {
                    Manufacturer = car.Manufacturer,
                    Color = car.Color,
                }));
            semCars.Release();
        }

        public CarsVM(CarsRepository repository)
        {
            repo = repository;
            Cars.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(Cars));
                if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    var items = e.OldItems;
                    Task.Run(async () =>
                    {
                        foreach (var item in items)
                            if((item as CarVM).Id.HasValue)
                                await repo.RemoveCar(new CarUpdateRequest() { Id = (item as CarVM).Id.Value });
                    });
                }
            };
            Colors.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(Colors));
            };
            Manufacturers.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(Manufacturers));
            };


            Task.Run(async () =>
            {
                await semLists.WaitAsync();
                await Application.Current.Dispatcher.InvokeAsync(() => Colors.Clear());
                await foreach (var col in repo.GetColors())
                    await Application.Current.Dispatcher.InvokeAsync(() => Colors.Add(new ColorVM()
                    {
                        Id = col.Id,
                        Name = col.Name,
                        Code = col.Code
                    }));

                await Application.Current.Dispatcher.InvokeAsync(() => Manufacturers.Clear());
                await foreach (var m in repo.GetManufacturers())
                    await Application.Current.Dispatcher.InvokeAsync(() => Manufacturers.Add(new ManufacturerVM()
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Country = m.Country
                    }));
                semLists.Release();
            });

            Task.Run(UpdateCars);
        }

        public ObservableCollection<CarVM> Cars { get; } = new ObservableCollection<CarVM>();


        public class ManufacturerVM
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Country { get; set; }
        }

        public ObservableCollection<ManufacturerVM> Manufacturers { get; } = new ObservableCollection<ManufacturerVM>();


        public class ColorVM
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
        }
        public ObservableCollection<ColorVM> Colors { get; } = new ObservableCollection<ColorVM>();
    }
}
