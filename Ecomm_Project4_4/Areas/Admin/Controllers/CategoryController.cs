using Ecomm_Project4_4.Data;
using Ecomm_Project4_4.DataAccess.Repository.IRepository;
using Ecomm_Project4_4.Helper;
using Ecomm_Project4_4.Models;
using Ecomm_Project4_4.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =WebSiteRole.Role_Admin+","+WebSiteRole.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            CategoryVM vm = new CategoryVM();
           vm.Categories = _unitOfWork.Category.GetAll();
            return View(vm);
        }
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
            CategoryVM vm = new();
            if (id == null || id == 0)
            {                
                return View(vm);
            }
            else
            {
                vm.Category = _unitOfWork.Category.Get(x => x.Id == id);
                if (vm.Category == null)
                {
                    return NotFound();
                }
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//cross site scripting se prevent krne ke liye
        public IActionResult CreateUpdate(CategoryVM vm)
        {
            if (ModelState.IsValid)//server side validation ke liye 
            {
                if (vm.Category.Id == 0)
                {
                    _unitOfWork.Category.Add(vm.Category);
                    TempData["success"] = "Category Created Successfully";
                }
                else
                {
                    _unitOfWork.Category.Update(vm.Category);
                    TempData["success"] = "Category Updated Successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(vm);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.Category.Get(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var category = _unitOfWork.Category.Get(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }

    }
}
