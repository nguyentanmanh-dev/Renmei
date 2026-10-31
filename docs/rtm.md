# RTM: Requirements Traceability Matrix

| Phiên bản : 0.1 | Ngày : 31-10-2026 | Trạng thái : Draft | Cổng : Mọi cổng | Người duyệt : Nguyễn Tấn Mạnh |

| Mã YC		 | Mô tả Yêu cầu   | SDD					   | Mã nguồn (Code)								| Kiểm thử (Test Cases) | Trạng thái |
| **FR-001** | Đăng nhập JWT   | SDD §4.1 (Auth), ADR-002  | `AuthController.cs`, `GenerateJwtTokenCommand` | TC-001, TC-002		| Chưa làm |
| **FR-002** | Tra cứu Kanji   | SDD §4.2 (Kanji Module)   | `KanjiController.cs`, `GetKanjiQuery`			| TC-010..TC-015		| Chưa làm |
| **FR-003** | Thuật toán SM-2 | SDD §5.1 (SM-2 Algorithm) | `Sm2Calculator.cs`								| TC-020..TC-025		| Chưa làm |

*Ghi chú cập nhật*: 
- Cột **Thiết kế** hoàn thành tại cổng **G2**.
- Cột **Mã nguồn** hoàn thành tại cổng **G3**.
- Cột **Kiểm thử** hoàn thành tại cổng **G4**.