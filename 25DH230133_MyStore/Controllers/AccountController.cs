using _25DH230133_MyStore.Models;
using _25DH230133_MyStore.Models.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace _25DH230133_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private string CurrentUsername()
        {
            if (User != null && User.Identity.IsAuthenticated) return User.Identity.Name;
            return Session["Username"] as string;
        }

        // GET: Account/Register
        public ActionResult Register() { return View(new RegisterVM()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);
            using (var db = new MyStoreEntities())
            {
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                var user = new User { Username = model.Username.Trim(), Password = model.Password, UserRole = "C" };
                db.Users.Add(user);
                db.Customers.Add(new Customer
                {
                    Username = user.Username,
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    CustomerEmail = model.CustomerEmail,
                    CustomerAddress = model.CustomerAddress
                });
                db.SaveChanges();
            }
            TempData["Message"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // GET: Account/Login
        public ActionResult Login(string returnUrl) { return View(new LoginVM { ReturnUrl = returnUrl }); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);
            using (var db = new MyStoreEntities())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                    return View(model);
                }
                Session["Username"] = user.Username;
                Session["UserRole"] = user.UserRole;
                FormsAuthentication.SetAuthCookie(user.Username, model.RememberMe);
                if (user.UserRole != null && user.UserRole.Trim() == "A")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Remove("Username");
            Session.Remove("UserRole");
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public ActionResult Profile()
        {
            var username = CurrentUsername();
            if (string.IsNullOrWhiteSpace(username)) return RedirectToAction("Login");
            using (var db = new MyStoreEntities())
            {
                var customer = db.Customers.FirstOrDefault(c => c.Username == username);
                if (customer == null) return HttpNotFound();
                return View("Profile", customer);
            }
        }

        public ActionResult ProfileInfo() { return Profile(); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(string CustomerName, string CustomerPhone, string CustomerEmail, string CustomerAddress)
        {
            var username = CurrentUsername();
            if (string.IsNullOrWhiteSpace(username)) return RedirectToAction("Login");
            if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(CustomerPhone))
            {
                TempData["Error"] = "Họ tên và số điện thoại không được để trống.";
                return RedirectToAction("Profile");
            }
            using (var db = new MyStoreEntities())
            {
                var c = db.Customers.FirstOrDefault(x => x.Username == username);
                if (c == null) return HttpNotFound();
                c.CustomerName = CustomerName; c.CustomerPhone = CustomerPhone;
                c.CustomerEmail = CustomerEmail; c.CustomerAddress = CustomerAddress;
                db.SaveChanges();
            }
            TempData["Message"] = "Đã cập nhật thông tin.";
            return RedirectToAction("Profile");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var username = CurrentUsername();
            if (string.IsNullOrWhiteSpace(username)) return RedirectToAction("Login");
            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6 || NewPassword != ConfirmPassword)
            {
                TempData["Error"] = "Mật khẩu mới cần ít nhất 6 ký tự và hai ô phải khớp.";
                return RedirectToAction("Profile");
            }
            using (var db = new MyStoreEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.Username == username);
                if (user == null || user.Password.Trim() != CurrentPassword)
                {
                    TempData["Error"] = "Mật khẩu hiện tại không đúng.";
                    return RedirectToAction("Profile");
                }
                user.Password = NewPassword;
                db.SaveChanges();
            }
            TempData["Message"] = "Đã đổi mật khẩu.";
            return RedirectToAction("Profile");
        }
    }
}