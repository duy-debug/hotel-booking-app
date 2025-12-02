- Admin
QuanLyNguoiDung
- GetAllAdminAndCustomer
- CreateAdminAndCustomer
- UpdateAdminAndCustomer
- EditRoleByUser
- ActivateUser
QuanLyLoaiPhong
- GetAllLoaiPhong
- CreateLoaiPhong
- UpdateLoaiPhong
- ActivateLoaiPhong
- DeleteLoaiPhong khi khong co phong nao su dung.
QuanLyPhong
- GetAllPhong
- CreatePhong
- UpdatePhong
- ActivatePhong
- GetHistoryStatePhong
QuanLyDonDatPhong
- GetAllDonDatPhongFilter
- SearchDonDatFilter
- CreateDonDatPhong(Offline)
- UpdateDonDatPhong
- ChangeStatusDonDatPhong
- CheckinDonDatPhong
- CheckoutDonDatPhong
- CancelDonDatPhong
QuanLyDichVu
- GetAllDichVu
- CreateDichVu
- UpdateDichVu
- ActivateDichVu
- DeleteDichVu khi khong co don dat phong nao su dung.
QuanLyDichVuDatPhong
- GetAllDichVuDatPhong
- CreateDichVuDatPhong
- UpdateDichVuDatPhong
- DeleteDichVuDatPhong (nếu chưa sử dụng)
QuanLyThanhToan
- GetAllThanhToanFilter
- SearchThanhToanFilter
- ProcessThanhToan
- ProcessThanhToanCompensation
- CreateThanhToan
- UpdateThanhToan
- ActivateThanhToan
- GetHistoryThanhToanFilter
BaoCao&ThongKe
    - RoomOccupancyRateReport:
    - By day/week/month/year
    - By room type
    - Revenue report:
    - Room revenue by type
    - Service revenue by type
    Total revenue
    - Revenue chart over time
    - Most booked rooms
    - Most used services
    - Room status report:
    - Number of available rooms
    - Number of booked rooms
    - Number of rooms in use
    - Number of rooms under maintenance
    Room booking report:
    - Total bookings
    - Number of bookings awaiting confirmation
    - Number of confirmed bookings
    - Number of cancelled bookings
    - Cancellation rate
KhachHang
NguoiDung
- RegisterCustomer
- LoginCustomer
- GetProfileCustomer
- UpdateProfileCustomer (not change Email)
- ForgotPasswordCustomer
Phong&LoaiPhong
- SearchPhong(Filter,SortResultWhenSearchPhong)
- GetAllListPhong&FilterTypePhong (Còn trống)
- GetAllListLoaiPhong
DatPhong (Online)
- CreateDonDatPhong(Online)
- MessageWhenCreateDonDatPhong
DatPhong
- GetAllDonDatPhongById
- GetDetailDonDatPhongById
- CancelDonDatPhong
- Print Invoice (Print/Download PDF Invoice,Send Invoice via Email)
DichVu & DichVuDatPhong
- GetAllDichVu
- GetDetailDichVu
- CreateDichVuDatPhong
- GetAllDichVuDatPhong
- CancelDichVuDatPhong
ThanhToan
- GetAllHistoryThanhToan
- Payment (Tien Mat, Chuyen Khoan)
- GetStatusThanhToan
- PrintInvoice