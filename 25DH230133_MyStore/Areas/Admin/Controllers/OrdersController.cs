
using System;
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
        private readonly MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(
            string searchTerm,
            string status,
            DateTime? fromDate,
            DateTime? toDate,
            int? page)
        {
            var q = db.Orders
                .Include(o => o.Customer)
                .AsQueryable();

            // Tìm theo mã đơn hàng hoặc tên khách hàng
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string keyword = searchTerm.Trim();
                int id;

                if (int.TryParse(keyword.TrimStart('#'), out id))
                {
                    q = q.Where(o =>
                        o.OrderID == id ||
                        o.Customer.CustomerName.Contains(keyword));
                }
                else
                {
                    q = q.Where(o =>
                        o.Customer.CustomerName.Contains(keyword));
                }
            }

            // Lọc theo trạng thái thanh toán
            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                q = q.Where(o => o.PaymentStatus == status);
            }

            // Lọc từ ngày
            if (fromDate.HasValue)
            {
                DateTime startDate = fromDate.Value.Date;
                q = q.Where(o => o.OrderDate >= startDate);
            }

            // Lọc đến ngày, bao gồm toàn bộ ngày được chọn
            if (toDate.HasValue)
            {
                DateTime endDate = toDate.Value.Date.AddDays(1);
                q = q.Where(o => o.OrderDate < endDate);
            }

            // Giữ lại dữ liệu tìm kiếm khi chuyển trang
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status ?? "all";
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(
                q.OrderByDescending(o => o.OrderDate)
                 .ToPagedList(page ?? 1, 15)
            );
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(400);

            var o = db.Orders
                .Include(x => x.Customer)
                .Include(x => x.OrderDetail.Select(d => d.Product))
                .FirstOrDefault(x => x.OrderID == id.Value);

            return o == null
                ? (ActionResult)HttpNotFound()
                : View(o);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string status)
        {
            var o = db.Orders.Find(id);

            if (o == null)
                return HttpNotFound();

            var allowed = new[]
            {
                "Chưa thanh toán",
                "Đã thanh toán",
                "Đang xử lý",
                "Đang giao",
                "Hoàn thành",
                "Đã hủy",
                "Chờ thanh toán PayPal",
                "Thanh toán tiền mặt",
                "Thanh toán paypal",
                "Trả góp"
            };

            if (allowed.Contains(status))
            {
                o.PaymentStatus = status;
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}

