using Ecomm_Project4_4.DataAccess.Repository.IRepository;
using Ecomm_Project4_4.Helper;
using Ecomm_Project4_4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.Cart.GetAll(sp => sp.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "cateogory");
            return View(products);
        }
        public IActionResult Details(int? ProductId)
        {

            Cart cart = new Cart()
            {
                Product = _unitOfWork.Product.Get(x => x.Id == ProductId, includeProperties: "cateogory"),
                ProductId = (int)ProductId,
                Count = 1
            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claim.Value;

            if (ModelState.IsValid)
            {
                var cartItem = _unitOfWork.Cart.Get(x => x.ApplicationUserId == claim.Value && x.ProductId == cart.ProductId);
                if (cartItem == null)
                    _unitOfWork.Cart.Add(cart);
                else
                    _unitOfWork.Cart.IncrementCartItem(cartItem, cart.Count);

                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
