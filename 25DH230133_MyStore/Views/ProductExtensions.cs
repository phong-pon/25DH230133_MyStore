using System;
using System.Web;

namespace _25DH230133_MyStore.Models
{
    // Bổ sung thuộc tính chỉ-đọc (không ánh xạ DB) để luôn có đường dẫn ảnh hợp lệ
    public partial class Product
    {
        public string ImageUrl
        {
            get
            {
                var img = ProductImage;
                if (string.IsNullOrWhiteSpace(img)) return VirtualPathUtility.ToAbsolute("~/Content/images/default_img.png");
                img = img.Trim();
                if (img.StartsWith("http", StringComparison.OrdinalIgnoreCase) || img.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) return img;
                if (img.StartsWith("~")) return VirtualPathUtility.ToAbsolute(img);
                if (img.StartsWith("/")) return img;
                return VirtualPathUtility.ToAbsolute("~/Content/images/" + img);
            }
        }
    }
}
