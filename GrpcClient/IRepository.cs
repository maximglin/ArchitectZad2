using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrpcClient
{
    public interface IRepository<Reply, Request>
    {
        Task<Reply> AddEntity(Request entity);
        Task<Reply> UpdateEntity(Request entity);
        Task<Reply> RemoveEntity(Request entity);
        IAsyncEnumerable<Reply> GetAll();
    }
}
