# HƯỚNG DẪN XÂY DỰNG HỆ THỐNG ASP.NET MVC

## 🎯 Tổng quan

Dự án: **Hotel Management System**  
Framework: **ASP.NET MVC 5** hoặc **ASP.NET Core MVC**  
Database: **SQL Server**  
ORM: **Entity Framework**

---

## 📁 CẤU TRÚC THỨ MỤC

```
HotelManagement/
│
├── Controllers/              # Xử lý logic nghiệp vụ
│   ├── AccountController.cs         # Đăng nhập, Đăng ký
│   ├── AdminController.cs           # Chức năng Admin
│   ├── RoomController.cs            # Quản lý phòng
│   ├── BookingController.cs         # Đặt phòng
│   ├── ServiceController.cs         # Dịch vụ
│   └── PaymentController.cs         # Thanh toán
│
├── Models/                   # Model classes (Entity)
│   ├── NguoiDung.cs
│   ├── LoaiPhong.cs
│   ├── Phong.cs
│   ├── DatPhong.cs
│   ├── DichVu.cs
│   ├── DichVuDatPhong.cs
│   ├── ThanhToan.cs
│   └── ViewModels/          # ViewModel cho View
│       ├── LoginViewModel.cs
│       ├── BookingViewModel.cs
│       └── DashboardViewModel.cs
│
├── Views/                    # Giao diện Razor
│   ├── Shared/
│   │   ├── _Layout.cshtml           # Layout chung
│   │   ├── _AdminLayout.cshtml      # Layout Admin
│   │   └── _CustomerLayout.cshtml   # Layout Khách hàng
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   └── Profile.cshtml
│   ├── Admin/
│   │   ├── Dashboard.cshtml
│   │   ├── ManageUsers.cshtml
│   │   ├── ManageRooms.cshtml
│   │   └── Reports.cshtml
│   ├── Room/
│   │   ├── Index.cshtml             # Danh sách phòng
│   │   ├── Details.cshtml           # Chi tiết phòng
│   │   └── Search.cshtml            # Tìm kiếm
│   ├── Booking/
│   │   ├── Create.cshtml            # Đặt phòng
│   │   ├── MyBookings.cshtml        # Đơn của tôi
│   │   └── Details.cshtml
│   └── Payment/
│       ├── Checkout.cshtml
│       └── Invoice.cshtml
│
├── Data/                     # Database Context
│   └── HotelDbContext.cs
│
├── Services/                 # Business Logic Layer
│   ├── IBookingService.cs
│   ├── BookingService.cs
│   ├── IPaymentService.cs
│   └── PaymentService.cs
│
└── wwwroot/                  # Static files
    ├── css/
    ├── js/
    └── images/
```

---

## 🗃️ MODELS (Entity Classes)

### NguoiDung.cs
```csharp
using System;
using System.ComponentModel.DataAnnotations;

public class NguoiDung
{
    [Key]
    [StringLength(5)]
    public string MaNguoiDung { get; set; }

    [Required]
    [StringLength(100)]
    public string HoTen { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(20)]
    public string SoDienThoai { get; set; }

    [Required]
    [StringLength(255)]
    public string MatKhau { get; set; }

    [Required]
    [StringLength(20)]
    public string VaiTro { get; set; } // "Quản trị" hoặc "Khách hàng"

    [StringLength(255)]
    public string DiaChi { get; set; }

    public DateTime? NgaySinh { get; set; }

    [StringLength(10)]
    public string GioiTinh { get; set; }

    public bool TrangThaiHoatDong { get; set; } = true;

    public DateTime NgayTao { get; set; } = DateTime.Now;

    public DateTime NgayCapNhat { get; set; } = DateTime.Now;

    // Navigation Properties
    public virtual ICollection<DatPhong> DatPhongs { get; set; }
}
```

### DatPhong.cs
```csharp
public class DatPhong
{
    [Key]
    [StringLength(5)]
    public string MaDatPhong { get; set; }

    [Required]
    public string MaKhachHang { get; set; }

    [Required]
    public string MaPhong { get; set; }

    [Required]
    public DateTime NgayNhanPhong { get; set; }

    [Required]
    public DateTime NgayTraPhong { get; set; }

    public int SoKhach { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTien { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TienDatCoc { get; set; }

    [StringLength(20)]
    public string TrangThaiDatPhong { get; set; }

    public string YeuCauDacBiet { get; set; }

    public DateTime NgayDat { get; set; } = DateTime.Now;

    public string NguoiTao { get; set; }

    // Navigation Properties
    public virtual NguoiDung KhachHang { get; set; }
    public virtual Phong Phong { get; set; }
    public virtual ICollection<DichVuDatPhong> DichVuDatPhongs { get; set; }
    public virtual ICollection<ThanhToan> ThanhToans { get; set; }
}
```

---

## 🎮 CONTROLLERS

### AccountController.cs (Đăng ký, Đăng nhập)
```csharp
public class AccountController : Controller
{
    private readonly HotelDbContext _context;

    public AccountController(HotelDbContext context)
    {
        _context = context;
    }

    // GET: /Account/Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.NguoiDungs
                .FirstOrDefault(u => u.Email == model.Email && u.MatKhau == model.Password);

            if (user != null && user.TrangThaiHoatDong)
            {
                // Lưu session
                Session["UserId"] = user.MaNguoiDung;
                Session["UserName"] = user.HoTen;
                Session["UserRole"] = user.VaiTro;

                // Redirect theo vai trò
                if (user.VaiTro == "Quản trị")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
        }

        return View(model);
    }

    // GET: /Account/Register
    public ActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new NguoiDung
            {
                MaNguoiDung = GenerateUserId(),
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                MatKhau = model.MatKhau, // Nên hash password
                VaiTro = "Khách hàng",
                TrangThaiHoatDong = true
            };

            _context.NguoiDungs.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        return View(model);
    }

    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Login");
    }
}
```

### BookingController.cs (Đặt phòng)
```csharp
[Authorize] // Yêu cầu đăng nhập
public class BookingController : Controller
{
    private readonly HotelDbContext _context;

    public BookingController(HotelDbContext context)
    {
        _context = context;
    }

    // GET: /Booking/Create
    public ActionResult Create(string maPhong, DateTime? checkIn, DateTime? checkOut)
    {
        var phong = _context.Phongs
            .Include(p => p.LoaiPhong)
            .FirstOrDefault(p => p.MaPhong == maPhong);

        var model = new BookingViewModel
        {
            MaPhong = maPhong,
            TenPhong = phong.SoPhong,
            NgayNhanPhong = checkIn ?? DateTime.Today.AddDays(1),
            NgayTraPhong = checkOut ?? DateTime.Today.AddDays(2),
            GiaPhong = phong.LoaiPhong.GiaCoBan
        };

        return View(model);
    }

    // POST: /Booking/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(BookingViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = Session["UserId"].ToString();
            var soNgay = (model.NgayTraPhong - model.NgayNhanPhong).Days;
            var tongTien = model.GiaPhong * soNgay;

            var booking = new DatPhong
            {
                MaDatPhong = GenerateBookingId(),
                MaKhachHang = userId,
                MaPhong = model.MaPhong,
                NgayNhanPhong = model.NgayNhanPhong,
                NgayTraPhong = model.NgayTraPhong,
                SoKhach = model.SoKhach,
                TongTien = tongTien,
                TienDatCoc = tongTien * 0.3m, // 30%
                TrangThaiDatPhong = "Chờ xác nhận",
                YeuCauDacBiet = model.YeuCauDacBiet,
                NguoiTao = userId
            };

            _context.DatPhongs.Add(booking);
            _context.SaveChanges();

            return RedirectToAction("Payment", "Payment", new { id = booking.MaDatPhong });
        }

        return View(model);
    }

    // GET: /Booking/MyBookings
    public ActionResult MyBookings()
    {
        var userId = Session["UserId"].ToString();
        var bookings = _context.DatPhongs
            .Include(d => d.Phong)
            .Include(d => d.Phong.LoaiPhong)
            .Where(d => d.MaKhachHang == userId)
            .OrderByDescending(d => d.NgayDat)
            .ToList();

        return View(bookings);
    }
}
```

### AdminController.cs (Admin)
```csharp
[Authorize(Roles = "Quản trị")] // Chỉ Admin
public class AdminController : Controller
{
    private readonly HotelDbContext _context;

    public AdminController(HotelDbContext context)
    {
        _context = context;
    }

    public ActionResult Dashboard()
    {
        var model = new DashboardViewModel
        {
            TongPhong = _context.Phongs.Count(),
            PhongTrong = _context.Phongs.Count(p => p.TrangThai == "Còn trống"),
            DonChoXacNhan = _context.DatPhongs.Count(d => d.TrangThaiDatPhong == "Chờ xác nhận"),
            DoanhThuThang = _context.ThanhToans
                .Where(t => t.NgayThanhToan.Month == DateTime.Now.Month)
                .Sum(t => t.SoTien)
        };

        return View(model);
    }

    // Xác nhận đơn đặt phòng ONLINE
    [HttpPost]
    public ActionResult ConfirmBooking(string id)
    {
        var booking = _context.DatPhongs.Find(id);
        if (booking != null)
        {
            booking.TrangThaiDatPhong = "Đã xác nhận";
            _context.SaveChanges();

            // Cập nhật trạng thái phòng
            var phong = _context.Phongs.Find(booking.MaPhong);
            phong.TrangThai = "Đã đặt";
            _context.SaveChanges();
        }

        return RedirectToAction("ManageBookings");
    }
}
```

---

## 🎨 VIEWS (Razor)

### Login.cshtml
```html
@model LoginViewModel

<div class="login-container">
    <h2>Đăng nhập</h2>
    
    @using (Html.BeginForm("Login", "Account", FormMethod.Post))
    {
        @Html.AntiForgeryToken()
        
        <div class="form-group">
            @Html.LabelFor(m => m.Email)
            @Html.TextBoxFor(m => m.Email, new { @class = "form-control" })
            @Html.ValidationMessageFor(m => m.Email)
        </div>
        
        <div class="form-group">
            @Html.LabelFor(m => m.Password)
            @Html.PasswordFor(m => m.Password, new { @class = "form-control" })
            @Html.ValidationMessageFor(m => m.Password)
        </div>
        
        <button type="submit" class="btn btn-primary">Đăng nhập</button>
    }
    
    <p>Chưa có tài khoản? @Html.ActionLink("Đăng ký ngay", "Register")</p>
</div>
```

### Search.cshtml (Tìm phòng)
```html
@model IEnumerable<Phong>

<h2>Tìm kiếm phòng</h2>

<form method="get" action="@Url.Action("Search", "Room")">
    <div class="row">
        <div class="col-md-3">
            <label>Ngày nhận phòng</label>
            <input type="date" name="checkIn" class="form-control" />
        </div>
        <div class="col-md-3">
            <label>Ngày trả phòng</label>
            <input type="date" name="checkOut" class="form-control" />
        </div>
        <div class="col-md-2">
            <label>Số khách</label>
            <input type="number" name="guests" class="form-control" />
        </div>
        <div class="col-md-2">
            <button type="submit" class="btn btn-primary">Tìm kiếm</button>
        </div>
    </div>
</form>

<div class="room-list">
    @foreach (var phong in Model)
    {
        <div class="room-card">
            <img src="@phong.LoaiPhong.HinhAnh" alt="@phong.LoaiPhong.TenLoaiPhong" />
            <h3>@phong.LoaiPhong.TenLoaiPhong</h3>
            <p>Phòng số: @phong.SoPhong</p>
            <p>Giá: @phong.LoaiPhong.GiaCoBan.ToString("N0") VNĐ/đêm</p>
            <a href="@Url.Action("Details", "Room", new { id = phong.MaPhong })" 
               class="btn btn-info">Xem chi tiết</a>
            <a href="@Url.Action("Create", "Booking", new { maPhong = phong.MaPhong })" 
               class="btn btn-success">Đặt ngay</a>
        </div>
    }
</div>
```

---

## 🔐 AUTHORIZATION (Phân quyền)

### Sử dụng Custom Authorize Attribute
```csharp
public class CustomAuthorize : AuthorizeAttribute
{
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        if (httpContext.Session["UserId"] == null)
            return false;

        if (!string.IsNullOrEmpty(Roles))
        {
            var userRole = httpContext.Session["UserRole"]?.ToString();
            return Roles.Split(',').Contains(userRole);
        }

        return true;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectResult("~/Account/Login");
    }
}
```

---

## 📦 NuGet Packages cần cài

```
Install-Package Microsoft.AspNet.Mvc
Install-Package EntityFramework
Install-Package Microsoft.AspNet.Identity.EntityFramework
Install-Package Bootstrap
Install-Package jQuery
```

---

## 🚀 BƯỚC TIẾN HÀNH

1. ✅ Tạo database từ file SQL đã có
2. ✅ Tạo ASP.NET MVC Project
3. ✅ Thêm Entity Framework và tạo DbContext
4. ✅ Tạo Models từ database (Database First)
5. ✅ Tạo Controllers và Views
6. ✅ Implement Authentication & Authorization
7. ✅ Test chức năng

---

**Bạn cần tôi hỗ trợ thêm gì không?**
