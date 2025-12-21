using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project_65130650.Helpers
{
    /// <summary>
    /// Lớp hỗ trợ các thao tác liên quan đến Email (Gửi mã xác nhận, thông báo...)
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// Gửi email bất đồng bộ bằng máy chủ SMTP (Gmail)
        /// </summary>
        /// <param name="toEmail">Địa chỉ email người nhận</param>
        /// <param name="subject">Tiêu đề email</param>
        /// <param name="body">Nội dung email (hỗ trợ định dạng HTML)</param>
        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Bước 1: Đọc cấu hình từ file Web.config
                var host = ConfigurationManager.AppSettings["EmailHost"];
                var port = int.Parse(ConfigurationManager.AppSettings["EmailPort"]);
                var userName = ConfigurationManager.AppSettings["EmailUserName"];
                var password = ConfigurationManager.AppSettings["EmailPassword"]; // Mã mật khẩu ứng dụng 16 ký tự
                var fromEmail = ConfigurationManager.AppSettings["EmailFrom"];

                // Bước 2: Thiết lập máy chủ SMTP
                var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(userName, password),
                    EnableSsl = true // Luôn sử dụng SSL để đảm bảo bảo mật
                };

                // Bước 3: Soạn tin nhắn
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Sheraton Nha Trang Hotel"), // Tên hiển thị người gửi
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Cho phép sử dụng thẻ HTML trong nội dung
                };

                // Thêm người nhận
                mailMessage.To.Add(toEmail);

                // Bước 4: Thực hiện gửi mail bất đồng bộ (giúp website không bị treo khi giữ kết nối với SMTP)
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Ném ngoại lệ để tầng Controller có thể bắt và hiển thị thông báo lỗi
                throw new Exception("Lỗi hệ thống gửi mail: " + ex.Message);
            }
        }
    }
}
