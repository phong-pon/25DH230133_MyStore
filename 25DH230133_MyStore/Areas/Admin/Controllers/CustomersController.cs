using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using _25DH230133_MyStore.Models;
using PagedList;
namespace _25DH230133_MyStore.Areas.Admin.Controllers
{
    [_25DH230133_MyStore.Areas.Admin.AdminAuthorize]
    public class CustomersController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();
        public ActionResult Index(string searchTerm, string sortOrder, int? page)
        {
            var q = db.Customers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm)) q = q.Where(c => c.CustomerName.Contains(searchTerm) || c.CustomerEmail.Contains(searchTerm) || c.CustomerPhone.Contains(searchTerm) || c.Username.Contains(searchTerm));
            q = sortOrder == "name_desc" ? q.OrderByDescending(c => c.CustomerName) : q.OrderBy(c => c.CustomerName);
            ViewBag.SearchTerm = searchTerm; ViewBag.SortOrder = sortOrder;
            return View(q.ToPagedList(page ?? 1, 10));
        }
        public ActionResult Details(int? id) { if (!id.HasValue) return new HttpStatusCodeResult(400); var c=db.Customers.Find(id); return c==null?(ActionResult)HttpNotFound():View(c); }
        public ActionResult Orders(int? id) { if (!id.HasValue) return new HttpStatusCodeResult(400); var c=db.Customers.Find(id); if(c==null)return HttpNotFound(); ViewBag.Customer=c; return View(db.Orders.Include(o=>o.OrderDetail).Where(o=>o.CustomerID==id.Value).OrderByDescending(o=>o.OrderDate).ToList()); }
        protected override void Dispose(bool disposing) { if(disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
