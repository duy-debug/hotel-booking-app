using System.Web;
using System.Web.Optimization;

namespace Project_65130650
{
    public class BundleConfig
    {
        /**
         * Để biết thêm thông tin về Bundling (đóng gói), vui lòng truy cập https://go.microsoft.com/fwlink/?LinkId=301862
         */
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Đóng gói thư viện jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Đóng gói thư viện Validation (Kiểm tra dữ liệu đầu vào) của jQuery
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            /**
             * Sử dụng phiên bản phát triển của Modernizr để phát triển và học hỏi. Sau đó, khi bạn
             * đã sẵn sàng cho sản xuất, hãy sử dụng công cụ xây dựng tại https://modernizr.com để chọn các bài kiểm tra cần thiết.
             */
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Đóng gói các tệp JavaScript của thư viện Bootstrap
            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            // Đóng gói các tệp CSS (Giao diện) bao gồm Bootstrap và CSS tùy chỉnh của dự án
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
