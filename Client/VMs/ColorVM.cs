using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrpcClient;
using Microsoft.Extensions.DependencyInjection;

namespace Client
{
    class ColorVM : DescriptionVMBase
    {
        IRepository<ColorMessage, ColorMessage> repo;
        public ColorVM() { this.repo = (App.Current as App).ServiceProvider.GetRequiredService<IRepository<ColorMessage, ColorMessage>>(); }
        public ColorVM(IRepository<ColorMessage, ColorMessage> repo, int id, string name, string code)
            : base(id, name, code)
        {
            this.repo = repo;
        }
        public string Code { get => Description; set { Description = value; OnPropertyChanged(nameof(Code)); } }

        protected override async Task<DataMessage> Addition(DataMessage request)
        {
            var dm = await repo.AddEntity(new ColorMessage() { Id = request.Id, Name = request.Name, Description = request.Description});
            return new DataMessage() { Id = dm.Id, Name = dm.Name, Description = dm.Description };
        }
        protected override async Task<DataMessage> Updation(DataMessage request)
        {
            var dm = await repo.UpdateEntity(new ColorMessage() { Id = request.Id, Name = request.Name, Description = request.Description });
            return new DataMessage() { Id = dm.Id, Name = dm.Name, Description = dm.Description };
        }
    }
}
