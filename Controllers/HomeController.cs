using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetSample.Models;
using AspNetSample.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace AspNetSample.Controllers
{

    public class HomeController : Controller
    {
          
       private IEmployeeRepository _emprepo;
        private readonly IWebHostEnvironment hostingEnvironment;
        public HomeController(IEmployeeRepository emprepo,
                                  IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            _emprepo = emprepo;
        }
        [Route("")]
        public ViewResult Index()
        {
            ViewBag.Title = "Index";
            return View(_emprepo.GetAllEmployee());
        }
        public ViewResult Details(int id)
        {
            
            // throw new Exception("Error in Details View");
            Employee employee = _emprepo.GetEmployee(id);

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

            ViewBag.Title = "Details";
            HomeDetailsViewModel emp = new HomeDetailsViewModel(){
                Employee = employee,
                PageTitle = "DetailsPage"
            };
            
            return View(emp);
            
        }
        [Authorize]
        public IActionResult DeleteEmployee(int id){
            Employee employee = _emprepo.GetEmployee(id);

            if (employee != null)
            {
                _emprepo.Delete(id);
            }
            return RedirectToAction("index");
        }
        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
        
        [HttpGet]
        [Authorize]
        public ViewResult Create(){
            ViewBag.Title = "Create";
            return View();

        }
        
        /*
        public IActionResult Create(Employee employee)
        {
            ViewBag.Title = "Create";
            employee.Email = employee.Email.Trim();
            ModelState.Clear();
            TryValidateModel(employee);
            if (ModelState.IsValid)
            {
            Employee newEmployee = _emprepo.Add(employee);
            return RedirectToAction("details", new { id = newEmployee.Id });
            }
           
            return View();
        }  */
        [HttpPost]
        [Authorize]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // If the Photo property on the incoming model object is not null, then the user
                // has selected an image to upload.
                if (model.Photo != null)
                {
                    uniqueFileName = ProcessUploadedFile(model);
                }

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    // Store the file name in PhotoPath property of the employee object
                    // which gets saved to the Employees database table
                    PhotoPath = uniqueFileName
                };

                _emprepo.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        
        [HttpGet]
        [Authorize]
        public ViewResult Edit(int id)
        {
            Employee employee = _emprepo.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }
        
    // Through model binding, the action method parameter
    // EmployeeEditViewModel receives the posted edit form data
    [HttpPost]
    [Authorize]
    public IActionResult Edit(EmployeeEditViewModel model)
    {
        // Check if the provided data is valid, if not rerender the edit view
        // so the user can correct and resubmit the edit form
        if (ModelState.IsValid)
        {
            // Retrieve the employee being edited from the database
            Employee employee = _emprepo.GetEmployee(model.Id);
            // Update the employee object with the data in the model object
            employee.Name = model.Name;
            employee.Email = model.Email;
            employee.Department = model.Department;

            // If the user wants to change the photo, a new photo will be
            // uploaded and the Photo property on the model object receives
            // the uploaded photo. If the Photo property is null, user did
            // not upload a new photo and keeps his existing photo
            if (model.Photo != null)
            {
                // If a new photo is uploaded, the existing photo must be
                // deleted. So check if there is an existing photo and delete
                if (model.ExistingPhotoPath != null)
                {
                    string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                        "images", model.ExistingPhotoPath);
                    System.IO.File.Delete(filePath);
                }
                // Save the new photo in wwwroot/images folder and update
                // PhotoPath property of the employee object which will be
                // eventually saved in the database
                employee.PhotoPath = ProcessUploadedFile(model);
            }

            // Call update method on the repository service passing it the
            // employee object to update the data in the database table
            Employee updatedEmployee = _emprepo.Update(employee);

            return RedirectToAction("index");
        }

        return View(model);
    }

    }
}
    
