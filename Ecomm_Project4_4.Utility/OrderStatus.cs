using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.Utility
{
   public static class OrderStatus
    {
        public const string OrderStatusPending = "Order_Pending";
        public const string OrderStatusRefunded = "Order_Refunded";
        public const string OrderStatusApproved = "Order_Approved";
        public const string OrderStatusCancelled = "Order_Cancelled";
        public const string OrderStatusShipped= "Order_Shipped";
    }
}
