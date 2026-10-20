using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;

namespace _25DH230133_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        //tiêu chí để search theo tên , mô tả,loại sp
        public string SearchTerm { get; set; }

        //các tiêu chí để search theo giá 
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // thứ tự sắp xếp
        public string SortOrder { get; set; }

        //các thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } //Trang hiện tại
        public int PageSize { get; set; } = 10;//Số lượng sản phẩm mỗi trang

        //danh sách sản phẩm đã phân trang
        public PagedList.IPagedList<Product> Products { get; set; }

        // danh sách sản phẩm thỏa điều kiện tìm kiếm
        //public List<Product> Products { get; set; } 
    }
}