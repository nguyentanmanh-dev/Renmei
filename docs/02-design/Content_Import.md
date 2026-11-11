# Đặc tả Pipeline Import Kanji N5–N1

| Phiên bản : 1.0 | Ngày : 11-11-2026 | Trạng thái : Approved | Cổng : R1-G2 | Người duyệt : Nguyễn Tấn Mạnh |

## 1. Quy trình Pipeline
Pipeline thực hiện tự động kiểm tra dữ liệu từ thô đến CSDL qua 4 bước:

1. **License và Provenance Check:** Xác minh nguồn gốc dữ liệu và thời gian kiểm tra bản quyền `licenseCheckedAt`.
2. **Schema và Required Validation:** Kiểm tra các trường cấu trúc bắt buộc.
3. **Deduplication Check:** Kiểm tra trùng lặp trên file thô và CSDL.
4. **Status Assignment:** Gán trạng thái xuất bản dữ liệu tự động.

## 2. Mã lỗi và Quy tắc xử lý

| Mã lỗi | Loại kiểm tra | Mô tả | Trạng thái xử lý |
|---|---|---|---|
| `LICENSE_MISSING` | License | Nguồn dữ liệu không nằm trong sổ `content-sources.json`. | Rejected |
| `LICENSE_UNVERIFIED` | License | Trường `licenseCheckedAt` trong sổ nguồn bị trống. | Rejected |
| `PROVENANCE_MISSING` | Provenance | Thiếu tham chiếu nguồn (`SourceRef`) hoặc JLPT level. | Rejected |
| `DUPLICATE_IN_FILE` | Trùng lặp | Ký tự trùng xuất hiện nhiều lần trong cùng tập tin. | Rejected |
| `DUPLICATE_IN_DB` | Trùng lặp | Ký tự Kanji đã tồn tại sẵn dưới CSDL. | Skipped |
| `REQUIRED_MISSING:<field>`| Đầy đủ | Thiếu các trường bắt buộc (readings, meanings, examples). | Draft |

## 3. Sơ đồ Gán Trạng thái
- **`Rejected`:** Vi phạm License, Provenance, hoặc Trùng lặp.
- **`Draft`:** Thiếu trường bắt buộc OR thuộc cấp độ N3, N2, N1.
- **`InReview`:** Đủ trường thuộc N4 OR N5 nhưng chưa nằm trong `n5-approved.txt`.
- **`Published`:** **Chỉ áp dụng cho N5** + Đủ trường dữ liệu + Đã được kiểm tra thủ công trong `n5-approved.txt`.