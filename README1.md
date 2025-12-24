# Hệ Thống Quản Lý Đặt Phòng Khách Sạn

Một ứng dụng web toàn diện được thiết kế để tối ưu hóa quy trình vận hành khách sạn và mang lại trải nghiệm đặt phòng thuận tiện cho khách hàng.

## Các Chức Năng Chính

### Dành cho Quản trị viên (Admin)
- **Quản lý Người dùng:** Quản lý tài khoản Admin, nhân viên và khách hàng.
- **Quản lý Phòng & Loại phòng:** Thiết lập danh mục phòng, loại phòng và trạng thái phòng.
- **Quản lý Dịch vụ:** Quản lý các dịch vụ đi kèm và đơn giá tương ứng.
- **Vận hành Đặt phòng:** Tiếp nhận đặt phòng tại quầy, quản lý check-in/check-out và trạng thái đơn đặt.
- **Thanh toán & Hóa đơn:** Xử lý thanh toán (Tiền mặt/Chuyển khoản) và lưu trữ lịch sử giao dịch.
- **Báo cáo & Thống kê:** Theo dõi doanh thu, công suất phòng và hiệu quả kinh doanh qua biểu đồ.

### Dành cho Khách hàng
- **Đặt phòng Trực tuyến:** Tìm kiếm, lọc và đặt phòng nhanh chóng.
- **Quản lý Cá nhân:** Cập nhật thông tin hồ sơ, xem lịch sử đặt phòng và dịch vụ đã dùng.
- **Yêu cầu Dịch vụ:** Xem và đăng ký các dịch vụ bổ sung cho đơn đặt phòng.
- **Hóa đơn:** Xem lịch sử thanh toán, in hoặc tải hóa đơn dưới dạng PDF.

## Công Nghệ Sử Dụng
- **Backend:** C# (.NET Framework), ASP.NET MVC
- **Truy xuất dữ liệu:** Entity Framework
- **Cơ sở dữ liệu:** Microsoft SQL Server
- **Frontend:** HTML5, CSS3, JavaScript/jQuery

## Hướng Dẫn Cài Đặt
1. **Thiết lập Cơ sở dữ liệu:** 
   - Chạy file script `Project_65130650.sql` trong SQL Server.
   - Cập nhật chuỗi kết nối (connection string) trong file `Web.config`.
2. **Chạy Dự án:**
   - Mở file `Project_65130650.sln` bằng Visual Studio.
   - Khôi phục (Restore) các gói NuGet.
   - Nhấn `F5` để khởi chạy ứng dụng.