# GIẢI THÍCH VỀ THUỘC TÍNH `trangThaiHoatDong`

## ❓ Câu hỏi: Tại sao cần thuộc tính `trangThaiHoatDong`?

Thuộc tính `trangThaiHoatDong` (BIT - TRUE/FALSE) xuất hiện trong các bảng:
- ✅ `NguoiDung` - Kích hoạt/Vô hiệu hóa tài khoản
- ✅ `LoaiPhong` - Kích hoạt/Vô hiệu hóa loại phòng
- ✅ `Phong` - Kích hoạt/Vô hiệu hóa phòng
- ✅ `DichVu` - Kích hoạt/Vô hiệu hóa dịch vụ

---

## 🎯 TẠI SAO CẦN GIỮ THUỘC TÍNH NÀY?

### **KHUYẾN NGHỊ: TUYỆT ĐỐI NÊN GIỮ!**

Đây là implementation của **SOFT DELETE PATTERN** - một best practice trong thiết kế database cho hệ thống thực tế.

---

## 📊 SO SÁNH: SOFT DELETE vs HARD DELETE

### 1. **HARD DELETE** (Xóa thật sự - KHÔNG NÊN)
```sql
-- Xóa hẳn khỏi database
DELETE FROM NguoiDung WHERE maNguoiDung = 'ND001';
```

❌ **Vấn đề:**
- Mất hẳn dữ liệu, không thể khôi phục
- Phá vỡ Foreign Key nếu có dữ liệu liên quan
- Mất lịch sử, không audit được
- Vi phạm pháp luật về lưu trữ dữ liệu khách hàng

### 2. **SOFT DELETE** (Đánh dấu không hoạt động - NÊN DÙNG)
```sql
-- Chỉ đánh dấu là không hoạt động
UPDATE NguoiDung 
SET trangThaiHoatDong = 0 
WHERE maNguoiDung = 'ND001';
```

✅ **Lợi ích:**
- Giữ nguyên dữ liệu, có thể khôi phục
- Không phá vỡ Foreign Key
- Giữ lịch sử đầy đủ
- Tuân thủ quy định pháp luật

---

## 💡 TRƯỜNG HỢP SỬ DỤNG THỰC TẾ

### 📌 **Bảng: NguoiDung**

#### Tình huống 1: Khách hàng vi phạm chính sách
```
Khách hàng ND006 đã:
- Hủy đơn liên tục 5 lần
- Đặt phòng ảo
- Không thanh toán

→ Admin cần "khóa" tài khoản này
```

**Giải pháp với `trangThaiHoatDong`:**
```sql
-- Vô hiệu hóa tài khoản
UPDATE NguoiDung 
SET trangThaiHoatDong = 0,
    ngayCapNhat = GETDATE()
WHERE maNguoiDung = 'ND006';
```

**Kết quả:**
- ✅ Khách không thể đăng nhập
- ✅ Lịch sử đặt phòng vẫn còn (để tham khảo)
- ✅ Dữ liệu thanh toán vẫn còn (để đối soát)
- ✅ Có thể kích hoạt lại nếu khách hàng khắc phục

**Nếu dùng DELETE:**
```sql
DELETE FROM NguoiDung WHERE maNguoiDung = 'ND006';
```
❌ **Lỗi:** Cannot delete because Foreign Key exists in `DatPhong`  
❌ Mất hết lịch sử đặt phòng của khách  
❌ Không thể audit được ai đã đặt phòng đó

---

#### Tình huống 2: Nhân viên nghỉ việc
```
Admin ND002 (Nguyễn Thị Bình) nghỉ việc
Nhưng đã xử lý 50+ đơn đặt phòng
```

**Giải pháp:**
```sql
-- Vô hiệu hóa tài khoản nhân viên
UPDATE NguoiDung 
SET trangThaiHoatDong = 0
WHERE maNguoiDung = 'ND002';
```

**Lợi ích:**
- ✅ Nhân viên không thể đăng nhập
- ✅ Lịch sử "ai xử lý đơn nào" vẫn còn
- ✅ Có thể xem báo cáo hiệu suất của nhân viên cũ
- ✅ Dữ liệu audit đầy đủ

---

### 📌 **Bảng: LoaiPhong**

#### Tình huống: Ngừng kinh doanh loại phòng
```
Khách sạn quyết định:
- Không còn cung cấp "Presidential Suite" (LP013)
- Nhưng trong quá khứ đã có 20 đơn đặt phòng loại này
- Cần xem lại doanh thu từ loại phòng này
```

**Giải pháp:**
```sql
-- Vô hiệu hóa loại phòng
UPDATE LoaiPhong 
SET trangThaiHoatDong = 0
WHERE maLoaiPhong = 'LP013';
```

**Kết quả:**
- ✅ Không hiển thị trong tìm kiếm cho khách
- ✅ Lịch sử đặt phòng cũ vẫn còn
- ✅ Có thể xem báo cáo: "Doanh thu từ Presidential Suite"
- ✅ Dữ liệu vẫn đầy đủ nếu muốn kinh doanh lại

**Nếu dùng DELETE:**
```sql
DELETE FROM LoaiPhong WHERE maLoaiPhong = 'LP013';
```
❌ **Lỗi:** Cannot delete - Foreign Key từ bảng `Phong`  
❌ Không biết đơn cũ thuộc loại phòng gì  
❌ Mất dữ liệu báo cáo doanh thu

---

### 📌 **Bảng: Phong**

#### Tình huống: Phòng cần sửa chữa lâu dài
```
Phòng 601 (P0013 - Presidential Suite):
- Bị hư hỏng nặng
- Cần sửa chữa 6 tháng
- Không thể cho thuê
```

**Giải pháp kết hợp:**
```sql
-- Cập nhật trạng thái
UPDATE Phong 
SET trangThai = N'Bảo trì',           -- Đang bảo trì
    trangThaiHoatDong = 0              -- Không cho phép đặt
WHERE maPhong = 'P0013';
```

**Phân biệt 2 thuộc tính:**

| Thuộc tính | Ý nghĩa | Giá trị |
|-----------|---------|---------|
| `trangThai` | Tình trạng phòng **TẠM THỜI** | Còn trống / Đã đặt / Đang sử dụng / **Bảo trì** |
| `trangThaiHoatDong` | Phòng có **HOẠT ĐỘNG KINH DOANH** không? | 1 = Có / 0 = Không |

**Trường hợp khác nhau:**

**A. Phòng bảo trì TẠM THỜI (1-2 ngày)**
```sql
trangThai = N'Bảo trì'
trangThaiHoatDong = 1  -- Vẫn hoạt động, chỉ tạm bảo trì
```
→ Sau 2 ngày sửa xong, chuyển về "Còn trống"

**B. Phòng ngừng hoạt động LÂU DÀI (6 tháng hoặc vĩnh viễn)**
```sql
trangThai = N'Bảo trì'
trangThaiHoatDong = 0  -- Ngừng hoạt động lâu dài
```
→ Không cho đặt, không hiển thị trong hệ thống

---

### 📌 **Bảng: DichVu**

#### Tình huống: Ngừng cung cấp dịch vụ theo mùa
```
Dịch vụ "Hồ bơi cao cấp" (DV022):
- Mùa đông (tháng 12-2): Đóng cửa bảo trì
- Mùa hè: Hoạt động bình thường
```

**Giải pháp:**
```sql
-- Tháng 12: Tạm ngừng
UPDATE DichVu 
SET trangThaiHoatDong = 0
WHERE maDichVu = 'DV022';

-- Tháng 3: Kích hoạt lại
UPDATE DichVu 
SET trangThaiHoatDong = 1
WHERE maDichVu = 'DV022';
```

**Lợi ích:**
- ✅ Không hiển thị dịch vụ cho khách khi đóng cửa
- ✅ Lịch sử sử dụng dịch vụ vẫn còn
- ✅ Dễ dàng kích hoạt lại khi mở cửa
- ✅ Báo cáo theo mùa chính xác

---

## 🔍 CÁCH SỬ DỤNG TRONG CODE

### 1. **Khi QUERY - Chỉ lấy dữ liệu ĐANG HOẠT ĐỘNG**

```csharp
// ĐÚNG: Chỉ lấy loại phòng đang hoạt động
var loaiPhongs = _context.LoaiPhongs
    .Where(lp => lp.TrangThaiHoatDong == true)
    .ToList();

// ĐÚNG: Chỉ cho phép đăng nhập tài khoản đang hoạt động
var user = _context.NguoiDungs
    .FirstOrDefault(u => u.Email == email 
                      && u.MatKhau == password 
                      && u.TrangThaiHoatDong == true);

// ĐÚNG: Chỉ hiển thị dịch vụ đang cung cấp
var dichVus = _context.DichVus
    .Where(dv => dv.TrangThaiHoatDong == true)
    .ToList();
```

### 2. **Khi "XÓA" - Dùng SOFT DELETE**

```csharp
// ĐÚNG: Soft Delete
public void DeactivateAccount(string userId)
{
    var user = _context.NguoiDungs.Find(userId);
    if (user != null)
    {
        user.TrangThaiHoatDong = false;
        user.NgayCapNhat = DateTime.Now;
        _context.SaveChanges();
    }
}

// SAI: Hard Delete
public void DeleteAccount(string userId)
{
    var user = _context.NguoiDungs.Find(userId);
    _context.NguoiDungs.Remove(user); // ❌ KHÔNG NÊN
    _context.SaveChanges();
}
```

### 3. **Kích hoạt lại**

```csharp
// Kích hoạt lại tài khoản
public void ReactivateAccount(string userId)
{
    var user = _context.NguoiDungs.Find(userId);
    if (user != null)
    {
        user.TrangThaiHoatDong = true;
        user.NgayCapNhat = DateTime.Now;
        _context.SaveChanges();
    }
}
```

---

## 📋 BẢN TÓM TẮT QUYẾT ĐỊNH

### ✅ **NÊN GIỮ `trangThaiHoatDong` KHI:**

| Tình huống | Lý do |
|-----------|-------|
| Có Foreign Key tới bảng khác | Tránh lỗi khi xóa |
| Cần lưu lịch sử | Audit trail |
| Có thể kích hoạt lại | Linh hoạt |
| Dữ liệu có giá trị | Phân tích, báo cáo |
| Yêu cầu pháp luật | GDPR, luật bảo vệ dữ liệu |

### ❌ **CÓ THỂ BỎ `trangThaiHoatDong` KHI:**

| Tình huống | Giải pháp thay thế |
|-----------|-------------------|
| Bảng tra cứu đơn giản, không có FK | Có thể DELETE thật |
| Dữ liệu tạm thời, không quan trọng | Có thể DELETE |
| Cache, session | Có thể DELETE |

---

## 🎯 KẾT LUẬN VÀ KHUYẾN NGHỊ

### **CHO DỰ ÁN KHÁCH SẠN CỦA BẠN:**

#### ✅ **BẮT BUỘC GIỮ:**

1. **NguoiDung.trangThaiHoatDong**
   - Khóa tài khoản khách hàng vi phạm
   - Vô hiệu hóa tài khoản nhân viên nghỉ việc
   - Giữ lịch sử "ai đã làm gì"

2. **LoaiPhong.trangThaiHoatDong**
   - Ngừng kinh doanh loại phòng
   - Giữ lịch sử doanh thu
   - Có thể kinh doanh lại

3. **Phong.trangThaiHoatDong**
   - Phòng ngừng hoạt động lâu dài
   - Phòng bị hỏng, chờ sửa chữa
   - Khác với `trangThai` (tạm thời)

4. **DichVu.trangThaiHoatDong**
   - Dịch vụ theo mùa
   - Giữ lịch sử sử dụng
   - Dễ kích hoạt lại

---

## 📝 CODE MẪU: XỬ LÝ TRONG VIEW

### Admin Dashboard - Quản lý Người dùng
```csharp
// Controller
public ActionResult ManageUsers()
{
    // Hiển thị CẢ tài khoản đang hoạt động VÀ đã vô hiệu hóa
    var users = _context.NguoiDungs
        .OrderByDescending(u => u.TrangThaiHoatDong) // Active trước
        .ThenBy(u => u.HoTen)
        .ToList();
    
    return View(users);
}

[HttpPost]
public ActionResult ToggleUserStatus(string userId)
{
    var user = _context.NguoiDungs.Find(userId);
    if (user != null)
    {
        // Toggle trạng thái
        user.TrangThaiHoatDong = !user.TrangThaiHoatDong;
        user.NgayCapNhat = DateTime.Now;
        _context.SaveChanges();
    }
    
    return RedirectToAction("ManageUsers");
}
```

### View (Razor)
```html
<table class="table">
    <thead>
        <tr>
            <th>Mã</th>
            <th>Họ tên</th>
            <th>Email</th>
            <th>Vai trò</th>
            <th>Trạng thái</th>
            <th>Thao tác</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var user in Model)
        {
            <tr class="@(user.TrangThaiHoatDong ? "" : "text-muted")">
                <td>@user.MaNguoiDung</td>
                <td>@user.HoTen</td>
                <td>@user.Email</td>
                <td>@user.VaiTro</td>
                <td>
                    @if (user.TrangThaiHoatDong)
                    {
                        <span class="badge bg-success">Đang hoạt động</span>
                    }
                    else
                    {
                        <span class="badge bg-danger">Đã vô hiệu hóa</span>
                    }
                </td>
                <td>
                    <form method="post" action="@Url.Action("ToggleUserStatus")">
                        <input type="hidden" name="userId" value="@user.MaNguoiDung" />
                        @if (user.TrangThaiHoatDong)
                        {
                            <button type="submit" class="btn btn-sm btn-warning">
                                <i class="fa fa-ban"></i> Vô hiệu hóa
                            </button>
                        }
                        else
                        {
                            <button type="submit" class="btn btn-sm btn-success">
                                <i class="fa fa-check"></i> Kích hoạt
                            </button>
                        }
                    </form>
                </td>
            </tr>
        }
    </tbody>
</table>
```

---

## 🚀 BEST PRACTICES

### 1. **Luôn check `trangThaiHoatDong` khi query**
```csharp
// ✅ ĐÚNG
var activeRooms = _context.Phongs
    .Where(p => p.TrangThaiHoatDong == true)
    .ToList();

// ❌ SAI - Sẽ lấy cả phòng đã vô hiệu hóa
var allRooms = _context.Phongs.ToList();
```

### 2. **Không bao giờ DELETE trực tiếp**
```csharp
// ✅ ĐÚNG - Soft Delete
user.TrangThaiHoatDong = false;

// ❌ SAI - Hard Delete
_context.Users.Remove(user);
```

### 3. **Ghi log khi thay đổi trạng thái**
```csharp
// Nên có bảng AuditLog
var log = new AuditLog
{
    Action = "DEACTIVATE_USER",
    UserId = userId,
    PerformedBy = currentAdminId,
    Timestamp = DateTime.Now,
    Details = $"Vô hiệu hóa tài khoản {user.Email}"
};
_context.AuditLogs.Add(log);
```

---

## 📌 TÓM TẮT

| Thuộc tính | Mục đích | Quyết định |
|-----------|----------|------------|
| `NguoiDung.trangThaiHoatDong` | Khóa/Mở tài khoản | ✅ **GIỮ** |
| `LoaiPhong.trangThaiHoatDong` | Ngừng/Tiếp tục kinh doanh loại phòng | ✅ **GIỮ** |
| `Phong.trangThaiHoatDong` | Ngừng hoạt động phòng lâu dài | ✅ **GIỮ** |
| `DichVu.trangThaiHoatDong` | Tạm ngừng/Kích hoạt dịch vụ | ✅ **GIỮ** |

### **⭐ KHUYẾN NGHỊ CUỐI CÙNG:**
**TUYỆT ĐỐI GIỮ THUỘC TÍNH `trangThaiHoatDong` - Đây là best practice cho hệ thống production!**

---

© 2025 - Hotel Management System - Best Practices
