# Test Plan: Kế hoạch Kiểm thử

| Phiên bản | Ngày | Trạng thái | Cổng | Người duyệt |
|---|---|---|---|---|
| 0.1 | 15-11-2026 | Draft | R1-G2 / R1-G4 | Nguyễn Tấn Mạnh |

## 1. Phạm vi và Môi trường kiểm thử
- **Phạm vi:** Toàn bộ yêu cầu chức năng và phi chức năng thuộc phạm vi Release 1 `FR-001` đến `FR-015` và `NFR-001` đến `NFR-012`, bao gồm API Backend .NET WebApi, Pipeline Importer, thuật toán lặp lại ngắt quãng SM-2 và ứng dụng di động .NET MAUI.
- **Môi trường kiểm thử:**
  - **Unit & Integration Test:** xUnit trên .NET SDK, chạy tự động qua GitHub Actions CI.
  - **Cơ sở dữ liệu:** SQL Server 2022 Docker Container hoặc LocalDB áp dụng schema chuẩn UTC `SYSUTCDATETIME()`.
  - **API & Mobile:** Local Development Server `http://localhost:5000/api/v1` và Android Emulator API 34+.

## 2. Danh mục Test Case ánh xạ từ RTM
| TC-ID | Yêu cầu liên kết | Mô tả kịch bản kiểm thử | Loại kiểm thử |
| :--- | :--- | :--- | :--- |
| TC-AUTH-01 | FR-001, ADR-004 | Đăng ký, đăng nhập, cấp mới và xoay vòng Refresh Token, phát hiện tái sử dụng token | Integration |
| TC-KANJI-01 | FR-004, FR-005 | Tra cứu Kanji theo cấp độ, kiểm tra đủ trường Hán Việt, ví dụ và URL media | Integration |
| TC-SRS-01 | FR-007, ADR-007 | Chấm điểm ôn tập `rating` 0–>5, tính toán SM-2 và kiểm tra idempotent qua `client_event_id` | Unit / Integration |
| TC-PLACE-01 | FR-008, FR-014 | Làm bài kiểm tra đầu vào và chặn vô hiệu hóa câu hỏi khi dưới ngưỡng tối thiểu | Integration |
| TC-ADMIN-01 | FR-010, FR-012 | Luồng duyệt Kanji `Draft` -> `InReview` -> `Published`, xem revisions và xử lý feedback | Integration |
| TC-NFR-012 | NFR-012 | Mở màn hình "Giới thiệu và ghi công" từ menu chính và kiểm tra hiển thị nội dung `ATTRIBUTION.md` | UI / Manual |

