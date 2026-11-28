# ADR-004: Cơ chế Xác thực và Phân quyền sử dụng JWT và Refresh Token

* **Trạng thái:** Accepted
* **Người quyết định**: Nguyễn Tấn Mạnh
* **Ngày quyết định**: 27-11-2026
* **Yêu cầu liên quan:** NFR-002, FR-001, FR-011, FR-038

## Bối cảnh
- Hệ thống cần cơ chế xác thực an toàn, phi trạng thái cho ứng dụng di động và ứng dụng Admin Web kết nối qua REST API. Hệ thống phải có khả năng lập tức vô hiệu hóa quyền truy cập khi người dùng bị khóa tài khoản hoặc bị hạ quyền. Ngoài ra, kiến trúc tương lai cần hỗ trợ kết nối Real-time qua WebSocket / SignalR cho tính năng nhắn tin và Đấu trường mà không làm tăng tải CSDL ở mỗi request.
- Hệ thống cần lưu trữ mật khẩu người dùng an toàn, chống dò mật khẩu ngoại tuyến và hỗ trợ nâng cấp độ khó trong tương lai mà không buộc người dùng đặt lại mật khẩu.

## Các phương án đã xem xét

| Phương án | Ưu điểm | Nhược điểm |
|---|---|---|
| **(a) JWT Access Token ngắn hạn + Refresh Token xoay vòng** | Stateless cho hầu hết các API request, giảm tải CSDL, hỗ trợ mobile app lưu token dài hạn an toàn, dễ truyền qua SignalR qua Query String/Header. | Phải tự xây dựng cơ chế thu hồi và xoay vòng Refresh Token ở Backend. |
| **(b) Cookie-based Session truyền thống** | Dễ triển khai trên Web, tự động thu hồi ngay khi xóa Session ở Server. | Khó làm việc với ứng dụng di động Native, gặp rào cản Cross-Domain và CSRF trên API di động. |
| **(c) Reference Token** | Thu hồi ngay lập tức ở mọi request, kiểm soát an toàn 100%. | Mọi API request đều phải truy vấn CSDL/Redis để kiểm tra token, gây chai nghẽn hiệu năng. |

## Quyết định
Chọn **Phương án (a) JWT Access Token ngắn hạn kết hợp Refresh Token xoay vòng**.

Các thông số kỹ thuật được chốt:
1. **Thuật toán ký và khóa:** Sử dụng HMAC-SHA256 `HS256`. Khóa secret key được đọc từ biến môi trường `ENV`, không commit vào Git repositories.
2. **Thời gian hiệu lực:** Access Token sống **15 phút**; Refresh Token sống **7 ngày**.
3. **An toàn Refresh Token:** Refresh Token được băm SHA256 trước khi lưu vào CSDL `RefreshTokens`. Khi Refresh Token được sử dụng, nó sẽ bị hủy và thay thế bằng một token mới. Nếu phát hiện một Refresh Token cũ đã bị thay thế cố tình tái sử dụng, toàn bộ chuỗi token của người dùng đó sẽ bị thu hồi ngay lập tức để chống tấn công giả mạo.
4. **Phân quyền và khóa tài khoản:** Lưu `UserRole` trong JWT Claims. Phân quyền API theo Policy-based Auth `RequireRole("Admin")`. Khi tài khoản bị vô hiệu hóa Middleware sẽ chặn việc cấp Access Token mới từ Refresh Token.
5. **Lưu trữ trên Client:** Ứng dụng di động .NET MAUI sử dụng `SecureStorage` của thiết bị để lưu trữ cặp Token.

Sử dụng `PasswordHasher<object>` của `Microsoft.AspNetCore.Identity` PBKDF2-HMAC-SHA512, muối ngẫu nhiên 128-bit, định dạng V3 có tiền tố phiên bản phía sau abstraction `IPasswordHasher` ở tầng `Application`.
- Số vòng lặp đọc từ cấu hình `Security:PasswordHashing:IterationCount`, tốc độ trung bình `<điền ms đã đo>` ms/lần trên môi trường phát triển.
- Khi `Verify` trả về `PasswordCheck.SuccessRehashNeeded`, `AuthService.AuthenticateAsync` tự động băm lại mật khẩu với `IterationCount` mới ngay tại lần đăng nhập thành công.

Quyết định về Access Token JWT và Refresh Token
- **Thuật toán ký JWT:** `HS256` khóa đối xứng tối thiểu 32 byte đọc từ User Secrets / biến môi trường `Jwt__SigningKey`, kiểm tra fail-fast qua `.ValidateOnStart()`.
- **Thời hạn và Claims:** Access token sống **15 phút** `ClockSkew = 30s`, chỉ chứa `sub`, `jti`, `iat`, `nbf`, `exp`, `iss`, `aud`, `role` — tuyệt đối không chứa email, tên hiển thị hay mật khẩu băm.
- **Refresh token & Lưu trữ:** Chuỗi ngẫu nhiên 32 byte mã hóa Base64Url 43 ký tự, chỉ lưu bản băm **SHA-256** 64 ký tự hex vào cột `TokenHash`.
- **Xoay vòng và Phát hiện dùng lại:**
  - Mỗi lần làm mới sẽ thu hồi nguyên tử token cũ `UPDATE ... WHERE Id = @id AND RevokedAt IS NULL` và phát hành cặp token mới.
  - Cột `RevokedAt` vừa đóng vai trò cờ thu hồi vừa là mốc tính **thời gian ân hạn 10 giây** `ReuseGraceSeconds`. Dùng lại token đã thu hồi trong 10 giây đầu chỉ trả `401` xử lý request song song do mạng, dùng lại sau 10 giây bị coi là đánh cắp và thu hồi toàn bộ refresh token đang hoạt động của người dùng đó chấp nhận đăng xuất mọi thiết bị do ERD đóng băng chưa có `FamilyId`.

## Hệ quả
- **Lợi ích:** Đảm bảo hiệu năng API cao, API không cần soi CSDL kiểm tra token trong 15 phút sống của Access Token, hỗ trợ trải nghiệm đăng nhập vô tận mượt mà cho ứng dụng di động.
- **Đánh đổi và rủi ro:** Access Token không thể thu hồi ngay lập tức trong vòng 15 phút hiệu lực ngoại trừ việc duy trì một danh sách đen tạm thời.
- **Việc phải làm theo:** Tích hợp việc đọc JWT qua Query String `access_token` cho Hub kết nối SignalR trong giai đoạn R4/R5.
- OWASP ưu tiên Argon2id tối thiểu 19 MiB RAM, 2 vòng lặp hơn PBKDF2. Dự án chấp nhận dùng PBKDF2-HMAC-SHA512 có sẵn của Microsoft để tránh phụ thuộc gói bên thứ ba ở giai đoạn đầu và giữ Clean Architecture gọn nhẹ.
- Nhờ đóng gói sau `IPasswordHasher`, việc chuyển sang Argon2id sau này chỉ cần thay một lớp cài đặt ở `Renmei.Infrastructure` và tận dụng cơ chế `SuccessRehashNeeded` để chuyển đổi dần khi người dùng đăng nhập.

## Khi nào xem lại
Xem lại khi hệ thống tích hợp đăng nhập mạng xã hội OAuth2 / OpenID Connect với Google, Apple hoặc khi yêu cầu thu hồi quyền truy cập tức thì Real-time Revocation < 1 giây trở thành bắt buộc cho toàn bộ API.