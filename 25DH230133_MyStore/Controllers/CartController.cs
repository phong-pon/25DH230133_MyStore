using _25DH230133_MyStore.Models;
using _25DH230133_MyStore.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace _25DH230133_MyStore.Controllers
{
    public class CartController : Controller
    {
        //private readonly ApplicationDbContext db = new ApplicationDbContext();
        private MyStoreEntities db = new MyStoreEntities();

        // Hàm Lẩy dịch vụ giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // Hien thị gio hàng không gom nhom theo danh mục
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }

        // Hiển thị giỏ hàng đã gom nhóm sản phẩm theo danh mục
        public ActionResult Index2(int? page)
        {
            var cart = GetCartService().GetCart();
            var categories = cart.Items.Select(ci => ci.Category).Distinct().ToList();
            var inCartIds = cart.Items.Select(ci => ci.ProductID).ToList();

            // sản phẩm cùng danh mục với các sản phẩm trong giỏ và chưa có trong giỏ
            var similarProducts = db.Products
                .Where(p => categories.Contains(p.Category.CategoryName) && !inCartIds.Contains(p.ProductID))
                .OrderBy(p => p.ProductID);

            // Đoạn code liên quan tới phân trang
            // Lấy số trang hiện tại (mặc định là 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = cart.PageSize;  // Số sản phẩm mỗi trang
            cart.PageNumber = pageNumber;

            cart.SimilarProducts = similarProducts.ToPagedList(pageNumber, pageSize);
            return View("Index2", cart);
        }

        // Thêm san phẩm vao gio
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cartService = GetCartService();
                cartService.GetCart().AddItem(product.ProductID, product.ProductImage,
                product.ProductName, product.ProductPrice, quantity, product.Category.CategoryName);
            }
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveFromCart(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        // Làm trống giỏ hàng
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }

    }
}