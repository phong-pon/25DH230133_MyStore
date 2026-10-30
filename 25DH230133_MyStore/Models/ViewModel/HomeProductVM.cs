using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;

namespace _25DH230133_MyStore.Models.ViewModel
{
    public class HomeProductVM
    {
        //tiêu chí tìm kiếm 
        public string SearchTerm { get; set; }  

        //các thuộc tính hỗ trợ paging
        public int PageNumber { get; set; } // vị trí hiện tại
        public int PageSize { get; set; } = 10; // Số lượng sản phẩm hiển thị trên mỗi trang

        //danh sách sản phẩm nổi bật 
        public List<Product> FeaturedProducts { get; set; }

        // danh sách sản phẩm mới đã phân trang
        public PagedList.IPagedList<Product> NewProducts { get; set; } // Danh sách sản phẩm mới


    }
}