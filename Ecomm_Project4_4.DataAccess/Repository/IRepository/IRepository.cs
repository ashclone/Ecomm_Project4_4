using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.DataAccess.Repository.IRepository
{
   public interface IRepository<T> where T: class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate=null, string includeProperties=null);
        T Get(Expression<Func<T, bool>> predicate=null, string includeProperties=null);
        void Add(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entity);
    }

}
