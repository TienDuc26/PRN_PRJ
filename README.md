# Hệ thống quản lý tour du lịch - TourViet

Hệ thống quản lý và đặt tour du lịch trực tuyến được xây dựng bằng **ASP.NET Core MVC (.NET 8)**, Entity Framework Core, SQL Server và ASP.NET Core Identity.

## Yêu cầu môi trường

- .NET 8 SDK
- SQL Server hoặc LocalDB (mặc định dùng LocalDB)
- Visual Studio 2022 / VS Code / Rider

## Cài đặt & chạy

```bash
# 1. Khôi phục packages
dotnet restore

# 2. Cập nhật database (tự động tạo schema + seed data khi chạy app)
dotnet ef database update

# 3. Chạy ứng dụng
dotnet run
```

Mở trình duyệt tại: `http://localhost:5000`

## Tài khoản mặc định

| Role     | Email                 | Mật khẩu     |
|----------|-----------------------|--------------|
| ADMIN    | admin@tour.com        | Admin@123    |
| STAFF    | staff@tour.com        | Staff@123    |
| CUSTOMER | customer@tour.com     | Customer@123 |

## Cấu trúc dự án

```
PRN222-PROJECT/
├── Areas/Admin/                  # Khu vực quản trị (STAFF/ADMIN)
│   ├── Controllers/
│   └── Views/
├── Controllers/                  # Controller cho Customer
├── Data/
│   ├── AppDbContext.cs
│   ├── Seed/DbInitializer.cs
│   └── Migrations/
├── Models/
│   ├── Entities/                 # ApplicationUser, Tour, Booking...
│   └── Enums/Enums.cs            # TourStatus, BookingStatus...
├── Services/
│   ├── Interfaces/
│   └── Implementations/          # Business logic
├── Helpers/                      # CodeGenerator, PriceCalculator, FileUpload
├── ViewModels/
├── Views/                        # Razor views cho Customer
└── wwwroot/
    ├── css/, js/
    └── uploads/                  # Ảnh tour, avatar, review
```

## Tính năng chính

### Phía khách hàng
- ✅ Đăng ký / đăng nhập / quên mật khẩu
- ✅ Xem danh sách tour với tìm kiếm, lọc, sắp xếp, phân trang
- ✅ Xem chi tiết tour, lịch trình, lịch khởi hành
- ✅ Đặt tour với luồng: chọn lịch → nhập khách → áp khuyến mãi → tính tiền → xác nhận
- ✅ Áp dụng mã khuyến mãi (kiểm tra real-time qua AJAX)
- ✅ Thanh toán (chuyển khoản / tiền mặt)
- ✅ Quản lý đơn của tôi, hủy đơn theo chính sách
- ✅ Đánh giá tour sau khi hoàn thành
- ✅ Quản lý hồ sơ cá nhân, đổi mật khẩu, upload avatar
- ✅ Xem thông báo

### Phía Admin/Staff
- ✅ Dashboard với các chỉ số: doanh thu, top tour, điểm đến phổ biến
- ✅ CRUD Tour, Điểm đến, Lịch trình, Lịch khởi hành
- ✅ Quản lý Booking: xem, xác nhận, hoàn thành, hủy
- ✅ Quản lý Thanh toán & hoàn tiền
- ✅ Quản lý Khách hàng (khóa/mở khóa)
- ✅ Quản lý Hướng dẫn viên + phân công (cảnh báo trùng lịch BR-06)
- ✅ Quản lý Đánh giá (ẩn/hiện)
- ✅ Quản lý Khuyến mãi (BR-07)
- ✅ Quản lý người dùng (chỉ Admin) + đổi role
- ✅ Báo cáo doanh thu, báo cáo tour (xuất CSV)
- ✅ Nhật ký hoạt động (Audit log - chỉ Admin)

## Quy tắc nghiệp vụ đã áp dụng

- **BR-01**: Chỉ đặt được lịch OPEN, chưa khởi hành, còn chỗ
- **BR-02**: Chống overbooking bằng `UPDATE ... WHERE (MaxGuests - BookedGuests) >= N` trong transaction
- **BR-03**: Hủy booking: trước 15 ngày hoàn 100%, 7-14 ngày hoàn 70%, dưới 7 ngày không hoàn
- **BR-04**: Chỉ đánh giá khi booking đã COMPLETED
- **BR-05**: Soft delete cho Tour/Schedule (chuyển INACTIVE)
- **BR-06**: Kiểm tra overlap lịch HDV khi phân công
- **BR-07**: Validate đầy đủ điều kiện mã KM (thời hạn, lượt dùng, đơn tối thiểu)

## Bảo mật

- ASP.NET Core Identity với 3 role: CUSTOMER, STAFF, ADMIN
- Mật khẩu hash bằng PBKDF2
- Cookie authentication với timeout 8 giờ
- Phân quyền bằng `[Authorize(Roles = "ADMIN,STAFF")]` ở backend
- Anti-forgery token cho mọi POST form
- Validation cả client-side (DataAnnotations) và server-side

## Công nghệ sử dụng

- ASP.NET Core MVC 8.0
- Entity Framework Core 8 (Code-First)
- SQL Server / LocalDB
- ASP.NET Core Identity
- Bootstrap 5 + Bootstrap Icons
- Chart.js (dashboard)

## Liên hệ / Hỗ trợ

Tài liệu yêu cầu: xem file `yeu-cau-he-thong-quan-ly-tour-du-lich.md`
