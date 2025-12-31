using System.Web;
using System.Web.Mvc;

namespace Project_65130650
{
    public class FilterConfig
    {
        /**
         * Đăng ký các bộ lọc toàn cục (Global Filters) được áp dụng cho mọi Action và Controller
         */
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Bộ lọc xử lý lỗi: Tự động chuyển hướng đến trang lỗi khi có ngoại lệ xảy ra trong ứng dụng
            filters.Add(new HandleErrorAttribute());
        }
    }
}
