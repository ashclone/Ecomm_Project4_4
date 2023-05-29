using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.Models.ViewModels
{
  public class CartVM
    {
        public IEnumerable<Cart> listOfCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public string Key { get; set; }
        public string Currency { get; set; }
        public int Payment_Capture { get; set; }
        public string paymentId { get; set; }
        public string OrderId { get; set; }
        public string signature { get; set; }


    }
}
