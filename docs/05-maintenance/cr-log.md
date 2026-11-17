# Change Request Log (Nhật ký Yêu cầu Thay đổi)

| Phiên bản : 0.1 | Ngày : 31-10-2026 | Trạng thái : Draft | Cổng : sau G2 | Người duyệt : Nguyễn Tấn Mạnh |

| Mã CR		 | Ngày tạo   | Mô tả ngắn						| Lý do								  | Ảnh hưởng       | Quyết định | Đưa vào Release | Ngày duyệt |
|---|---|---|---|---|---|
| **CR-001** | 2026-11-15 | Bổ sung Login với Google OAuth  | Phản hồi người dùng giai đoạn Alpha | +16h			| Approved	 | Release 1.1	   | 2026-11-16 |
| **CR-002** | 2026-11-20 | Đổi thuật toán SM-2 sang FSRS   | Tối ưu đường cong quên			  | +40h			| Rejected	 | -			   | 2026-11-21 |

# Nhật ký Yêu cầu Thay đổi

- **Mốc đóng băng thiết kế R1 Baseline:** `17-11-2026`, Cổng `R1-G2`, Tag Git: `r1-design-frozen`
- **Quy tắc áp dụng sau khi đóng băng:**
  - **Tầng 1: không đổi hành vi/hợp đồng:** Sửa lỗi chính tả, bổ sung ví dụ minh họa, thêm ghi chú giải thích hoặc chỉ mục tối ưu hiệu năng -> Sửa trực tiếp qua PR và ghi chú vào lịch sử tài liệu.
  - **Tầng 2: Đổi hành vi hoặc hợp đồng:** Thêm, xóa, sửa bảng, cột, kiểu dữ liệu, endpoint OpenAPI, mã lỗi hoặc luật nghiệp vụ -> Bắt buộc tạo mã **CR-xxx** trong tệp này, đánh giá tác động và cập nhật đồng bộ `srs.md`, `sdd.md`, `schema.sql`, `OpenAPI` và `rtm.md`.

## Danh sách Change Request

| Mã CR | Ngày đề xuất | Tiêu đề và Mô tả thay đổi | Lý do | Tác động | Trạng thái | Bản phát hành |
| :---: | :---: | :--- | :--- | :--- | :---: | :---: |
| **CR-001** | *Mẫu* | *Ví dụ: Bổ sung cột/endpoint mới cho R1* | *Mô tả nguyên nhân kỹ thuật hoặc nghiệp vụ* | *giờ, ảnh hưởng schema.sql, learner.yaml, rtm.md* | *Template* | *R1* |