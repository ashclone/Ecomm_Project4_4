using Ecomm_Project4_4.Data;
using Ecomm_Project4_4.DataAccess.Repository.IRepository;
using Ecomm_Project4_4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public void UpdatePaymentStatus(int orderHeaderId,string sessionId, string paymentIntentId)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == orderHeaderId);
            orderHeader.PaymentIntentid = paymentIntentId;
            orderHeader.SessionId = sessionId;
            orderHeader.DateOfPayment = DateTime.Now;
           
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateOrderStatus(int orderHeaderId, string orderStatus, string paymentStatus = null)
        {
            var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == orderHeaderId);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
            }
            if (paymentStatus != null)
            {
                order.PaymentStatus = paymentStatus;
            }
           
        }
        public void OrderShipped(int orderHeaderId, string trackingId, DateTime shippingDate)
        {
            var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == orderHeaderId);
            if (order != null)
            {
                order.TrackingNumber = trackingId;
                order.DateOfShipping = shippingDate;
            }
        }
    }
}
