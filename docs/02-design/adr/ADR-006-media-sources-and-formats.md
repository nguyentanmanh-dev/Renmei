# ADR-006: Nguồn dữ liệu, định dạng Media và quy ước lưu trữ cho thẻ Kanji

- **Trạng thái:** Accepted
* **Người quyết định**: Nguyễn Tấn Mạnh
* **Ngày quyết định**: 13-11-2026
* **Yêu cầu liên quan**: FR-005, ERD bảng `Kanji`, `data/content-sources.json`


## 1. Bối cảnh

Theo đặc tả yêu cầu phần mềm `SRS.md`, `FR-005`, mỗi thẻ học Kanji cần cung cấp đầy đủ 3 loại media: **hình minh họa gợi nhớ**, **ảnh động nét viết** và **âm thanh phát âm**.

1. **Ý nghĩa của trường `AudioUrl`:** SRS `FR-005 AC-02` mô tả chung chung là *"âm thanh của chữ đó"*, trong khi nguồn dữ liệu mở chất lượng cao nhất **Kanji alive**, giấy phép `CC BY 4.0` cung cấp bản thu âm người bản ngữ đọc **từ ghép ví dụ thông dụng** chứ không phải âm đọc đơn lẻ từng chữ.
2. **Định dạng ảnh động nét viết `StrokeOrderGifUrl` vs `MP4/WebM`:** Cột trong ERD đang đặt tên là `StrokeOrderGifUrl`, nhưng Kanji alive cung cấp video định dạng **MP4 (248x248)** và **WebM**, chuyển đổi sang GIF sẽ làm tăng dung lượng lên gấp 5–10 lần và giảm độ mượt nét vẽ.
3. **Nguồn và hình minh họa `MnemonicImageUrl`:** Không có bộ hình minh họa gợi nhớ mở nào đủ đồng bộ và an toàn bản quyền cho toàn bộ danh sách Kanji.


## 2. Quyết định

### Quyết định 1: Chốt ý nghĩa `AudioUrl` là âm thanh của từ ghép ví dụ chính
- **Lựa chọn:** Áp dụng `AudioUrl` lưu đường dẫn tệp âm thanh phát âm **từ ghép ví dụ chính** gắn với chữ Kanji đó (ví dụ: chữ 学 `学` phát âm từ `学生 - がくせい`).
- **Hành động đồng bộ:** Cập nhật lại mô tả `FR-005 AC-02` trong `docs/01-requirements/SRS.md` để phản ánh chính xác hành vi bấm loa phát từ ví dụ chính.

### Quyết định 2: Sử dụng định dạng MP4 cho ảnh động nét viết và điều chỉnh tên cột ERD
- **Lựa chọn:** Sử dụng trực tiếp tệp **MP4 (H.264)** từ **Kanji alive** làm nguồn chính cho toàn bộ 80 chữ N5. Giữ **KanjiVG** làm nguồn dự phòng cho các chữ mở rộng ở N4–N1 nếu Kanji alive không có.
- **Thay đổi thiết kế:** Đổi tên cột `StrokeOrderGifUrl` trong bảng `Kanji` thành **`StrokeOrderAnimationUrl`** và sử dụng thành phần phát media gốc `MediaElement` trên .NET MAUI ở chế độ lặp tự động `Loop = true`, tắt tiếng.

### Quyết định 3: Tự biên soạn Hình minh họa tối giản định dạng WebP và điều chỉnh quỹ thời gian
- **Lựa chọn:** Hình minh họa gợi nhớ `MnemonicImageUrl` sẽ được **tự vẽ, biên soạn theo phong cách vector phẳng tối giản** và xuất ra định dạng **WebP (512x512px)** để chủ sở hữu ứng dụng giữ toàn quyền bản quyền, tránh rủi ro pháp lý từ ảnh AI hoặc ảnh không rõ nguồn gốc.
- **Điều chỉnh kế hoạch:** Cập nhật quỹ thời gian sản xuất media N5 ở tuần 17, 18 từ 8 giờ lên **16 giờ** và ưu tiên hoàn thiện trọn bộ N5 trước khi mở rộng sang N4–N1.

### Quyết định 4: Quy ước đặt tên tệp Media theo mã Unicode
- Tất cả tệp media sau khi tải về phải được chuẩn hóa tên theo **mã Unicode Hex** của ký tự Kanji để độc lập với nguồn dữ liệu:
  - Ảnh động nét viết: `media/n5/stroke/<unicode>.mp4`, ví dụ: `media/n5/stroke/5b66.mp4` cho chữ `学`
  - Hình minh họa: `media/n5/image/<unicode>.webp`, ví dụ: `media/n5/image/5b66.webp`
  - Âm thanh ví dụ: `media/n5/audio/<unicode>-1.opus` hoặc `media/n5/audio/<unicode>-1.mp3`

## 3. Hệ quả

- **Tích cực:**
  - Tận dụng được ngay 100% kho dữ liệu nét viết tay và giọng đọc người bản ngữ chuẩn xác cho 80 chữ N5 từ Kanji alive.
  - Tổng dung lượng gói media N5 cực nhẹ khoảng **~9.5 MB** cho cả 3 loại, dễ dàng đóng gói offline hoặc lưu trữ trên Cloud Storage với chi phí thấp.
- **Tiêu cực / Nợ kỹ thuật cần lưu ý:**
  - Cần cập nhật lại dòng mô tả `FR-005 AC-02` trong `SRS.md` và cập nhật lại bảng ước lượng giờ làm việc cho tuần 17, 18 trên Notion Board.
  - Phải hiển thị đầy đủ thông tin ghi công `ATTRIBUTION.md` trên màn hình giới thiệu của ứng dụng ở kế hoạch tuần 27.