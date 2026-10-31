# SRS: Software Requirements Specification

| Phiên bản : 0.1 | Ngày : 31-10-2026 | Trạng thái : Draft | Cổng : R1-G1 | Người duyệt : Nguyễn Tấn Mạnh |

## 1. Giới thiệu
- **Mục đích**: Tài liệu xác định toàn bộ yêu cầu chức năng và phi chức năng cho ứng dụng học Kanji Renmei (Release 1).
- **Phạm vi**: Hệ thống Backend Web API (.NET 10) và ứng dụng di động Android ( .NET MAUI).

## 2. Mô tả Tổng quan
- **Persona**: Học viên tiếng Nhật trình độ N5-N3 cần công cụ ôn tập lặp lại ngắt quãng (SRS) có hệ thống.
- **Ràng buộc kĩ thuật**:
  - Backend: Clean Architecture, .NET 10 Web API.
  - Mobile: .NET MAUI (ưu tiên nền tảng Android cho Release 1).

## 3. Functional Requirements
| Mã YC		 | Tên chức năng      | Mô tả chi tiết											| Ưu tiên | Tiêu chí chấp nhận								
| **FR-001** | Đăng nhập hệ thống | Cho phép người dùng đăng nhập bằng Email,Mật khẩu		| P0	  | Trả về JWT Access Token hợp lệ; báo lỗi 401 nếu sai mật khẩu. 
| **FR-002** | Tra cứu Kanji	  | Tra cứu từ vựng/Kanji theo Âm Hán Việt, Onyomi, Kunyomi | P0	  | Thời gian phản hồi API < 200ms với 2,000 bản ghi. 
| **FR-003** | Ôn tập SM-2		  | Tính toán lịch ôn tập Kanji dựa trên thuật toán SM-2	| P0	  | Cập nhật chính xác `Interval` và `Repetition` sau mỗi phiên chấm điểm 0-5.

## 4. Non-Functional Requirements
- **NFR-001 (Hiệu năng)**: API phản hồi dưới 300ms cho 95% số lượng request trong điều kiện tải bình thường.
- **NFR-002 (Bảo mật)**: Mật khẩu người dùng bắt buộc được mã hóa hash BCrypt trước khi lưu vào Database.
- **NFR-003 (Ngoại tuyến)**: App Mobile cho phép học viên ôn tập offline và đồng bộ dữ liệu khi có mạng.

## 5. Ngoài phạm vi
- Chưa hỗ trợ nhận diện chữ viết tay Kanji qua Camera/AI.
- Chưa hỗ trợ thanh toán trực tuyến qua cổng Momo/ZaloPay.

## 6. Chỉ số Thành công
1. **Chỉ số 1: Tỷ lệ nhớ trong 7 ngày**
   - **Mô tả:** Tỷ lệ phần trăm các từ Kanji học viên trả lời đúng trong các phiên ôn tập sau 7 ngày kể từ lần học đầu tiên.
   - **Đánh giá:** Đạt chỉ số nếu tỷ lệ nhớ >= 85%. Đo lường dựa trên lịch sử đánh giá chất lượng phản hồi (phân điểm 0-5) của thuật toán SM-2 trong cơ sở dữ liệu.

2. **Chỉ số 2: Thói quen học và giờ giấc**
   - **Mô tả:** Mức độ duy trì chuỗi học tập và khung giờ học cố định hằng ngày của người dùng.
   - **Đánh giá:** Đạt chỉ số nếu người dùng duy trì chuỗi học >= 5 ngày/tuần và thực hiện phiên ôn tập trong khoảng khung giờ ưu tiên đã đăng ký với độ lệch 1 giờ.

3. **Chỉ số 3: Tỷ lệ giữ chân vào ngày thứ 7**
   - **Mô tả:** Tỷ lệ phần trăm người dùng mới tiếp tục quay lại ứng dụng và thực hiện ít nhất 1 phiên học, ôn tập vào ngày thứ 7 sau khi đăng ký tài khoản.
   - **Đánh giá:** Mục tiêu đạt >= 40% số lượng người dùng mới trong tuần. Đo lường tự động qua nhật ký truy cập.

## 7. Giả định và Rủi ro
- **Giả định**: .NET 10 SDK giữ tính ổn định tương thích ngược tốt với các thư viện NuGet core.
- **Rủi ro**: Thuật toán SM-2 có thể gây dồn lịch học nếu học viên bỏ dở quá 7 ngày.

