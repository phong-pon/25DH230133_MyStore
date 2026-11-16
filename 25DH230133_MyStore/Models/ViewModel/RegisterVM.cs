using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _25DH230133_MyStore.Models.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập"), StringLength(50), Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu"), StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự"), DataType(DataType.Password), Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu"), DataType(DataType.Password), Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp"), Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ tên"), StringLength(100), Display(Name = "Họ tên")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại"), StringLength(15), Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ"), Display(Name = "Email")]
        public string CustomerEmail { get; set; }
        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }
    }
}