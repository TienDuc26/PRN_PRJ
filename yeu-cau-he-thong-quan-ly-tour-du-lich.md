# TÀI LIỆU YÊU CẦU PHẦN MỀM
# HỆ THỐNG QUẢN LÝ TOUR DU LỊCH

**Phiên bản:** 1.0  
**Ngày:** 21/08/2026  
**Trạng thái:** Draft / Baseline Requirements

---

## 1. Tổng quan dự án

### 1.1. Tên dự án

**Hệ thống quản lý tour du lịch**

### 1.2. Mục tiêu

Xây dựng một website hỗ trợ công ty du lịch quản lý toàn bộ hoạt động liên quan đến tour, đồng thời cung cấp cho khách hàng khả năng tìm kiếm, xem thông tin và đặt tour trực tuyến.

Hệ thống tập trung vào các nghiệp vụ:

- Quản lý tour du lịch.
- Quản lý điểm đến và lịch trình.
- Quản lý lịch khởi hành.
- Quản lý khách hàng.
- Quản lý đơn đặt tour.
- Quản lý thanh toán.
- Quản lý hướng dẫn viên.
- Quản lý đánh giá.
- Quản lý khuyến mãi.
- Thống kê và báo cáo.

### 1.3. Đối tượng sử dụng

Hệ thống gồm các nhóm người dùng chính:

1. **Khách hàng (Customer):** tìm kiếm, xem và đặt tour.
2. **Nhân viên (Staff):** quản lý tour, lịch khởi hành, khách hàng và đơn đặt tour.
3. **Quản trị viên (Admin):** quản lý toàn bộ hệ thống và phân quyền.

---

# 2. Phạm vi dự án

## 2.1. Phạm vi trong hệ thống

Hệ thống phải hỗ trợ:

- Quản lý tài khoản và xác thực.
- Quản lý tour.
- Quản lý điểm đến.
- Quản lý lịch trình tour.
- Quản lý lịch khởi hành.
- Quản lý đặt tour.
- Quản lý người tham gia tour.
- Quản lý thanh toán.
- Quản lý khách hàng.
- Quản lý hướng dẫn viên.
- Phân công hướng dẫn viên.
- Quản lý đánh giá.
- Quản lý khuyến mãi.
- Thông báo.
- Dashboard quản trị.
- Báo cáo và thống kê.

## 2.2. Ngoài phạm vi phiên bản MVP

Các chức năng sau có thể được phát triển ở phiên bản nâng cao:

- Gợi ý tour bằng AI.
- Chatbot tư vấn tự động.
- Theo dõi vị trí đoàn theo GPS.
- Ứng dụng mobile native.
- Tích hợp nhiều cổng thanh toán cùng lúc.
- Hệ thống CRM nâng cao.
- Dynamic pricing theo thời gian thực.

---

# 3. Vai trò và phân quyền

## 3.1. Customer

Khách hàng có thể:

- Đăng ký tài khoản.
- Đăng nhập / đăng xuất.
- Quản lý hồ sơ.
- Xem danh sách tour.
- Tìm kiếm và lọc tour.
- Xem chi tiết tour.
- Xem lịch khởi hành.
- Đặt tour.
- Thanh toán.
- Xem lịch sử đặt tour.
- Hủy tour theo chính sách.
- Đánh giá tour sau khi hoàn thành.
- Xem thông báo.

## 3.2. Staff

Nhân viên có thể:

- Đăng nhập hệ thống quản trị.
- Quản lý tour.
- Quản lý điểm đến.
- Quản lý lịch trình.
- Quản lý lịch khởi hành.
- Quản lý đơn đặt tour.
- Quản lý khách hàng.
- Quản lý hướng dẫn viên.
- Phân công hướng dẫn viên.
- Xử lý thanh toán.
- Quản lý đánh giá.
- Xem báo cáo nghiệp vụ.

## 3.3. Admin

Admin có toàn bộ quyền của Staff và thêm:

- Quản lý tài khoản nhân viên.
- Quản lý vai trò và quyền.
- Quản lý cấu hình hệ thống.
- Quản lý khuyến mãi.
- Xem báo cáo tổng hợp.
- Quản lý nhật ký hoạt động.

---

# 4. Yêu cầu chức năng

## 4.1. Authentication & Account

### FR-AUTH-01 — Đăng ký

Hệ thống cho phép khách hàng tạo tài khoản bằng:

- Họ tên.
- Email.
- Số điện thoại.
- Mật khẩu.
- Xác nhận mật khẩu.

Yêu cầu:

- Email không được trùng.
- Số điện thoại không được trùng nếu hệ thống yêu cầu duy nhất.
- Mật khẩu phải đáp ứng chính sách bảo mật.
- Hệ thống thông báo lỗi khi dữ liệu không hợp lệ.

### FR-AUTH-02 — Đăng nhập

Người dùng đăng nhập bằng email và mật khẩu.

Hệ thống phải:

- Kiểm tra thông tin đăng nhập.
- Xác định role.
- Tạo phiên đăng nhập hoặc access token.
- Chuyển người dùng đến giao diện tương ứng.

### FR-AUTH-03 — Đăng xuất

Người dùng có thể đăng xuất khỏi hệ thống.

### FR-AUTH-04 — Quên mật khẩu

Người dùng có thể yêu cầu đặt lại mật khẩu thông qua email.

### FR-AUTH-05 — Quản lý hồ sơ

Customer có thể cập nhật:

- Họ tên.
- Số điện thoại.
- Địa chỉ.
- Ngày sinh.
- Giới tính.
- Ảnh đại diện.

---

# 5. Quản lý tour

## 5.1. FR-TOUR-01 — Danh sách tour

Hệ thống hiển thị:

- Mã tour.
- Tên tour.
- Ảnh đại diện.
- Điểm đến.
- Thời lượng.
- Giá từ.
- Đánh giá.
- Trạng thái.

Danh sách phải hỗ trợ phân trang.

## 5.2. FR-TOUR-02 — Tìm kiếm tour

Khách hàng có thể tìm theo:

- Tên tour.
- Điểm đến.
- Mã tour.

## 5.3. FR-TOUR-03 — Lọc tour

Cho phép lọc theo:

- Điểm đến.
- Khoảng giá.
- Thời lượng.
- Ngày khởi hành.
- Loại tour.
- Trạng thái.

## 5.4. FR-TOUR-04 — Sắp xếp

Cho phép:

- Giá thấp đến cao.
- Giá cao đến thấp.
- Mới nhất.
- Phổ biến nhất.
- Đánh giá cao nhất.

## 5.5. FR-TOUR-05 — Xem chi tiết tour

Trang chi tiết phải có:

- Mã tour.
- Tên tour.
- Hình ảnh.
- Mô tả.
- Điểm đến.
- Thời lượng.
- Giá.
- Lịch trình.
- Dịch vụ bao gồm.
- Dịch vụ không bao gồm.
- Chính sách.
- Lịch khởi hành.
- Số chỗ còn lại.
- Đánh giá.

## 5.6. FR-TOUR-06 — Admin CRUD tour

Admin/Staff có thể:

- Tạo tour.
- Xem tour.
- Cập nhật tour.
- Xóa tour.
- Kích hoạt / ngừng hoạt động tour.

Không cho phép xóa vật lý tour đã phát sinh đơn đặt tour nếu việc xóa làm mất tính toàn vẹn dữ liệu. Trong trường hợp đó phải chuyển tour sang trạng thái INACTIVE.

---

# 6. Quản lý điểm đến

## FR-DEST-01 — CRUD điểm đến

Admin/Staff có thể:

- Thêm điểm đến.
- Xem điểm đến.
- Sửa điểm đến.
- Xóa / vô hiệu hóa điểm đến.

Thông tin điểm đến:

- Mã.
- Tên.
- Tỉnh/thành phố.
- Quốc gia.
- Mô tả.
- Hình ảnh.
- Trạng thái.

## FR-DEST-02 — Tour theo điểm đến

Hệ thống phải cho phép xem danh sách tour thuộc một điểm đến.

---

# 7. Quản lý lịch trình tour

## FR-ITIN-01 — Tạo lịch trình

Một tour có thể có nhiều ngày trong lịch trình.

Mỗi ngày có:

- Thứ tự ngày.
- Tiêu đề.
- Nội dung.
- Địa điểm.
- Thời gian.
- Bữa ăn.
- Khách sạn.
- Ghi chú.
- Hình ảnh tùy chọn.

## FR-ITIN-02 — CRUD lịch trình

Admin/Staff có thể:

- Thêm.
- Sửa.
- Xóa.
- Sắp xếp lại các ngày.

---

# 8. Quản lý lịch khởi hành

## FR-SCHEDULE-01 — Tạo lịch khởi hành

Một tour có thể có nhiều lịch khởi hành.

Thông tin:

- Mã lịch.
- Tour.
- Ngày bắt đầu.
- Ngày kết thúc.
- Giờ tập trung.
- Địa điểm tập trung.
- Số chỗ tối đa.
- Số chỗ đã đặt.
- Số chỗ còn lại.
- Giá áp dụng.
- Trạng thái.

## FR-SCHEDULE-02 — Theo dõi số chỗ

Hệ thống tự động tính:

`Số chỗ còn lại = Số chỗ tối đa - Số chỗ đã đặt`

Không cho phép số chỗ đã đặt vượt quá số chỗ tối đa.

## FR-SCHEDULE-03 — Trạng thái lịch

Các trạng thái:

- `OPEN` — Đang mở đặt.
- `FULL` — Đã đủ chỗ.
- `CLOSED` — Đã đóng.
- `CANCELLED` — Đã hủy.

## FR-SCHEDULE-04 — CRUD lịch khởi hành

Admin/Staff có thể:

- Tạo.
- Xem.
- Sửa.
- Đóng.
- Hủy.
- Xóa lịch chưa phát sinh giao dịch.

---

# 9. Đặt tour

## FR-BOOKING-01 — Chọn lịch

Khách hàng chọn một lịch khởi hành còn chỗ.

## FR-BOOKING-02 — Nhập số lượng khách

Khách hàng nhập:

- Người lớn.
- Trẻ em.
- Các nhóm đối tượng khác nếu hệ thống có chính sách giá riêng.

## FR-BOOKING-03 — Nhập thông tin người tham gia

Mỗi người tham gia có thể gồm:

- Họ tên.
- Ngày sinh.
- Giới tính.
- Số giấy tờ tùy thân.
- Số điện thoại.
- Email.
- Ghi chú.

## FR-BOOKING-04 — Áp dụng khuyến mãi

Khách hàng có thể nhập mã khuyến mãi hợp lệ.

Hệ thống phải kiểm tra:

- Mã tồn tại.
- Còn hiệu lực.
- Đủ điều kiện.
- Chưa vượt số lần sử dụng.
- Đúng phạm vi áp dụng.

## FR-BOOKING-05 — Tính tiền

Hệ thống tự động tính:

`Tạm tính = Giá tour × số lượng khách`

`Giảm giá = Giá trị khuyến mãi`

`Tổng tiền = Tạm tính - Giảm giá + Phụ phí`

Công thức cụ thể có thể được cấu hình tùy chính sách kinh doanh.

## FR-BOOKING-06 — Xác nhận đặt tour

Sau khi xác nhận, hệ thống tạo:

- Mã booking duy nhất.
- Thông tin tour.
- Lịch khởi hành.
- Danh sách người tham gia.
- Tổng tiền.
- Trạng thái đơn.
- Trạng thái thanh toán.

## FR-BOOKING-07 — Trạng thái booking

Các trạng thái đề xuất:

- `PENDING` — Chờ xử lý.
- `CONFIRMED` — Đã xác nhận.
- `PAID` — Đã thanh toán.
- `CANCELLED` — Đã hủy.
- `COMPLETED` — Đã hoàn thành.

---

# 10. Thanh toán

## FR-PAY-01 — Phương thức thanh toán

MVP hỗ trợ:

- Chuyển khoản.
- Tiền mặt.
- Có thể mở rộng sang cổng thanh toán trực tuyến.

## FR-PAY-02 — Trạng thái thanh toán

- `UNPAID`.
- `PARTIAL_PAID`.
- `PAID`.
- `REFUNDED`.

## FR-PAY-03 — Lưu giao dịch

Mỗi giao dịch lưu:

- Mã giao dịch.
- Booking.
- Số tiền.
- Phương thức.
- Thời gian.
- Trạng thái.
- Ghi chú.

## FR-PAY-04 — Hoàn tiền

Khi booking được hủy theo chính sách, Staff/Admin có thể xử lý hoàn tiền.

---

# 11. Quản lý khách hàng

## FR-CUSTOMER-01 — Danh sách khách hàng

Admin/Staff có thể:

- Xem danh sách.
- Tìm kiếm.
- Lọc.
- Xem chi tiết.

## FR-CUSTOMER-02 — Hồ sơ khách hàng

Hiển thị:

- Thông tin cá nhân.
- Tổng số booking.
- Tổng số tiền đã thanh toán.
- Lịch sử đặt tour.
- Lịch sử hủy tour.
- Đánh giá đã tạo.

## FR-CUSTOMER-03 — Khóa tài khoản

Admin có thể:

- Khóa tài khoản.
- Mở khóa tài khoản.

Tài khoản bị khóa không được đăng nhập.

---

# 12. Quản lý hướng dẫn viên

## FR-GUIDE-01 — CRUD hướng dẫn viên

Thông tin:

- Mã hướng dẫn viên.
- Họ tên.
- Ngày sinh.
- Số điện thoại.
- Email.
- Địa chỉ.
- Kinh nghiệm.
- Ngôn ngữ.
- Trạng thái.

## FR-GUIDE-02 — Phân công

Admin/Staff có thể phân công hướng dẫn viên cho từng lịch khởi hành.

Hệ thống phải cảnh báo nếu hướng dẫn viên đã được phân công vào lịch bị trùng thời gian.

## FR-GUIDE-03 — Theo dõi lịch làm việc

Hiển thị các lịch tour mà hướng dẫn viên phụ trách.

---

# 13. Đánh giá tour

## FR-REVIEW-01 — Đánh giá

Chỉ khách hàng đã hoàn thành tour mới được đánh giá.

Đánh giá gồm:

- Số sao 1–5.
- Nội dung.
- Hình ảnh tùy chọn.

## FR-REVIEW-02 — Quản lý đánh giá

Admin/Staff có thể:

- Xem.
- Ẩn.
- Hiển thị lại.
- Xóa đánh giá vi phạm chính sách.

## FR-REVIEW-03 — Điểm trung bình

Hệ thống tự động tính điểm trung bình của tour dựa trên các đánh giá hợp lệ.

---

# 14. Khuyến mãi

## FR-PROMO-01 — CRUD khuyến mãi

Thông tin:

- Mã.
- Tên chương trình.
- Loại giảm.
- Giá trị giảm.
- Giảm tối đa.
- Đơn tối thiểu.
- Thời gian bắt đầu.
- Thời gian kết thúc.
- Số lượt sử dụng.
- Số lượt đã sử dụng.
- Trạng thái.

## FR-PROMO-02 — Kiểm tra mã

Hệ thống phải từ chối mã:

- Không tồn tại.
- Hết hạn.
- Chưa bắt đầu.
- Hết lượt.
- Không đủ điều kiện.

---

# 15. Thông báo

## FR-NOTI-01 — Thông báo booking

Hệ thống tạo thông báo khi:

- Đặt tour thành công.
- Booking được xác nhận.
- Thanh toán thành công.
- Booking bị hủy.
- Tour bị thay đổi.
- Tour sắp khởi hành.

## FR-NOTI-02 — Đọc thông báo

Khách hàng có thể:

- Xem danh sách thông báo.
- Đánh dấu đã đọc.
- Xem chi tiết.

---

# 16. Dashboard quản trị

Dashboard phải hiển thị các chỉ số chính:

- Tổng số tour.
- Tổng số khách hàng.
- Tổng số booking.
- Tổng doanh thu.
- Booking đang chờ.
- Tour sắp khởi hành.
- Tour bán chạy.
- Điểm đến phổ biến.

## FR-DASH-01 — Thống kê doanh thu

Cho phép xem doanh thu theo:

- Ngày.
- Tuần.
- Tháng.
- Quý.
- Năm.

## FR-DASH-02 — Thống kê booking

Hiển thị số lượng booking theo trạng thái.

## FR-DASH-03 — Tour phổ biến

Xếp hạng tour theo số lượng booking.

---

# 17. Báo cáo

## FR-REPORT-01 — Báo cáo doanh thu

Có thể lọc theo khoảng thời gian.

## FR-REPORT-02 — Báo cáo tour

Thống kê:

- Số lượt đặt.
- Số khách.
- Doanh thu.
- Tỷ lệ lấp đầy.

## FR-REPORT-03 — Xuất báo cáo

Phiên bản nâng cao có thể hỗ trợ:

- Excel.
- CSV.
- PDF.

---

# 18. Quản lý tài khoản và phân quyền

## FR-USER-01 — Quản lý user

Admin có thể:

- Xem.
- Tìm kiếm.
- Tạo.
- Sửa.
- Khóa.
- Mở khóa.

## FR-USER-02 — Role

Các role mặc định:

- `CUSTOMER`
- `STAFF`
- `ADMIN`

## FR-USER-03 — Authorization

Backend phải kiểm tra quyền đối với mọi API yêu cầu quyền quản trị.

Không được chỉ dựa vào việc ẩn nút trên frontend.

---

# 19. Yêu cầu giao diện

## 19.1. Giao diện khách hàng

Các trang tối thiểu:

1. Trang chủ.
2. Danh sách tour.
3. Chi tiết tour.
4. Đăng nhập.
5. Đăng ký.
6. Hồ sơ.
7. Đặt tour.
8. Thanh toán.
9. Đơn của tôi.
10. Chi tiết booking.
11. Đánh giá.
12. Thông báo.

## 19.2. Giao diện quản trị

Các trang tối thiểu:

1. Dashboard.
2. Quản lý tour.
3. Quản lý điểm đến.
4. Quản lý lịch trình.
5. Quản lý lịch khởi hành.
6. Quản lý booking.
7. Quản lý khách hàng.
8. Quản lý hướng dẫn viên.
9. Phân công hướng dẫn viên.
10. Quản lý thanh toán.
11. Quản lý đánh giá.
12. Quản lý khuyến mãi.
13. Quản lý người dùng.
14. Báo cáo.

## 19.3. Responsive

Website phải hoạt động tốt trên:

- Desktop.
- Tablet.
- Mobile.

---

# 20. Yêu cầu phi chức năng

## NFR-01 — Bảo mật

Hệ thống phải:

- Hash mật khẩu bằng thuật toán an toàn.
- Không lưu mật khẩu dạng plain text.
- Validate dữ liệu ở backend.
- Phân quyền ở backend.
- Bảo vệ API yêu cầu xác thực.
- Chống SQL Injection.
- Chống XSS.
- Không trả về thông tin nhạy cảm không cần thiết.
- Có cơ chế quản lý token/session an toàn.

## NFR-02 — Hiệu năng

- API phổ biến nên phản hồi trong thời gian hợp lý.
- Danh sách lớn phải có pagination.
- Có debounce cho tìm kiếm nếu cần.
- Hình ảnh phải được tối ưu.
- Tránh N+1 query.

## NFR-03 — Tính nhất quán dữ liệu

Các nghiệp vụ đặt tour và thanh toán phải đảm bảo transaction.

Đặc biệt:

- Không được overbooking.
- Việc cập nhật số chỗ phải nhất quán.
- Booking và payment phải có quan hệ rõ ràng.

## NFR-04 — Khả năng mở rộng

Thiết kế hệ thống cho phép mở rộng:

- Nhiều cổng thanh toán.
- Nhiều loại tour.
- Nhiều role.
- Nhiều chi nhánh.
- Nhiều ngôn ngữ.
- Nhiều loại tiền tệ.

## NFR-05 — Khả năng bảo trì

Code phải:

- Có cấu trúc module rõ ràng.
- Tách frontend/backend nếu sử dụng kiến trúc SPA/API.
- Có validation.
- Có logging.
- Có xử lý lỗi thống nhất.
- Có tài liệu API.

---

# 21. Quy tắc nghiệp vụ

## BR-01 — Đặt tour

Khách hàng chỉ được đặt lịch:

- Đang mở.
- Chưa đầy.
- Chưa quá ngày khởi hành.

## BR-02 — Không overbooking

Số người trong booking mới không được làm:

`Số khách đã đặt > Số chỗ tối đa`

## BR-03 — Hủy booking

Khách hàng chỉ được hủy theo chính sách của tour.

Ví dụ:

- Trước 15 ngày: hoàn 100%.
- Trước 7–14 ngày: hoàn 70%.
- Dưới 7 ngày: không hoàn.

Các tỷ lệ trên chỉ là ví dụ và phải được cấu hình theo chính sách thực tế.

## BR-04 — Đánh giá

Chỉ booking đã hoàn thành mới được đánh giá.

Mỗi booking có thể giới hạn số lần đánh giá theo thiết kế.

## BR-05 — Xóa dữ liệu

Không xóa vật lý dữ liệu nghiệp vụ quan trọng đã phát sinh giao dịch nếu việc xóa làm mất lịch sử.

Ưu tiên:

- Soft delete.
- Inactive.
- Archived.

## BR-06 — Hướng dẫn viên

Một hướng dẫn viên không được phân công hai tour có thời gian chồng lấn.

## BR-07 — Khuyến mãi

Một mã khuyến mãi chỉ được áp dụng nếu đáp ứng toàn bộ điều kiện của chương trình.

---

# 22. Quy trình nghiệp vụ chính

## 22.1. Quy trình đặt tour

```text
Khách hàng
    |
    v
Tìm kiếm tour
    |
    v
Xem chi tiết
    |
    v
Chọn lịch khởi hành
    |
    v
Nhập số lượng khách
    |
    v
Nhập thông tin người tham gia
    |
    v
Nhập mã khuyến mãi
    |
    v
Hệ thống tính tiền
    |
    v
Xác nhận booking
    |
    v
Thanh toán
    |
    v
Booking được xác nhận
    |
    v
Tour khởi hành
    |
    v
Hoàn thành
    |
    v
Khách hàng đánh giá
```

## 22.2. Quy trình quản lý tour

```text
Admin/Staff
    |
    v
Tạo điểm đến
    |
    v
Tạo tour
    |
    v
Tạo lịch trình
    |
    v
Tạo lịch khởi hành
    |
    v
Mở bán
    |
    v
Khách hàng đặt tour
```

## 22.3. Quy trình phân công hướng dẫn viên

```text
Tạo lịch khởi hành
        |
        v
Xem hướng dẫn viên khả dụng
        |
        v
Chọn hướng dẫn viên
        |
        v
Kiểm tra trùng lịch
        |
   +----+----+
   |         |
  Có        Không
   |         |
   v         v
Báo lỗi    Phân công
```

---

# 23. Mô hình dữ liệu đề xuất

Các bảng chính:

```text
users
roles
destinations
tours
tour_itineraries
tour_schedules
bookings
booking_customers
payments
guides
guide_assignments
reviews
promotions
promotion_usages
notifications
audit_logs
```

## 23.1. users

Các trường đề xuất:

- id
- role_id
- full_name
- email
- phone
- password_hash
- avatar
- date_of_birth
- gender
- address
- status
- created_at
- updated_at

## 23.2. destinations

- id
- name
- city
- country
- description
- image
- status
- created_at
- updated_at

## 23.3. tours

- id
- destination_id
- code
- name
- description
- duration_days
- duration_nights
- base_price
- thumbnail
- included_services
- excluded_services
- policy
- status
- created_at
- updated_at

## 23.4. tour_itineraries

- id
- tour_id
- day_number
- title
- description
- location
- meals
- hotel
- notes
- image

## 23.5. tour_schedules

- id
- tour_id
- start_date
- end_date
- meeting_time
- meeting_point
- max_guests
- booked_guests
- price
- status
- created_at
- updated_at

## 23.6. bookings

- id
- user_id
- schedule_id
- booking_code
- adults
- children
- subtotal
- discount
- surcharge
- total_amount
- status
- booked_at
- cancelled_at
- completed_at

## 23.7. booking_customers

- id
- booking_id
- full_name
- date_of_birth
- gender
- identity_number
- phone
- email
- note

## 23.8. payments

- id
- booking_id
- transaction_code
- amount
- method
- status
- paid_at
- note

## 23.9. guides

- id
- full_name
- date_of_birth
- phone
- email
- address
- experience
- languages
- status

## 23.10. guide_assignments

- id
- guide_id
- schedule_id
- assigned_at
- note

## 23.11. reviews

- id
- booking_id
- user_id
- tour_id
- rating
- content
- status
- created_at

## 23.12. promotions

- id
- code
- name
- discount_type
- discount_value
- max_discount
- min_order_value
- start_at
- end_at
- usage_limit
- usage_count
- status

---

# 24. Quan hệ dữ liệu chính

```text
Role 1 --- N User

User 1 --- N Booking

Destination 1 --- N Tour

Tour 1 --- N TourItinerary

Tour 1 --- N TourSchedule

TourSchedule 1 --- N Booking

Booking 1 --- N BookingCustomer

Booking 1 --- N Payment

Guide 1 --- N GuideAssignment

TourSchedule 1 --- N GuideAssignment

Tour 1 --- N Review

User 1 --- N Review

Promotion 1 --- N PromotionUsage
```

---

# 25. API đề xuất

Nếu xây dựng backend REST API, có thể tổ chức như sau.

## Authentication

```text
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/logout
POST   /api/auth/forgot-password
POST   /api/auth/reset-password
GET    /api/auth/me
```

## Tours

```text
GET    /api/tours
GET    /api/tours/:id
POST   /api/tours
PUT    /api/tours/:id
DELETE /api/tours/:id
```

## Destinations

```text
GET    /api/destinations
GET    /api/destinations/:id
POST   /api/destinations
PUT    /api/destinations/:id
DELETE /api/destinations/:id
```

## Schedules

```text
GET    /api/tours/:tourId/schedules
POST   /api/tours/:tourId/schedules
PUT    /api/schedules/:id
DELETE /api/schedules/:id
```

## Bookings

```text
POST   /api/bookings
GET    /api/bookings
GET    /api/bookings/:id
PUT    /api/bookings/:id
POST   /api/bookings/:id/cancel
```

## Payments

```text
POST   /api/payments
GET    /api/payments/:id
POST   /api/payments/:id/refund
```

## Reviews

```text
GET    /api/tours/:tourId/reviews
POST   /api/tours/:tourId/reviews
PUT    /api/reviews/:id
DELETE /api/reviews/:id
```

---

# 26. Yêu cầu validation

Một số validation tối thiểu:

### Email

Phải đúng định dạng email.

### Số điện thoại

Chỉ chấp nhận định dạng số điện thoại hợp lệ theo thị trường triển khai.

### Giá tour

- Không âm.
- Có thể bằng 0 nếu có loại tour miễn phí.
- Phải có đơn vị tiền tệ rõ ràng.

### Ngày

- Ngày kết thúc không được trước ngày bắt đầu.
- Lịch khởi hành không được tạo với ngày đã quá hạn.
- Thời gian booking phải phù hợp chính sách.

### Số lượng khách

- Phải là số nguyên dương.
- Không vượt số chỗ còn lại.

---

# 27. Xử lý lỗi

API nên sử dụng response thống nhất.

Ví dụ:

```json
{
  "success": false,
  "message": "Lịch khởi hành đã hết chỗ",
  "code": "SCHEDULE_FULL",
  "data": null
}
```

Thành công:

```json
{
  "success": true,
  "message": "Đặt tour thành công",
  "data": {
    "bookingCode": "BK202608210001"
  }
}
```

Các HTTP status đề xuất:

- `200` — Thành công.
- `201` — Tạo thành công.
- `400` — Dữ liệu không hợp lệ.
- `401` — Chưa xác thực.
- `403` — Không có quyền.
- `404` — Không tìm thấy.
- `409` — Xung đột dữ liệu.
- `422` — Validation error.
- `500` — Lỗi server.

---

# 28. Logging và Audit

Admin có thể xem nhật ký:

- Đăng nhập.
- Đăng xuất.
- Tạo tour.
- Sửa tour.
- Xóa / vô hiệu hóa tour.
- Thay đổi giá.
- Xác nhận booking.
- Hủy booking.
- Hoàn tiền.
- Thay đổi quyền.

Audit log tối thiểu:

- user_id.
- action.
- entity_type.
- entity_id.
- old_value nếu cần.
- new_value nếu cần.
- IP.
- created_at.

---

# 29. Tiêu chí nghiệm thu MVP

MVP được xem là đạt khi:

- [ ] Người dùng có thể đăng ký và đăng nhập.
- [ ] Customer có thể xem danh sách tour.
- [ ] Customer có thể tìm kiếm và lọc tour.
- [ ] Customer có thể xem chi tiết tour.
- [ ] Customer có thể xem lịch khởi hành.
- [ ] Customer có thể đặt tour.
- [ ] Hệ thống không cho phép overbooking.
- [ ] Customer có thể xem lịch sử booking.
- [ ] Staff/Admin có thể CRUD tour.
- [ ] Staff/Admin có thể CRUD lịch khởi hành.
- [ ] Staff/Admin có thể quản lý booking.
- [ ] Admin có thể quản lý người dùng.
- [ ] Admin có thể phân quyền.
- [ ] Dashboard hiển thị các thống kê cơ bản.
- [ ] Website responsive.
- [ ] API có xác thực và phân quyền.
- [ ] Các dữ liệu nghiệp vụ quan trọng được lưu trữ nhất quán.

---

# 30. Phạm vi phát triển theo giai đoạn

## Phase 1 — Foundation

- Thiết kế database.
- Authentication.
- Role & permission.
- Layout frontend.
- API foundation.

## Phase 2 — Tour

- Destination.
- Tour.
- Itinerary.
- Schedule.
- Search/filter.

## Phase 3 — Booking

- Booking.
- Booking customers.
- Price calculation.
- Cancellation.
- Booking management.

## Phase 4 — Payment

- Payment.
- Transaction.
- Refund.
- Payment history.

## Phase 5 — Operations

- Guide.
- Guide assignment.
- Notification.
- Review.

## Phase 6 — Management

- Promotion.
- Dashboard.
- Reports.
- Audit log.

## Phase 7 — Testing & Deployment

- Unit test.
- Integration test.
- E2E test.
- Security test.
- Performance test.
- Deployment.

---

# 31. Đề xuất công nghệ

Đây là đề xuất, có thể thay đổi theo yêu cầu môn học.

## Frontend

Một trong các lựa chọn:

- React + TypeScript.
- Next.js + TypeScript.
- Vue + TypeScript.

## Backend

Một trong các lựa chọn:

- Node.js + NestJS.
- Node.js + Express.
- Java + Spring Boot.
- C# + ASP.NET Core.

## Database

- PostgreSQL.
- MySQL.

## Authentication

- JWT + Refresh Token.
- Hoặc session-based authentication.

## Storage

- Local storage trong môi trường development.
- Object storage/CDN trong production.

## Deployment

Có thể triển khai:

- Frontend trên Vercel/Cloudflare Pages.
- Backend trên Render/Railway/Fly.io hoặc VPS.
- Database trên PostgreSQL managed service.

---

# 32. Kiến trúc đề xuất

Đề xuất kiến trúc:

```text
                 ┌──────────────────────┐
                 │      Customer        │
                 │   Web Application    │
                 └──────────┬───────────┘
                            │
                            │ HTTPS
                            v
                 ┌──────────────────────┐
                 │       Backend        │
                 │      REST API        │
                 ├──────────────────────┤
                 │ Authentication       │
                 │ Tour Module          │
                 │ Booking Module       │
                 │ Payment Module       │
                 │ Guide Module         │
                 │ Review Module        │
                 │ Report Module        │
                 └──────────┬───────────┘
                            │
              ┌─────────────┼─────────────┐
              v             v             v
        ┌──────────┐  ┌──────────┐  ┌──────────┐
        │PostgreSQL│  │  Storage │  │ Payment  │
        │ Database │  │ / Images │  │ Gateway  │
        └──────────┘  └──────────┘  └──────────┘
```

---

# 33. Các rủi ro cần lưu ý

## Rủi ro 1 — Overbooking

Hai khách hàng đặt cùng lúc có thể làm vượt số chỗ.

**Giải pháp:** sử dụng transaction/locking hoặc cơ chế atomic update ở database.

## Rủi ro 2 — Thanh toán không đồng bộ

Payment thành công nhưng frontend không nhận được response.

**Giải pháp:** sử dụng transaction ID, webhook và cơ chế kiểm tra trạng thái thanh toán.

## Rủi ro 3 — Xóa dữ liệu nghiệp vụ

Xóa tour đã có booking có thể làm mất lịch sử.

**Giải pháp:** soft delete/inactive.

## Rủi ro 4 — Phân quyền sai

Customer có thể gọi trực tiếp API admin.

**Giải pháp:** authorization bắt buộc ở backend.

## Rủi ro 5 — Trùng lịch hướng dẫn viên

Một hướng dẫn viên được phân công hai tour cùng thời gian.

**Giải pháp:** kiểm tra overlap trước khi tạo assignment.

---

# 34. Checklist triển khai

## Backend

- [ ] Project structure.
- [ ] Database schema.
- [ ] Migration.
- [ ] Authentication.
- [ ] Authorization.
- [ ] User API.
- [ ] Tour API.
- [ ] Destination API.
- [ ] Itinerary API.
- [ ] Schedule API.
- [ ] Booking API.
- [ ] Payment API.
- [ ] Guide API.
- [ ] Review API.
- [ ] Promotion API.
- [ ] Notification API.
- [ ] Dashboard API.
- [ ] Validation.
- [ ] Error handling.
- [ ] Logging.
- [ ] API documentation.

## Frontend

- [ ] Customer layout.
- [ ] Admin layout.
- [ ] Home page.
- [ ] Tour listing.
- [ ] Tour detail.
- [ ] Login.
- [ ] Register.
- [ ] Profile.
- [ ] Booking flow.
- [ ] Payment page.
- [ ] My bookings.
- [ ] Review.
- [ ] Admin dashboard.
- [ ] Tour management.
- [ ] Schedule management.
- [ ] Booking management.
- [ ] Customer management.
- [ ] Guide management.
- [ ] Promotion management.
- [ ] Reports.
- [ ] Responsive UI.

## Testing

- [ ] Unit test.
- [ ] API test.
- [ ] Integration test.
- [ ] E2E booking test.
- [ ] Authentication test.
- [ ] Authorization test.
- [ ] Overbooking test.
- [ ] Payment test.
- [ ] Responsive test.
- [ ] Security test.

---

# 35. Kết luận

Hệ thống quản lý tour du lịch được xây dựng nhằm số hóa quy trình quản lý và đặt tour của công ty du lịch.

Luồng nghiệp vụ trọng tâm:

```text
Tour
  ↓
Lịch khởi hành
  ↓
Khách hàng
  ↓
Booking
  ↓
Thanh toán
  ↓
Phân công hướng dẫn viên
  ↓
Khởi hành
  ↓
Hoàn thành
  ↓
Đánh giá
```

Phiên bản MVP nên tập trung vào:

1. Authentication & Authorization.
2. Tour Management.
3. Schedule Management.
4. Booking Management.
5. Customer Management.
6. Dashboard.
7. Review cơ bản.

Sau khi MVP ổn định, có thể mở rộng Payment Gateway, Promotion, Guide Management, Notification và Reporting nâng cao.

---

# 36. Thứ tự thực hiện đề xuất

Để tránh xây dựng lan man, thứ tự triển khai nên là:

```text
01. Database & ERD
        ↓
02. Authentication
        ↓
03. User & Role
        ↓
04. Destination
        ↓
05. Tour
        ↓
06. Itinerary
        ↓
07. Tour Schedule
        ↓
08. Booking
        ↓
09. Payment
        ↓
10. Guide
        ↓
11. Review
        ↓
12. Promotion
        ↓
13. Notification
        ↓
14. Dashboard
        ↓
15. Testing
        ↓
16. Deployment
```

**Kết thúc tài liệu yêu cầu — Version 1.0**
