CREATE DATABASE Project_65130650;
GO

USE Project_65130650;
GO

-- =============================================
-- Bảng: NguoiDung
-- Mô tả: Quản lý người dùng (Admin và Khách hàng)
-- =============================================
CREATE TABLE NguoiDung (
    maNguoiDung VARCHAR(5) PRIMARY KEY,
    hoTen NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    soDienThoai NVARCHAR(20),
    matKhau NVARCHAR(255) NOT NULL,
    vaiTro NVARCHAR(20) NOT NULL CHECK (vaiTro IN (N'Quản trị', N'Khách hàng')),
    diaChi NVARCHAR(255),
    ngaySinh DATE,
    gioiTinh NVARCHAR(10) CHECK (gioiTinh IN (N'Nam', N'Nữ', N'Khác')),
    trangThaiHoatDong BIT DEFAULT 1,
    ngayTao DATETIME DEFAULT GETDATE(),
    ngayCapNhat DATETIME DEFAULT GETDATE()
);
GO
-- =============================================
-- Bảng: LoaiPhong
-- Mô tả: Loại phòng khách sạn
-- =============================================
CREATE TABLE LoaiPhong (
    maLoaiPhong VARCHAR(5) PRIMARY KEY,
    tenLoaiPhong NVARCHAR(50) NOT NULL,
    moTa NVARCHAR(500),
    giaCoBan DECIMAL(18,2) NOT NULL,
    soNguoiToiDa INT NOT NULL,
    loaiGiuong NVARCHAR(50),
    dienTichPhong DECIMAL(10,2), -- Đơn vị: m²
    tienNghi NVARCHAR(1000), -- WiFi, TV, Minibar,...
    hinhAnh NVARCHAR(255),
    trangThaiHoatDong BIT DEFAULT 1,
    ngayTao DATETIME DEFAULT GETDATE()
);
GO
-- =============================================
-- Bảng: Phong
-- Mô tả: Phòng khách sạn
-- =============================================
CREATE TABLE Phong (
    maPhong VARCHAR(5) PRIMARY KEY,
    soPhong NVARCHAR(10) NOT NULL UNIQUE,
    maLoaiPhong VARCHAR(5) NOT NULL,
    tang INT NOT NULL,
    trangThai NVARCHAR(20) NOT NULL DEFAULT N'Còn trống' 
        CHECK (trangThai IN (N'Còn trống', N'Đã đặt', N'Đang sử dụng', N'Bảo trì')),
    moTa NVARCHAR(500),
    trangThaiHoatDong BIT DEFAULT 1,
    ngayTao DATETIME DEFAULT GETDATE(),
    ngayCapNhat DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (maLoaiPhong) REFERENCES LoaiPhong(maLoaiPhong)
);
GO
-- =============================================
-- Bảng: DatPhong
-- Mô tả: Quản lý đặt phòng
-- =============================================
CREATE TABLE DatPhong (
    maDatPhong VARCHAR(5) PRIMARY KEY,
    maKhachHang VARCHAR(5) NOT NULL,
    maPhong VARCHAR(5) NOT NULL,
    ngayNhanPhong DATETIME NOT NULL,
    ngayTraPhong DATETIME NOT NULL,
    soKhach INT NOT NULL,
    tienPhong DECIMAL(18,2) NOT NULL,  -- Đổi tên từ tongTien: Tiền phòng đã thỏa thuận tại thời điểm đặt
    tienDatCoc DECIMAL(18,2) DEFAULT 0,
    trangThaiDatPhong NVARCHAR(20) NOT NULL DEFAULT N'Chờ xác nhận'
        CHECK (trangThaiDatPhong IN (N'Chờ xác nhận', N'Đã xác nhận', N'Đã nhận phòng', N'Đã trả phòng', N'Đã hủy')),
    yeuCauDacBiet NVARCHAR(1000),
    ngayDat DATETIME DEFAULT GETDATE(),
    nguoiTao VARCHAR(5), -- Người tạo đặt phòng: Admin hoặc chính khách hàng
    ngayCapNhat DATETIME DEFAULT GETDATE(),
    lyDoHuy NVARCHAR(500),  
    ngayHuy DATETIME,
    FOREIGN KEY (maKhachHang) REFERENCES NguoiDung(maNguoiDung),
    FOREIGN KEY (maPhong) REFERENCES Phong(maPhong),
    FOREIGN KEY (nguoiTao) REFERENCES NguoiDung(maNguoiDung),
    CONSTRAINT CHK_ngayDatPhong CHECK (ngayTraPhong > ngayNhanPhong),
    CONSTRAINT CHK_tienDatCoc CHECK (tienDatCoc <= tienPhong AND tienDatCoc >= 0)
);
GO
-- =============================================
-- Bảng: DichVu
-- Mô tả: Dịch vụ khách sạn
-- =============================================
CREATE TABLE DichVu (
    maDichVu VARCHAR(5) PRIMARY KEY,
    tenDichVu NVARCHAR(100) NOT NULL,
    moTa NVARCHAR(500),
    giaDichVu DECIMAL(18,2) NOT NULL,
    loaiDichVu NVARCHAR(50), -- Spa, Nhà hàng, Giặt ủi, Phục vụ phòng.
    trangThaiHoatDong BIT DEFAULT 1,
    hinhAnh NVARCHAR(255),
    ngayTao DATETIME DEFAULT GETDATE()
);
GO
-- =============================================
-- Bảng: DichVuDatPhong
-- Mô tả: Dịch vụ được sử dụng trong đặt phòng
-- =============================================
CREATE TABLE DichVuDatPhong (
    maDichVuDatPhong VARCHAR(5) PRIMARY KEY,
    maDatPhong VARCHAR(5) NOT NULL,
    maDichVu VARCHAR(5) NOT NULL,
    soLuong INT DEFAULT 1,
    donGia DECIMAL(18,2) NOT NULL,
    thanhTien DECIMAL(18,2) NOT NULL,
    ngaySuDung DATETIME DEFAULT GETDATE(),
    ghiChu NVARCHAR(500),
    FOREIGN KEY (maDatPhong) REFERENCES DatPhong(maDatPhong),
    FOREIGN KEY (maDichVu) REFERENCES DichVu(maDichVu),
    -- Validation: Đảm bảo thanhTien = soLuong × donGia
    CONSTRAINT CHK_ThanhTienDichVu CHECK (thanhTien = soLuong * donGia),
    CONSTRAINT CHK_SoLuongDichVu CHECK (soLuong > 0)
);
GO
-- =============================================
-- Bảng: ThanhToan
-- Mô tả: Thanh toán (Hỗ trợ thanh toán từng phần)
-- LƯU Ý: Một đơn đặt phòng có thể có nhiều lần thanh toán (đặt cọc, thanh toán cuối)
-- =============================================
CREATE TABLE ThanhToan (
    maThanhToan VARCHAR(5) PRIMARY KEY,
    maDatPhong VARCHAR(5) NOT NULL,
    ngayThanhToan DATETIME DEFAULT GETDATE(),
    -- Số tiền thanh toán lần này
    soTien DECIMAL(18,2) NOT NULL,
    -- Thông tin chi tiết (tùy chọn - dùng để ghi chú)
    -- Chỉ điền đầy đủ trong lần thanh toán cuối cùng
    tienPhong DECIMAL(18,2) DEFAULT 0,      -- Tổng tiền phòng (chỉ ghi chú)
    tienDichVu DECIMAL(18,2) DEFAULT 0,     -- Tổng tiền dịch vụ (chỉ ghi chú)
    giamGia DECIMAL(18,2) DEFAULT 0,        -- Giảm giá nếu có
    phuongThucThanhToan NVARCHAR(50) NOT NULL 
        CHECK (phuongThucThanhToan IN (N'Tiền mặt', N'Chuyển khoản')),
    trangThaiThanhToan NVARCHAR(20) DEFAULT N'Thành công'
        CHECK (trangThaiThanhToan IN (N'Chờ xử lý', N'Thành công', N'Thất bại', N'Đã hoàn tiền')),
    maGiaoDich NVARCHAR(100),
    ghiChu NVARCHAR(500),
    nguoiXuLy VARCHAR(5), -- Admin xử lý thanh toán
    FOREIGN KEY (maDatPhong) REFERENCES DatPhong(maDatPhong),
    FOREIGN KEY (nguoiXuLy) REFERENCES NguoiDung(maNguoiDung),
    -- Validation: Số tiền phải > 0
    CONSTRAINT CHK_SoTienThanhToan CHECK (soTien > 0),
    CONSTRAINT CHK_GiamGiaThanhToan CHECK (giamGia >= 0)
);
GO
INSERT INTO NguoiDung (maNguoiDung, hoTen, email, soDienThoai, matKhau, vaiTro, diaChi, ngaySinh, gioiTinh, trangThaiHoatDong)
VALUES 
-- Admin users (5)
('ND001', N'Trần Văn An', 'admin.an@sheraton.com', '0901234567', 'Admin@123', N'Quản trị', N'123 Lê Lợi, Q1, TP.HCM', '1985-03-15', N'Nam', 1),
('ND002', N'Nguyễn Thị Bình', 'admin.binh@sheraton.com', '0901234568', 'Admin@123', N'Quản trị', N'456 Nguyễn Huệ, Q1, TP.HCM', '1988-07-20', N'Nữ', 1),
('ND003', N'Lê Hoàng Cường', 'admin.cuong@sheraton.com', '0901234569', 'Admin@123', N'Quản trị', N'789 Trần Hưng Đạo, Q5, TP.HCM', '1990-11-10', N'Nam', 1),
('ND004', N'Phạm Thị Dung', 'admin.dung@sheraton.com', '0901234570', 'Admin@123', N'Quản trị', N'321 Pasteur, Q3, TP.HCM', '1992-05-25', N'Nữ', 1),
('ND005', N'Hoàng Văn Em', 'admin.em@sheraton.com', '0901234571', 'Admin@123', N'Quản trị', N'654 Võ Văn Tần, Q3, TP.HCM', '1987-09-30', N'Nam', 1),

-- Customer users (25)
('ND006', N'Nguyễn Minh Hiếu', 'hieu.nguyen@gmail.com', '0912345678', 'Customer@123', N'Khách hàng', N'15 Lý Tự Trọng, Q1, TP.HCM', '1995-01-15', N'Nam', 1),
('ND007', N'Trần Thị Hoa', 'hoa.tran@gmail.com', '0912345679', 'Customer@123', N'Khách hàng', N'28 Điện Biên Phủ, Q3, TP.HCM', '1993-06-22', N'Nữ', 1),
('ND008', N'Lê Quang Khải', 'khai.le@yahoo.com', '0912345680', 'Customer@123', N'Khách hàng', N'42 Nam Kỳ Khởi Nghĩa, Q1, TP.HCM', '1990-08-18', N'Nam', 1),
('ND009', N'Phạm Thị Lan', 'lan.pham@hotmail.com', '0912345681', 'Customer@123', N'Khách hàng', N'67 Hai Bà Trưng, Q1, TP.HCM', '1996-03-10', N'Nữ', 1),
('ND010', N'Võ Minh Nam', 'nam.vo@gmail.com', '0912345682', 'Customer@123', N'Khách hàng', N'88 Đồng Khởi, Q1, TP.HCM', '1988-12-05', N'Nam', 1),
('ND011', N'Đặng Thị Oanh', 'oanh.dang@gmail.com', '0912345683', 'Customer@123', N'Khách hàng', N'99 Nguyễn Thị Minh Khai, Q3, TP.HCM', '1994-07-14', N'Nữ', 1),
('ND012', N'Bùi Văn Phong', 'phong.bui@yahoo.com', '0912345684', 'Customer@123', N'Khách hàng', N'111 Cách Mạng Tháng 8, Q10, TP.HCM', '1991-02-28', N'Nam', 1),
('ND013', N'Ngô Thị Quỳnh', 'quynh.ngo@gmail.com', '0912345685', 'Customer@123', N'Khách hàng', N'202 Lý Chính Thắng, Q3, TP.HCM', '1997-11-20', N'Nữ', 1),
('ND014', N'Trương Văn Sơn', 'son.truong@hotmail.com', '0912345686', 'Customer@123', N'Khách hàng', N'303 Võ Thị Sáu, Q3, TP.HCM', '1989-04-16', N'Nam', 1),
('ND015', N'Lý Thị Tâm', 'tam.ly@gmail.com', '0912345687', 'Customer@123', N'Khách hàng', N'404 Trường Sa, Q3, TP.HCM', '1995-09-08', N'Nữ', 1),
('ND016', N'Đinh Văn Út', 'ut.dinh@yahoo.com', '0912345688', 'Customer@123', N'Khách hàng', N'505 Hoàng Sa, Q1, TP.HCM', '1992-05-30', N'Nam', 1),
('ND017', N'Mai Thị Vân', 'van.mai@gmail.com', '0912345689', 'Customer@123', N'Khách hàng', N'606 Cống Quỳnh, Q1, TP.HCM', '1998-01-25', N'Nữ', 1),
('ND018', N'Phan Văn Xuân', 'xuan.phan@hotmail.com', '0912345690', 'Customer@123', N'Khách hàng', N'707 Bùi Viện, Q1, TP.HCM', '1987-10-12', N'Nam', 1),
('ND019', N'Dương Thị Yến', 'yen.duong@gmail.com', '0912345691', 'Customer@123', N'Khách hàng', N'808 Đề Thám, Q1, TP.HCM', '1993-06-18', N'Nữ', 1),
('ND020', N'Hồ Văn Anh', 'anh.ho@yahoo.com', '0912345692', 'Customer@123', N'Khách hàng', N'909 Phạm Ngũ Lão, Q1, TP.HCM', '1996-03-07', N'Nam', 1),
('ND021', N'Vũ Thị Bảo', 'bao.vu@gmail.com', '0912345693', 'Customer@123', N'Khách hàng', N'1010 Nguyễn Trãi, Q5, TP.HCM', '1990-12-22', N'Nữ', 1),
('ND022', N'Lâm Văn Cường', 'cuong.lam@hotmail.com', '0912345694', 'Customer@123', N'Khách hàng', N'1111 Lê Hồng Phong, Q10, TP.HCM', '1994-08-14', N'Nam', 1),
('ND023', N'Châu Thị Diễm', 'diem.chau@gmail.com', '0912345695', 'Customer@123', N'Khách hàng', N'1212 3 Tháng 2, Q10, TP.HCM', '1991-04-03', N'Nữ', 1),
('ND024', N'Tôn Văn Đức', 'duc.ton@yahoo.com', '0912345696', 'Customer@123', N'Khách hàng', N'1313 Nguyễn Văn Cừ, Q5, TP.HCM', '1988-11-28', N'Nam', 1),
('ND025', N'La Thị Hương', 'huong.la@gmail.com', '0912345697', 'Customer@123', N'Khách hàng', N'1414 An Dương Vương, Q5, TP.HCM', '1995-07-19', N'Nữ', 1),
('ND026', N'Tạ Văn Kiên', 'kien.ta@hotmail.com', '0912345698', 'Customer@123', N'Khách hàng', N'1515 Hùng Vương, Q5, TP.HCM', '1992-02-11', N'Nam', 1),
('ND027', N'Ông Thị Linh', 'linh.ong@gmail.com', '0912345699', 'Customer@123', N'Khách hàng', N'1616 Trần Phú, Q5, TP.HCM', '1997-09-26', N'Nữ', 1),
('ND028', N'Từ Văn Minh', 'minh.tu@yahoo.com', '0912345700', 'Customer@123', N'Khách hàng', N'1717 Lạc Long Quân, Q11, TP.HCM', '1989-05-15', N'Nam', 1),
('ND029', N'Đỗ Thị Nga', 'nga.do@gmail.com', '0912345701', 'Customer@123', N'Khách hàng', N'1818 Âu Cơ, Tân Bình, TP.HCM', '1993-01-08', N'Nữ', 1),
('ND030', N'Lư Văn Phú', 'phu.lu@hotmail.com', '0912345702', 'Customer@123', N'Khách hàng', N'1919 Hòa Bình, Tân Phú, TP.HCM', '1996-10-20', N'Nam', 1);
GO

-- =============================================
-- 2. LoaiPhong - 30 room types
-- Format: LP001, LP002, ..., LP030
-- =============================================
INSERT INTO LoaiPhong (maLoaiPhong, tenLoaiPhong, moTa, giaCoBan, soNguoiToiDa, loaiGiuong, dienTichPhong, tienNghi, hinhAnh, trangThaiHoatDong)
VALUES 
('LP001', N'Standard Single', N'Phòng đơn tiêu chuẩn, view thành phố', 1200000, 1, N'1 Single Bed', 22, N'WiFi, TV, Minibar, Điều hòa, Bàn làm việc', 'standard-single.jpg', 1),
('LP002', N'Standard Double', N'Phòng đôi tiêu chuẩn, view thành phố', 1500000, 2, N'1 Double Bed', 25, N'WiFi, TV, Minibar, Điều hòa, Két sắt', 'standard-double.jpg', 1),
('LP003', N'Standard Twin', N'Phòng đôi 2 giường đơn, view thành phố', 1500000, 2, N'2 Single Beds', 25, N'WiFi, TV, Minibar, Điều hòa, Bàn làm việc', 'standard-twin.jpg', 1),
('LP004', N'Superior Single', N'Phòng đơn cao cấp, view sông', 1800000, 1, N'1 Queen Bed', 28, N'WiFi, Smart TV, Minibar, Điều hòa, Bàn làm việc, Két sắt', 'superior-single.jpg', 1),
('LP005', N'Superior Double', N'Phòng đôi cao cấp, view sông', 2200000, 2, N'1 King Bed', 30, N'WiFi, Smart TV, Minibar, Điều hòa, Bồn tắm, Ban công', 'superior-double.jpg', 1),
('LP006', N'Deluxe City View', N'Phòng Deluxe view thành phố tuyệt đẹp', 2500000, 2, N'1 King Bed', 35, N'WiFi, Smart TV 50", Minibar cao cấp, Điều hòa, Bồn tắm, Ban công rộng', 'deluxe-city.jpg', 1),
('LP007', N'Deluxe River View', N'Phòng Deluxe view sông Sài Gòn', 2800000, 2, N'1 King Bed', 35, N'WiFi, Smart TV 50", Minibar cao cấp, Điều hòa, Jacuzzi, Ban công', 'deluxe-river.jpg', 1),
('LP008', N'Deluxe Ocean View', N'Phòng Deluxe view biển tuyệt đẹp', 3000000, 2, N'1 King Bed', 38, N'WiFi, Smart TV 55", Minibar cao cấp, Điều hòa, Jacuzzi, Ban công rộng', 'deluxe-ocean.jpg', 1),
('LP009', N'Executive Single', N'Phòng Executive dành cho doanh nhân', 2800000, 1, N'1 King Bed', 32, N'WiFi, Smart TV, Minibar, Điều hòa, Bàn làm việc lớn, Két sắt điện tử', 'executive-single.jpg', 1),
('LP010', N'Executive Double', N'Phòng Executive rộng rãi', 3200000, 2, N'1 King Bed', 40, N'WiFi, Smart TV 55", Minibar, Điều hòa, Phòng khách nhỏ, Bồn tắm massage', 'executive-double.jpg', 1),
('LP011', N'Junior Suite', N'Suite nhỏ với phòng khách riêng', 4000000, 2, N'1 King Bed', 45, N'WiFi, Smart TV 55", Minibar, Điều hòa, Phòng khách, Bồn tắm Jacuzzi', 'junior-suite.jpg', 1),
('LP012', N'Grand Suite', N'Suite lớn sang trọng', 5000000, 3, N'1 King Bed + Sofa Bed', 55, N'WiFi, 2 Smart TV 55", Minibar, Điều hòa, Phòng khách rộng, Bồn Jacuzzi, 2 Ban công', 'grand-suite.jpg', 1),
('LP013', N'Presidential Suite', N'Suite Tổng thống đẳng cấp nhất', 10000000, 4, N'2 King Beds', 100, N'WiFi, 3 Smart TV 65", Minibar cao cấp, 3 Điều hòa, Phòng khách 40m², Bồn Jacuzzi, Bếp nhỏ, Ban công riêng', 'presidential.jpg', 1),
('LP014', N'Family Room', N'Phòng gia đình rộng rãi', 3500000, 4, N'1 King + 2 Single Beds', 50, N'WiFi, 2 Smart TV, Minibar, Điều hòa, Khu vực sinh hoạt', 'family-room.jpg', 1),
('LP015', N'Connecting Rooms', N'2 phòng liền kề có cửa nối', 4000000, 4, N'2 King Beds', 60, N'WiFi, 2 Smart TV, 2 Minibar, 2 Điều hòa, Cửa nối giữa 2 phòng', 'connecting-rooms.jpg', 1),
('LP016', N'Studio Apartment', N'Căn hộ Studio có bếp nhỏ', 3800000, 2, N'1 King Bed', 42, N'WiFi, Smart TV, Minibar, Điều hòa, Bếp nhỏ, Bàn ăn', 'studio.jpg', 1),
('LP017', N'One Bedroom Apartment', N'Căn hộ 1 phòng ngủ', 5500000, 3, N'1 King Bed', 65, N'WiFi, 2 Smart TV, Minibar, Điều hòa, Bếp đầy đủ, Phòng khách, Bàn ăn', '1br-apartment.jpg', 1),
('LP018', N'Two Bedroom Apartment', N'Căn hộ 2 phòng ngủ', 8000000, 5, N'2 King Beds', 90, N'WiFi, 3 Smart TV, Minibar, 2 Điều hòa, Bếp đầy đủ, Phòng khách lớn, Phòng ăn', '2br-apartment.jpg', 1),
('LP019', N'Penthouse', N'Penthouse tầng cao nhất', 15000000, 6, N'3 King Beds', 150, N'WiFi, 4 Smart TV 65", 2 Minibar, 3 Điều hòa, Bếp cao cấp, Phòng khách 60m², 3 Ban công, Sân thượng riêng', 'penthouse.jpg', 1),
('LP020', N'Honeymoon Suite', N'Suite tân hôn lãng mạn', 6000000, 2, N'1 King Bed', 48, N'WiFi, Smart TV, Minibar, Điều hòa, Bồn tắm Jacuzzi đôi, Trang trí lãng mạn, Champagne', 'honeymoon.jpg', 1),
('LP021', N'Accessible Room', N'Phòng tiện nghi người khuyết tật', 1800000, 2, N'1 King Bed', 35, N'WiFi, TV, Minibar, Điều hòa, Phòng tắm dành cho người khuyết tật, Tay vịn', 'accessible.jpg', 1),
('LP022', N'Premium Club Room', N'Phòng Club với quyền lợi đặc biệt', 3800000, 2, N'1 King Bed', 38, N'WiFi, Smart TV, Minibar, Điều hòa, Quyền Club Lounge, Bữa sáng miễn phí', 'club-room.jpg', 1),
('LP023', N'Club Suite', N'Suite Club đẳng cấp', 5500000, 3, N'1 King Bed + Sofa', 58, N'WiFi, Smart TV 55", Minibar, Điều hòa, Phòng khách, Quyền Club Lounge, Happy Hour', 'club-suite.jpg', 1),
('LP024', N'Corner Suite', N'Suite góc 2 mặt view', 6500000, 3, N'1 King Bed', 65, N'WiFi, 2 Smart TV, Minibar, Điều hòa, Phòng khách, 2 Ban công góc, View 270°', 'corner-suite.jpg', 1),
('LP025', N'Garden View Room', N'Phòng view vườn xanh mát', 2000000, 2, N'1 Queen Bed', 30, N'WiFi, Smart TV, Minibar, Điều hòa, Ban công view vườn', 'garden-view.jpg', 1),
('LP026', N'Pool Access Room', N'Phòng có cửa ra hồ bơi', 3200000, 2, N'1 King Bed', 35, N'WiFi, Smart TV, Minibar, Điều hòa, Cửa trực tiếp ra hồ bơi', 'pool-access.jpg', 1),
('LP027', N'Cabana Room', N'Phòng kiểu Cabana bên hồ bơi', 4200000, 2, N'1 King Bed', 40, N'WiFi, Smart TV, Minibar, Điều hòa, Giường tắm nắng riêng, Gazebo', 'cabana.jpg', 1),
('LP028', N'Loft Suite', N'Suite kiểu Loft 2 tầng', 7000000, 4, N'1 King + 2 Single Beds', 75, N'WiFi, 2 Smart TV, Minibar, 2 Điều hòa, Thiết kế 2 tầng, Cầu thang nội thất', 'loft-suite.jpg', 1),
('LP029', N'Royal Suite', N'Suite Hoàng gia siêu sang', 12000000, 4, N'2 King Beds', 110, N'WiFi, 3 Smart TV 65", 2 Minibar, 2 Điều hòa, Phòng ăn riêng, Phòng làm việc, Bồn Jacuzzi, Piano', 'royal-suite.jpg', 1),
('LP030', N'Villa', N'Biệt thự riêng tư', 20000000, 8, N'4 King Beds', 200, N'WiFi, 5 Smart TV, Bếp đầy đủ, 4 Điều hòa, Hồ bơi riêng, Vườn riêng, BBQ, Phòng khách lớn, Phòng ăn', 'villa.jpg', 1);
GO

-- =============================================
-- 3. Phong - 30 rooms
-- Format: P0001, P0002, ..., P0030
-- =============================================
INSERT INTO Phong (maPhong, soPhong, maLoaiPhong, tang, trangThai, moTa, trangThaiHoatDong)
VALUES 
('P0001', '101', 'LP001', 1, N'Còn trống', N'Phòng Standard Single tầng 1', 1),
('P0002', '102', 'LP002', 1, N'Đã đặt', N'Phòng Standard Double tầng 1', 1),
('P0003', '201', 'LP003', 2, N'Đang sử dụng', N'Phòng Standard Twin tầng 2', 1),
('P0004', '202', 'LP004', 2, N'Còn trống', N'Phòng Superior Single tầng 2', 1),
('P0005', '203', 'LP005', 2, N'Đã đặt', N'Phòng Superior Double tầng 2', 1),
('P0006', '301', 'LP006', 3, N'Đang sử dụng', N'Phòng Deluxe City View tầng 3', 1),
('P0007', '302', 'LP007', 3, N'Còn trống', N'Phòng Deluxe River View tầng 3', 1),
('P0008', '303', 'LP008', 3, N'Đã đặt', N'Phòng Deluxe Ocean View tầng 3', 1),
('P0009', '401', 'LP009', 4, N'Còn trống', N'Phòng Executive Single tầng 4', 1),
('P0010', '402', 'LP010', 4, N'Đang sử dụng', N'Phòng Executive Double tầng 4', 1),
('P0011', '501', 'LP011', 5, N'Đã đặt', N'Junior Suite tầng 5', 1),
('P0012', '502', 'LP012', 5, N'Còn trống', N'Grand Suite tầng 5', 1),
('P0013', '601', 'LP013', 6, N'Còn trống', N'Presidential Suite tầng 6', 1),
('P0014', '602', 'LP014', 6, N'Đã đặt', N'Family Room tầng 6', 1),
('P0015', '701', 'LP015', 7, N'Đang sử dụng', N'Connecting Rooms tầng 7', 1),
('P0016', '702', 'LP016', 7, N'Còn trống', N'Studio Apartment tầng 7', 1),
('P0017', '801', 'LP017', 8, N'Đã đặt', N'One Bedroom Apartment tầng 8', 1),
('P0018', '802', 'LP018', 8, N'Còn trống', N'Two Bedroom Apartment tầng 8', 1),
('P0019', '901', 'LP019', 9, N'Còn trống', N'Penthouse tầng 9', 1),
('P0020', '902', 'LP020', 9, N'Đã đặt', N'Honeymoon Suite tầng 9', 1),
('P0021', '103', 'LP021', 1, N'Còn trống', N'Accessible Room tầng 1', 1),
('P0022', '204', 'LP022', 2, N'Đang sử dụng', N'Premium Club Room tầng 2', 1),
('P0023', '304', 'LP023', 3, N'Đã đặt', N'Club Suite tầng 3', 1),
('P0024', '403', 'LP024', 4, N'Còn trống', N'Corner Suite tầng 4', 1),
('P0025', '503', 'LP025', 5, N'Đang sử dụng', N'Garden View Room tầng 5', 1),
('P0026', '104', 'LP026', 1, N'Đã đặt', N'Pool Access Room tầng 1', 1),
('P0027', '105', 'LP027', 1, N'Còn trống', N'Cabana Room tầng 1', 1),
('P0028', '603', 'LP028', 6, N'Bảo trì', N'Loft Suite tầng 6 - đang bảo trì', 1),
('P0029', '703', 'LP029', 7, N'Đã đặt', N'Royal Suite tầng 7', 1),
('P0030', 'V01', 'LP030', 1, N'Còn trống', N'Villa - Khu biệt thự', 1);
GO

-- =============================================
-- 4. DichVu - 30 services
-- Format: DV001, DV002, ..., DV030
-- =============================================
INSERT INTO DichVu (maDichVu, tenDichVu, moTa, giaDichVu, loaiDichVu, hinhAnh, trangThaiHoatDong)
VALUES 
('DV001', N'Massage toàn thân 60 phút', N'Massage thư giãn toàn thân với tinh dầu thiên nhiên', 800000, N'Spa', 'massage-60.jpg', 1),
('DV002', N'Massage toàn thân 90 phút', N'Massage chuyên sâu kết hợp đá nóng', 1200000, N'Spa', 'massage-90.jpg', 1),
('DV003', N'Chăm sóc da mặt', N'Chăm sóc da mặt chuyên nghiệp với mỹ phẩm cao cấp', 900000, N'Spa', 'facial.jpg', 1),
('DV004', N'Gội đầu dưỡng sinh', N'Gội đầu massage thư giãn', 300000, N'Spa', 'hair-spa.jpg', 1),
('DV005', N'Bữa sáng buffet', N'Buffet sáng đa dạng món Á - Âu', 350000, N'Nhà hàng', 'breakfast.jpg', 1),
('DV006', N'Bữa trưa set menu', N'Set menu trưa 3 món', 450000, N'Nhà hàng', 'lunch-set.jpg', 1),
('DV007', N'Bữa tối à la carte', N'Thực đơn tối cao cấp theo yêu cầu', 800000, N'Nhà hàng', 'dinner.jpg', 1),
('DV008', N'Cocktail Bar', N'Đồ uống có cồn tại quầy bar', 200000, N'Nhà hàng', 'cocktail.jpg', 1),
('DV009', N'Room Service - Bữa sáng', N'Bữa sáng phục vụ tại phòng', 400000, N'Phục vụ phòng', 'room-breakfast.jpg', 1),
('DV010', N'Room Service - Bữa trưa', N'Bữa trưa phục vụ tại phòng', 500000, N'Phục vụ phòng', 'room-lunch.jpg', 1),
('DV011', N'Room Service - Bữa tối', N'Bữa tối phục vụ tại phòng', 650000, N'Phục vụ phòng', 'room-dinner.jpg', 1),
('DV012', N'Room Service - Đồ uống', N'Đồ uống phục vụ tại phòng', 150000, N'Phục vụ phòng', 'room-drinks.jpg', 1),
('DV013', N'Giặt khô', N'Dịch vụ giặt khô quần áo cao cấp', 80000, N'Giặt ủi', 'dry-clean.jpg', 1),
('DV014', N'Giặt ủi thường', N'Giặt ủi quần áo thông thường', 50000, N'Giặt ủi', 'laundry.jpg', 1),
('DV015', N'Ủi áo sơ mi', N'Ủi áo sơ mi chuyên nghiệp', 30000, N'Giặt ủi', 'iron-shirt.jpg', 1),
('DV016', N'Ủi vest/suit', N'Ủi vest hoặc bộ suit', 80000, N'Giặt ủi', 'iron-suit.jpg', 1),
('DV017', N'Đưa đón sân bay - 1 chiều', N'Dịch vụ đưa đón sân bay Tân Sơn Nhất', 500000, N'Vận chuyển', 'airport-transfer.jpg', 1),
('DV018', N'Đưa đón sân bay - Khứ hồi', N'Dịch vụ đưa đón sân bay khứ hồi', 900000, N'Vận chuyển', 'airport-roundtrip.jpg', 1),
('DV019', N'Thuê xe 4 chỗ - Nửa ngày', N'Thuê xe 4 chỗ có tài xế nửa ngày', 1200000, N'Vận chuyển', 'car-halfday.jpg', 1),
('DV020', N'Thuê xe 7 chỗ - Cả ngày', N'Thuê xe 7 chỗ có tài xế cả ngày', 2500000, N'Vận chuyển', 'car-fullday.jpg', 1),
('DV021', N'Thuê xe limousine', N'Thuê xe limousine sang trọng theo giờ', 3000000, N'Vận chuyển', 'limousine.jpg', 1),
('DV022', N'Hồ bơi cao cấp', N'Vé sử dụng hồ bơi tầng thượng', 200000, N'Giải trí', 'pool.jpg', 1),
('DV023', N'Phòng gym', N'Vé sử dụng phòng gym hiện đại', 150000, N'Giải trí', 'gym.jpg', 1),
('DV024', N'Sauna & Steam', N'Sử dụng phòng sauna và xông hơi', 250000, N'Giải trí', 'sauna.jpg', 1),
('DV025', N'Yoga class', N'Lớp yoga buổi sáng với HLV', 300000, N'Giải trí', 'yoga.jpg', 1),
('DV026', N'Tennis court', N'Thuê sân tennis 1 giờ', 400000, N'Giải trí', 'tennis.jpg', 1),
('DV027', N'Karaoke VIP room', N'Phòng karaoke VIP theo giờ', 500000, N'Giải trí', 'karaoke.jpg', 1),
('DV028', N'Baby sitting', N'Dịch vụ trông trẻ chuyên nghiệp theo giờ', 200000, N'Dịch vụ khác', 'babysit.jpg', 1),
('DV029', N'Trang trí phòng lãng mạn', N'Trang trí phòng cho dịp đặc biệt', 1500000, N'Dịch vụ khác', 'romantic-decor.jpg', 1),
('DV030', N'Florist - Hoa tươi', N'Hoa tươi trang trí phòng', 800000, N'Dịch vụ khác', 'flowers.jpg', 1);
GO

-- =============================================
-- 5. DatPhong - 30 bookings
-- Format: DP001, DP002, ..., DP030
-- =============================================
INSERT INTO DatPhong (maDatPhong, maKhachHang, maPhong, ngayNhanPhong, ngayTraPhong, soKhach, tienPhong, tienDatCoc, trangThaiDatPhong, yeuCauDacBiet, nguoiTao, lyDoHuy, ngayHuy)
VALUES 
('DP001', 'ND006', 'P0002', '2025-12-01', '2025-12-03', 2, 3000000, 1000000, N'Đã xác nhận', N'Yêu cầu phòng tầng cao', 'ND006', NULL, NULL),
('DP002', 'ND007', 'P0003', '2025-12-02', '2025-12-05', 2, 4500000, 1500000, N'Đã nhận phòng', N'Yêu cầu 2 chìa khóa phòng', 'ND001', NULL, NULL),
('DP003', 'ND008', 'P0005', '2025-12-03', '2025-12-06', 2, 6600000, 2000000, N'Đã xác nhận', N'Check-in sớm 12h trưa', 'ND008', NULL, NULL),
('DP004', 'ND009', 'P0006', '2025-12-04', '2025-12-07', 2, 7500000, 2500000, N'Đã nhận phòng', N'Yêu cầu view đẹp', 'ND002', NULL, NULL),
('DP005', 'ND010', 'P0008', '2025-12-05', '2025-12-08', 2, 9000000, 3000000, N'Đã xác nhận', N'Honeymoon package', 'ND010', NULL, NULL),
('DP006', 'ND011', 'P0010', '2025-12-06', '2025-12-09', 2, 9600000, 3000000, N'Đã nhận phòng', NULL, 'ND011', NULL, NULL),
('DP007', 'ND012', 'P0011', '2025-12-07', '2025-12-10', 2, 12000000, 4000000, N'Đã xác nhận', N'Late check-out 14h', 'ND003', NULL, NULL),
('DP008', 'ND013', 'P0014', '2025-12-08', '2025-12-11', 4, 10500000, 3500000, N'Đã xác nhận', N'Yêu cầu thêm giường phụ', 'ND013', NULL, NULL),
('DP009', 'ND014', 'P0015', '2025-12-09', '2025-12-12', 4, 12000000, 4000000, N'Đã nhận phòng', N'Gia đình có trẻ nhỏ', 'ND004', NULL, NULL),
('DP010', 'ND015', 'P0017', '2025-12-10', '2025-12-13', 3, 16500000, 5500000, N'Đã xác nhận', N'Business trip', 'ND015', NULL, NULL),
('DP011', 'ND016', 'P0020', '2025-12-11', '2025-12-14', 2, 18000000, 6000000, N'Đã xác nhận', N'Tuần trăng mật, cần trang trí lãng mạn', 'ND005', NULL, NULL),
('DP012', 'ND017', 'P0022', '2025-12-12', '2025-12-15', 2, 11400000, 3800000, N'Đã nhận phòng', NULL, 'ND017', NULL, NULL),
('DP013', 'ND018', 'P0023', '2025-12-13', '2025-12-16', 3, 16500000, 5500000, N'Đã xác nhận', N'Yêu cầu Club Lounge access', 'ND001', NULL, NULL),
('DP014', 'ND019', 'P0025', '2025-12-14', '2025-12-17', 2, 6000000, 2000000, N'Đã nhận phòng', N'Yêu cầu phòng yên tĩnh', 'ND019', NULL, NULL),
('DP015', 'ND020', 'P0026', '2025-12-15', '2025-12-18', 2, 9600000, 3000000, N'Đã xác nhận', NULL, 'ND020', NULL, NULL),
('DP016', 'ND021', 'P0029', '2025-12-16', '2025-12-19', 4, 36000000, 12000000, N'Đã xác nhận', N'VIP guest, cần phục vụ đặc biệt', 'ND002', NULL, NULL),
('DP017', 'ND022', 'P0004', '2025-11-20', '2025-11-23', 1, 5400000, 0, N'Đã trả phòng', NULL, 'ND022', NULL, NULL),
('DP018', 'ND023', 'P0007', '2025-11-21', '2025-11-24', 2, 8400000, 0, N'Đã trả phòng', N'Yêu cầu gối thêm', 'ND003', NULL, NULL),
('DP019', 'ND024', 'P0009', '2025-11-22', '2025-11-25', 1, 8400000, 0, N'Đã trả phòng', NULL, 'ND024', NULL, NULL),
('DP020', 'ND025', 'P0012', '2025-11-23', '2025-11-26', 3, 15000000, 0, N'Đã trả phòng', N'Corporate booking', 'ND004', NULL, NULL),
('DP021', 'ND026', 'P0001', '2025-12-20', '2025-12-22', 1, 2400000, 800000, N'Chờ xác nhận', NULL, 'ND026', NULL, NULL),
('DP022', 'ND027', 'P0002', '2025-12-21', '2025-12-23', 2, 3000000, 1000000, N'Chờ xác nhận', N'First time guest', 'ND027', NULL, NULL),
('DP023', 'ND028', 'P0016', '2025-12-22', '2025-12-25', 2, 11400000, 3800000, N'Chờ xác nhận', N'Yêu cầu bếp đầy đủ', 'ND005', NULL, NULL),
('DP024', 'ND029', 'P0018', '2025-12-23', '2025-12-26', 5, 24000000, 8000000, N'Chờ xác nhận', N'Extended stay', 'ND029', NULL, NULL),
('DP025', 'ND030', 'P0013', '2025-11-15', '2025-11-18', 4, 30000000, 10000000, N'Đã hủy', N'VIP booking', 'ND001', N'Khách thay đổi lịch trình', '2025-11-10'),
('DP026', 'ND006', 'P0024', '2025-12-25', '2025-12-28', 3, 19500000, 6500000, N'Chờ xác nhận', N'Dịp Giáng sinh', 'ND006', NULL, NULL),
('DP027', 'ND007', 'P0019', '2025-12-26', '2025-12-29', 6, 45000000, 15000000, N'Chờ xác nhận', N'Penthouse cho sự kiện', 'ND002', NULL, NULL),
('DP028', 'ND008', 'P0030', '2025-12-27', '2025-12-30', 8, 60000000, 20000000, N'Chờ xác nhận', N'Villa cho đại gia đình', 'ND003', NULL, NULL),
('DP029', 'ND009', 'P0027', '2025-12-28', '2025-12-31', 2, 12600000, 4200000, N'Chờ xác nhận', N'Đón năm mới bên hồ bơi', 'ND009', NULL, NULL),
('DP030', 'ND010', 'P0021', '2025-12-29', '2026-01-01', 2, 5400000, 1800000, N'Chờ xác nhận', N'Accessible room cho người khuyết tật', 'ND004', NULL, NULL);
GO

-- =============================================
-- 6. DichVuDatPhong - 30 service usage records
-- Format: DVD01, DVD02, ..., DVD30
-- =============================================
INSERT INTO DichVuDatPhong (maDichVuDatPhong, maDatPhong, maDichVu, soLuong, donGia, thanhTien, ngaySuDung, ghiChu)
VALUES 
('DVD01', 'DP002', 'DV001', 2, 800000, 1600000, '2025-12-03', N'Massage cho 2 người'),
('DVD02', 'DP002', 'DV005', 3, 350000, 1050000, '2025-12-02', N'Buffet sáng 3 ngày'),
('DVD03', 'DP003', 'DV002', 1, 1200000, 1200000, '2025-12-04', N'Massage 90 phút'),
('DVD04', 'DP003', 'DV009', 3, 400000, 1200000, '2025-12-03', N'Room service bữa sáng'),
('DVD05', 'DP004', 'DV005', 3, 350000, 1050000, '2025-12-04', N'Buffet sáng'),
('DVD06', 'DP004', 'DV013', 5, 80000, 400000, '2025-12-05', N'Giặt khô đồ công sở'),
('DVD07', 'DP005', 'DV001', 2, 800000, 1600000, '2025-12-06', N'Spa for couple'),
('DVD08', 'DP005', 'DV007', 2, 800000, 1600000, '2025-12-07', N'Bữa tối lãng mạn'),
('DVD09', 'DP006', 'DV029', 1, 1500000, 1500000, '2025-12-06', N'Trang trí phòng honeymoon'),
('DVD10', 'DP006', 'DV030', 2, 800000, 1600000, '2025-12-07', N'Hoa hồng trang trí'),
('DVD11', 'DP007', 'DV005', 3, 350000, 1050000, '2025-12-07', N'Buffet sáng'),
('DVD12', 'DP007', 'DV017', 1, 500000, 500000, '2025-12-07', N'Đưa đón sân bay đến'),
('DVD13', 'DP008', 'DV005', 3, 350000, 1050000, '2025-12-08', NULL),
('DVD14', 'DP008', 'DV022', 4, 200000, 800000, '2025-12-09', N'Vé hồ bơi cho gia đình'),
('DVD15', 'DP009', 'DV001', 2, 800000, 1600000, '2025-12-10', N'Massage thư giãn'),
('DVD16', 'DP009', 'DV019', 1, 1200000, 1200000, '2025-12-11', N'Thuê xe đi tham quan'),
('DVD17', 'DP010', 'DV005', 3, 350000, 1050000, '2025-12-10', N'Buffet sáng'),
('DVD18', 'DP010', 'DV025', 2, 300000, 600000, '2025-12-11', N'Yoga cho 2 người'),
('DVD19', 'DP011', 'DV029', 1, 1500000, 1500000, '2025-12-11', N'Trang trí honeymoon'),
('DVD20', 'DP011', 'DV007', 2, 800000, 1600000, '2025-12-12', N'Romantic dinner'),
('DVD21', 'DP012', 'DV001', 2, 800000, 1600000, '2025-12-13', N'Spa massage'),
('DVD22', 'DP012', 'DV018', 1, 900000, 900000, '2025-12-13', N'Đưa đón sân bay khứ hồi'),
('DVD23', 'DP013', 'DV005', 3, 350000, 1050000, '2025-12-14', NULL),
('DVD24', 'DP013', 'DV028', 8, 200000, 1600000, '2025-12-15', N'Baby sitting 8 giờ'),
('DVD25', 'DP014', 'DV005', 3, 350000, 1050000, '2025-12-15', N'Buffet sáng'),
('DVD26', 'DP014', 'DV022', 2, 200000, 400000, '2025-12-16', N'Hồ bơi'),
('DVD27', 'DP015', 'DV006', 3, 450000, 1350000, '2025-12-16', N'Business lunch'),
('DVD28', 'DP015', 'DV014', 10, 50000, 500000, '2025-12-17', N'Giặt ủi đồ công sở'),
('DVD29', 'DP016', 'DV029', 1, 1500000, 1500000, '2025-12-16', N'Decoration for VIP'),
('DVD30', 'DP016', 'DV030', 3, 800000, 2400000, '2025-12-17', N'Premium flowers');
GO

-- =============================================
-- 7. ThanhToan - 36 payment records
-- Format: TT001, TT002, ..., TT036
-- LƯU Ý: soTien = số tiền thanh toán lần này (hỗ trợ partial payment)
-- =============================================
INSERT INTO ThanhToan (maThanhToan, maDatPhong, ngayThanhToan, soTien, tienPhong, tienDichVu, giamGia, phuongThucThanhToan, trangThaiThanhToan, maGiaoDich, ghiChu, nguoiXuLy)
VALUES 
-- DP001: tienPhong = 3,000,000 (không có dịch vụ)
('TT001', 'DP001', '2025-11-28', 1000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN001', N'Đặt cọc', 'ND001'),
('TT002', 'DP001', '2025-12-03', 2000000, 3000000, 0, 0, N'Tiền mặt', N'Thành công', 'TXN002', N'Thanh toán còn lại', 'ND002'),
-- DP002: tienPhong = 4,500,000, dịch vụ = 2,650,000
('TT003', 'DP002', '2025-11-30', 1500000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN003', N'Đặt cọc', 'ND001'),
('TT004', 'DP002', '2025-12-05', 5650000, 4500000, 2650000, 0, N'Tiền mặt', N'Thành công', 'TXN004', N'Check-out (phòng + dịch vụ)', 'ND003'),
-- DP003: tienPhong = 6,600,000, dịch vụ = 2,400,000
('TT005', 'DP003', '2025-12-01', 2000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN005', N'Đặt cọc', 'ND003'),
-- DP004: tienPhong = 7,500,000, dịch vụ = 1,450,000
('TT006', 'DP004', '2025-12-02', 2500000, 0, 0, 0, N'Tiền mặt', N'Thành công', 'TXN006', N'Đặt cọc', 'ND001'),
('TT007', 'DP004', '2025-12-07', 6450000, 7500000, 1450000, 0, N'Chuyển khoản', N'Thành công', 'TXN007', N'Check-out (phòng + dịch vụ)', 'ND002'),
-- DP005: tienPhong = 9,000,000, dịch vụ = 3,200,000
('TT008', 'DP005', '2025-12-03', 3000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN008', N'Đặt cọc honeymoon', 'ND002'),
-- DP006: tienPhong = 9,600,000, dịch vụ = 3,100,000
('TT009', 'DP006', '2025-12-04', 3000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN009', N'Đặt cọc', 'ND001'),
('TT010', 'DP006', '2025-12-09', 9700000, 9600000, 3100000, 0, N'Tiền mặt', N'Thành công', 'TXN010', N'Check-out (phòng + dịch vụ)', 'ND002'),
-- DP007: tienPhong = 12,000,000, dịch vụ = 1,550,000
('TT011', 'DP007', '2025-12-05', 4000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN011', N'Đặt cọc', 'ND003'),
-- DP008: tienPhong = 10,500,000, dịch vụ = 1,850,000
('TT012', 'DP008', '2025-12-06', 3500000, 0, 0, 0, N'Tiền mặt', N'Thành công', 'TXN012', N'Đặt cọc', 'ND002'),
-- DP009: tienPhong = 12,000,000, dịch vụ = 2,800,000
('TT013', 'DP009', '2025-12-07', 4000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN013', N'Đặt cọc', 'ND001'),
('TT014', 'DP009', '2025-12-12', 10800000, 12000000, 2800000, 0, N'Tiền mặt', N'Thành công', 'TXN014', N'Check-out (phòng + dịch vụ)', 'ND004'),
-- DP010: tienPhong = 16,500,000, dịch vụ = 1,650,000
('TT015', 'DP010', '2025-12-08', 5500000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN015', N'Đặt cọc', 'ND004'),
-- DP011: tienPhong = 18,000,000, dịch vụ = 3,100,000
('TT016', 'DP011', '2025-12-09', 6000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN016', N'Đặt cọc honeymoon', 'ND002'),
-- DP012: tienPhong = 11,400,000, dịch vụ = 2,500,000
('TT017', 'DP012', '2025-12-10', 3800000, 0, 0, 0, N'Tiền mặt', N'Thành công', 'TXN017', N'Đặt cọc', 'ND001'),
('TT018', 'DP012', '2025-12-15', 10100000, 11400000, 2500000, 0, N'Chuyển khoản', N'Thành công', 'TXN018', N'Check-out (phòng + dịch vụ)', 'ND002'),
-- DP013: tienPhong = 16,500,000, dịch vụ = 2,650,000
('TT019', 'DP013', '2025-12-11', 5500000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN019', N'Đặt cọc', 'ND003'),
-- DP014: tienPhong = 6,000,000, dịch vụ = 1,450,000
('TT020', 'DP014', '2025-12-12', 2000000, 0, 0, 0, N'Tiền mặt', N'Thành công', 'TXN020', N'Đặt cọc', 'ND002'),
('TT021', 'DP014', '2025-12-17', 5450000, 6000000, 1450000, 0, N'Chuyển khoản', N'Thành công', 'TXN021', N'Check-out (phòng + dịch vụ)', 'ND001'),
-- DP015: tienPhong = 9,600,000, dịch vụ = 1,850,000
('TT022', 'DP015', '2025-12-13', 3000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN022', N'Đặt cọc', 'ND001'),
-- DP016: tienPhong = 36,000,000, dịch vụ = 3,900,000
('TT023', 'DP016', '2025-12-14', 12000000, 0, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN023', N'Đặt cọc VIP', 'ND005'),
-- DP017: tienPhong = 5,400,000 (ĐÃ TRẢ PHÒNG)
('TT024', 'DP017', '2025-11-23', 5400000, 5400000, 0, 0, N'Tiền mặt', N'Thành công', 'TXN024', N'Full payment', 'ND002'),
-- DP018: tienPhong = 8,400,000 (ĐÃ TRẢ PHÒNG)
('TT025', 'DP018', '2025-11-24', 8400000, 8400000, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN025', N'Full payment', 'ND001'),
-- DP019: tienPhong = 8,400,000 (ĐÃ TRẢ PHÒNG)
('TT026', 'DP019', '2025-11-25', 8400000, 8400000, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN026', N'Full payment', 'ND003'),
-- DP020: tienPhong = 15,000,000 (ĐÃ TRẢ PHÒNG)
('TT027', 'DP020', '2025-11-26', 15000000, 15000000, 0, 0, N'Chuyển khoản', N'Thành công', 'TXN027', N'Corporate booking', 'ND004'),
-- DP021: tienPhong = 2,400,000 (CHỜ XÁC NHẬN)
('TT028', 'DP021', '2025-12-18', 800000, 0, 0, 0, N'Chuyển khoản', N'Chờ xử lý', 'TXN028', N'Đặt cọc', 'ND001'),
-- DP022: tienPhong = 3,000,000 (CHỜ XÁC NHẬN)
('TT029', 'DP022', '2025-12-19', 1000000, 0, 0, 0, N'Chuyển khoản', N'Chờ xử lý', 'TXN029', N'Đặt cọc', 'ND002'),
-- DP023: tienPhong = 11,400,000 (CHỜ XÁC NHẬN)
('TT030', 'DP023', '2025-12-20', 3800000, 0, 0, 0, N'Tiền mặt', N'Chờ xử lý', 'TXN030', N'Đặt cọc', 'ND001'),
-- DP024: tienPhong = 24,000,000 (CHỜ XÁC NHẬN)
('TT031', 'DP024', '2025-12-21', 8000000, 0, 0, 0, N'Chuyển khoản', N'Chờ xử lý', 'TXN031', N'Đặt cọc', 'ND003'),
-- DP025: tienPhong = 30,000,000 (ĐÃ HỦY - HOÀN TIỀN)
('TT032', 'DP025', '2025-11-08', 10000000, 0, 0, 0, N'Chuyển khoản', N'Đã hoàn tiền', 'TXN032', N'Hoàn tiền do hủy', 'ND005'),
-- DP026: tienPhong = 19,500,000 (CHỜ XÁC NHẬN)
('TT033', 'DP026', '2025-12-22', 6500000, 0, 0, 0, N'Chuyển khoản', N'Chờ xử lý', 'TXN033', N'Đặt cọc', 'ND002'),
-- DP027: tienPhong = 45,000,000 (CHỜ XÁC NHẬN)
('TT034', 'DP027', '2025-12-23', 15000000, 0, 0, 0, N'Chuyển khoản', N'Chờ xử lý', 'TXN034', N'Đặt cọc Penthouse', 'ND004'),
-- DP028: tienPhong = 60,000,000 (CHỜ XÁC NHẬN)
('TT035', 'DP028', '2025-12-24', 20000000, 0, 0, 0, N'Chuyển khoản', N'Chờ xử lý', 'TXN035', N'Đặt cọc Villa', 'ND005'),
-- DP029: tienPhong = 12,600,000 (CHỜ XÁC NHẬN)
('TT036', 'DP029', '2025-12-25', 4200000, 0, 0, 0, N'Tiền mặt', N'Chờ xử lý', 'TXN036', N'Đặt cọc', 'ND001');
GO


