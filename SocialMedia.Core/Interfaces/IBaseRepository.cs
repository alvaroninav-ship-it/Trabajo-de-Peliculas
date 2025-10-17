using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity 
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int? id);

        Task Insert(T entity);

        Task Update(T entity);
        Task Delete(int id);
    }
}
