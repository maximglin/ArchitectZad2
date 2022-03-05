using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrpcClient;
using Microsoft.Extensions.DependencyInjection;

namespace Client
{
    class ManufacturerVM : DescriptionVMBase
    {
        IRepository<ManufacturerMessage, ManufacturerMessage> repo;
        public ManufacturerVM() { this.repo = (App.Current as App).ServiceProvider.GetRequiredService<IRepository<ManufacturerMessage, ManufacturerMessage>>(); }
        public ManufacturerVM(IRepository<ManufacturerMessage, ManufacturerMessage> repo, int id, string name, string country)
            : base(id, name, country)
        {
            this.repo = repo;
        }

        public string Country { get => Description; set { Description = value; OnPropertyChanged(nameof(Country)); } }

        protected override async Task<DataMessage> Addition(DataMessage request)
        {
            var dm = await repo.AddEntity(new ManufacturerMessage() { Id = request.Id, Name = request.Name, Description = request.Description });
            return new DataMessage() { Id = dm.Id, Name = dm.Name, Description = dm.Description };
        }
        protected override async Task<DataMessage> Updation(DataMessage request)
        {
            var dm = await repo.UpdateEntity(new ManufacturerMessage() { Id = request.Id, Name = request.Name, Description = request.Description });
            return new DataMessage() { Id = dm.Id, Name = dm.Name, Description = dm.Description };
        }
    }
}
