# SDD: Software Design Document

| Phiên bản : 1.0 | Ngày : 02-11-2026 | Trạng thái : Draft | Cổng : R1-G2 | Người duyệt : Nguyễn Tấn Mạnh |

### ERD Diagram

![Renmei Database ERD Schema](./erd.svg)


### Danh sách các phân hệ bảng CSDL
1. **Phân hệ Core Learning:**
   - **Xác thực và quản lý người dùng:** `users`, `roles`, `user_roles`, `refresh_tokens`, `user_streaks`
   - **Nội dung Kanji và Media:** `kanji`, `kanji_media`
   - **Tiến độ SM-2 và học tập:** `user_kanji_progress`, `review_logs`, `daily_queues`
   - **Đánh giá và chat:** `placement_questions`, `placement_options`, `chat_scenarios`

2. **Phân hệ Admin System:**
   - **Truy vết:** `audit_logs`
   - **Quản lý phiên bản:** `kanji_revisions`
   - **Tiếp nhận báo lỗi:** `feedbacks`
   - **Báo cáo và analytics:** `daily_metrics`

