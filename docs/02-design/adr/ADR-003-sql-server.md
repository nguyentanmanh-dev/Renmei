# ADR-003: Dùng SQL Server làm Cơ sở dữ liệu chính

* **Trạng thái:** Accepted
* **Người quyết định**: Nguyễn Tấn Mạnh
* **Ngày quyết định**: 09-11-2026
* **Yêu cầu liên quan:** NFR-001, NFR-004, FR-001, FR-010, FR-011

## Bối cảnh
Hệ thống Renmei quản lý cấu trúc dữ liệu quan hệ chặt chẽ người dùng, Kanji, Lịch sử phiên bản, Tiến độ SM-2, Nhật ký. Dự án yêu cầu tính toàn vẹn giao dịch cao khi xử lý các thao tác như trừ điểm rank, chấp nhận kết bạn, hoặc ghi nhận lượt đấu. Đồng thời, hệ thống sử dụng .NET 10 LTS và Entity Framework Core làm framework chính. Thị trường tuyển dụng lập trình viên .NET tại Việt Nam cũng ưu tiên kỹ năng tối ưu hóa trên Microsoft SQL Server.

## Các phương án đã xem xét

| Phương án | Ưu điểm | Nhược điểm |
|---|---|---|
| **(a) Microsoft SQL Server** | Tích hợp hoàn hảo với .NET 10 và EF Core, hỗ trợ các tính năng mạnh mẽ `ROWVERSION` cho Optimistic Concurrency, `SYSUTCDATETIME()`, Filtered Index; phổ biến với nhà tuyển dụng .NET. | Chi phí hosting thương mại đắt hơn PostgreSQL, bản Free bị giới hạn dung lượng RAM/DB. |
| **(b) PostgreSQL** | Mã nguồn mở, hoàn toàn miễn phí, chi phí hosting rẻ: Render, Supabase, AWS RDS PostgreSQL, hỗ trợ kiểu dữ liệu JSON native tốt. | Tích hợp một số tính năng concurrency control phức tạp hơn đôi chút so với SQL Server trong hệ sinh thái .NET. |
| **(c) SQLite** | Cực kỳ nhẹ, không tốn chi phí server CSDL riêng biệt. | Không chịu được tải ghi đồng thời cao, không phù hợp làm DB chính cho ứng dụng Web đa người dùng. |

## Quyết định
Chúng tôi chọn **Phương án (a) Microsoft SQL Server** phiên bản SQL Server 2022 / Azure SQL Database làm CSDL chính của hệ thống Backend Server vì tính tương thích tối đa với EF Core và công cụ quản trị quen thuộc. 
SQLite vẫn được sử dụng nhưng chỉ giới hạn ở phía Client ứng dụng di động .NET MAUI để phục vụ tính năng học ngoại tuyến.

## Hệ quả
- **Lợi ích:** 
  - Đã tận dụng các tính năng riêng của SQL Server trong ERD cột `rowversion` chống xung đột ghi dữ liệu Kanji/User, `SYSUTCDATETIME()`, Filtered Unique Index cho kết bạn và bạn bè.
  - Khả năng tooling tốt với SQL Server Management Studio và Azure Data Studio.
- **Đánh đổi và rủi ro:** Chi phí hosting CSDL hàng tháng khi triển khai Cloud sẽ cao hơn so với PostgreSQL. Việc chuyển đổi CSDL sang Engine khác về sau sẽ tốn công sức do thiết kế ERD đã gắn với một số tính năng riêng của SQL Server.
- **Việc phải làm theo:** Thực thi và duy trì kịch bản sao lưu và phục hồi định kỳ, cấu hình đúng Indexing cho các câu truy vấn tìm kiếm Kanji và Leaderboard.

## Khi nào xem lại
Xem lại nếu chi phí hosting SQL Server trên Cloud vượt quá ngân sách cho phép của dự án hoặc môi trường sản xuất không cung cấp tùy chọn SQL Server ở mức giá hợp lý.