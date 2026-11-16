using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using _25DH230133_MyStore.Models;
using _25DH230133_MyStore.Models.ViewModel;

namespace _25DH230133_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private Cart GetCart() { return new CartService(Session).GetCart(); }

        // GET: Order
        public ActionResult Index()
        {
            return RedirectToAction("OrderHistory");
        }

        // GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            //Kiểm tra giỏ hàng trong session,
            //nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ
            var cart = GetCart().Items.ToList();
            if (!cart.Any()) {
                return RedirectToAction("Index", "Home");
            }
            //Xác thực người dùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới trang Đăng nhập
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) { return RedirectToAction("Login", "Account"); }
            //Lấy thông tin khách hàng từ CSDL, nếu chưa có thì chuyển hướng tới trang Đăng nhập
            //nếu có rồi thì lấy địa chỉ của khách hàng và gán vào ShippingAddress của CheckoutVM
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null) { return RedirectToAction("Login", "Account"); }

            var model = new CheckoutVM { //Tạo dữ liệu hiển thị cho CheckoutVM
                CartItems = cart, //Lấy danh sách các sản phẩm trong giỏ hàng
                TotalAmount = cart.Sum(item => item.TotalPrice), //Tổng giá trị của các mặt hàng trong giỏ
                OrderDate = DateTime.Now, //Mặc định lấy bằng thời điểm đặt hàng
                ShippingAddress = customer.CustomerAddress, // Lấy địa chỉ mặc định từ bảng Customer
                CustomerID = customer.CustomerID, // Lấy mã khách hàng từ bảng Customer
                Username = customer.Username, // Lấy tên đăng nhập từ bảng Customer
                ShippingMethod = "Giao hàng nhanh",
                PaymentMethod = "Tiền mặt"
            };

            return View(model);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            var cart = GetCart().Items.ToList();
            //Nếu giỏ hàng rỗng sẽ điều hướng tới trang Home
            if (!cart.Any()) { return RedirectToAction("Index", "Home"); }
            //Nếu người dùng chưa đăng nhập sẽ điều hướng tới trang Login
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) { return RedirectToAction("Login", "Account"); }
            //nếu khách hàng không khớp với tên đăng nhập, sẽ điều hướng tới trang Login
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null) { return RedirectToAction("Login", "Account"); }

            if (ModelState.IsValid) {
                // Thiết lập PaymentStatus dựa trên PaymentMethod
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod) {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": paymentStatus = "Thanh toán paypal"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chưa thanh toán"; break;
                }
                //Tạo đơn hàng và chi tiết đơn hàng liên quan (tổng tiền tính lại từ giỏ hàng ở server)
                var order = new Order {
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Sum(item => item.TotalPrice),
                    PaymentStatus = paymentStatus,
                    PaymentMethod = model.PaymentMethod,
                    DeliveryMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    OrderDetail = cart.Select(item => new OrderDetail {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    }).ToList()
                };
                //Lưu đơn hàng vào CSDL
                db.Orders.Add(order);
                db.SaveChanges();
                // Xóa giỏ hàng sau khi đặt hàng thành công
                Session["Cart"] = null;
                // Điều hướng tới trang Xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            model.CartItems = cart;
            model.TotalAmount = cart.Sum(item => item.TotalPrice);
            model.OrderDate = DateTime.Now;
            model.CustomerID = customer.CustomerID;
            model.Username = customer.Username;
            return View(model);
        }

        // GET: Order/OrderSuccess/5
        [Authorize]
        public ActionResult OrderSuccess(int? id)
        {
            var order = db.Orders.Include(o => o.OrderDetail.Select(d => d.Product))
                .FirstOrDefault(o => o.OrderID == id && o.Customer.Username == User.Identity.Name);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        // GET: Order/OrderHistory  (Lịch sử mua hàng)
        [Authorize]
        public ActionResult OrderHistory(string searchTerm, string status)
        {
            var customer = db.Customers.FirstOrDefault(c => c.Username == User.Identity.Name);
            if (customer == null) return View(new List<Order>());
            var q = db.Orders.Include(o => o.OrderDetail.Select(d => d.Product)).Where(o => o.CustomerID == customer.CustomerID);
            if (!String.IsNullOrWhiteSpace(status) && status != "all")
                q = q.Where(o => o.PaymentStatus == status);
            if (!String.IsNullOrWhiteSpace(searchTerm))
            {
                int orderId;
                if (int.TryParse(searchTerm.TrimStart('#'), out orderId))
                    q = q.Where(o => o.OrderID == orderId || o.OrderDetail.Any(d => d.Product.ProductName.Contains(searchTerm)));
                else
                    q = q.Where(o => o.OrderDetail.Any(d => d.Product.ProductName.Contains(searchTerm)));
            }
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status ?? "all";
            return View("OrderHistory", q.OrderByDescending(o => o.OrderDate).ToList());
        }

        public ActionResult MyOrder(string searchTerm, string status) { return OrderHistory(searchTerm, status); }

        // GET: Order/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var order = db.Orders.Include(o => o.OrderDetail.Select(d => d.Product))
                .FirstOrDefault(o => o.OrderID == id && o.Customer.Username == User.Identity.Name);
            if (order == null) return HttpNotFound();
            return View("OrderSuccess", order);
        }

        // POST: Order/Reorder/5  (Mua lại)
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Reorder(int id)
        {
            var order = db.Orders.Include(o => o.OrderDetail.Select(d => d.Product.Category))
                .FirstOrDefault(o => o.OrderID == id && o.Customer.Username == User.Identity.Name);
            if (order == null) return HttpNotFound();
            var cart = GetCart();
            foreach (var detail in order.OrderDetail)
            {
                var product = detail.Product;
                if (product == null) continue;
                cart.AddItem(product.ProductID, product.ImageUrl, product.ProductName, product.ProductPrice,
                    detail.Quantity, product.Category == null ? "Khác" : product.Category.CategoryName);
            }
            return RedirectToAction("Index2", "Cart");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
