using project.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace project.Controllers
{
    public class AdminController : Controller
    {
        libraryprojectEntities db = new libraryprojectEntities();
        // GET: Admin
        public ActionResult Index()
        {
            string d = Convert.ToString(Session["UserName"]);
            ViewBag.Message = d + " ,Welcome Back Admin";
            var result = db.Customer.ToList();

            return View(result);
        }
        public ActionResult Create(int id = 0)
        {
            Customer c1 = new Customer();
            return View(c1);
        }

        // POST: Customer/Create
        [HttpPost]
        public ActionResult Create(Customer c1)
        {
            var result = db.Customer.FirstOrDefault(test => test.UserName == c1.UserName);

            if (result != null)
            {
                ViewBag.Message = "Username already exists";
            }
            else
            {
                // Hash the password before saving it to the database
                c1.Password = HashPasswordSHA256(c1.Password);

                db.Customer.Add(c1);
                db.SaveChanges();

                Session["UserName"] = c1.UserName;
                Session["Password"] = c1.Password;  // It's generally not recommended to store the password in session

                return RedirectToAction("Index");
            }

            return View();
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

        public ActionResult Edit(int id)
        {
            var result = db.Customer.Where(test => test.Customer_ID == id).FirstOrDefault();
            return View(result);
        }

        // POST: Book/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, [Bind(Exclude = "Password")] Customer b1)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCustomer = db.Customer.Find(id);

                    if (existingCustomer != null)
                    {
                        // Update only the fields you want to modify
                        existingCustomer.Customer_Name = b1.Customer_Name;
                        existingCustomer.Gender = b1.Gender;
                        existingCustomer.Email = b1.Email;
                        existingCustomer.UserName = b1.UserName;
                        existingCustomer.Admin = b1.Admin;
                        // Do not update the password field

                        db.Entry(existingCustomer).State = EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }

                // If ModelState is not valid or customer is not found, return to the view with errors
                return View(b1);
            }
            catch
            {
                // Handle exceptions appropriately
                return View();
            }
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int id)
        {
            var result = db.Customer.Where(test => test.Customer_ID == id).FirstOrDefault();
            return View(result);
        }

        // POST: Book/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var result = db.Customer.Where(test => test.Customer_ID == id).FirstOrDefault();
                db.Customer.Remove(result);
                db.SaveChanges();
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Details(int id)
        {
            var result = db.Customer.Where(test => test.Customer_ID == id).FirstOrDefault();
            return View(result);
        }
        public ActionResult History(int id)
        {
            var result = db.Borrow.Where(test => test.Customer_ID == id).ToList();
            return View(result);
        }

    }
}