using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using _25DH230133_MyStore.Models;
using PagedList;
namespace _25DH230133_MyStore.Areas.Admin.Controllers
{
    [_25DH230133_MyStore.Areas.Admin.AdminAuthorize]
    public class OrdersController : Controller
    {
        private readonly MyStoreEntities db=new MyStoreEntities();
        public ActionResult Index(string searchTerm, int? page)
        {
            var q=db.Orders.Include(o=>o.Customer).AsQueryable(); int id;
            if(!string.IsNullOrWhiteSpace(searchTerm) && int.TryParse(searchTerm.TrimStart('#'),out id)) q=q.Where(o=>o.OrderID==id || o.Customer.CustomerName.Contains(searchTerm));
            else if(!string.IsNullOrWhiteSpace(searchTerm)) q=q.Where(o=>o.Customer.CustomerName.Contains(searchTerm));
            return View(q.OrderByDescending(o=>o.OrderDate).ToPagedList(page??1,10));
        }
        public ActionResult Details(int? id) { if(!id.HasValue)return new HttpStatusCodeResult(400); var o=db.Orders.Include(x=>x.Customer).Include(x=>x.OrderDetail.Select(d=>d.Product)).FirstOrDefault(x=>x.OrderID==id.Value); return o==null?(ActionResult)HttpNotFound():View(o); }
        [HttpPost,ValidateAntiForgeryToken] public ActionResult UpdateStatus(int id,string status) { var o=db.Orders.Find(id); if(o==null)return HttpNotFound(); var allowed=new[]{"Chưa thanh toán","Đã thanh toán","Đang xử lý","Đang giao","Hoàn thành","Đã hủy","Chờ thanh toán PayPal","Thanh toán tiền mặt","Thanh toán paypal","Trả góp"}; if(allowed.Contains(status)) { o.PaymentStatus=status; db.SaveChanges(); } return RedirectToAction("Details",new{id}); }
        protected override void Dispose(bool disposing) { if(disposing)db.Dispose(); base.Dispose(disposing); }
    }
}
