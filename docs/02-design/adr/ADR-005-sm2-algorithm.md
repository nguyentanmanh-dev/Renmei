# ADR-005: Lựa chọn Thuật toán Ôn tập Lặp lại Ngắt quãng SM-2

* **Trạng thái:** Accepted
* **Người quyết định**: Nguyễn Tấn Mạnh
* **Ngày quyết định**: 09-11-2026
* **Yêu cầu liên quan:** FR-003, FR-004, FR-005, NFR-001, NFR-003

## Bối cảnh
Tính năng cốt lõi của Renmeilà giúp người học ghi nhớ Kanji hiệu quả theo phương pháp lặp lại ngắt quãng. Chỉ số mục tiêu quan trọng của sản phẩm là đạt tỷ lệ nhớ từ sau 7 ngày >= 85%. Hệ thống cần một thuật toán vừa đủ chính xác, có thể tùy biến cá nhân hóa theo năng lực từng người dùng, tính toán nhanh ở Backend Server, và đủ nhẹ để có thể chạy thử nghiệm Offline trên ứng dụng di động khi không có kết nối mạng.

## Các phương án đã xem xét

| Phương án | Ưu điểm | Nhược điểm |
|---|---|---|
| **(a) Thuật toán SM-2** | Thuật toán kinh điển, tài liệu và công thức rõ ràng, tính toán cực nhanh, dễ lập trình và viết Unit Test, không cần dữ liệu lịch sử lớn. | Chưa tối ưu hoàn hảo bằng các thuật toán hiện đại dựa trên AI/Machine Learning cho các tập dữ liệu cực lớn. |
| **(b) Hệ thống Leitner** | Cực kỳ đơn giản, dễ cài đặt bằng các mảng cố định hộp 1 đến hộp 5. | Khoảng thời gian ôn tập cố định, ít tính cá nhân hóa độ khó của từng từ vựng riêng biệt. |
| **(c) Thuật toán FSRS** | Độ chính xác dự đoán khả năng nhớ cao hơn SM-2, thuật toán tiên tiến nhất hiện nay. | Yêu cầu tập dữ liệu lịch sử ôn tập thật khổng lồ để huấn luyện các tham số ban đầu, phức tạp khi triển khai ở giai đoạn đầu dự án. |

## Quyết định
Chúng tôi chọn **Phương án (a) Thuật toán SM-2** làm thuật toán khởi nguồn cho hệ thống học tập của Renmei.

Các quy tắc thuật toán cốt lõi được chốt trong ADR:
1. **Hệ số độ dễ:** Mặc định EF = 2.5. Giới hạn EF_min = 1.3.
2. **Khoảng thời gian ôn tập:**
   - Lần đúng 1: Interval = 1 ngày.
   - Lần đúng 2: Interval = 6 ngày.
   - Lần đúng > 2: Interval = Interval(n-1) x EF.
3. **Trường hợp ôn trễ N ngày:** Nếu người học ôn muộn N ngày so với `due_date`, khoảng thời gian ôn tập tiếp theo được tính theo công thức điều chỉnh SRS:
   Interval = [Interval(n-1) + N] x EF
4. **Trường hợp đánh giá Quên:** Reset chuỗi đúng `repetitions = 0`, lùi `interval = 1` ngày, giữ nguyên hoặc trừ EF.
5. **Điều kiện công nhận Thành thạo:** Thẻ Kanji đạt trạng thái `Mastered` khi `interval >= 21` ngày và chuỗi đánh giá Nhớ liên tục >= 3 lần.

## Hệ quả
- **Lợi ích:** 
  - Thuật toán đơn giản, phản hồi tức thì, giúp server tính toán hàng triệu bản ghi ôn tập trong vài miligiây.
  - Mọi lượt ôn đều ghi vết đầy đủ vào bảng `ReviewLogs`. Lịch sử dữ liệu này đảm bảo trong tương lai có thể chuyển đổi sang FSRS mà không bị mất dữ liệu quá khứ.
  - Các kết quả từ trò chơi đấu trường sẽ không làm thay đổi trực tiếp chỉ số SM-2 của thẻ để tránh làm nhiễu lịch học tập chuẩn.
- **Đánh đổi và rủi ro:** Đôi khi khoảng cách giữa các lần ôn có thể giãn ra quá nhanh nếu người dùng bấm "Dễ" nhiều lần liên tục.
- **Việc phải làm theo:** Xây dựng bộ Unit Test bao phủ 100% các kịch bản biên của `Sm2EngineService`.

## Khi nào xem lại
Xem lại để nâng cấp lên FSRS khi hệ thống Renmei tích lũy đủ dữ liệu học tập thật của ít nhất 1.000 người dùng tích cực với hơn 100.000 bản ghi `ReviewLogs`.