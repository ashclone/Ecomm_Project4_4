using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICartRepository Cart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }
        void Save();
    }
}
