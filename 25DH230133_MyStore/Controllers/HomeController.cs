using _25DH230133_MyStore.Models;
using _25DH230133_MyStore.Models.ViewModel;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace _25DH230133_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index (string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Product.AsQueryable();
            // tìm kiếm sản phẩm dựa trên từ khóa 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm)||
                                        p.ProductDescription.Contains(searchTerm)||
                                        p.Category.CategoryName.Contains(searchTerm));
            }
            //đoạn code liên quan đến phân trang
            //lấy số trang hiện tại mặc định là 1 nếu không có giá trị truyền vào
            int pageNumber = page ?? 1;
            int pageSize = 6; //số sản phẩm mỗi trang 

            //lấy top 10 sản phẩm bán chạy nhất 
            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetail.Count()).Take(10).ToList();

            // lấy 20 sản phẩm bán ế nhất và phân trang 

            model.NewProducts = products.OrderBy(p => p.OrderDetail.Count()).Take(20).ToPagedList(pageNumber, pageSize);

            return View(model);

        }
        // GET: Home/ProductDetails/5
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product pro = db.Product.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            // lấy tất cả các sản phẩm cùng danh mục
            var products = db.Product.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();

            // Đoạn code liên quan tới phân trang
            // Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;  // Số sản phẩm mỗi trang
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize);
            model.TopProducts = products.OrderByDescending(p => p.OrderDetail.Count()).Take(8).ToPagedList(pageNumber, pageSize);

            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }

            return View(model);
        }
    }
}