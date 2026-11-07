# ADR-007: Thang chấm SM-2 và Ánh xạ Giao diện Nút bấm Ôn tập

* **Trạng thái**: Approved
* **Người quyết định**: Nguyễn Tấn Mạnh
* **Ngày quyết định**: 07-11-2026

## 1. Bối cảnh
Thuật toán ôn tập lặp lại ngắt quãng dựa trên đánh giá phản hồi của người học theo thang điểm quality 0 đến 5.

## 2. Quyết định
1. **Lưu trữ CSDL:** Cột `rating` trong bảng `ReviewLogs` lưu trực tiếp giá trị số nguyên từ 0 đến 5.
2. **Nguyên tắc SRS:** Điểm rating >= 3 được tính là **Nhớ**, điểm rating < 3 được tính là **Quên**.
3. **Ánh xạ 4 nút bấm UI ➔ Thang điểm SM-2:**
   - **Nút Quên:** rating = 0, 1, 2 ➔ Đánh dấu Quên, reset `interval` về 1 ngày, giữ nguyên hoặc giảm léger `ease_factor`.
   - **Nút Khó:** rating = 3 ➔ Đánh dấu Nhớ, tăng nhẹ `interval`, giảm `ease_factor` 0.15.
   - **Nút Nhớ:** rating = 4 ➔ Đánh dấu Nhớ, `interval` tính theo công thức SM-2 tiêu chuẩn.
   - **Nút Dễ:** rating = 5 ➔ Đánh dấu Nhớ, thưởng thêm hệ số `interval` và tăng `ease_factor` 0.15.
4. **Thu thập dữ liệu phụ:** Lưu `time_taken_ms` vào `ReviewLogs` để phục vụ phân tích Analytics, ➔ Hệ thống nhận biết đây là "nhớ ngập ngừng" để điều chỉnh độ khó bài tập về sau.

## 3. Hệ quả
- **Tích cực:** Giảm tải nhận thức cho người học khi chỉ cần chọn 1 trong 4 mức độ rõ ràng, đồng thời đảm bảo tính chính xác của toán học SM-2 ở Backend.
- **Thách thức:** Frontend cần gửi chính xác mã $rating$ ($1, 3, 4, 5$) tương ứng với nút bấm về API `POST /api/v1/reviews`.