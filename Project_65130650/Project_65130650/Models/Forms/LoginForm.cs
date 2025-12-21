using System.ComponentModel.DataAnnotations;

namespace Project_65130650.Models.Forms
{
    /// <summary>
    /// Model đại diện cho form "Đăng nhập"
    /// Được dùng để nhận và kiểm tra tính hợp lệ của thông tin tài khoản
    /// </summary>
    public class LoginForm
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Email người dùng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}