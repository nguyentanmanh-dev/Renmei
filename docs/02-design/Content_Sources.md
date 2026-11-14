# Quyết định nguồn dữ liệu Kanji N5–N1, Bản quyền & Quy trình cập nhật

- **Ngày lập:** 14-11-2026
- **Người thực hiện:** Nguyễn Tấn Mạnh
- **Phạm vi:** Dữ liệu Kanji N5–N1, cách đọc, cấp độ JLPT và câu ví dụ.


## 1. Bảng chốt danh sách trường dữ liệu được dùng & loại bỏ

| Nguồn | Trường DÙNG | Trường KHÔNG DÙNG | Lý do |
| :--- | :--- | :--- | :--- |
| **KANJIDIC2** | `literal`, `ja_on`, `ja_kun`, nghĩa tiếng Anh, `stroke_count`, `rad_value`, `grade`, `freq`, `vietnam` | `SKIP`, `Four Corner`, `Morohashi`, `De Roo`, `S&H`, `Pinyin`, âm Hàn | Các mã tra cứu thuộc bản quyền bên thứ ba, riêng mã `SKIP` dùng giấy phép `CC BY-NC-SA 4.0` nên phải loại bỏ. |
| **Danh sách cấp độ JLPT** | Ký tự Kanji và cấp độ | Nghĩa và cách đọc kèm theo trong tệp | Toàn bộ cách đọc và nghĩa lấy thống nhất từ KANJIDIC2 để tránh xung đột dữ liệu. |
| **Tatoeba** | Câu tiếng Nhật, bản dịch tiếng Việt hoặc Anh, `sentence_id`, `owner`, `license` | Âm thanh `audio`, nhãn `tags` | Âm thanh trên Tatoeba có giấy phép riêng theo từng người đọc, không dùng. |


## 2. Cấp độ JLPT N5-N1

- **Tính chất dữ liệu:** Kể từ năm 2010, Japan Foundation và JEES không công bố danh sách Kanji chính thức cho N5–N1. Bảng cấp độ trong dự án là **danh sách gợi ý không chính thức của cộng đồng**, trong đó **N2 và N3 là ước lượng** do cấp 2 cũ được tách đôi.
- **Kiểm tra chất lượng:** Đã đếm số lượng từng cấp, kiểm tra không có ký tự trùng lặp giữa các cấp và đối chiếu ký tự tồn tại trong KANJIDIC2.
- **Phương án dự phòng:** Nếu trang `tanos.co.uk` không xác minh được văn bản license chính thức, hệ thống sẽ chuyển sang suy diễn từ trường `jlpt` của KANJIDIC2 `4` -> `N5`, `3` -> `N4`, `1` -> `N1`, riêng cấp `2` tách thủ công sang `N2`/`N3` dựa theo lớp học `grade` tại Nhật và ghi chú là ước lượng.


## 3. Quy tắc tuyển chọn câu ví dụ Tatoeba

Để minh bạch cách lựa chọn dữ liệu theo khuyến nghị của Tatoeba, ứng dụng áp dụng quy tắc tuyển chọn sau: chỉ trích xuất các câu tiếng Nhật tự nhiên có chứa chữ Kanji mục tiêu, độ dài ngắn gọn phù hợp trình độ học (ưu tiên 5–20 ký tự), đi kèm bản dịch tiếng Việt đã kiểm duyệt hoặc bản dịch tiếng Anh chuẩn xác. Mỗi câu ví dụ được lưu độc lập cùng `sentence_id`, tên người sở hữu `owner` và loại giấy phép của riêng câu đó, hệ thống chỉ chấp nhận các câu mang giấy phép **`CC BY 2.0 FR`** hoặc **`CC0`** và loại bỏ toàn bộ các câu không rõ giấy phép hoặc cấm thương mại.


## 4. Quy trình cập nhật dữ liệu định kỳ

- **Lịch kiểm tra:** Định kỳ **mỗi quý 1 lần** cho cả **KANJIDIC2** và **Tatoeba**.
- **Người thực hiện:** Nguyễn Tấn Mạnh.
- **Các bước cập nhật:**
  1. Tải bản mới nhất vào `data/sources/` và tính mã băm `SHA-256`. Nếu mã băm không đổi so với `data/provenance.json`, kết thúc kỳ kiểm tra.
  2. Nếu có thay đổi, chạy script so sánh `diff` giữa bản cũ và bản mới để kiểm tra các chỉnh sửa về cách đọc, số nét hoặc câu ví dụ bị xóa, sửa trên Tatoeba.
  3. Cập nhật `data/provenance.json`, chạy `Importer` ở chế độ kiểm thử `DryRun` trước khi nạp chính thức vào DB.
  4. Ghi nhận kết quả từng đợt cập nhật vào tệp nhật ký `docs/04-operations/data-update-log.md`.