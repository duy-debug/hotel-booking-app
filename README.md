# HỆ THỐNG QUẢN LÝ ĐẶT PHÒNG KHÁCH SẠN
## 📌 Tổng Quan
Hệ thống quản lý đặt phòng khách sạn hỗ trợ hai hình thức đặt phòng:
- **Online**: Khách hàng tự đặt phòng qua website/app
- **Offline**: Admin tạo đặt phòng trực tiếp tại quầy lễ tân

Hệ thống có 2 vai trò chính:
- **Admin (Quản trị viên)**: Quản lý toàn bộ hệ thống
- **Khách hàng**: Đặt phòng và sử dụng dịch vụ

---

## 🗄️ Cấu Trúc Database

### Các bảng chính:
1. **NguoiDung** - Quản lý người dùng (Admin và Khách hàng)
2. **LoaiPhong** - Các loại phòng khách sạn
3. **Phong** - Phòng khách sạn
4. **DatPhong** - Quản lý đặt phòng
5. **DichVu** - Dịch vụ khách sạn
6. **DichVuDatPhong** - Dịch vụ được sử dụng trong đặt phòng
7. **ThanhToan** - Quản lý thanh toán

---

## 👤 CHỨC NĂNG CHI TIẾT THEO VAI TRÒ

# 🔷 1. ADMIN (QUẢN TRỊ VIÊN)

## A. Quản lý Người dùng
**Bảng: `NguoiDung`**

### Chức năng:
- ✅ Xem danh sách tất cả người dùng (Admin và Khách hàng)
- ✅ Tạo tài khoản mới cho Admin hoặc Khách hàng
- ✅ Chỉnh sửa thông tin người dùng:
  - Họ tên (`hoTen`)
  - Email (`email`)
  - Số điện thoại (`soDienThoai`)
  - Địa chỉ (`diaChi`)
  - Ngày sinh (`ngaySinh`)
  - Giới tính (`gioiTinh`: Nam/Nữ/Khác)
- ✅ Kích hoạt/Vô hiệu hóa tài khoản (`trangThaiHoatDong`)
- ✅ Đặt lại mật khẩu cho người dùng
- ✅ Phân quyền vai trò (`vaiTro`: Quản trị/Khách hàng)

### Mục đích:
- Quản lý tất cả người dùng trong hệ thống
- Đảm bảo an toàn tài khoản
- Phân quyền truy cập phù hợp

---

## B. Quản lý Loại phòng
**Bảng: `LoaiPhong`**

### Chức năng:
- ✅ Xem danh sách tất cả loại phòng (30 loại)
- ✅ Thêm loại phòng mới với đầy đủ thông tin:
  - Tên loại phòng (`tenLoaiPhong`): Standard, Superior, Deluxe, Suite, Apartment, Villa...
  - Mô tả (`moTa`)
  - Giá cơ bản (`giaCoBan`)
  - Số người tối đa (`soNguoiToiDa`)
  - Loại giường (`loaiGiuong`): Single Bed, Double Bed, King Bed...
  - Diện tích phòng (`dienTichPhong`) - đơn vị m²
  - Tiện nghi (`tienNghi`): WiFi, TV, Minibar, Điều hòa, Bồn tắm, Ban công...
  - Hình ảnh (`hinhAnh`)
- ✅ Cập nhật thông tin loại phòng (bao gồm điều chỉnh giá)
- ✅ Kích hoạt/Vô hiệu hóa loại phòng (`trangThaiHoatDong`)
- ✅ Xóa loại phòng (chỉ khi chưa có phòng nào sử dụng)

### Mục đích:
- Quản lý danh mục loại phòng
- Cập nhật giá theo mùa/chương trình khuyến mãi
- Hiển thị thông tin cho khách hàng tìm kiếm

---

## C. Quản lý Phòng
**Bảng: `Phong`**

### Chức năng:
- ✅ Xem danh sách tất cả phòng (30 phòng) và trạng thái
- ✅ Thêm phòng mới:
  - Số phòng (`soPhong`): 101, 102, 201...
  - Loại phòng (`maLoaiPhong`)
  - Tầng (`tang`)
  - Mô tả (`moTa`)
- ✅ Cập nhật thông tin phòng
- ✅ Thay đổi trạng thái phòng (`trangThai`):
  - **Còn trống**: Phòng sẵn sàng cho khách đặt
  - **Đã đặt**: Phòng đã được đặt nhưng chưa check-in
  - **Đang sử dụng**: Khách đang ở
  - **Bảo trì**: Phòng đang sửa chữa/bảo trì
- ✅ Xem lịch sử trạng thái phòng
- ✅ Kích hoạt/Vô hiệu hóa phòng (`trangThaiHoatDong`)

### Mục đích:
- Quản lý tình trạng phòng real-time
- Theo dõi phòng cần bảo trì
- Tối ưu hóa việc sử dụng phòng

---

## D. Quản lý Đặt phòng
**Bảng: `DatPhong`**

### Chức năng:

### D.1. Xem và Tìm kiếm
- ✅ Xem tất cả đơn đặt phòng (Online và Offline)
- ✅ Lọc theo:
  - Trạng thái: Chờ xác nhận, Đã xác nhận, Đã nhận phòng, Đã trả phòng, Đã hủy
  - Ngày đặt
  - Khách hàng
  - Phòng
  - Người tạo (Admin/Khách hàng)

### D.2. Tạo đặt phòng OFFLINE (Walk-in)
- ✅ **Tạo đơn đặt phòng cho khách đến trực tiếp**:
  - Chọn khách hàng từ danh sách hoặc tạo mới (khách lần đầu)
  - Chọn phòng còn trống (`maPhong`)
  - Chọn ngày nhận phòng (`ngayNhanPhong`)
  - Chọn ngày trả phòng (`ngayTraPhong`)
  - Nhập số khách (`soKhach`)
  - Nhập yêu cầu đặc biệt (`yeuCauDacBiet`) - nếu có
  - Hệ thống tự động tính tổng tiền (`tongTien`)
  - Nhận tiền đặt cọc (`tienDatCoc`)
  - Ghi nhận `nguoiTao` = mã Admin
  - Trạng thái: "Đã xác nhận" hoặc "Đã nhận phòng" (nếu check-in ngay)

### D.3. Xử lý đơn đặt phòng ONLINE
- ✅ **Xác nhận đơn đặt phòng** từ khách hàng:
  - Kiểm tra tính khả dụng của phòng
  - Kiểm tra thanh toán đặt cọc
  - Chuyển trạng thái: "Chờ xác nhận" → "Đã xác nhận"
  - Gửi email xác nhận cho khách
- ✅ **Từ chối đơn đặt phòng**:
  - Nhập lý do từ chối
  - Hoàn tiền đặt cọc (nếu có)

### D.4. Check-in
- ✅ **Nhận phòng**:
  - Kiểm tra giấy tờ khách hàng
  - Xác nhận đặt cọc
  - Trả chìa khóa phòng
  - Chuyển trạng thái: "Đã xác nhận" → "Đã nhận phòng"
  - Cập nhật trạng thái phòng: "Đã đặt" → "Đang sử dụng"

### D.5. Check-out
- ✅ **Trả phòng**:
  - Kiểm tra phòng
  - Tính tổng tiền (phòng + dịch vụ)
  - Thu tiền còn lại
  - Nhận chìa khóa
  - Chuyển trạng thái: "Đã nhận phòng" → "Đã trả phòng"
  - Cập nhật trạng thái phòng: "Đang sử dụng" → "Còn trống"

### D.6. Cập nhật và Hủy
- ✅ Cập nhật thông tin đặt phòng:
  - Thay đổi ngày nhận/trả phòng
  - Thay đổi số khách
  - Cập nhật yêu cầu đặc biệt
  - Điều chỉnh phòng (nếu cần)
- ✅ **Hủy đặt phòng**:
  - Nhập lý do hủy (`lyDoHuy`)
  - Ghi nhận ngày hủy (`ngayHuy`)
  - Chuyển trạng thái → "Đã hủy"
  - Xử lý hoàn tiền (nếu có chính sách)
  - Cập nhật trạng thái phòng về "Còn trống"

### D.7. Báo cáo
- ✅ Xem lịch sử đặt phòng theo khách hàng
- ✅ Báo cáo thống kê:
  - Tỷ lệ lấp phòng theo ngày/tháng/năm
  - Doanh thu theo phòng/loại phòng
  - Số lượng đơn theo trạng thái
  - Tỷ lệ hủy đơn

### Mục đích:
- Quản lý toàn bộ quy trình đặt phòng
- Xử lý cả online và offline
- Tối ưu hóa doanh thu và trải nghiệm khách hàng

---

## E. Quản lý Dịch vụ
**Bảng: `DichVu`**

### Chức năng:
- ✅ Xem danh sách tất cả dịch vụ (30 dịch vụ)
- ✅ Thêm dịch vụ mới:
  - Tên dịch vụ (`tenDichVu`)
  - Mô tả (`moTa`)
  - Giá dịch vụ (`giaDichVu`)
  - Loại dịch vụ (`loaiDichVu`):
    - **Spa**: Massage, Chăm sóc da mặt, Gội đầu...
    - **Nhà hàng**: Buffet, Set menu, À la carte...
    - **Phục vụ phòng**: Room service các bữa ăn và đồ uống
    - **Giặt là**: Giặt khô, Giặt ủi, Ủi đồ...
    - **Vận chuyển**: Đưa đón sân bay, Thuê xe...
    - **Giải trí**: Hồ bơi, Gym, Sauna, Yoga, Tennis, Karaoke...
    - **Dịch vụ khác**: Baby sitting, Trang trí phòng, Hoa tươi...
  - Hình ảnh (`hinhAnh`)
- ✅ Cập nhật thông tin dịch vụ (tên, mô tả, giá)
- ✅ Kích hoạt/Vô hiệu hóa dịch vụ (`trangThaiHoatDong`)
- ✅ Xóa dịch vụ (chỉ khi chưa được sử dụng)

### Mục đích:
- Quản lý danh mục dịch vụ khách sạn
- Cập nhật giá dịch vụ
- Tăng doanh thu từ dịch vụ bổ sung

---

## F. Quản lý Dịch vụ đặt phòng
**Bảng: `DichVuDatPhong`**

### Chức năng:
- ✅ **Thêm dịch vụ cho đơn đặt phòng**:
  - Chọn đơn đặt phòng (`maDatPhong`)
  - Chọn dịch vụ (`maDichVu`)
  - Nhập số lượng (`soLuong`)
  - Hệ thống tự động lấy đơn giá (`donGia`) từ bảng DichVu
  - Tự động tính thành tiền (`thanhTien = soLuong × donGia`)
  - Ghi nhận ngày sử dụng (`ngaySuDung`)
  - Ghi chú (`ghiChu`) - nếu có
- ✅ Xem danh sách dịch vụ đã sử dụng theo đơn đặt phòng
- ✅ Cập nhật số lượng dịch vụ
- ✅ Xóa dịch vụ đã thêm (nếu chưa sử dụng)
- ✅ Xem báo cáo:
  - Doanh thu từ dịch vụ theo loại
  - Dịch vụ được sử dụng nhiều nhất
  - Doanh thu dịch vụ theo tháng

### Mục đích:
- Quản lý việc sử dụng dịch vụ của khách
- Tính toán chính xác chi phí dịch vụ
- Tăng doanh thu từ dịch vụ bổ sung

---

## G. Quản lý Thanh toán
**Bảng: `ThanhToan`**

### Chức năng:

### G.1. Xem và Tìm kiếm
- ✅ Xem tất cả giao dịch thanh toán
- ✅ Lọc theo:
  - Trạng thái: Chờ xử lý, Thành công, Thất bại, Đã hoàn tiền
  - Phương thức: Tiền mặt, Chuyển khoản, Ví điện tử
  - Ngày thanh toán
  - Đơn đặt phòng
  - Khách hàng

### G.2. Xử lý Thanh toán
- ✅ **Nhận thanh toán**:
  - Chọn đơn đặt phòng (`maDatPhong`)
  - Nhập số tiền (`soTien`)
  - Chọn phương thức thanh toán (`phuongThucThanhToan`):
    - **Tiền mặt**: Nhận tiền tại quầy
    - **Chuyển khoản**: Nhận qua tài khoản ngân hàng
    - **Ví điện tử**: MoMo, ZaloPay, VNPay...
  - Nhập mã giao dịch (`maGiaoDich`) - nếu có
  - Ghi chú (`ghiChu`) - nếu cần
  - Cập nhật trạng thái thanh toán (`trangThaiThanhToan`)
  - Ghi nhận người xử lý (`nguoiXuLy` = mã Admin)
  - Tự động ghi nhận ngày thanh toán (`ngayThanhToan`)

### G.3. Xử lý Hoàn tiền
- ✅ **Hoàn tiền cho khách**:
  - Chọn giao dịch cần hoàn
  - Nhập lý do hoàn tiền
  - Chuyển trạng thái → "Đã hoàn tiền"
  - Cập nhật thông tin hoàn tiền

### G.4. Xác nhận Thanh toán Online
- ✅ Xác nhận thanh toán từ khách hàng qua:
  - Chuyển khoản ngân hàng
  - Ví điện tử (MoMo, ZaloPay, VNPay)
- ✅ Đối chiếu mã giao dịch
- ✅ Cập nhật trạng thái: "Chờ xử lý" → "Thành công"
### G.5. Báo cáo
- ✅ Xem lịch sử thanh toán theo đơn đặt phòng
- ✅ Báo cáo doanh thu:
  - Theo ngày/tháng/năm
  - Theo phương thức thanh toán
  - Tổng doanh thu phòng
  - Tổng doanh thu dịch vụ
  - Doanh thu thuần (sau hoàn tiền)
- ✅ Báo cáo công nợ (tiền chưa thanh toán)

### Mục đích:
- Quản lý tài chính chính xác
- Theo dõi dòng tiền
- Tạo báo cáo doanh thu

---

## H. Báo cáo & Thống kê

### Chức năng:
- ✅ **Báo cáo tỷ lệ lấp phòng**:
  - Theo ngày/tuần/tháng/năm
  - Theo loại phòng
  - So sánh với kỳ trước
- ✅ **Báo cáo doanh thu**:
  - Doanh thu phòng theo loại
  - Doanh thu dịch vụ theo loại
  - Tổng doanh thu
  - Biểu đồ doanh thu theo thời gian
- ✅ **Phòng được đặt nhiều nhất**
- ✅ **Dịch vụ được sử dụng nhiều nhất**
- ✅ **Báo cáo tình trạng phòng**:
  - Số phòng còn trống
  - Số phòng đã đặt
  - Số phòng đang sử dụng
  - Số phòng bảo trì
- ✅ **Báo cáo đơn đặt phòng**:
  - Tổng số đơn
  - Số đơn chờ xác nhận
  - Số đơn đã xác nhận
  - Số đơn đã hủy
  - Tỷ lệ hủy đơn

### Mục đích:
- Hỗ trợ ra quyết định kinh doanh
- Tối ưu hóa doanh thu
- Phát hiện xu hướng và cơ hội

---

# 🔷 2. KHÁCH HÀNG

## A. Quản lý Tài khoản cá nhân
**Bảng: `NguoiDung`**

### Chức năng:
- ✅ **Đăng ký tài khoản mới** (Online):
  - Nhập họ tên (`hoTen`)
  - Nhập email (`email`) - phải unique
  - Nhập số điện thoại (`soDienThoai`)
  - Tạo mật khẩu (`matKhau`)
  - Nhập địa chỉ (`diaChi`) - tùy chọn
  - Nhập ngày sinh (`ngaySinh`) - tùy chọn
  - Chọn giới tính (`gioiTinh`) - tùy chọn
  - Vai trò mặc định: "Khách hàng"
  - Trạng thái mặc định: Hoạt động
- ✅ **Đăng nhập** vào hệ thống:
  - Sử dụng email và mật khẩu
- ✅ **Xem thông tin cá nhân**
- ✅ **Cập nhật thông tin cá nhân**:
  - Họ tên, số điện thoại
  - Địa chỉ
  - Ngày sinh, giới tính
  - **Lưu ý**: Không thể thay đổi email (dùng để đăng nhập)
- ✅ **Đổi mật khẩu**:
  - Nhập mật khẩu cũ
  - Nhập mật khẩu mới
  - Xác nhận mật khẩu mới
- ✅ **Quên mật khẩu**:
  - Nhập email
  - Nhận link reset qua email
  - Tạo mật khẩu mới

### Mục đích:
- Quản lý thông tin cá nhân
- Đảm bảo an toàn tài khoản
- Cá nhân hóa trải nghiệm

---

## B. Tìm kiếm & Xem thông tin Phòng
**Bảng: `LoaiPhong`, `Phong`**

### Chức năng:

### B.1. Tìm kiếm phòng
- ✅ **Tìm kiếm theo tiêu chí**:
  - **Ngày nhận phòng - Ngày trả phòng**: Hệ thống chỉ hiển thị phòng còn trống trong khoảng thời gian này
  - **Loại phòng**: Standard, Superior, Deluxe, Suite, Apartment, Villa
  - **Số người**: Lọc theo `soNguoiToiDa`
  - **Khoảng giá**: Từ - Đến
  - **Tiện nghi**: WiFi, TV, Minibar, Bồn tắm, Ban công, Bếp...
  - **Diện tích**: Tối thiểu bao nhiêu m²
- ✅ **Sắp xếp kết quả**:
  - Giá tăng dần/giảm dần
  - Diện tích lớn nhất/nhỏ nhất
  - Số người tối đa
  - Mới nhất

### B.2. Xem danh sách phòng
- ✅ Xem danh sách phòng còn trống
- ✅ Lọc theo loại phòng
- ✅ Hiển thị thông tin cơ bản:
  - Tên loại phòng
  - Giá
  - Số người tối đa
  - Hình ảnh thumbnail

### B.3. Xem chi tiết loại phòng
- ✅ **Thông tin đầy đủ**:
  - Hình ảnh phòng (gallery)
  - Tên loại phòng (`tenLoaiPhong`)
  - Mô tả chi tiết (`moTa`)
  - Giá cơ bản (`giaCoBan`) - theo đêm
  - Số người tối đa (`soNguoiToiDa`)
  - Loại giường (`loaiGiuong`)
  - Diện tích phòng (`dienTichPhong`) m²
  - Tiện nghi (`tienNghi`): Danh sách chi tiết
  - Chính sách: Check-in, Check-out, Hủy phòng
- ✅ **Xem tình trạng phòng**:
  - Số phòng còn trống
  - Lịch trống trong tháng
- ✅ **Xem đánh giá** (nếu có tính năng review)

### B.4. So sánh phòng
- ✅ So sánh tối đa 3-4 loại phòng cùng lúc
- ✅ So sánh theo:
  - Giá
  - Diện tích
  - Tiện nghi
  - Số người

### Mục đích:
- Giúp khách tìm phòng phù hợp
- Cung cấp đầy đủ thông tin
- Tăng tỷ lệ chuyển đổi

---

## C. Đặt phòng ONLINE
**Bảng: `DatPhong`**

### Chức năng:

### C.1. Tạo đơn đặt phòng
- ✅ **Quy trình đặt phòng**:
  
  **Bước 1: Chọn phòng**
  - Chọn loại phòng từ kết quả tìm kiếm
  - Chọn phòng cụ thể (hoặc hệ thống tự chọn)
  - Chọn ngày nhận phòng (`ngayNhanPhong`)
  - Chọn ngày trả phòng (`ngayTraPhong`)
  - Hệ thống kiểm tra tính khả dụng
  
  **Bước 2: Nhập thông tin**
  - Nhập số khách (`soKhach`)
  - Kiểm tra không vượt quá `soNguoiToiDa`
  - Nhập yêu cầu đặc biệt (`yeuCauDacBiet`) - nếu có:
    - Tầng cao/thấp
    - View đẹp
    - Giường phụ
    - Gối thêm
    - Bữa sáng
    - Trang trí lãng mạn
    - Check-in sớm/Check-out muộn
    - Và các yêu cầu khác...
  
  **Bước 3: Chọn dịch vụ bổ sung** (tùy chọn)
  - Spa
  - Bữa ăn
  - Đưa đón sân bay
  - Thuê xe
  - Vé giải trí
  
  **Bước 4: Xem tổng tiền**
  - Hệ thống tự động tính:
    - Tiền phòng = `giaCoBan × số đêm`
    - Tiền dịch vụ (nếu có)
    - Tổng tiền (`tongTien`)
    - Tiền đặt cọc (`tienDatCoc`) - thường 30-50%
  
  **Bước 5: Thanh toán đặt cọc**
  - Chọn phương thức thanh toán
  - Chuyển khoan
  
  **Bước 6: Xác nhận**
  - Ghi nhận `nguoiTao` = mã khách hàng (chính mình)
  - Trạng thái ban đầu: "Chờ xác nhận"
  - Tự động ghi nhận `ngayDat`
  - Nhận email xác nhận đặt phòng

### C.2. Chính sách đặt cọc
- ✅ Đặt cọc online qua:
  - Chuyển khoản ngân hàng
  - Ví điện tử: MoMo, ZaloPay, VNPay
- ✅ Mức đặt cọc:
  - Phòng thường: 30-50% tổng tiền
  - Suite/Villa: 50-100%
  - Dịp cao điểm: 50-100%

### C.3. Theo dõi đơn đặt phòng
- ✅ Nhận thông báo khi:
  - Admin xác nhận đơn
  - Đơn bị từ chối (kèm lý do)
  - Sắp đến ngày check-in (nhắc nhở)
  - Thanh toán thành công

### Mục đích:
- Cho phép khách tự đặt phòng 24/7
- Giảm tải công việc cho Admin
- Thuận tiện và nhanh chóng

---

## D. Quản lý đơn đặt phòng của mình
**Bảng: `DatPhong`**

### Chức năng:

### D.1. Xem danh sách đơn đặt phòng
- ✅ Xem tất cả đơn đặt phòng của mình (theo `maKhachHang`)
- ✅ Lọc theo trạng thái:
  - **Chờ xác nhận**: Đơn mới tạo, chưa được Admin xác nhận
  - **Đã xác nhận**: Admin đã xác nhận, chờ đến ngày check-in
  - **Đã nhận phòng**: Đã check-in, đang ở
  - **Đã trả phòng**: Đã check-out, kết thúc
  - **Đã hủy**: Đơn đã bị hủy
- ✅ Sắp xếp:
  - Mới nhất
  - Sắp diễn ra
  - Đã qua

### D.2. Xem chi tiết đơn đặt phòng
- ✅ **Thông tin đầy đủ**:
  - Mã đặt phòng (`maDatPhong`)
  - **Thông tin phòng**:
    - Số phòng
    - Loại phòng
    - Tầng
    - Hình ảnh
  - **Thông tin đặt phòng**:
    - Ngày nhận phòng (`ngayNhanPhong`)
    - Ngày trả phòng (`ngayTraPhong`)
    - Số đêm = ngayTraPhong - ngayNhanPhong
    - Số khách (`soKhach`)
    - Yêu cầu đặc biệt (`yeuCauDacBiet`)
  - **Thông tin tài chính**:
    - Tổng tiền (`tongTien`)
    - Tiền đã đặt cọc (`tienDatCoc`)
    - Tiền phòng
    - Tiền dịch vụ
    - Còn lại phải trả
  - **Trạng thái**:
    - Trạng thái đặt phòng (`trangThaiDatPhong`)
    - Ngày đặt (`ngayDat`)
    - Người tạo (`nguoiTao`)
- ✅ **Dịch vụ đã sử dụng** (nếu có):
  - Danh sách dịch vụ
  - Số lượng, đơn giá, thành tiền
  - Ngày sử dụng
- ✅ **Lịch sử thanh toán**:
  - Ngày thanh toán
  - Số tiền
  - Phương thức
  - Trạng thái

### D.3. Hủy đặt phòng
- ✅ **Điều kiện hủy**:
  - Chỉ được hủy khi trạng thái = "Chờ xác nhận" hoặc "Đã xác nhận"
  - Không được hủy khi đã check-in
  - Tùy theo chính sách hủy của khách sạn
- ✅ **Quy trình hủy**:
  - Chọn đơn cần hủy
  - Nhập lý do hủy (`lyDoHuy`)
  - Xác nhận hủy
  - Hệ thống tự động:
    - Ghi nhận `ngayHuy`
    - Chuyển `trangThaiDatPhong` → "Đã hủy"
    - Cập nhật trạng thái phòng về "Còn trống"
    - Tạo yêu cầu hoàn tiền (nếu có đặt cọc)
  - Nhận email xác nhận hủy
- ✅ **Chính sách hoàn tiền**:
  - Hủy trước 7 ngày: Hoàn 100%
  - Hủy 3-7 ngày: Hoàn 50%
  - Hủy dưới 3 ngày: Không hoàn
  - (Tùy theo chính sách khách sạn)

### D.4. In hóa đơn
- ✅ In/Tải hóa đơn PDF
- ✅ Gửi hóa đơn qua email

### D.5. Đánh giá sau khi ở
- ✅ Đánh giá phòng (1-5 sao)
- ✅ Đánh giá dịch vụ
- ✅ Viết nhận xét
- ✅ Upload hình ảnh

### Mục đích:
- Quản lý lịch sử đặt phòng
- Theo dõi tình trạng đơn
- Linh hoạt hủy/thay đổi

---

## E. Sử dụng Dịch vụ
**Bảng: `DichVu`, `DichVuDatPhong`**
### Chức năng:

### E.1. Xem danh sách dịch vụ
- ✅ **Xem tất cả dịch vụ khách sạn** (30 dịch vụ):
  
  **1. Spa (5 dịch vụ)**
  - Massage toàn thân 60/90 phút
  - Chăm sóc da mặt
  - Gội đầu dưỡng sinh
  
  **2. Nhà hàng (4 dịch vụ)**
  - Bữa sáng buffet
  - Bữa trưa set menu
  - Bữa tối à la carte
  - Cocktail Bar
  
  **3. Phục vụ phòng - Room Service (4 dịch vụ)**
  - Bữa sáng
  - Bữa trưa
  - Bữa tối
  - Đồ uống
  
  **4. Giặt là (4 dịch vụ)**
  - Giặt khô
  - Giặt ủi thường
  - Ủi áo sơ mi
  - Ủi vest/suit
  
  **5. Vận chuyển (5 dịch vụ)**
  - Đưa đón sân bay 1 chiều
  - Đưa đón sân bay khứ hồi
  - Thuê xe 4 chỗ - Nửa ngày
  - Thuê xe 7 chỗ - Cả ngày
  - Thuê xe limousine
  
  **6. Giải trí (6 dịch vụ)**
  - Hồ bơi cao cấp
  - Phòng gym
  - Sauna & Steam
  - Yoga class
  - Tennis court
  - Karaoke VIP room
  
  **7. Dịch vụ khác (3 dịch vụ)**
  - Baby sitting (theo giờ)
  - Trang trí phòng lãng mạn
  - Florist - Hoa tươi

### E.2. Xem chi tiết dịch vụ
- ✅ **Thông tin chi tiết**:
  - Tên dịch vụ (`tenDichVu`)
  - Mô tả (`moTa`)
  - Giá dịch vụ (`giaDichVu`)
  - Loại dịch vụ (`loaiDichVu`)
  - Hình ảnh (`hinhAnh`)
  - Đơn vị tính (lần, giờ, bộ, người...)
  - Thời gian phục vụ
  - Điều khoản sử dụng

### E.3. Đặt dịch vụ
- ✅ **Điều kiện**:
  - Phải có đơn đặt phòng hợp lệ
  - Trạng thái đơn: "Đã xác nhận" hoặc "Đã nhận phòng"
- ✅ **Quy trình đặt**:
  - Chọn đơn đặt phòng (`maDatPhong`)
  - Chọn dịch vụ muốn sử dụng (`maDichVu`)
  - Chọn số lượng (`soLuong`)
  - Hệ thống tự động lấy đơn giá (`donGia`) từ bảng DichVu
  - Hệ thống tự động tính thành tiền (`thanhTien`)
  - Chọn ngày sử dụng (`ngaySuDung`)
  - Ghi chú yêu cầu đặc biệt (`ghiChu`) - nếu có
  - Xác nhận đặt dịch vụ
- ✅ **Đặt dịch vụ trước** (khi đặt phòng):
  - Đặt cùng lúc với phòng
  - Đặt sau khi đơn được xác nhận
- ✅ **Đặt dịch vụ sau** (khi đã check-in):
  - Gọi điện đến lễ tân/reception
  - Đặt qua app/website
  - Đặt trực tiếp tại quầy dịch vụ

### E.4. Xem dịch vụ đã đặt
- ✅ Xem danh sách dịch vụ đã đặt theo từng đơn phòng
- ✅ Thông tin hiển thị:
  - Tên dịch vụ
  - Số lượng
  - Đơn giá
  - Thành tiền
  - Ngày sử dụng
  - Ghi chú
  - Trạng thái (Chờ xác nhận/Đã xác nhận/Đã sử dụng/Đã hủy)

### E.5. Hủy dịch vụ
- ✅ **Điều kiện hủy**:
  - Chỉ hủy được khi dịch vụ chưa sử dụng
  - Phụ thuộc chính sách hủy của từng dịch vụ
- ✅ Chọn dịch vụ cần hủy
- ✅ Xác nhận hủy
- ✅ Hoàn tiền (nếu đã thanh toán trước)

### Mục đích:
- Nâng cao trải nghiệm khách hàng
- Tăng doanh thu từ dịch vụ
- Tự động hóa quy trình đặt dịch vụ

---

## F. Thanh toán
**Bảng: `ThanhToan`**

### Chức năng:

### F.1. Xem lịch sử thanh toán
- ✅ Xem tất cả giao dịch thanh toán của mình
- ✅ Lọc theo:
  - Đơn đặt phòng
  - Trạng thái thanh toán
  - Phương thức thanh toán
  - Khoảng thời gian

### F.2. Xem chi tiết hóa đơn
- ✅ **Thông tin hóa đơn**:
  - Mã đặt phòng
  - Thông tin phòng
  - Thời gian ở: Ngày nhận - Ngày trả (X đêm)
  
  **Chi phí phòng**:
  - Giá phòng/đêm
  - Số đêm
  - Tổng tiền phòng
  
  **Chi phí dịch vụ**:
  - Danh sách dịch vụ đã sử dụng
  - Số lượng × Đơn giá = Thành tiền
  - Tổng tiền dịch vụ
  
  **Tổng cộng**:
  - Tổng tiền phòng + dịch vụ
  - Thuế VAT (nếu có)
  - **Tổng tiền phải trả**
  - Tiền đã thanh toán (đặt cọc + các lần thanh toán)
  - **Còn lại phải trả**

### F.3. Thanh toán online
- ✅ **Phương thức thanh toán** (`phuongThucThanhToan`):
  
  **1. Chuyển khoản ngân hàng**:
  - Hiển thị thông tin tài khoản khách sạn
  - Khách chuyển khoản và nhập mã giao dịch
  - Admin xác nhận sau khi nhận tiền
  
  **2. Ví điện tử**:
  - **MoMo**: Quét QR hoặc nhập SĐT
  - **ZaloPay**: Quét QR hoặc nhập SĐT
  - **VNPay**: Quét QR hoặc nhập thẻ
  - Thanh toán tức thì, tự động xác nhận
  
  **3. Tiền mặt** (khi check-out):
  - Thanh toán tại quầy lễ tân
  - Nhận hóa đơn giấy

### F.4. Xem trạng thái thanh toán
- ✅ **Trạng thái** (`trangThaiThanhToan`):
  - **Chờ xử lý**: Đã gửi yêu cầu thanh toán, chờ Admin xác nhận
  - **Thành công**: Đã thanh toán thành công
  - **Thất bại**: Thanh toán thất bại (hết hạn, lỗi...)
  - **Đã hoàn tiền**: Đã được hoàn tiền (do hủy đơn...)

### F.5. In hóa đơn điện tử
- ✅ In hóa đơn PDF
- ✅ Tải hóa đơn về máy
- ✅ Gửi hóa đơn qua email
- ✅ Hóa đơn bao gồm:
  - Thông tin khách sạn
  - Thông tin khách hàng
  - Chi tiết phòng và dịch vụ
  - Tổng tiền
  - Lịch sử thanh toán
  - Chữ ký điện tử

### F.6. Yêu cầu hóa đơn VAT
- ✅ Nhập thông tin công ty:
  - Tên công ty
  - Mã số thuế
  - Địa chỉ
  - Email nhận hóa đơn
- ✅ Admin xuất hóa đơn VAT

### Mục đích:
- Thanh toán linh hoạt, tiện lợi
- Minh bạch chi phí
- Quản lý tài chính cá nhân

---

## G. Khác

### G.1. Lịch sử đặt phòng
- ✅ Xem tất cả đơn đặt phòng đã từng đặt
- ✅ Thống kê:
  - Tổng số đêm đã ở
  - Tổng chi tiêu
  - Loại phòng ưa thích
  - Dịch vụ thường dùng

### G.2. Đặt lại phòng cũ
- ✅ Đặt lại phòng đã từng ở
- ✅ Sao chép thông tin từ đơn cũ
- ✅ Chỉ cần chọn ngày mới

### G.3. Yêu thích / Wishlist
- ✅ Lưu phòng yêu thích
- ✅ Nhận thông báo khi có khuyến mãi
- ✅ Đặt nhanh từ danh sách yêu thích

### G.4. Đánh giá & Nhận xét
- ✅ Đánh giá sau khi check-out:
  - Đánh giá phòng (1-5 sao)
  - Đánh giá dịch vụ (1-5 sao)
  - Đánh giá nhân viên (1-5 sao)
  - Viết nhận xét chi tiết
  - Upload hình ảnh thực tế
- ✅ Xem đánh giá của khách khác
- ✅ Sắp xếp theo:
  - Mới nhất
  - Đánh giá cao nhất
  - Đánh giá thấp nhất

### G.5. Liên hệ hỗ trợ
- ✅ Chat trực tuyến với Admin
- ✅ Gọi điện hotline
- ✅ Gửi email hỗ trợ
- ✅ FAQ - Câu hỏi thường gặp

### G.6. Nhận thông báo
- ✅ **Email**:
  - Xác nhận đăng ký
  - Xác nhận đặt phòng
  - Xác nhận thanh toán
  - Nhắc nhở check-in (1 ngày trước)
  - Hủy đơn
  - Hoàn tiền
  - Khuyến mãi
- ✅ **SMS**:
  - Xác nhận đặt phòng
  - Nhắc nhở check-in
  - Mã OTP xác thực
- ✅ **Push Notification** (App):
  - Cập nhật trạng thái đơn
  - Khuyến mãi
  - Tin tức

### G.7. Chương trình khách hàng thân thiết
- ✅ Tích điểm khi đặt phòng
- ✅ Quy đổi điểm:
  - Giảm giá phòng
  - Miễn phí dịch vụ
  - Nâng hạng phòng
- ✅ Bậc thành viên:
  - Bạc: 5+ đơn
  - Vàng: 10+ đơn
  - Platinum: 20+ đơn
  - Diamond: 50+ đơn

---

# 📊 SO SÁNH PHÂN BIỆT: ONLINE vs OFFLINE

| **Tiêu chí** | **Đặt phòng ONLINE** | **Đặt phòng OFFLINE** |
|--------------|---------------------|----------------------|
| **Người thực hiện** | Khách hàng tự đặt | Admin tạo cho khách |
| **Người tạo (`nguoiTao`)** | Mã khách hàng | Mã Admin |
| **Thời gian** | 24/7, bất cứ lúc nào | Trong giờ làm việc của khách sạn |
| **Trạng thái ban đầu** | "Chờ xác nhận" | "Đã xác nhận" hoặc "Đã nhận phòng" |
| **Xác nhận** | Admin phải xác nhận sau | Admin xác nhận ngay khi tạo |
| **Thanh toán đặt cọc** | Online (Chuyển khoản/Ví điện tử) | Tiền mặt tại quầy hoặc Chuyển khoản |
| **Thanh toán phần còn lại** | Online hoặc khi check-out | Khi check-out (thường tiền mặt) |
| **Hủy đơn** | Khách tự hủy (theo chính sách) | Khách yêu cầu, Admin xử lý |
| **Ưu điểm** | Tiện lợi, nhanh chóng, 24/7 | Tư vấn trực tiếp, linh hoạt |
| **Nhược điểm** | Phải chờ xác nhận | Phụ thuộc giờ làm việc |

---

# 🔐 PHÂN QUYỀN RÕ RÀNG

## ADMIN được phép:
- ✅ CRUD (Tạo, Đọc, Cập nhật) trên TẤT CẢ các bảng
- ✅ Xem thông tin của TẤT CẢ khách hàng
- ✅ Tạo đặt phòng cho BẤT KỲ khách hàng nào
- ✅ Xác nhận/Từ chối đặt phòng của khách
- ✅ Check-in, Check-out
- ✅ Xử lý thanh toán và hoàn tiền
- ✅ Xem tất cả báo cáo và thống kê
- ✅ Quản lý giá phòng và dịch vụ

## KHÁCH HÀNG KHÔNG được phép:
- ❌ Xem thông tin khách hàng khác
- ❌ Xem đơn đặt phòng của người khác
- ❌ Tự xác nhận đơn đặt phòng của mình
- ❌ Thay đổi trạng thái phòng
- ❌ Chỉnh sửa giá phòng/dịch vụ
- ❌ Xóa lịch sử thanh toán
- ❌ Xem báo cáo tổng thể của khách sạn
- ❌ Tạo tài khoản Admin

## KHÁCH HÀNG chỉ được phép:
- ✅ Quản lý tài khoản của CHÍNH MÌNH
- ✅ Xem và đặt phòng còn trống
- ✅ Xem đơn đặt phòng của CHÍNH MÌNH
- ✅ Hủy đơn của CHÍNH MÌNH (theo chính sách)
- ✅ Đặt dịch vụ cho đơn phòng của CHÍNH MÌNH
- ✅ Thanh toán cho đơn phòng của CHÍNH MÌNH
- ✅ Xem lịch sử của CHÍNH MÌNH

---

# 🎯 LUỒNG XỬ LÝ CHÍNH

## Luồng 1: Đặt phòng ONLINE
```
Khách hàng → Tìm phòng → Chọn phòng → Nhập thông tin → Chọn dịch vụ (optional)
→ Thanh toán đặt cọc → Đơn "Chờ xác nhận"
→ Admin kiểm tra → Xác nhận/Từ chối
→ Nếu xác nhận: "Đã xác nhận" → Khách nhận email
→ Đến ngày check-in → Admin check-in → "Đã nhận phòng"
→ Khách ở và sử dụng dịch vụ
→ Đến ngày check-out → Admin check-out → Thanh toán phần còn lại → "Đã trả phòng"
```

## Luồng 2: Đặt phòng OFFLINE
```
Khách walk-in → Liên hệ lễ tân → Admin tạo đơn đặt phòng
→ Nhập thông tin khách (hoặc tạo tài khoản mới)
→ Chọn phòng, ngày, số khách
→ Tính tiền → Nhận đặt cọc (tiền mặt)
→ Đơn "Đã xác nhận"
→ Nếu check-in ngay: "Đã nhận phòng"
→ Khách ở và sử dụng dịch vụ
→ Check-out → Thanh toán phần còn lại → "Đã trả phòng"
```

## Luồng 3: Hủy đặt phòng
```
Khách hàng → Xem đơn đặt phòng → Chọn "Hủy đơn" → Nhập lý do
→ Xác nhận hủy → Đơn "Đã hủy"
→ Hệ thống tính phí hủy (theo chính sách)
→ Admin xử lý hoàn tiền → "Đã hoàn tiền"
→ Khách nhận lại tiền (nếu có)
```

---

# 📈 MỞ RỘNG TRONG TƯƠNG LAI

## Chức năng có thể thêm:
- 🔄 Tích hợp API thanh toán quốc tế (Stripe, PayPal)
- 📱 Mobile App (iOS, Android)
- 🤖 Chatbot AI hỗ trợ tự động
- 🌍 Đa ngôn ngữ (Tiếng Anh, Tiếng Hàn, Tiếng Nhật...)
- 💳 Chương trình khách hàng thân thiết nâng cao
- 🎁 Mã giảm giá, Voucher
- 📊 Dashboard Analytics cho Admin
- 🔔 Hệ thống thông báo real-time
- 🗺️ Tích hợp Google Maps
- ⭐ Hệ thống đánh giá và review
- 🏆 Gamification (tích điểm, phần thưởng)
- 📧 Email Marketing tự động

---

# 🛠️ CÔNG NGHỆ ĐỀ XUẤT

## Backend:
- ASP.NET Core / Node.js / Laravel
- SQL Server
- RESTful API / GraphQL

## Frontend:
- React / Vue.js / Angular
- Bootstrap / Tailwind CSS
- Mobile: React Native / Flutter

## Others:
- Payment Gateway: VNPay, MoMo, ZaloPay
- Email Service: SendGrid
- SMS Service: Twilio / Esms
- Cloud: Azure / AWS
- CI/CD: GitHub Actions / Azure DevOps

---

# 📞 LIÊN HỆ

- **Sinh viên**: [Tên của bạn]
- **MSSV**: 65130650
- **Email**: [Email của bạn]
- **Trường**: NTU
- **Học kỳ**: 1, Năm học 2025-2026

---

# 📝 GHI CHÚ

- Database được thiết kế để hỗ trợ cả Online và Offline booking
- Tất cả chức năng đều dựa trên cấu trúc database trong file `Project_65130650.sql`
- Cần implement thêm logic validation và business rules khi phát triển ứng dụng
- Bảo mật phải được ưu tiên (mã hóa mật khẩu, SQL injection, XSS...)
- Cần có cơ chế backup database định kỳ

---

**© 2025 - Hotel Management System - Project_65130650**
