using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrpcClient;

namespace Client
{
    class ColorVM : DescriptionVMBase
    {
        public ColorVM() { }
        public ColorVM(CarsRepository repo, int id, string name, string code)
            : base(repo, id, name, code)
        {

        }
        public string Code { get => Description; set { Description = value; OnPropertyChanged(nameof(Code)); } }

        protected override async Task<DataMessage> Addition(DataMessage request)
        {
            return await repo.AddColor(request);
        }
        protected override async Task<DataMessage> Updation(DataMessage request)
        {
            return await repo.UpdateColor(request);
        }
    }
}
