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

        SemaphoreSlim sem = new SemaphoreSlim(1, 1);
        private async Task UpdateCars()
        {
            await sem.WaitAsync();
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
            sem.Release();
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
                OnPropertyChanged(nameof(ColorsSelection));
                OnPropertyChanged(nameof(Colors));
                OnPropertyChanged(nameof(Cars));

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    var items = e.OldItems;
                    Task.Run(async () =>
                    {
                        foreach (var item in items)
                            if ((item as DescriptionVMBase).Id.HasValue)
                                await repo.RemoveColor(new DataMessage() { Id = (item as DescriptionVMBase).Id.Value });
                        await UpdateCars();
                    });
                }
            };
            Manufacturers.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(ManufacturersSelection));
                OnPropertyChanged(nameof(Manufacturers));
                OnPropertyChanged(nameof(Cars));

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    var items = e.OldItems;
                    Task.Run(async () =>
                    {
                        foreach (var item in items)
                            if ((item as DescriptionVMBase).Id.HasValue)
                                await repo.RemoveManufacturer(new DataMessage() { Id = (item as DescriptionVMBase).Id.Value });
                        await UpdateCars();
                    });
                }
            };


            Task.Run(async () =>
            {
                await sem.WaitAsync();
                await Application.Current.Dispatcher.InvokeAsync(() => Colors.Clear());
                await foreach (var col in repo.GetColors())
                {
                    var cVM = new ColorVM(repo, col.Id, col.Name, col.Code);
                    //await Application.Current.Dispatcher.InvokeAsync(() => Colors.Add(cVM));
                    await Application.Current.Dispatcher.InvokeAsync(() => Colors.Add(cVM));
                }

                await Application.Current.Dispatcher.InvokeAsync(() => Manufacturers.Clear());
                await foreach (var m in repo.GetManufacturers())
                {
                    var mVM = new ManufacturerVM(repo, m.Id, m.Name, m.Country);
                    //await Application.Current.Dispatcher.InvokeAsync(() => Manufacturers.Add(mVM));
                    await Application.Current.Dispatcher.InvokeAsync(() => Manufacturers.Add(mVM));
                }
                    
                sem.Release();

                await UpdateCars();
            });//.ContinueWith(async (o) => { await UpdateCars(); }, TaskScheduler.Default);
        }

        public ObservableCollection<CarVM> Cars { get; } = new ObservableCollection<CarVM>();



        

        public IEnumerable<ManufacturerVM> ManufacturersSelection => Manufacturers.Where(m => m.Id != null);

        public ObservableCollection<ManufacturerVM> Manufacturers { get; } = new ObservableCollection<ManufacturerVM>();

        
        public IEnumerable<ColorVM> ColorsSelection => Colors.Where(c => c.Id != null);
        public ObservableCollection<ColorVM> Colors { get; } = new ObservableCollection<ColorVM>();
    }
}
