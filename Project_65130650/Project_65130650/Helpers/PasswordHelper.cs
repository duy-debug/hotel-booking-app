using System;
using System.Security.Cryptography;
using System.Text;

namespace Project_65130650.Helpers
{
    /// <summary>
    /// Lớp hỗ trợ băm mật khẩu sử dụng thuật toán SHA256
    /// Mục đích: Bảo mật mật khẩu người dùng trước khi lưu vào database
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Băm mật khẩu sử dụng thuật toán SHA256
        /// </summary>
        /// <param name="password">Mật khẩu gốc dạng plain text</param>
        /// <returns>Chuỗi mật khẩu đã được băm dạng hex</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                // Chuyển mật khẩu thành mảng byte
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                
                // Tính toán hash
                byte[] hash = sha256.ComputeHash(bytes);
                
                // Chuyển đổi mảng byte thành chuỗi hex
                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2"));
                }
                
                return result.ToString();
            }
        }

        /// <summary>
        /// Kiểm tra mật khẩu plain text có khớp với mật khẩu đã băm không
        /// </summary>
        /// <param name="plainPassword">Mật khẩu gốc dạng plain text</param>
        /// <param name="hashedPassword">Mật khẩu đã được băm từ database</param>
        /// <returns>True nếu khớp, False nếu không khớp</returns>
        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(hashedPassword))
                return false;

            // Băm mật khẩu plain text và so sánh với mật khẩu đã băm
            string hashedInput = HashPassword(plainPassword);
            return string.Equals(hashedInput, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
