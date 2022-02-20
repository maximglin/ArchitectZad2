using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrpcClient;

namespace Client
{
    abstract class DescriptionVMBase : BaseVM
    {
        public CarsRepository repo;
        public DescriptionVMBase() { }
        public DescriptionVMBase(CarsRepository repo, int id, string name, string desc)
        {
            this.repo = repo;
            this.id = id;
            this.name = name;
            this.desc = desc;

            updatedData = new DataMessage() { Id = id, Name = name, Description = desc };
        }

        protected int? id = null;
        protected string name = "", desc = "";
        public int? Id { get => id; set { id = value; OnPropertyChanged(nameof(Id)); Update(); } }
        public string Name { get => name; set { name = value; OnPropertyChanged(nameof(Name)); Update(); } }
        public string Description { get => desc; set { desc = value; OnPropertyChanged(nameof(Description)); Update(); } }


        protected abstract Task<DataMessage> Addition(DataMessage request);
        protected abstract Task<DataMessage> Updation(DataMessage request);
        DataMessage updatedData;
        SemaphoreSlim sem = new SemaphoreSlim(1, 1);
        void Update()
        {
            Task.Run(async () =>
            {
                await sem.WaitAsync();

                if (id == null)
                {
                    updatedData = await Addition(new DataMessage()
                    {
                        Name = this.Name,
                        Description = this.Description
                    });
                    this.Id = updatedData.Id;
                }
                else if (updatedData != null && (
                updatedData.Name != name ||
                updatedData.Description != desc
                ))
                {
                    updatedData = await Updation(new DataMessage()
                    {
                        Id = id.Value,
                        Name = this.Name,
                        Description = this.Description
                    });
                }
                sem.Release();
            });
        }
    }
    
}
