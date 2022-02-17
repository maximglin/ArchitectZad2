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
                await Application.Current.Dispatcher.InvokeAsync(() => Cars.Add(car));
            sem.Release();
        }

        public CarsVM(CarsRepository repository)
        {
            repo = repository;
            Cars.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(Cars));
            };
            
            Task.Run(UpdateCars);
        }

        public ObservableCollection<CarReply> Cars { get; } = new ObservableCollection<CarReply>();

    }
}
