using Ecomm_Project4_4.DataAccess.Repository.IRepository;
using Ecomm_Project4_4.Models;
using Ecomm_Project4_4.Models.ViewModels;
using Ecomm_Project4_4.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region api
        public IActionResult AllOrders(string status)
        {

            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
                switch (status)
                {
                    case "allorders":
                        orderHeaders = orderHeaders.ToList();
                        break;
                    case "pending":
                        orderHeaders = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.PaymentPending).ToList();
                        break;
                    case "approved":
                        orderHeaders = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.PaymentApproved).ToList();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

            return Json(new { data = orderHeaders });
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OrderDetail(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        public IActionResult OrderDetail(OrderVM vm)
        {

            var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == vm.OrderHeader.Id, includeProperties: "ApplicationUser");
            _unitOfWork.OrderHeader.OrderShipped(vm.OrderHeader.Id, vm.OrderHeader.TrackingNumber, vm.OrderHeader.DateOfShipping);
            //var OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == vm.OrderHeader.Id, includeProperties: "Product");
            _unitOfWork.Save();
            TempData["success"] = "Order Successfully  Shipped ";
            
            return RedirectToAction("OrderDetail", "Order", new { id = vm.OrderHeader.Id });
        }
        public IActionResult OrderCancelled(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeProperties: "ApplicationUser");
            var orderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id);
            if (orderHeader != null&&orderDetail!=null)
            {
                _unitOfWork.OrderHeader.Delete(orderHeader);
                _unitOfWork.OrderDetail.DeleteRange(orderDetail);
                _unitOfWork.Save();
                TempData["success"] = "Order Cancelled Successfully ";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Data not found in data base";
                return RedirectToAction("OrderDetail", "Order", new { id = id });
            }
            
           
        }
    }
}
