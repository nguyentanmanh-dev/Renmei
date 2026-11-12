# Go/no-go tháng 1

- **Ngày:** 12-11-2026 | **Người quyết định:** Nguyễn Tấn Mạnh | **Kết quả:** Go

## Số liệu

| Tiêu chí | Ngưỡng mục tiêu | Số đo thực tế | Kết quả |
| :--- | :--- | :--- | :--- |
| **Tiến độ việc P0 tháng 1** | >= 90% có bằng chứng | Toàn bộ thẻ P0 tuần 1 đến 4 đã xong và có link | Go |
| **Độ trễ còn lại** | còn 1/3 tuần làm việc | Các việc nền R1-G2 đã hoàn tất | Go |
| **Nền thiết kế** | SRS duyệt, ERD/OpenAPI sẵn sàng, License đã kiểm tra | SRS đã duyệt, Bộ ADR cốt lõi và sổ License `content-sources.json` đã hoàn tất | Go |
| **Nội dung** | Bảng đối chiếu Hán Việt N5 xong, danh sách duyệt có | Đã đối chiếu xong 80 chữ N5 | Go |
| **Người dùng** | Đạt mục tiêu khảo sát, đăng ký chờ | Đạt mục tiêu khảo sát nhu cầu học Kanji của sinh viên | Go |
| **Sức lực** | Giờ thực tế ổn định so với kế hoạch | Ổn định, cân bằng tốt với lịch học trên trường | Go |

## Kiểm soát phạm vi
- **Xác nhận không phình phạm vi R1:** Toàn bộ các yêu cầu chức năng mở rộng thuộc phân hệ Đấu trường, Minigame và Kết nối xã hội. Phạm vi của R1 chỉ tập trung vào lõi học tập: Xác thực `FR-001`, kiểm tra đầu vào `FR-002`, thuật toán SM-2 `FR-003` `FR-005`, chuỗi ngày học `FR-006` và quản trị nội dung `FR-010`–`FR-015`.

## Việc đã dời hoặc cắt

| Việc | Lý do | Dời tới |
| :--- | :--- | :--- |
| Chuẩn bị Media cho 80 chữ N5 | Tránh mâu thuẫn với quy tắc xuất bản tuần 12, ưu tiên hoàn thiện luồng dữ liệu chữ trước | Tuần 17–18 |
| Bổ sung dữ liệu chi tiết cho các cấp độ N4–N1 | Giữ ở trạng thái `InReview`/`Draft` để tập trung 100% chất lượng cho N5 ở R1 | Sau khi hoàn thành R1 |

## Rủi ro tháng 2 và cách giảm

1. **Rủi ro lệch cấu trúc CSDL khi viết Migration EF Core:**
   - *Cách giảm:* Chốt cứng file thiết kế ERD và đối chiếu từng cột với Ma trận truy vết RTM trước khi chạy lệnh `dotnet ef migrations add`.
2. **Rủi ro chồng chéo thời gian giữa thi cử trên trường và tiến độ code Backend:**
   - *Cách giảm:* Giữ kỷ luật chia nhỏ task theo ngày, ưu tiên làm dứt điểm các luồng P0 Auth, SM-2 Engine trước, sẵn sàng cắt bớt các task P2 nếu tuần đó có lịch thi.
3. **Rủi ro lỗi tích hợp khi nạp dữ liệu từ Importer Console vào SQL Server thật:**
   - *Cách giảm:* Tận dụng bộ Unit Test `Importer.Tests` và chạy thử nghiệm trên môi trường Database cục bộ LocalDB, Docker với kịch bản chạy lặp 2 lần để kiểm tra tính chống trùng lặp trước khi merge vào `develop`.

## Bài học tháng 1
- Việc thiết lập kỷ luật quy trình ngay từ đầu Git branching theo ngày, Conventional Commits, ma trận RTM và kiểm tra bản quyền nguồn dữ liệu giúp tránh nợ kỹ thuật và giảm thiểu tối đa rủi ro sai sót dữ liệu về sau.
- Cần luôn rà soát kỹ ranh giới giữa các release R1 với R4/R5 để không bị cuốn vào việc làm thêm tính năng phụ làm chậm tiến độ các hạng mục cốt lõi P0.