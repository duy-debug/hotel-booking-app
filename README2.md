# CÁC CÂU TRUY VẤN SQL - HỆ THỐNG QUẢN LÝ KHÁCH SẠN

## 1. ADMIN - QUẢN LÝ NGƯỜI DÙNG

### GetAllAdminAndCustomer
```sql
-- Lấy tất cả người dùng (Admin + Khách hàng)
SELECT * FROM NguoiDung 
ORDER BY vaiTro, ngayTao DESC;

-- Lọc theo vai trò
SELECT * FROM NguoiDung WHERE vaiTro = N'Quản trị';
SELECT * FROM NguoiDung WHERE vaiTro = N'Khách hàng';
```

### CreateAdminAndCustomer
```sql
-- Tạo Admin mới
INSERT INTO NguoiDung (maNguoiDung, hoTen, email, soDienThoai, matKhau, vaiTro, diaChi, ngaySinh, gioiTinh)
VALUES ('ND031', N'Nguyễn Văn Test', 'test@admin.com', '0901111111', 'Pass@123', N'Quản trị', 
        N'123 Test St', '1990-01-01', N'Nam');

-- Tạo Khách hàng mới
INSERT INTO NguoiDung (maNguoiDung, hoTen, email, soDienThoai, matKhau, vaiTro, diaChi, ngaySinh, gioiTinh)
VALUES ('ND032', N'Trần Thị Test', 'test@customer.com', '0902222222', 'Pass@123', N'Khách hàng', 
        N'456 Test St', '1995-05-05', N'Nữ');
```

### UpdateAdminAndCustomer
```sql
-- Cập nhật thông tin người dùng
UPDATE NguoiDung 
SET hoTen = N'Nguyễn Văn An Updated',
    soDienThoai = '0909999999',
    diaChi = N'New Address',
    ngayCapNhat = GETDATE()
WHERE maNguoiDung = 'ND001';
```

### EditRoleByUser
```sql
-- Thay đổi vai trò người dùng
UPDATE NguoiDung 
SET vaiTro = N'Quản trị',
    ngayCapNhat = GETDATE()
WHERE maNguoiDung = 'ND006';
```

### ActivateUser
```sql
-- Kích hoạt/Vô hiệu hóa người dùng
UPDATE NguoiDung 
SET trangThaiHoatDong = 1, -- 1: Active, 0: Inactive
    ngayCapNhat = GETDATE()
WHERE maNguoiDung = 'ND006';
```

## 2. ADMIN - QUẢN LÝ LOẠI PHÒNG

### GetAllLoaiPhong
```sql
-- Lấy tất cả loại phòng
SELECT * FROM LoaiPhong 
WHERE trangThaiHoatDong = 1
ORDER BY giaCoBan;
```

### CreateLoaiPhong
```sql
INSERT INTO LoaiPhong (maLoaiPhong, tenLoaiPhong, moTa, giaCoBan, soNguoiToiDa, loaiGiuong, dienTichPhong, tienNghi)
VALUES ('LP031', N'Test Room Type', N'Test description', 2000000, 2, N'1 King Bed', 30, N'WiFi, TV');
```

### UpdateLoaiPhong
```sql
UPDATE LoaiPhong 
SET tenLoaiPhong = N'Updated Room Type',
    giaCoBan = 2500000
WHERE maLoaiPhong = 'LP001';
```

### ActivateLoaiPhong
```sql
UPDATE LoaiPhong 
SET trangThaiHoatDong = 0
WHERE maLoaiPhong = 'LP001';
```

### DeleteLoaiPhong
```sql
-- Chỉ xóa khi không có phòng nào sử dụng
DELETE FROM LoaiPhong 
WHERE maLoaiPhong = 'LP031' 
AND NOT EXISTS (SELECT 1 FROM Phong WHERE maLoaiPhong = 'LP031');
```

## 3. ADMIN - QUẢN LÝ PHÒNG

### GetAllPhong
```sql
SELECT P.*, LP.tenLoaiPhong, LP.giaCoBan
FROM Phong P
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
ORDER BY P.tang, P.soPhong;
```

### CreatePhong
```sql
INSERT INTO Phong (maPhong, soPhong, maLoaiPhong, tang, trangThai, moTa)
VALUES ('P0031', '1001', 'LP001', 10, N'Còn trống', N'Test room');
```

### UpdatePhong
```sql
UPDATE Phong 
SET trangThai = N'Bảo trì',
    moTa = N'Đang bảo trì định kỳ',
    ngayCapNhat = GETDATE()
WHERE maPhong = 'P0001';
```

### GetHistoryStatePhong
```sql
-- Lịch sử trạng thái phòng qua các đơn đặt
SELECT P.soPhong, DP.maDatPhong, DP.ngayNhanPhong, DP.ngayTraPhong, 
       DP.trangThaiDatPhong, ND.hoTen
FROM DatPhong DP
JOIN Phong P ON DP.maPhong = P.maPhong
JOIN NguoiDung ND ON DP.maKhachHang = ND.maNguoiDung
WHERE P.maPhong = 'P0001'
ORDER BY DP.ngayNhanPhong DESC;
```

## 4. ADMIN - QUẢN LÝ ĐƠN ĐẶT PHÒNG

### GetAllDonDatPhongFilter
```sql
-- Lấy tất cả đơn đặt phòng với filter
SELECT DP.*, ND.hoTen, P.soPhong, LP.tenLoaiPhong
FROM DatPhong DP
JOIN NguoiDung ND ON DP.maKhachHang = ND.maNguoiDung
JOIN Phong P ON DP.maPhong = P.maPhong
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
WHERE DP.trangThaiDatPhong = N'Đã xác nhận' -- Filter theo trạng thái
ORDER BY DP.ngayNhanPhong DESC;
```

### SearchDonDatFilter
```sql
-- Tìm kiếm đơn đặt phòng
SELECT DP.*, ND.hoTen, P.soPhong
FROM DatPhong DP
JOIN NguoiDung ND ON DP.maKhachHang = ND.maNguoiDung
JOIN Phong P ON DP.maPhong = P.maPhong
WHERE ND.hoTen LIKE N'%Nguyễn%' 
   OR P.soPhong LIKE '%101%'
   OR DP.maDatPhong LIKE '%DP001%';
```

### CreateDonDatPhong (Offline)
```sql
INSERT INTO DatPhong (maDatPhong, maKhachHang, maPhong, ngayNhanPhong, ngayTraPhong, 
                      soKhach, tienPhong, tienDatCoc, trangThaiDatPhong, nguoiTao)
VALUES ('DP031', 'ND006', 'P0001', '2025-12-20', '2025-12-22', 
        2, 2400000, 800000, N'Đã xác nhận', 'ND001');
```

### CheckinDonDatPhong
```sql
-- Check-in: Cập nhật trạng thái đơn đặt và phòng
BEGIN TRANSACTION;
UPDATE DatPhong SET trangThaiDatPhong = N'Đã nhận phòng' WHERE maDatPhong = 'DP001';
UPDATE Phong SET trangThai = N'Đang sử dụng' WHERE maPhong = (SELECT maPhong FROM DatPhong WHERE maDatPhong = 'DP001');
COMMIT;
```

### CheckoutDonDatPhong
```sql
-- Check-out: Cập nhật trạng thái
BEGIN TRANSACTION;
UPDATE DatPhong SET trangThaiDatPhong = N'Đã trả phòng' WHERE maDatPhong = 'DP002';
UPDATE Phong SET trangThai = N'Còn trống' WHERE maPhong = (SELECT maPhong FROM DatPhong WHERE maDatPhong = 'DP002');
COMMIT;
```

### CancelDonDatPhong
```sql
UPDATE DatPhong 
SET trangThaiDatPhong = N'Đã hủy',
    lyDoHuy = N'Khách yêu cầu hủy',
    ngayHuy = GETDATE()
WHERE maDatPhong = 'DP001';
```

## 5. ADMIN - QUẢN LÝ DỊCH VỤ

### GetAllDichVu
```sql
SELECT * FROM DichVu 
WHERE trangThaiHoatDong = 1
ORDER BY loaiDichVu, tenDichVu;
```

### CreateDichVu
```sql
INSERT INTO DichVu (maDichVu, tenDichVu, moTa, giaDichVu, loaiDichVu)
VALUES ('DV031', N'Test Service', N'Test description', 500000, N'Spa');
```

### UpdateDichVu
```sql
UPDATE DichVu 
SET tenDichVu = N'Updated Service',
    giaDichVu = 600000
WHERE maDichVu = 'DV001';
```

### DeleteDichVu
```sql
-- Chỉ xóa khi không có đơn đặt nào sử dụng
DELETE FROM DichVu 
WHERE maDichVu = 'DV031'
AND NOT EXISTS (SELECT 1 FROM DichVuDatPhong WHERE maDichVu = 'DV031');
```

## 6. ADMIN - QUẢN LÝ DỊCH VỤ ĐẶT PHÒNG

### GetAllDichVuDatPhong
```sql
SELECT DVD.*, DV.tenDichVu, DV.loaiDichVu, DP.maDatPhong
FROM DichVuDatPhong DVD
JOIN DichVu DV ON DVD.maDichVu = DV.maDichVu
JOIN DatPhong DP ON DVD.maDatPhong = DP.maDatPhong
ORDER BY DVD.ngaySuDung DESC;
```

### CreateDichVuDatPhong
```sql
INSERT INTO DichVuDatPhong (maDichVuDatPhong, maDatPhong, maDichVu, soLuong, donGia, thanhTien)
VALUES ('DVD31', 'DP001', 'DV001', 2, 800000, 1600000);
```

### UpdateDichVuDatPhong
```sql
UPDATE DichVuDatPhong 
SET soLuong = 3,
    thanhTien = 3 * donGia
WHERE maDichVuDatPhong = 'DVD01';
```

### DeleteDichVuDatPhong
```sql
DELETE FROM DichVuDatPhong 
WHERE maDichVuDatPhong = 'DVD01';
```

## 7. ADMIN - QUẢN LÝ THANH TOÁN

### GetAllThanhToanFilter
```sql
SELECT TT.*, DP.maDatPhong, ND.hoTen
FROM ThanhToan TT
JOIN DatPhong DP ON TT.maDatPhong = DP.maDatPhong
JOIN NguoiDung ND ON DP.maKhachHang = ND.maNguoiDung
WHERE TT.trangThaiThanhToan = N'Thành công'
ORDER BY TT.ngayThanhToan DESC;
```

### CreateThanhToan
```sql
INSERT INTO ThanhToan (maThanhToan, maDatPhong, soTien, phuongThucThanhToan, ghiChu, nguoiXuLy)
VALUES ('TT037', 'DP001', 1000000, N'Tiền mặt', N'Thanh toán đặt cọc', 'ND001');
```

### GetHistoryThanhToanFilter
```sql
-- Lịch sử thanh toán của một đơn đặt
SELECT * FROM ThanhToan 
WHERE maDatPhong = 'DP002'
ORDER BY ngayThanhToan;
```

## 8. ADMIN - BÁO CÁO & THỐNG KÊ

### Room Occupancy Rate Report
```sql
-- Tỷ lệ lấp đầy phòng theo ngày
SELECT 
    CAST(ngayNhanPhong AS DATE) AS Ngay,
    COUNT(DISTINCT maPhong) AS SoPhongDaDat,
    (SELECT COUNT(*) FROM Phong WHERE trangThaiHoatDong = 1) AS TongSoPhong,
    CAST(COUNT(DISTINCT maPhong) * 100.0 / (SELECT COUNT(*) FROM Phong WHERE trangThaiHoatDong = 1) AS DECIMAL(5,2)) AS TyLeLayDay
FROM DatPhong
WHERE trangThaiDatPhong IN (N'Đã xác nhận', N'Đã nhận phòng')
GROUP BY CAST(ngayNhanPhong AS DATE)
ORDER BY Ngay DESC;

-- Theo loại phòng
SELECT 
    LP.tenLoaiPhong,
    COUNT(DISTINCT P.maPhong) AS SoPhong,
    COUNT(DISTINCT CASE WHEN P.trangThai != N'Còn trống' THEN P.maPhong END) AS SoPhongDangDung,
    CAST(COUNT(DISTINCT CASE WHEN P.trangThai != N'Còn trống' THEN P.maPhong END) * 100.0 / COUNT(DISTINCT P.maPhong) AS DECIMAL(5,2)) AS TyLeLayDay
FROM LoaiPhong LP
LEFT JOIN Phong P ON LP.maLoaiPhong = P.maLoaiPhong
GROUP BY LP.tenLoaiPhong;
```

### Revenue Report
```sql
-- Doanh thu từ phòng theo loại
SELECT 
    LP.tenLoaiPhong,
    COUNT(DP.maDatPhong) AS SoDonDat,
    SUM(DP.tienPhong) AS TongDoanhThuPhong
FROM DatPhong DP
JOIN Phong P ON DP.maPhong = P.maPhong
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
WHERE DP.trangThaiDatPhong = N'Đã trả phòng'
GROUP BY LP.tenLoaiPhong
ORDER BY TongDoanhThuPhong DESC;

-- Doanh thu từ dịch vụ theo loại
SELECT 
    DV.loaiDichVu,
    SUM(DVD.thanhTien) AS TongDoanhThuDichVu
FROM DichVuDatPhong DVD
JOIN DichVu DV ON DVD.maDichVu = DV.maDichVu
GROUP BY DV.loaiDichVu
ORDER BY TongDoanhThuDichVu DESC;

-- Tổng doanh thu
SELECT 
    SUM(TT.soTien) AS TongDoanhThu,
    SUM(CASE WHEN TT.phuongThucThanhToan = N'Tiền mặt' THEN TT.soTien ELSE 0 END) AS DoanhThuTienMat,
    SUM(CASE WHEN TT.phuongThucThanhToan = N'Chuyển khoản' THEN TT.soTien ELSE 0 END) AS DoanhThuChuyenKhoan
FROM ThanhToan TT
WHERE TT.trangThaiThanhToan = N'Thành công';
```

### Most Booked Rooms
```sql
SELECT TOP 10
    P.soPhong,
    LP.tenLoaiPhong,
    COUNT(DP.maDatPhong) AS SoLanDat
FROM DatPhong DP
JOIN Phong P ON DP.maPhong = P.maPhong
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
GROUP BY P.soPhong, LP.tenLoaiPhong
ORDER BY SoLanDat DESC;
```

### Most Used Services
```sql
SELECT TOP 10
    DV.tenDichVu,
    DV.loaiDichVu,
    SUM(DVD.soLuong) AS TongSoLuongSuDung,
    SUM(DVD.thanhTien) AS TongDoanhThu
FROM DichVuDatPhong DVD
JOIN DichVu DV ON DVD.maDichVu = DV.maDichVu
GROUP BY DV.tenDichVu, DV.loaiDichVu
ORDER BY TongSoLuongSuDung DESC;
```

### Room Status Report
```sql
SELECT 
    trangThai,
    COUNT(*) AS SoLuong
FROM Phong
WHERE trangThaiHoatDong = 1
GROUP BY trangThai;
```

### Room Booking Report
```sql
SELECT 
    COUNT(*) AS TongSoDonDat,
    SUM(CASE WHEN trangThaiDatPhong = N'Chờ xác nhận' THEN 1 ELSE 0 END) AS ChoXacNhan,
    SUM(CASE WHEN trangThaiDatPhong = N'Đã xác nhận' THEN 1 ELSE 0 END) AS DaXacNhan,
    SUM(CASE WHEN trangThaiDatPhong = N'Đã nhận phòng' THEN 1 ELSE 0 END) AS DaNhanPhong,
    SUM(CASE WHEN trangThaiDatPhong = N'Đã trả phòng' THEN 1 ELSE 0 END) AS DaTraPhong,
    SUM(CASE WHEN trangThaiDatPhong = N'Đã hủy' THEN 1 ELSE 0 END) AS DaHuy,
    CAST(SUM(CASE WHEN trangThaiDatPhong = N'Đã hủy' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS TyLeHuy
FROM DatPhong;
```

## 9. KHÁCH HÀNG - NGƯỜI DÙNG

### RegisterCustomer
```sql
INSERT INTO NguoiDung (maNguoiDung, hoTen, email, soDienThoai, matKhau, vaiTro)
VALUES ('ND033', N'New Customer', 'newcust@email.com', '0903333333', 'Pass@123', N'Khách hàng');
```

### LoginCustomer
```sql
SELECT * FROM NguoiDung 
WHERE email = 'hieu.nguyen@gmail.com' 
  AND matKhau = 'Customer@123'
  AND vaiTro = N'Khách hàng'
  AND trangThaiHoatDong = 1;
```

### GetProfileCustomer
```sql
SELECT * FROM NguoiDung 
WHERE maNguoiDung = 'ND006';
```

### UpdateProfileCustomer
```sql
UPDATE NguoiDung 
SET hoTen = N'Updated Name',
    soDienThoai = '0905555555',
    diaChi = N'New Address',
    ngayCapNhat = GETDATE()
WHERE maNguoiDung = 'ND006';
```

## 10. KHÁCH HÀNG - PHÒNG & LOẠI PHÒNG

### SearchPhong & Filter
```sql
-- Tìm phòng còn trống theo ngày và loại phòng
SELECT P.*, LP.*
FROM Phong P
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
WHERE P.trangThai = N'Còn trống'
  AND P.trangThaiHoatDong = 1
  AND LP.trangThaiHoatDong = 1
  AND P.maPhong NOT IN (
      SELECT maPhong FROM DatPhong 
      WHERE trangThaiDatPhong IN (N'Đã xác nhận', N'Đã nhận phòng')
        AND ngayNhanPhong <= '2025-12-25'
        AND ngayTraPhong >= '2025-12-20'
  )
ORDER BY LP.giaCoBan;
```

### GetAllListPhong & FilterTypePhong
```sql
SELECT P.*, LP.*
FROM Phong P
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
WHERE P.trangThai = N'Còn trống'
  AND LP.maLoaiPhong = 'LP001'
ORDER BY P.soPhong;
```

## 11. KHÁCH HÀNG - ĐẶT PHÒNG

### CreateDonDatPhong (Online)
```sql
INSERT INTO DatPhong (maDatPhong, maKhachHang, maPhong, ngayNhanPhong, ngayTraPhong, 
                      soKhach, tienPhong, tienDatCoc, trangThaiDatPhong, nguoiTao)
VALUES ('DP032', 'ND006', 'P0001', '2025-12-25', '2025-12-27', 
        2, 2400000, 800000, N'Chờ xác nhận', 'ND006');
```

### GetAllDonDatPhongById
```sql
SELECT DP.*, P.soPhong, LP.tenLoaiPhong
FROM DatPhong DP
JOIN Phong P ON DP.maPhong = P.maPhong
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
WHERE DP.maKhachHang = 'ND006'
ORDER BY DP.ngayDat DESC;
```

### GetDetailDonDatPhongById
```sql
SELECT DP.*, P.soPhong, LP.*, ND.hoTen, ND.email, ND.soDienThoai
FROM DatPhong DP
JOIN Phong P ON DP.maPhong = P.maPhong
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
JOIN NguoiDung ND ON DP.maKhachHang = ND.maNguoiDung
WHERE DP.maDatPhong = 'DP001';
```

### Print Invoice
```sql
-- Dữ liệu cho hóa đơn
SELECT 
    DP.maDatPhong,
    DP.ngayNhanPhong,
    DP.ngayTraPhong,
    DP.tienPhong,
    ND.hoTen,
    ND.email,
    ND.soDienThoai,
    P.soPhong,
    LP.tenLoaiPhong,
    COALESCE(SUM(DVD.thanhTien), 0) AS tongTienDichVu,
    DP.tienPhong + COALESCE(SUM(DVD.thanhTien), 0) AS tongCong
FROM DatPhong DP
JOIN NguoiDung ND ON DP.maKhachHang = ND.maNguoiDung
JOIN Phong P ON DP.maPhong = P.maPhong
JOIN LoaiPhong LP ON P.maLoaiPhong = LP.maLoaiPhong
LEFT JOIN DichVuDatPhong DVD ON DP.maDatPhong = DVD.maDatPhong
WHERE DP.maDatPhong = 'DP002'
GROUP BY DP.maDatPhong, DP.ngayNhanPhong, DP.ngayTraPhong, DP.tienPhong,
         ND.hoTen, ND.email, ND.soDienThoai, P.soPhong, LP.tenLoaiPhong;
```

## 12. KHÁCH HÀNG - DỊCH VỤ

### GetAllDichVu
```sql
SELECT * FROM DichVu 
WHERE trangThaiHoatDong = 1
ORDER BY loaiDichVu, giaDichVu;
```

### GetDetailDichVu
```sql
SELECT * FROM DichVu 
WHERE maDichVu = 'DV001';
```

### GetAllDichVuDatPhong
```sql
-- Dịch vụ của khách hàng theo đơn đặt
SELECT DVD.*, DV.tenDichVu, DV.loaiDichVu
FROM DichVuDatPhong DVD
JOIN DichVu DV ON DVD.maDichVu = DV.maDichVu
WHERE DVD.maDatPhong = 'DP002'
ORDER BY DVD.ngaySuDung DESC;
```

## 13. KHÁCH HÀNG - THANH TOÁN

### GetAllHistoryThanhToan
```sql
-- Lịch sử thanh toán của khách hàng
SELECT TT.*, DP.maDatPhong
FROM ThanhToan TT
JOIN DatPhong DP ON TT.maDatPhong = DP.maDatPhong
WHERE DP.maKhachHang = 'ND006'
ORDER BY TT.ngayThanhToan DESC;
```

### Payment
```sql
INSERT INTO ThanhToan (maThanhToan, maDatPhong, soTien, phuongThucThanhToan, ghiChu)
VALUES ('TT038', 'DP001', 2000000, N'Chuyển khoản', N'Thanh toán phần còn lại');
```

### GetStatusThanhToan
```sql
-- Kiểm tra trạng thái thanh toán của đơn đặt
SELECT 
    DP.maDatPhong,
    DP.tienPhong,
    COALESCE(SUM(DVD.thanhTien), 0) AS tongTienDichVu,
    DP.tienPhong + COALESCE(SUM(DVD.thanhTien), 0) AS tongPhaiTra,
    COALESCE(SUM(TT.soTien), 0) AS daThanhToan,
    DP.tienPhong + COALESCE(SUM(DVD.thanhTien), 0) - COALESCE(SUM(TT.soTien), 0) AS conLai
FROM DatPhong DP
LEFT JOIN DichVuDatPhong DVD ON DP.maDatPhong = DVD.maDatPhong
LEFT JOIN ThanhToan TT ON DP.maDatPhong = TT.maDatPhong AND TT.trangThaiThanhToan = N'Thành công'
WHERE DP.maDatPhong = 'DP001'
GROUP BY DP.maDatPhong, DP.tienPhong;
```
