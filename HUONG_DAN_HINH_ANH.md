# Hướng dẫn sử dụng hình ảnh từ Database

## Format lưu đường dẫn ảnh trong Database

Trong bảng `LoaiPhong`, cột `hinhAnh` (kiểu `nvarchar(255)`) lưu đường dẫn ảnh.

### ✅ Các format được hỗ trợ:

#### 1. Đường dẫn đầy đủ (khuyến nghị):
```
~/Images/phonghangsang.jpeg
```

#### 2. Đường dẫn tương đối:
```
Images/phonghangsang.jpeg
```

#### 3. Chỉ tên file:
```
phonghangsang.jpeg
```

**Lưu ý:** Hệ thống sẽ tự động chuẩn hóa tất cả các format trên thành `~/Images/phonghangsang.jpeg`

### Nhiều ảnh (phân cách bằng dấu phẩy hoặc chấm phẩy):

```
~/Images/phonghangsang.jpeg,~/Images/phongvip1.jpg,~/Images/phongvip2.jpg
```

Hoặc:

```
phonghangsang.jpeg,phongvip1.jpg,phongvip2.jpg
```

Hoặc:

```
Images/phonghangsang.jpeg;Images/phongvip1.jpg;Images/phongvip2.jpg
```

## Ví dụ SQL UPDATE:

```sql
-- Cập nhật 1 ảnh
UPDATE LoaiPhong 
SET hinhAnh = '~/Images/phonghangsang.jpeg'
WHERE maLoaiPhong = 'LP001';

-- Cập nhật nhiều ảnh
UPDATE LoaiPhong 
SET hinhAnh = '~/Images/phonghangsang.jpeg,~/Images/phongvip1.jpg,~/Images/phongvip2.jpg,~/Images/phongvip3.jpg'
WHERE maLoaiPhong = 'LP002';
```

## Cách hoạt động:

1. **Trang chủ (Home/Index.cshtml)**: Hiển thị ảnh đầu tiên từ danh sách
2. **Danh sách phòng (Room65130650/Index.cshtml)**: Hiển thị ảnh đầu tiên từ danh sách
3. **Chi tiết phòng (Home/RoomDetail.cshtml)**: 
   - Nếu có 1 ảnh: Hiển thị ảnh đó, KHÔNG có thumbnail slider
   - Nếu có 2+ ảnh: Hiển thị ảnh đầu tiên, CÓ thumbnail slider để chuyển đổi

## Lưu ý:

- Tất cả ảnh phải được đặt trong thư mục `~/Images/`
- Tên file ảnh phải khớp chính xác với tên trong database
- Hệ thống tự động trim khoảng trắng thừa
- Nếu không có ảnh hoặc đường dẫn rỗng, sẽ sử dụng ảnh mặc định: `~/Images/phonghangsang.jpeg`
