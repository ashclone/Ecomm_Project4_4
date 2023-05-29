using Ecomm_Project4_4.Data;
using Ecomm_Project4_4.DataAccess.Repository.IRepository;
using Ecomm_Project4_4.Helper;
using Ecomm_Project4_4.Models;
using Ecomm_Project4_4.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
          
            return View();
        }
        #region API
        public IActionResult AllProducts()
        {
            var Product = _unitOfWork.Product.GetAll(includeProperties: "cateogory");
            return Json(new { data = Product });
        }
        #endregion
        #region MyRegion

        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]//cross site scripting se prevent krne ke liye
        //public IActionResult Create(Category category )
        //{
        //    if (ModelState.IsValid)//server side validation ke liye 
        //    {
        //        _unitOfWork.Category.Add(category);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Category Created Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);


        //}
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var category = _unitOfWork.Category.Get(x=>x.Id==id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(category);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Category.Update(category);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Category Updated Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}
        #endregion
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM vm = new()
            {
                Product = new(),
                Categories=_unitOfWork.Category.GetAll().Select(x=>new SelectListItem()
                {
                    Text=x.Name,
                    Value=x.Id.ToString()
                })

            };

            if (id == null || id == 0)
            {                
                return View(vm);
            }
            else
            {
                vm.Product = _unitOfWork.Product.Get(x => x.Id == id);
                if (vm.Product == null)
                {
                    return NotFound();
                }
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//cross site scripting se prevent krne ke liye
        public IActionResult CreateUpdate(ProductVM vm, IFormFile file)
        {
            if (ModelState.IsValid)//server side validation ke liye 
            {
                string fileName = null;
                if (file != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "ProductImage");
                     fileName = Guid.NewGuid().ToString() + "-" +file.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    if (vm.Product.Id != 0)
                    {
                        var oldImgInDb2 = Path.Combine(_webHostEnvironment.WebRootPath,vm.Product.ImageUrl.TrimStart('\\'));
                        if (oldImgInDb2 != null)
                        {
                            if (System.IO.File.Exists(oldImgInDb2))
                            {
                                System.IO.File.Delete(oldImgInDb2);
                            }
                        }
                       /* var oldImgInDb = _unitOfWork.Product.Get(x => x.Id == vm.Product.Id).ImageUrl;
                        if (oldImgInDb != null)
                        {
                            var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImgInDb.Trim('\\'));
                            if (System.IO.File.Exists(imgPath))
                            {
                                System.IO.File.Delete(imgPath);
                            }
                        }*/
                    }

                    using(var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                    }
                    vm.Product.ImageUrl = @"\ProductImage\" + fileName;// important
                }
                else
                {
                    var productInDb = _unitOfWork.Product.Get(x => x.Id == vm.Product.Id);
                    vm.Product.ImageUrl = productInDb.ImageUrl;
                }
                if (vm.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(vm.Product);
                    TempData["success"] = "Product Created Successfully";
                }
                else
                {
                   
                    _unitOfWork.Product.Update(vm.Product);
                    TempData["success"] = "Product Updated Successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(vm);
        }
        #region MyRegion
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var Product = _unitOfWork.Product.Get(x => x.Id == id);
            if (Product == null)
            {
                return Json(new { success = false });
            }
            else
            {
                var oldImgInDb = Product.ImageUrl;
                if (oldImgInDb != null)
                {
                    var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImgInDb.Trim('\\'));
                    if (System.IO.File.Exists(imgPath))
                    {
                        System.IO.File.Delete(imgPath);
                    }
                }
                _unitOfWork.Product.Delete(Product);
                _unitOfWork.Save();
                return Json(new { success = true });
            }
        }       
        #endregion
    }
}
