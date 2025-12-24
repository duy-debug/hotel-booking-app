using System.ComponentModel.DataAnnotations;

namespace Project_65130650.Models.Forms
{
    /// <summary>
    /// Model đại diện cho form "Quên mật khẩu"
    /// Được dùng để nhận và validate Email người dùng muốn khôi phục
    /// </summary>
    public class ForgotPasswordForm65130650
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ (VD: example@gmail.com)")]
        [Display(Name = "Email đã đăng ký")]
        public string Email { get; set; }
    }

    /// <summary>
    /// Model đại diện cho form "Đặt lại mật khẩu"
    /// Chứa các thông tin cần thiết để xác thực mã và đổi mật khẩu mới
    /// </summary>
    public class ResetPasswordForm65130650
    {
        // Email của người dùng (thường được truyền ẩn từ bước trước)
        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã xác nhận 6 chữ số")]
        [Display(Name = "Mã xác nhận")]
        public string VerificationCode { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "{0} phải có từ {2} đến {1} ký tự.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Mật khẩu bảo mật phải có: ít nhất 8 ký tự, 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không trùng khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
