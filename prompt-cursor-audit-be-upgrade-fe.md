# PROMPT CHO CURSOR — RÀ SOÁT BACKEND THEO TỪNG ROLE/LUỒNG + NÂNG CẤP GIAO DIỆN

> Cách dùng: Dán vào Cursor Agent mode, đính kèm (@) file `yeu-cau-he-thong-quan-ly-tour-du-lich.md`
> và toàn bộ project hiện tại vào context. Nên chạy **PHẦN A (audit) trước, sửa xong mới sang PHẦN B
> (FE)** — vì FE đẹp mà BE sai luồng/sai quyền thì vẫn hỏng.

---

## BỐI CẢNH

Dự án ASP.NET Core MVC "Hệ thống quản lý tour du lịch" đã được dựng theo tài liệu yêu cầu đính kèm.
Bây giờ cần làm 2 việc:

1. **Audit toàn bộ Backend** — kiểm tra từng chức năng, với từng role (Customer/Staff/Admin), theo
   đúng luồng nghiệp vụ mô tả trong tài liệu, tìm và sửa lỗi/thiếu sót.
2. **Nâng cấp Frontend** — làm giao diện đẹp hơn, chi tiết hơn (nút bấm, nhãn chữ, thông báo), đặc
   biệt là Header và Footer phải nổi bật, sáng tạo, chuyên nghiệp hơn hẳn hiện tại.

Không được sửa đổi cấu trúc thư mục đã thống nhất trước đó (MVC thuần, 1 project, Areas cho
Admin/Staff). Không thêm framework FE mới (không React/Vue), vẫn Razor Views + Bootstrap +
CSS/JS thuần (có thể thêm thư viện JS nhẹ nếu cần, ví dụ AOS cho animation, SweetAlert2 cho popup).

---

## PHẦN A — AUDIT & FIX BACKEND

### A.1. Cách làm việc

Với **mỗi module** dưới đây, hãy:
1. Đọc lại đúng phần tương ứng trong tài liệu yêu cầu (số mục đã ghi kèm).
2. Kiểm tra Controller + Service + Repository/DbContext hiện tại có đáp ứng đủ chưa.
3. Với **mỗi role liên quan**, thử lại từng luồng thao tác (giả lập thủ công theo logic code, không
   chỉ đọc lướt) và liệt kê lỗi/thiếu sót tìm được trước khi sửa.
4. Sửa lỗi, bổ sung phần thiếu, viết chú thích ngắn tại chỗ đã sửa nếu là logic nghiệp vụ quan trọng.
5. Sau khi sửa xong 1 module, tóm tắt lại (dạng checklist ✅/❌ → đã fix) trước khi sang module kế.

### A.2. Checklist audit theo từng module và role

**1. Authentication & Account (mục 4)**
- [ ] Đăng ký: validate email trùng, phone trùng (nếu bắt buộc), độ mạnh mật khẩu, thông báo lỗi rõ
      ràng từng trường — không chỉ 1 câu lỗi chung chung.
- [ ] Đăng nhập: xác định đúng role sau khi login, redirect đúng khu vực (Customer → trang chủ,
      Staff/Admin → khu quản trị). Tài khoản bị khoá (mục 11 - FR-CUSTOMER-03) phải bị chặn đăng
      nhập với thông báo rõ ràng, không phải lỗi 500 hay lỗi sai mật khẩu.
- [ ] Quên mật khẩu: luồng gửi email → reset có hoạt động thật hay chỉ là UI giả.
- [ ] Quản lý hồ sơ: Customer chỉ sửa được hồ sơ của chính mình (kiểm tra không được sửa hồ sơ người
      khác bằng cách đổi id trên URL — thử kịch bản IDOR).

**2. Phân quyền tổng thể (mục 18, NFR-01, Rủi ro 4) — QUAN TRỌNG NHẤT**
- [ ] Rà soát **từng Controller/Action** trong Area Admin: có `[Authorize(Roles = "Admin,Staff")]`
      hoặc tương đương chưa? Có action nào quên gắn attribute không?
- [ ] Rà soát các action chỉ dành riêng Admin (quản lý user, phân quyền, cấu hình hệ thống, audit
      log) — Staff KHÔNG được truy cập, phải kiểm tra riêng chứ không dùng chung role Admin+Staff.
- [ ] Thử kịch bản: 1 Customer đã đăng nhập gọi thẳng URL của Admin/Staff (ví dụ
      `/Admin/Tour/Delete/1`) → phải bị chặn (403/redirect), không được chỉ dựa vào việc ẩn menu.
- [ ] Thử kịch bản: người dùng chưa đăng nhập gọi thẳng các action cần đăng nhập → redirect về login
      đúng, giữ lại returnUrl.

**3. Tour, Điểm đến, Lịch trình (mục 5, 6, 7)**
- [ ] Customer: danh sách có phân trang thật (không load hết rồi cắt ở view), tìm kiếm + lọc + sắp
      xếp hoạt động đúng kết hợp cùng lúc (VD: vừa lọc điểm đến vừa sắp xếp giá).
- [ ] Trang chi tiết tour hiển thị đủ toàn bộ trường liệt kê ở FR-TOUR-05, kể cả "số chỗ còn lại"
      lấy đúng từ lịch khởi hành gần nhất/đang mở, không hard-code.
- [ ] Staff/Admin: CRUD tour — thử xoá 1 tour đã có booking → phải tự động chuyển INACTIVE thay vì
      xoá cứng hoặc báo lỗi khó hiểu (theo FR-TOUR-06).
- [ ] Lịch trình: sắp xếp lại thứ tự ngày có lưu đúng thứ tự mới xuống DB không (kiểm tra kỹ, đây là
      lỗi hay gặp — chỉ đổi thứ tự trên UI mà không persist).

**4. Lịch khởi hành (mục 8)**
- [ ] Số chỗ còn lại = Số chỗ tối đa − Số chỗ đã đặt, tính đúng và **real-time** sau mỗi booking
      thành công/huỷ.
- [ ] Trạng thái tự động chuyển FULL khi hết chỗ, và cho phép đặt lại nếu có huỷ booking làm dư chỗ.
- [ ] Không cho tạo lịch khởi hành với ngày đã qua hạn (validate ngày ở cả client lẫn server).
- [ ] Xoá lịch: chỉ cho xoá nếu **chưa phát sinh giao dịch** — thử xoá lịch đã có booking phải bị
      chặn.

**5. Booking (mục 9, 22.1, BR-01, BR-02) — TRỌNG TÂM CHỐNG OVERBOOKING**
- [ ] Chạy thử kịch bản đặt đồng thời (concurrency): giả lập 2 request gần như cùng lúc đặt vào lịch
      chỉ còn 1 chỗ → chỉ 1 request được thành công, request kia phải nhận lỗi rõ ràng
      ("SCHEDULE_FULL" hoặc tương đương), không được để cả 2 cùng thành công (kiểm tra transaction/
      concurrency token có hoạt động thật, viết 1 test hoặc script mô phỏng nếu cần).
- [ ] Không cho đặt vào lịch đã CLOSED/CANCELLED/đã quá ngày khởi hành (BR-01).
- [ ] Áp mã khuyến mãi: kiểm tra đủ 5 điều kiện ở FR-BOOKING-04 (tồn tại, còn hiệu lực, đủ điều kiện,
      chưa vượt lượt dùng, đúng phạm vi áp dụng) — thử từng trường hợp sai để chắc chắn bị từ chối
      đúng thông báo.
- [ ] Công thức tính tiền đúng theo FR-BOOKING-05, hiển thị breakdown rõ ràng (tạm tính/giảm giá/phụ
      phí/tổng) chứ không chỉ hiện con số tổng.
- [ ] Mã booking sinh ra là duy nhất, không trùng (kiểm tra cơ chế sinh mã, tránh race condition khi
      sinh mã).
- [ ] Trạng thái booking chuyển đúng theo vòng đời PENDING → CONFIRMED → PAID → COMPLETED, và
      CANCELLED có thể xảy ra ở các bước phù hợp — không cho nhảy trạng thái tuỳ tiện (VD không thể
      từ PENDING nhảy thẳng COMPLETED).
- [ ] Huỷ booking: áp đúng chính sách hoàn tiền theo số ngày còn lại trước khởi hành (BR-03), tính
      toán % hoàn đúng cấu hình.

**6. Thanh toán (mục 10)**
- [ ] Trạng thái thanh toán UNPAID/PARTIAL_PAID/PAID/REFUNDED cập nhật đúng khi Staff xác nhận thanh
      toán, và đồng bộ đúng với trạng thái Booking (PAID booking chỉ khi Payment = PAID).
- [ ] Hoàn tiền: chỉ Staff/Admin thực hiện được, ghi nhận đúng transaction hoàn tiền, không chỉnh sửa
      trực tiếp số tiền gốc.

**7. Khách hàng, Hướng dẫn viên, Phân công (mục 11, 12)**
- [ ] Khoá tài khoản khách hàng → thử đăng nhập lại phải bị chặn ngay (không phải chặn ở lần sau).
- [ ] Phân công hướng dẫn viên: thử phân công 1 hướng dẫn viên vào 2 lịch khởi hành có thời gian
      chồng lấn (BR-06) → hệ thống phải cảnh báo/chặn thật, không chỉ là comment TODO trong code.

**8. Đánh giá (mục 13, BR-04)**
- [ ] Chỉ cho đánh giá khi booking đã COMPLETED — thử đánh giá khi booking đang PENDING/CONFIRMED
      phải bị chặn.
- [ ] Điểm trung bình tour tự động cập nhật lại ngay sau khi có đánh giá mới/bị ẩn/bị xoá (không chỉ
      tính 1 lần lúc tạo).
- [ ] Staff/Admin ẩn đánh giá → đánh giá đó không còn hiển thị phía Customer và không tính vào điểm
      trung bình.

**9. Khuyến mãi (mục 14, BR-07)**
- [ ] Số lượt đã sử dụng tăng đúng và atomic khi có booking áp dụng mã thành công (tránh
      race condition tương tự vấn đề overbooking).
- [ ] Test đủ các case ở FR-PROMO-02 (không tồn tại, hết hạn, chưa bắt đầu, hết lượt, không đủ điều
      kiện).

**10. Thông báo (mục 15)**
- [ ] Thông báo thực sự được tạo tự động tại đúng các thời điểm liệt kê (đặt thành công, xác nhận,
      thanh toán, huỷ, thay đổi tour, sắp khởi hành) — không phải chỉ có sẵn UI danh sách trống.
- [ ] Đánh dấu đã đọc cập nhật đúng trạng thái, không load lại nhầm toàn bộ về "chưa đọc".

**11. Dashboard & Báo cáo (mục 16, 17)**
- [ ] Số liệu Dashboard lấy từ dữ liệu thật trong DB (không hard-code), thử thêm/xoá dữ liệu và kiểm
      tra số liệu có đổi theo không.
- [ ] Thống kê doanh thu theo ngày/tuần/tháng/quý/năm tính đúng khoảng thời gian, không lệch múi giờ.

**12. Người dùng & Phân quyền (mục 18)**
- [ ] Admin tạo/sửa/khoá Staff mới hoạt động đúng, gán role đúng ngay khi tạo.
- [ ] Audit log (mục 28): thử thực hiện các hành động liệt kê (login, tạo/sửa/xoá tour, đổi giá, xác
      nhận booking, huỷ booking, hoàn tiền, đổi quyền) → kiểm tra có bản ghi log thật trong DB không,
      đủ các trường user_id/action/entity_type/entity_id/old_value/new_value/IP/created_at.

### A.3. Validation & Error handling tổng quát (mục 26, 27, NFR-01, NFR-02, NFR-03)
- [ ] Toàn bộ validate phải chạy **ở backend** dù frontend đã validate (không tin dữ liệu client).
- [ ] Format lỗi trả về nhất quán cho các action AJAX (theo tinh thần mục 27: success/message/code/
      data).
- [ ] Danh sách dài (tour, booking, customer, guide...) đều có pagination thật ở query, không
      `ToList()` hết rồi phân trang ở C#/view.
- [ ] Kiểm tra không có N+1 query ở các trang danh sách có include quan hệ (dùng `.Include()` hoặc
      projection hợp lý).
- [ ] Không lộ thông tin nhạy cảm (password hash, token nội bộ...) ra View/JSON response.

### A.4. Đầu ra mong muốn của Phần A

Sau khi audit + sửa xong toàn bộ, viết 1 bảng tổng kết dạng:

| Module | Role | Lỗi/thiếu sót phát hiện | Đã fix? |
|---|---|---|---|
| ... | ... | ... | ✅/❌ |

Nếu có lỗi không thể tự sửa (thiếu quyết định nghiệp vụ, ví dụ % hoàn tiền cụ thể), liệt kê rõ và hỏi
lại thay vì tự bịa.

---

## PHẦN B — NÂNG CẤP GIAO DIỆN (FE)

> Chỉ bắt đầu phần này sau khi Phần A đã ổn định, để không phải sửa lại UI do đổi luồng nghiệp vụ.

### B.1. Nguyên tắc chung
- Giữ nguyên Bootstrap 5 làm nền, nhưng **không dùng giao diện Bootstrap mặc định thô** — tuỳ biến
  màu sắc, bo góc, đổ bóng, khoảng cách, hiệu ứng hover/transition để có phong cách riêng, hiện đại,
  hợp chủ đề du lịch (gợi ý tông màu: xanh biển/xanh ngọc + cam đất/vàng nắng, hoặc chọn 1 bảng màu
  nhất quán rồi định nghĩa thành CSS variables dùng xuyên suốt, không hard-code màu rải rác).
- Toàn bộ chữ (label, placeholder, tooltip, thông báo) phải là tiếng Việt tự nhiên, rõ nghĩa, không
  cụt lủn kiểu "Tên" mà nên là "Tên tour", không chỉ "Ngày" mà nên là "Ngày khởi hành".
- Icon: dùng Bootstrap Icons hoặc Font Awesome nhất quán cho toàn bộ nút/menu, không icon lẫn lộn kiểu.
- Responsive thật sự trên desktop/tablet/mobile (test cả 3 breakpoint), không chỉ co giãn được mà
  phải sắp xếp lại hợp lý (VD: bảng dài trên mobile chuyển thành dạng card).

### B.2. Header (nâng cấp toàn diện)
- Thiết kế header 2 tầng nếu phù hợp: tầng trên nhỏ (hotline, email hỗ trợ, ngôn ngữ/tiền tệ nếu có),
  tầng chính (logo, menu chính, ô tìm kiếm nhanh tour, icon giỏ/đơn của tôi, icon thông báo có badge
  số lượng chưa đọc, avatar + dropdown tài khoản).
- Menu chính có dropdown mega-menu hoặc dropdown đẹp cho "Điểm đến" (liệt kê theo khu vực/quốc gia).
- Header có hiệu ứng sticky khi cuộn (thu gọn nhẹ chiều cao khi scroll xuống) và đổi nền
  trong suốt/màu khi ở trang chủ có banner lớn.
- Trạng thái đăng nhập rõ ràng: chưa đăng nhập hiện nút "Đăng nhập"/"Đăng ký" nổi bật (dạng button có
  màu nhấn), đã đăng nhập hiện avatar + tên + dropdown (Hồ sơ, Đơn của tôi, Thông báo, Đăng xuất).
- Header khu vực Admin/Staff: sidebar riêng gọn gàng, có thể thu/phóng (collapse), có breadcrumb ở
  đầu mỗi trang, hiển thị tên + role người đang đăng nhập.

### B.3. Footer (nâng cấp toàn diện)
- Footer nhiều cột, ví dụ: Giới thiệu công ty (logo + mô tả ngắn + mạng xã hội), Liên kết nhanh (Tour
  nổi bật, Điểm đến, Về chúng tôi, Liên hệ), Hỗ trợ khách hàng (Chính sách hủy, Điều khoản, FAQ), Liên
  hệ (địa chỉ, hotline, email, giờ làm việc), và có thể thêm ô đăng ký nhận bản tin (form nhập email,
  chỉ cần UI, không bắt buộc nối logic gửi mail thật nếu ngoài phạm vi MVP).
- Có dải "Đối tác/Chứng nhận" hoặc "Cam kết dịch vụ" (icon + text ngắn, VD: "Giá tốt nhất", "Hỗ trợ
  24/7", "Hủy linh hoạt", "Thanh toán an toàn") ngay phía trên phần footer chính để tăng độ tin cậy.
- Dòng cuối cùng: bản quyền + có thể thêm icon phương thức thanh toán chấp nhận.
- Màu nền footer tương phản tốt với nền trang, đảm bảo độ tương phản chữ đủ đọc rõ (accessibility).

### B.4. Chi tiết nút bấm & vi tương tác (micro-interactions)
- Chuẩn hoá hệ thống nút: Primary (hành động chính, VD "Đặt tour ngay", "Thanh toán", "Xác nhận"),
  Secondary (hành động phụ, VD "Xem chi tiết", "Quay lại"), Danger (hành động huỷ/xoá, có xác nhận
  bằng modal/SweetAlert trước khi thực thi, không dùng `confirm()` mặc định của trình duyệt).
- Nút có icon đi kèm chữ khi hợp lý (VD icon lịch cạnh "Xem lịch khởi hành"), có hiệu ứng hover/active
  rõ ràng (đổi màu nhẹ, nâng bóng, hoặc scale nhẹ), có trạng thái loading (spinner + disable) khi đang
  submit form để tránh người dùng bấm nhiều lần gây trùng dữ liệu (đặc biệt nút "Xác nhận đặt tour" và
  "Thanh toán" — đây liên quan trực tiếp rủi ro overbooking/trùng giao dịch).
- Trạng thái (status) hiển thị dạng badge màu sắc riêng biệt cho từng trạng thái (OPEN xanh lá, FULL
  cam, CLOSED xám, CANCELLED đỏ; tương tự cho trạng thái booking và payment), không chỉ hiện text đơn
  thuần.
- Empty state: khi danh sách rỗng (chưa có booking, chưa có thông báo, tìm kiếm không ra kết quả)
  phải có hình minh hoạ/icon + câu dẫn thân thiện + nút hành động gợi ý (VD "Chưa có đơn nào — Khám
  phá tour ngay"), không để trắng trang hoặc chỉ hiện chữ "No data".
- Toast/Alert thông báo kết quả hành động (thành công/thất bại) dùng thư viện đẹp (SweetAlert2/
  Toastr) thay vì `alert()` mặc định.

### B.5. Trang chi tiết & luồng đặt tour — ưu tiên trải nghiệm cao nhất
- Trang danh sách tour: card tour đẹp, có ảnh, badge giảm giá nếu có khuyến mãi, hiển thị sao đánh
  giá trung bình, giá rõ ràng ("Từ ... VNĐ/khách"), nút "Xem chi tiết" nổi bật.
- Trang chi tiết tour: bố cục rõ ràng (ảnh gallery, tab hoặc section riêng cho Lịch trình/Dịch vụ bao
  gồm-không bao gồm/Chính sách/Đánh giá), phần chọn lịch khởi hành dạng danh sách card có hiển thị số
  chỗ còn lại trực quan (progress bar hoặc badge số chỗ), disable rõ ràng các lịch đã FULL/CLOSED.
- Luồng đặt tour: hiển thị dạng step/stepper (VD 1. Chọn lịch → 2. Thông tin khách → 3. Khuyến mãi &
  thanh toán → 4. Xác nhận) để người dùng biết đang ở bước nào, tránh cảm giác form dài vô tận.
- Trang "Đơn của tôi": timeline trạng thái trực quan cho từng booking (PENDING → CONFIRMED → PAID →
  COMPLETED, có nhánh CANCELLED), không chỉ hiện 1 dòng chữ trạng thái.

### B.6. Khu vực Admin/Staff
- Dashboard: dùng biểu đồ (Chart.js) cho doanh thu theo thời gian, biểu đồ tròn/cột cho booking theo
  trạng thái, card số liệu tổng quan có icon + màu sắc phân biệt.
- Bảng dữ liệu (DataTable) cho danh sách tour/booking/customer/guide: có tìm kiếm, sắp xếp, phân
  trang phía client hoặc server tuỳ độ lớn dữ liệu, có nút hành động (sửa/xoá/xem) dạng icon rõ ràng
  kèm tooltip.
- Form tạo/sửa (tour, lịch trình nhiều ngày, lịch khởi hành...): chia section rõ ràng, validate hiển
  thị lỗi ngay dưới từng field bằng tiếng Việt, không dồn hết lỗi lên đầu form.

### B.7. Không được làm
- Không đổi tên route/action đã hoạt động đúng ở Phần A chỉ vì lý do thẩm mỹ.
- Không thêm thư viện FE nặng không cần thiết (giữ trang tải nhanh).
- Không phá vỡ luồng nghiệp vụ đã audit đúng ở Phần A khi chỉnh sửa View/JS.

### B.8. Đầu ra mong muốn của Phần B
Sau khi hoàn thành, liệt kê ngắn gọn: những trang/thành phần nào đã nâng cấp, thư viện JS/CSS nào đã
thêm (nếu có) và lý do, kèm ảnh chụp màn hình mô tả (nếu Cursor có khả năng) hoặc mô tả bằng text các
thay đổi chính ở Header/Footer.
