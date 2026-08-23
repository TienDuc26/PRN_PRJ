# PROMPT CHO CURSOR — XÂY DỰNG HỆ THỐNG QUẢN LÝ TOUR DU LỊCH (ASP.NET CORE MVC)

> Cách dùng: Copy toàn bộ nội dung bên dưới, dán vào Cursor (Composer/Agent mode), đính kèm file
> `yeu-cau-he-thong-quan-ly-tour-du-lich.md` vào context (@ file), rồi gửi. Nên làm theo từng PHASE
> (đã chia sẵn ở cuối prompt) thay vì bắt AI làm 1 lần toàn bộ — tỷ lệ thành công cao hơn nhiều.

---

## VAI TRÒ

Bạn là một Senior .NET Developer/Solution Architect. Nhiệm vụ của bạn là xây dựng **toàn bộ** dự án
web "Hệ thống quản lý tour du lịch" bằng **ASP.NET Core MVC (.NET 8)**, dựa trên tài liệu yêu cầu
đính kèm (`yeu-cau-he-thong-quan-ly-tour-du-lich.md`). Tài liệu đó là nguồn sự thật (source of truth)
cho toàn bộ nghiệp vụ, entity, API route, validation, trạng thái, và tiêu chí nghiệm thu — hãy đọc kỹ
và bám sát nó, không tự ý bỏ bớt chức năng hay đổi tên trạng thái/enum đã quy định (OPEN/FULL/CLOSED/
CANCELLED, PENDING/CONFIRMED/PAID/CANCELLED/COMPLETED, UNPAID/PARTIAL_PAID/PAID/REFUNDED...).

## RÀNG BUỘC QUAN TRỌNG NHẤT — KIẾN TRÚC & CẤU TRÚC THƯ MỤC

Đây là dự án **ASP.NET Core MVC thuần** (KHÔNG phải Clean Architecture nhiều project, KHÔNG tách
riêng Web API project + FE React/Angular, KHÔNG microservices). Chỉ dùng **1 solution, 1 project MVC
duy nhất**. Toàn bộ giao diện dùng **Razor Views + Bootstrap**, không dùng React/Vue/Next.js.

**Cấu trúc thư mục CHUẨN, không được tạo thêm thư mục không cần thiết ngoài danh sách này:**

```
TourManagement/
├── TourManagement.sln
└── TourManagement.Web/
    ├── Areas/
    │   ├── Admin/                  (dùng chung cho Admin + Staff, phân quyền bằng [Authorize(Roles=...)])
    │   │   ├── Controllers/
    │   │   ├── Views/
    │   │   └── ViewModels/
    │   └── Customer/               (nếu muốn tách khu vực khách hàng riêng; nếu không cần thì bỏ Area
    │                                 này và để các Controller khách hàng ở gốc project)
    ├── Controllers/                (Home, Account, Tour, Booking, Payment, Review... phía Customer)
    ├── Data/
    │   ├── AppDbContext.cs
    │   ├── Seed/                   (DbInitializer / seed data)
    │   └── Migrations/             (EF Core tự sinh)
    ├── Models/
    │   ├── Entities/                (các entity EF Core, đúng theo mục ERD trong tài liệu)
    │   └── Enums/                   (TourStatus, ScheduleStatus, BookingStatus, PaymentStatus...)
    ├── ViewModels/                  (DTO cho View, KHÔNG expose entity trực tiếp ra View khi không cần)
    ├── Services/
    │   ├── Interfaces/
    │   └── Implementations/         (TourService, BookingService, PaymentService, PromotionService...)
    ├── Repositories/                (nếu áp dụng Repository pattern; nếu không cần thì bỏ hẳn, gọi
    │                                 thẳng DbContext qua Service — không tạo cả hai kiểu cùng lúc)
    ├── Helpers/                     (mã hoá, sinh mã booking, tính giá, response chuẩn...)
    ├── Middlewares/                 (error handling, logging nếu cần)
    ├── Views/
    │   ├── Shared/                  (_Layout, _AdminLayout, partials)
    │   ├── Home/, Tour/, Booking/, Account/, Profile/, Notification/, Review/...
    ├── wwwroot/
    │   ├── css/, js/, lib/, uploads/ (ảnh tour, ảnh đại diện, ảnh review)
    ├── appsettings.json
    └── Program.cs
```

**Quy tắc bắt buộc:**
- KHÔNG tạo các folder thừa như `Core/`, `Infrastructure/`, `Application/`, `Domain/`, `Common/`,
  `Shared/` (ở cấp root), `API/` riêng, `Tests/` (trừ khi tới Phase 7 mới thêm 1 project test riêng).
- KHÔNG tạo song song cả REST API controllers (`/api/...`) và MVC controllers cho cùng 1 chức năng.
  Tài liệu có mô tả các endpoint dạng `/api/...` chỉ để tham khảo luồng dữ liệu/response — hãy chuyển
  nghĩa thành **MVC action + Razor View** (server-rendered), có thể dùng thêm **API endpoint nội bộ**
  (Minimal API hoặc `[ApiController]` route `/api/...`) CHỈ cho các phần thực sự cần gọi AJAX (ví dụ:
  kiểm tra mã khuyến mãi real-time, kiểm tra còn chỗ real-time, đánh dấu đã đọc thông báo). Không bắt
  buộc phải dựng full REST API cho mọi entity.
- Nếu không dùng Repository pattern thì đừng tạo folder `Repositories/` — chọn 1 trong 2 cách và giữ
  nhất quán toàn bộ dự án.
- Không tạo file/README/docs rườm rà ngoài 1 file `README.md` gốc mô tả cách chạy dự án.

## TECH STACK BẮT BUỘC

- **.NET 8**, ASP.NET Core MVC (Razor Views, không Razor Pages).
- **Entity Framework Core** (Code-First + Migrations), **SQL Server** (LocalDB cho dev).
- **ASP.NET Core Identity** cho Authentication, tuỳ biến để hỗ trợ 3 role: `CUSTOMER`, `STAFF`, `ADMIN`
  (theo đúng mục 18 — FR-USER-02 trong tài liệu). Dùng cookie authentication (không cần JWT vì đây là
  MVC server-rendered, trừ khi bạn thêm API riêng cho AJAX thì có thể vẫn dùng cookie auth cho cả 2).
- **AutoMapper** (tuỳ chọn) để map Entity ↔ ViewModel.
- **FluentValidation** hoặc DataAnnotations cho validate (theo mục 26 của tài liệu).
- Bootstrap 5 cho giao diện, responsive (mục 19.3, NFR).
- Logging bằng `ILogger` built-in (không cần thêm Serilog trừ khi cần ghi file).
- Ảnh: lưu local trong `wwwroot/uploads` (theo đề xuất "Local storage trong development" ở mục 31).

## NGHIỆP VỤ CẦN TRIỂN KHAI ĐẦY ĐỦ (bám sát tài liệu đính kèm)

Đọc và triển khai đúng các mục sau trong tài liệu (không tóm tắt qua loa, không bỏ sót):
1. Authentication & Account (mục 4): đăng ký, đăng nhập, đăng xuất, quên mật khẩu, quản lý hồ sơ.
2. Quản lý Tour (mục 5): danh sách + phân trang, tìm kiếm, lọc, sắp xếp, chi tiết, CRUD, soft-delete
   (chuyển INACTIVE thay vì xoá cứng nếu đã có booking).
3. Quản lý điểm đến (mục 6).
4. Quản lý lịch trình tour — nhiều ngày, sắp xếp lại thứ tự (mục 7).
5. Quản lý lịch khởi hành — tính số chỗ còn lại tự động, không cho vượt quá (mục 8, BR-02).
6. Đặt tour — luồng đầy đủ: chọn lịch → nhập số khách → nhập thông tin người tham gia → áp mã khuyến
   mãi → tính tiền tự động → xác nhận → sinh mã booking duy nhất (mục 9).
7. Thanh toán — chuyển khoản/tiền mặt, trạng thái, lưu giao dịch, hoàn tiền theo chính sách hủy (mục
   10, BR-03).
8. Quản lý khách hàng, khoá/mở khoá tài khoản (mục 11).
9. Quản lý hướng dẫn viên + phân công + cảnh báo trùng lịch (mục 12, BR-06).
10. Đánh giá tour — chỉ cho phép sau khi hoàn thành, tính điểm trung bình tự động (mục 13, BR-04).
11. Khuyến mãi — CRUD + kiểm tra điều kiện áp dụng đầy đủ (mục 14, BR-07).
12. Thông báo (mục 15).
13. Dashboard quản trị với các chỉ số + thống kê theo thời gian (mục 16).
14. Báo cáo (mục 17).
15. Quản lý tài khoản & phân quyền — authorization phải kiểm tra ở backend, không chỉ ẩn UI (mục 18,
    NFR-01, Rủi ro 4).
16. Áp dụng đầy đủ **Quy tắc nghiệp vụ (BR-01 → BR-07)** và xử lý 5 **rủi ro** nêu ở mục 33, đặc biệt:
    - **Chống overbooking**: dùng transaction + concurrency check (rowversion/optimistic concurrency
      hoặc transaction với khoá) khi trừ số chỗ trong Schedule.
    - **Soft delete** cho Tour/Schedule đã phát sinh giao dịch.
17. Response/error format: cố gắng theo tinh thần mục 27 (message rõ ràng, có mã lỗi) dù đây là MVC
    (dùng TempData/ModelState cho lỗi hiển thị UI, và JSON response chuẩn hoá cho các action AJAX).
18. Audit log tối thiểu theo mục 28 (ai, hành động gì, trên entity nào, khi nào).

## DATABASE

Thiết kế schema Code-First đầy đủ theo đúng entity + quan hệ đã mô tả trong tài liệu (Users/Roles,
Tour, Destination, Itinerary/ItineraryDay, TourSchedule, Booking, BookingParticipant, Payment,
Transaction, Guide, GuideAssignment, Review, Promotion, Notification, AuditLog...). Thiết lập đúng
khoá chính/khoá ngoại, ràng buộc unique (email, phone nếu yêu cầu), enum cho các trạng thái đã liệt kê
trong tài liệu, và index hợp lý cho các trường hay tìm kiếm/lọc (tên tour, điểm đến, ngày khởi hành).

Viết migration đầu tiên + 1 file seed data mẫu (vài điểm đến, vài tour có lịch trình + lịch khởi
hành, 1 tài khoản Admin mặc định, 1 Staff, 1 Customer) để chạy thử được ngay.

## TIÊU CHUẨN CODE

- Đặt tên rõ ràng theo convention C#/.NET (PascalCase cho class/method, camelCase cho biến local).
- Business logic nằm ở Service layer, Controller chỉ điều phối (mỏng — thin controller).
- Không viết logic tính tiền/kiểm tra khuyến mãi/kiểm tra overbooking trực tiếp trong Controller hay
  trong View.
- Dùng `async/await` cho toàn bộ truy vấn DB.
- ViewModel riêng cho từng màn hình cần input phức tạp (VD: `BookingCreateViewModel`,
  `TourFilterViewModel`), tránh bind thẳng Entity vào Form (chống overposting).
- Comment ngắn gọn ở những đoạn logic nghiệp vụ phức tạp (tính giá, check overlap lịch hướng dẫn
  viên, check overbooking).

## KẾT QUẢ MONG MUỐN CUỐI CÙNG

Đối chiếu đúng "Tiêu chí nghiệm thu MVP" ở mục 29 của tài liệu — sau khi hoàn thành, toàn bộ các gạch
đầu dòng trong mục đó phải chạy được thực tế (không phải chỉ có UI tĩnh).

---

## CÁCH TRIỂN KHAI — LÀM THEO TỪNG PHASE (đừng yêu cầu Cursor làm 1 lần toàn bộ)

Gửi lần lượt từng khối lệnh dưới đây cho Cursor, chờ hoàn thành và kiểm tra chạy được rồi mới sang
phase tiếp theo. Luôn nhắc Cursor đọc lại tài liệu yêu cầu trước mỗi phase.

**Phase 1 — Foundation**
"Khởi tạo solution + project theo đúng cấu trúc thư mục đã mô tả ở trên. Cài đặt EF Core, SQL Server,
ASP.NET Core Identity tuỳ biến 3 role CUSTOMER/STAFF/ADMIN. Tạo AppDbContext với toàn bộ entity theo
tài liệu, tạo migration đầu tiên, tạo seed data mẫu. Dựng layout Bootstrap cho Customer và layout
riêng cho khu vực Admin/Staff. Không tạo chức năng nghiệp vụ ở phase này, chỉ nền tảng."

**Phase 2 — Tour (mục 5, 6, 7, 8)**
"Triển khai đầy đủ Destination, Tour (CRUD + danh sách + tìm kiếm + lọc + sắp xếp + phân trang +
trang chi tiết), Itinerary (nhiều ngày, sắp xếp lại), TourSchedule (tính số chỗ còn lại tự động, các
trạng thái OPEN/FULL/CLOSED/CANCELLED). Cả phía Customer (xem) và phía Admin/Staff (quản lý)."

**Phase 3 — Booking (mục 9)**
"Triển khai luồng đặt tour đầy đủ theo đúng quy trình ở mục 22.1 của tài liệu: chọn lịch → nhập số
khách → nhập người tham gia → áp mã khuyến mãi → tính tiền → xác nhận → sinh mã booking. Đảm bảo
chống overbooking bằng transaction. Trang 'Đơn của tôi' và trang quản lý booking cho Admin/Staff."

**Phase 4 — Payment (mục 10)**
"Triển khai thanh toán (chuyển khoản/tiền mặt), lưu giao dịch, các trạng thái thanh toán, xử lý hoàn
tiền theo chính sách hủy ở BR-03."

**Phase 5 — Operations (mục 11, 12, 13, 15)**
"Triển khai quản lý khách hàng, hướng dẫn viên + phân công (cảnh báo trùng lịch theo BR-06), đánh giá
tour (chỉ cho phép sau khi hoàn thành, tính điểm trung bình), và thông báo."

**Phase 6 — Management (mục 14, 16, 17, 18, 28)**
"Triển khai khuyến mãi, dashboard với đầy đủ chỉ số, báo cáo, quản lý tài khoản/phân quyền, audit
log."

**Phase 7 — Hoàn thiện**
"Rà soát lại toàn bộ theo checklist mục 34 và tiêu chí nghiệm thu mục 29 trong tài liệu. Kiểm tra
responsive, kiểm tra authorization ở backend cho mọi action quản trị, viết 1 file README.md hướng dẫn
chạy project (migration, seed, tài khoản mặc định)."
