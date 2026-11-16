using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using _25DH230133_MyStore.Models;
using PagedList;
namespace _25DH230133_MyStore.Areas.Admin.Controllers
{
    [_25DH230133_MyStore.Areas.Admin.AdminAuthorize]
    public class UsersController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();
        public ActionResult Index(string searchTerm, int? page)
        {
            var q=db.Users.AsQueryable(); if(!string.IsNullOrWhiteSpace(searchTerm)) q=q.Where(u=>u.Username.Contains(searchTerm) || u.UserRole.Contains(searchTerm));
            ViewBag.SearchTerm=searchTerm; return View(q.OrderBy(u=>u.Username).ToPagedList(page??1,10));
        }
        public ActionResult Create(){return View(new User { UserRole="C" });}
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Username,Password,UserRole")] User user)
        {
            if(db.Users.Any(u=>u.Username==user.Username)) ModelState.AddModelError("Username","Tên đăng nhập đã tồn tại.");
            if(string.IsNullOrWhiteSpace(user.Password)) ModelState.AddModelError("Password","Mật khẩu không được để trống.");
            if(string.IsNullOrWhiteSpace(user.UserRole) || (user.UserRole!="A" && user.UserRole!="C")) ModelState.AddModelError("UserRole","Vai trò chỉ nhận A (Admin) hoặc C (Customer).");
            if(ModelState.IsValid){db.Users.Add(user);db.SaveChanges();return RedirectToAction("Index");} return View(user);
        }
        public ActionResult Edit(string id){if(string.IsNullOrWhiteSpace(id))return new HttpStatusCodeResult(400);var user=db.Users.Find(id);return user==null?(ActionResult)HttpNotFound():View(user);}
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Username,Password,UserRole")] User user)
        {
            var current=db.Users.Find(user.Username);if(current==null)return HttpNotFound();
            if(string.IsNullOrWhiteSpace(user.UserRole)||(user.UserRole!="A"&&user.UserRole!="C"))ModelState.AddModelError("UserRole","Vai trò chỉ nhận A hoặc C.");
            if(ModelState.IsValid){current.UserRole=user.UserRole;if(!string.IsNullOrWhiteSpace(user.Password))current.Password=user.Password;db.SaveChanges();return RedirectToAction("Index");} return View(user);
        }
        public ActionResult Delete(string id){if(string.IsNullOrWhiteSpace(id))return new HttpStatusCodeResult(400);var user=db.Users.Find(id);return user==null?(ActionResult)HttpNotFound():View(user);}
        [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user=db.Users.Include(u=>u.Customer).FirstOrDefault(u=>u.Username==id);if(user==null)return HttpNotFound();
            if(user.Customer.Any()){TempData["Error"]="Tài khoản đang liên kết với hồ sơ khách hàng, không thể xóa trực tiếp.";return RedirectToAction("Index");}
            db.Users.Remove(user);db.SaveChanges();return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing){if(disposing)db.Dispose();base.Dispose(disposing);}
    }
}
