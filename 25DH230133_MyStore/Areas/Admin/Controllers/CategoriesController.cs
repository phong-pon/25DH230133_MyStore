using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _25DH230133_MyStore.Models;

namespace _25DH230133_MyStore.Areas.Admin.Controllers
{
    [_25DH230133_MyStore.Areas.Admin.AdminAuthorize]
    public class CategoriesController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            return View(db.Categories.OrderBy(c => c.CategoryName).ToList());
        }

        private Category FindCategory(int? id)
        {
            if (!id.HasValue) return null;
            return db.Categories.Find(id.Value);
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var category = FindCategory(id);
            return category == null ? (ActionResult)HttpNotFound() : View(category);
        }

        public ActionResult Create() { return View(new Category()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["Message"] = "Đã thêm danh mục.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var category = FindCategory(id);
            return category == null ? (ActionResult)HttpNotFound() : View(category);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Đã cập nhật danh mục.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var category = FindCategory(id);
            return category == null ? (ActionResult)HttpNotFound() : View(category);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();
            if (db.Products.Any(p => p.CategoryID == id))
            {
                TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm.";
                return RedirectToAction("Index");
            }
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["Message"] = "Đã xóa danh mục.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
