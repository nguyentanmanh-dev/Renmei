# Kế hoạch sản xuất và quản lý Media cho Kanji N5

- **Ngày lập kế hoạch:** 13-11-2026
- **Người lập:** Nguyễn Tấn Mạnh
- **Phạm vi:** 80 chữ Kanji N5 đã duyệt trong `data/curated/n5-approved.txt`
- **Tài liệu tham chiếu:** `SRS.md`, `ADR-006-media-sources-and-formats.md`, `data/content-sources.json`


## 1. Kết quả kiểm tra độ phủ nguồn

Đối chiếu danh sách **80 chữ Kanji N5** trong `data/curated/n5-approved.txt` với kho dữ liệu **Kanji alive**:

| Hạng mục kiểm tra | Kết quả | Chi tiết |
| :--- | :--- | :--- |
| **Tổng số chữ N5 mục tiêu** | **80 chữ** | Danh sách đã qua đối chiếu Hán Việt và kiểm duyệt 8 tiêu chí |
| **Độ phủ trên Kanji alive** | **80 / 80 chữ** | Đầy đủ cả video nét viết và âm thanh từ ghép ví dụ |
| **Số chữ N5 bị thiếu** | **0 chữ** | Không cần dùng nguồn dự phòng KanjiVG cho đợt N5 |


## 2. Kế hoạch số lượng và ước tính dung lượng

### Giả định đo lường
Dựa trên kết quả đo thử 5 chữ mẫu (`一`, `日`, `月`, `学`, `駅`) với cấu hình chuẩn hóa theo `ADR-006`:
- **Ảnh động nét viết:** Video MP4 (H.264, 248x248px) từ Kanji alive, trung bình **~40 KB / tệp**.
- **Hình minh họa gợi nhớ:** Hình vector tối giản xuất chuẩn WebP, trung bình **~50 KB / tệp**.
- **Âm thanh từ ghép ví dụ:** Tệp âm thanh Opus hoặc MP3 từ Kanji alive, trung bình **~20 KB / tệp**.

### Bảng tổng hợp số lượng & dung lượng

| Loại Media | Số lượng tệp N5 | Định dạng | Dung lượng TB / tệp | Tổng dung lượng N5 | Dự phóng toàn bộ N5–N1 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Ảnh động nét viết** | 80 tệp | `.mp4` | ~40 KB | ~3.2 MB | ~85.4 MB |
| **Hình minh họa gợi nhớ** | 80 tệp | `.webp` | ~50 KB | ~4.0 MB | ~106.8 MB |
| **Âm thanh ví dụ chính** | 80 - 160 tệp | `.opus` hoặc `.mp3` | ~20 KB | ~1.6 – 3.2 MB | ~42.7 – 85.4 MB |
| **Phiên âm** | 80 bản ghi | Text | Không đáng kể | < 0.05 MB | < 1.5 MB |
| **TỔNG CỘNG** | **240 – 320 tệp** |  |  | **~8.8 – 10.4 MB** | **~235 – 278 MB** |

## 3. Quy ước đặt tên tệp và cấu trúc thư mục

Tất cả tệp media được chuẩn hóa tên theo **mã Unicode Hex** của ký tự Kanji để tách biệt hoàn toàn với cách đặt tên gốc của nhà cung cấp:

```text
media/
└── n5/
    ├── stroke/
    │   ├── 4e00.mp4
    │   └── 5b66.mp4
    ├── image/
    │   ├── 4e00.webp
    │   └── 5b66.webp
    ├── audio/
    │   ├── 4e00-1.opus/.mp3
    │   └── 5b66-1.opus/.mp3
    └── manifest_N5.csv