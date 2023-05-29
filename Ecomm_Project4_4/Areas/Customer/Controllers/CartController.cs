using Ecomm_Project4_4.DataAccess.Repository.IRepository;
using Ecomm_Project4_4.Helper;
using Ecomm_Project4_4.Models;
using Ecomm_Project4_4.Models.ViewModels;
using Ecomm_Project4_4.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RazorpaySettings _rozarpaySettings;
        public CartController(IUnitOfWork unitOfWork, IOptions<RazorpaySettings> rozarpaySettings)
        {
            _unitOfWork = unitOfWork;
            _rozarpaySettings = rozarpaySettings.Value;
        }
        public CartVM vm { get; set; }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            vm = new CartVM()
            {
                listOfCarts = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            foreach (var item in vm.listOfCarts)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price) * (item.Count);
            }
            return View(vm);
        }
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.Cart.Get(x => x.Id == id);
            _unitOfWork.Cart.IncrementCartItem(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.Cart.Get(x => x.Id == id);
            if (cart.Count > 1)
                _unitOfWork.Cart.DecrementCartItem(cart, 1);
            else
                _unitOfWork.Cart.Delete(cart);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var cart = _unitOfWork.Cart.Get(x => x.Id == id);
            _unitOfWork.Cart.Delete(cart);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            vm = new CartVM()
            {
                listOfCarts = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            vm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == claim.Value);
            vm.OrderHeader.Name = vm.OrderHeader.ApplicationUser.Name;
            vm.OrderHeader.City = vm.OrderHeader.ApplicationUser.City;
            vm.OrderHeader.State = vm.OrderHeader.ApplicationUser.State;
            vm.OrderHeader.PostalCode = vm.OrderHeader.ApplicationUser.PinCode;
            vm.OrderHeader.PhoneNumber = vm.OrderHeader.ApplicationUser.PhoneNumber;
            vm.OrderHeader.Address = vm.OrderHeader.ApplicationUser.Address;
            foreach (var item in vm.listOfCarts)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price) * (item.Count);
            }



            RazorpayClient client = new RazorpayClient(_rozarpaySettings.key_id, _rozarpaySettings.key_secret);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", vm.OrderHeader.OrderTotal * 100); // amount in the smallest currency unit

            options.Add("currency", "INR");
            Order order = client.Order.Create(options);
            var orderId = order.Attributes["id"].ToString();
            vm.OrderId = orderId;



            return View(vm);
        }
        [HttpPost]
        public IActionResult summary(CartVM vm )
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm.listOfCarts = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product");
            vm.OrderHeader.OrderStatus = OrderStatus.OrderStatusPending;
            vm.OrderHeader.PaymentStatus = PaymentStatus.PaymentPending;
            vm.OrderHeader.DateOfOrder = DateTime.Now;
            vm.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var item in vm.listOfCarts)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price) * (item.Count);
            }
            _unitOfWork.OrderHeader.Add(vm.OrderHeader);
            _unitOfWork.Save();
            foreach (var item in vm.listOfCarts)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.Product.Id,
                    OrderHeaderId = vm.OrderHeader.Id,
                    Count = item.Count,
                    Price = item.Product.Price
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            //stripe
            #region Rozarpay

            var Valid = CompareSignatures(vm.OrderId, vm.paymentId, vm.signature);
            if (Valid)
            {
                _unitOfWork.OrderHeader.UpdatePaymentStatus(vm.OrderHeader.Id, vm.OrderId, vm.paymentId);
                _unitOfWork.OrderHeader.UpdateOrderStatus(vm.OrderHeader.Id, OrderStatus.OrderStatusApproved, PaymentStatus.PaymentApproved);
                List<Cart> cart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == vm.OrderHeader.ApplicationUserId).ToList();
                _unitOfWork.Cart.DeleteRange(cart);
                _unitOfWork.Save();
                ViewBag.success = vm.OrderId;
                return RedirectToAction("ordersuccess", "Cart", new { id = vm.OrderHeader.Id });

            }
            else
            {
                return RedirectToAction("ordersuccess", "Cart", new { id = vm.OrderHeader.Id });

                // _unitOfWork.OrderHeader.UpdatePaymentStatus(vm.OrderHeader.Id, charge.Id, charge.PaymentIntentId);
                // _unitOfWork.Save();

                // return RedirectToAction("ordersuccess","Cart",new {id=vm.OrderHeader.Id });
                #endregion
            }
        }
        private bool CompareSignatures(string orderId, string paymentId, string razorPaySignature)
        {
            var text = orderId + "|" + paymentId;
            var secret = _rozarpaySettings.key_secret;
            var generatedSignature = CalculateSHA256(text, secret);
            if (generatedSignature == razorPaySignature)
                return true;
            else
                return false;

        }
        private string CalculateSHA256(string text, string secret)
        {
            string result = "";
            var encoding = Encoding.Default;
            byte[] hasedText = encoding.GetBytes(text),
                salted = encoding.GetBytes(secret);
            HMACSHA256 haser = new HMACSHA256(salted);
            byte[] hasedEncodingText = haser.ComputeHash(hasedText);
            result = string.Join("", hasedEncodingText.ToList().Select(x => x.ToString("x2")).ToArray());
            return result;
        }
        public IActionResult ordersuccess(int? id)
        {
            if (id != null || id != 0)
            {
                var order = _unitOfWork.OrderHeader.Get(x => x.Id == id);
                _unitOfWork.OrderHeader.UpdateOrderStatus((int)id, OrderStatus.OrderStatusApproved, PaymentStatus.PaymentApproved);
                return View(id);
            }
            else
            {
                TempData["error"] = "Payment not Confirmed Please Try again letter";
                return View();
            }

        }
    }
}
