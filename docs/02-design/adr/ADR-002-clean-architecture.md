# ADR-002: Áp dụng Clean Architecture cho Backend .NET 10

* **Trạng thái:** Accepted
* **Người quyết định**: Nguyễn Tấn Mạnh
* **Ngày quyết định**: 09-11-2026
* **Yêu cầu liên quan:** NFR-001, NFR-002, NFR-003, NFR-005, FR-003

## Bối cảnh
Hệ thống chứa các logic cốt lõi phức tạp như thuật toán SM-2, xếp hạng Elo Đấu trường, chính sách tương tác xã hội cần được kiểm thử độc lập mà không phụ thuộc vào CSDL hay các dịch vụ bên ngoài. Dự án cần một cấu trúc tách biệt rõ ràng trách nhiệm để tránh việc logic nghiệp vụ bị dính chặt vào khung ứng dụng Web API hoặc Entity Framework Core.

## Các phương án đã xem xét

| Phương án | Ưu điểm | Nhược điểm |
|---|---|---|
| **(a) Clean Architecture 4 lớp: Domain, Application, Infrastructure, WebApi** | Tách biệt hoàn toàn logic nghiệp vụ, dễ viết Unit Test cho Domain, dễ chia sẻ thư viện luật chơi giữa Server và Client .NET MAUI. | Tốn mã khuôn mẫu ban đầu, số lượng Project trong Solution nhiều hơn. |
| **(b) N-tier truyền thống Controller → Service → EF Core** | Đơn giản, dễ tiếp cận, ít lớp gian trung gian, tốc độ khởi tạo dự án nhanh. | Logic nghiệp vụ dễ bị phụ thuộc vào EF Core và DbContext, khó viết Unit Test cô lập logic SM-2 hay Elo rating. |
| **(c) Mỗi tính năng một thư mục** | Co cụm logic theo Feature, dễ tìm kiếm mã nguồn của một chức năng cụ thể. | Khó tái sử dụng mã dùng chung giữa các Feature, cấu trúc không đồng nhất nếu không kiểm soát tốt kỉ luật code. |

## Quyết định
Chúng tôi chọn **Phương án (a) Clean Architecture 4 lớp** để bảo vệ logic cốt lõi và tối ưu khả năng kiểm thử tự động. 

Để giữ cho kiến trúc không bị rườm rà đối với dự án một người:
- **Không dùng Repository/Unit of Work bọc đè lên EF Core:** Trong lớp Infrastructure/Application, sử dụng trực tiếp `DbContext` của EF Core cho các thao tác CRUD thông thường để tránh tạo các lớp wrapper trống không cần thiết.
- **Dùng Repository pattern có chọn lọc:** Chỉ áp dụng Interfaces cho các dịch vụ bên ngoài như cổng thanh toán, FCM Push Notification, Redis Cache hoặc các Domain Aggregate gốc cần tính cô lập cao.

## Hệ quả
- **Lợi ích:** 
  - Lớp `Domain` hoàn toàn thuần C#, không phụ thuộc EF Core, giúp kiểm thử thuật toán SM-2 và các luật chơi mini-game đạt coverage cao mà không cần tạo CSDL giả lập.
  - Tái sử dụng được bộ thư viện Domain models/Validation luật game trên ứng dụng di động .NET MAUI.
- **Đánh đổi và rủi ro:** Chi phí khởi tạo ban đầu tốn thời gian chuyển đổi DTO/Entity và quản lý 4 Project C#.
- **Việc phải làm theo:** Định nghĩa rõ ràng quy tắc phụ thuộc Dependency Rule: các lớp bên trong không được gọi lớp bên ngoài và cấu hình Dependency Injection trong `WebApi`.

## Khi nào xem lại
Xem lại nếu quá 50% thời gian phát triển tính năng bị lãng phí vào mã khuôn mẫu mà không thu lại được lợi ích thiết thực về mặt kiểm thử tự động.