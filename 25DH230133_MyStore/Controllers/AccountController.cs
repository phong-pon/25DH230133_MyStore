using _25DH230133_MyStore.Models;
using _25DH230133_MyStore.Models.ViewModel;
using System;
using UserEntity = _25DH230133_MyStore.Models.User;
using CustomerEntity = _25DH230133_MyStore.Models.Customer;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace _25DH230133_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }
        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            { 
                //kiểm tra xem tên đăng nhập đã ton tại chưa
                var existingUser = db.User.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập nay đa ton tai!");
                    return View(model);
                }
                //nếu chưa ton tại thì tạo bản ghi thông tin tài khoản trong bảng User
                var user = new UserEntity
                {
                    Username = model.Username,
                    Password = model.Password, // Lưu y: Nen ma hoa mat khau truoc khi lưu
                    UserRole = "Customer"
                };
                db.User.Add(user);
                // và tạo bản ghi thông tin khách hàng trong bang Customer
                var customer = new CustomerEntity
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username,
                };
                db.Customer.Add(customer);
                //lưu thong tin tai khoan va thong tin khach hang vao CSDL
                //db.SaveChanges();
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var entityErrors in ex.EntityValidationErrors)
                    {
                        foreach (var error in entityErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Entity: " + entityErrors.Entry.Entity.GetType().Name
                            );

                            System.Diagnostics.Debug.WriteLine(
                                "Property: " + error.PropertyName
                            );

                            System.Diagnostics.Debug.WriteLine(
                                "Error: " + error.ErrorMessage
                            );
                        }
                    }

                    throw;
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.User.SingleOrDefault(u => u.Username == model.Username
                && u.Password == model.Password
                && u.UserRole == "Customer");
                if (user != null)
                {
                    // Lưu trạng thai đang nhập vào session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    //lưu thong tin xac thuc nguoi dung vao cookie
                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }


            }
            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}