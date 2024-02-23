using project.Models;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System;

namespace project.Controllers
{
    public class CustomerController : Controller
    {
        libraryprojectEntities db = new libraryprojectEntities();
  
        public ActionResult Create(int id = 0)
        {
            Customer c1 = new Customer();
            return View(c1);
        }

       
        [HttpPost]
        public ActionResult Create(Customer c1)
        {
            var result = db.Customer.FirstOrDefault(test => test.UserName == c1.UserName || test.Email == c1.Email);
            if (result != null)
            {
                ViewBag.Message = "Username/Email already exists";
            }
            else
            {
                
                string hashedPassword = HashPasswordSHA256(c1.Password);
                c1.Password = hashedPassword;

                db.Customer.Add(c1);
                db.SaveChanges();
                Session["UserName"] = c1.UserName;
                return RedirectToAction("BookView", "Borrow");
            }
            return View();
        }


        public ActionResult Login(int id = 0)
        {
            Customer c1 = new Customer();
            return View(c1);
        }
      

        [HttpPost]
        public ActionResult Login(Customer c1)
        {
            using (libraryprojectEntities db = new libraryprojectEntities())
            {
                var customer = db.Customer.FirstOrDefault(c => c.UserName == c1.UserName);

                if (customer != null && VerifyPasswordSHA256(c1.Password.Trim(), customer.Password))
                {
                    if (customer.Admin.HasValue && customer.Admin.Value)
                    {
                        Session["Admin"] = "Admin";
                    }

                    Session["Password"] = c1.Password;
                    Session["UserName"] = c1.UserName;

                    return RedirectToAction("BookView", "Borrow");
                }
                else
                {
                    ViewBag.Message = "Username/Password Not Found";
                    return View();
                }
            }
        }

        private string HashPasswordSHA256(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedPasswordBytes);
            }
        }

        private bool VerifyPasswordSHA256(string enteredPassword, string hashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
                byte[] enteredPasswordHash = sha256.ComputeHash(enteredPasswordBytes);

                byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

                return StructuralComparisons.StructuralEqualityComparer.Equals(enteredPasswordHash, hashedPasswordBytes);
            }
        }


        public ActionResult Logout()
        {
            Session.Clear(); 
            Session.Abandon();
            Session["UserName"] = null;
            return RedirectToAction("Login");
        }


    }
}
