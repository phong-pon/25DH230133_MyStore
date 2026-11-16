using System.Linq;
using System.Web.Mvc;
using _25DH230133_MyStore.Models;

namespace _25DH230133_MyStore.Areas.Admin.Controllers
{
    [_25DH230133_MyStore.Areas.Admin.AdminAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new MyStoreEntities())
            {
                ViewBag.CategoryCount = db.Categories.Count();
                ViewBag.ProductCount = db.Products.Count();
                ViewBag.CustomerCount = db.Customers.Count();
                ViewBag.OrderCount = db.Orders.Count();
                ViewBag.Revenue = db.Orders.Where(o => o.PaymentStatus == "Đã thanh toán" || o.PaymentStatus == "Hoàn thành")
                    .Select(o => (decimal?)o.TotalAmount).Sum() ?? 0m;
            }
            return View();
        }
    }
}
