﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrpcClient;

namespace Client
{
    class ManufacturerVM : DescriptionVMBase
    {
        public ManufacturerVM() { }
        public ManufacturerVM(IRepository<DataMessage, DataMessage> repo, int id, string name, string country)
            : base(repo, id, name, country)
        {

        }

        public string Country { get => Description; set { Description = value; OnPropertyChanged(nameof(Country)); } }

        protected override async Task<DataMessage> Addition(DataMessage request)
        {
            return await repo.AddEntity(request);
        }
        protected override async Task<DataMessage> Updation(DataMessage request)
        {
            return await repo.UpdateEntity(request);
        }
    }
}
