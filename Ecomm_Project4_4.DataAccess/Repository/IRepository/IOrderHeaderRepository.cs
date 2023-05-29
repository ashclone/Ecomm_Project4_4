using Ecomm_Project4_4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void UpdateOrderStatus(int orderHeaderId, string orderStatus, string paymentStatus = null);
        void UpdatePaymentStatus(int orderHeaderId, string sessionId, string paymentIntentId);
        void OrderShipped(int orderHeaderId, string trackingId, DateTime shippingDate);
    }
}
