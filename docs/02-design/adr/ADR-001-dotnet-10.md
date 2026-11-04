# ADR-001: Lựa chọn .NET 10 làm Nền tảng Backend Web API chính

* **Trạng thái**: Approved
* **Người quyết định**: Nguyễn Tấn Mạnh
* **Ngày quyết định**: 04-11-2026


## 1. Bối cảnh

Hệ thống đòi hỏi một nền tảng Backend đảm bảo các tiêu chí:
1. **Hiệu năng cao & Tối ưu bộ nhớ**: Xử lý mượt mà các kết nối đồng thời và tính toán thuật toán lặp lại ngắt quãng.
2. **Hỗ trợ định hướng phát triển**: Tương thích hoàn hảo với Clean Architecture, Domain-Driven Design, Entity Framework Core và lập trình đa nền tảng .NET MAUI cho Mobile App.
3. **Tính ổn định dài hạn**: Cần một bản phát hành hỗ trợ dài hạn phục vụ duy trì và bảo trì ứng dụng trong nhiều năm.

## 2. Các phương án được xem xét

1. **.NET 10**: Phiên bản LTS mới nhất của Microsoft.
2. **.NET 8**: Phiên bản LTS tiền nhiệm đã ổn định.
3. **Node.js (TypeScript / NestJS)**: Nền tảng phổ biến cho ứng dụng Web/Mobile API.
4. **Go (Golang)**: Nền tảng tối ưu hiệu năng cao và microservices.

## 3. Quyết định

**.NET 10** làm nền tảng phát triển Backend Web API chính cho hệ thống Renmei.

### Lý do lựa chọn:
- **Tối ưu hóa hiệu năng vượt trội**: .NET 10 mang lại những cải tiến sâu về JIT Compilation, Native AOT, giúp giảm thời gian khởi động và tiết kiệm RAM tối đa cho Web API Services.
- **Tính ổn định dài hạn**: Là phiên bản LTS, .NET 10 được Microsoft hỗ trợ bảo mật và cập nhật kỹ thuật trong 3 năm, đảm bảo tính an toàn cho hệ thống sản phẩm.
- **Đồng bộ hệ sinh thái mã nguồn**: Sử dụng C# cho cả Backend .NET 10 Web API và Mobile App, giúp tối ưu hóa việc tái sử dụng DTOs, logic Validation và chia sẻ tri thức mã nguồn.
- **Hỗ trợ tốt nhất cho Clean Architecture**: Các thư viện như Entity Framework Core, MediatR, FluentValidation, và SignalR trên .NET 10 hoạt động cực kỳ mượt mà, hỗ trợ phân tách rõ ràng các layer Domain, Application, Infrastructure và API Presentation.

## 4. Hệ quả

### Tác động tích cực:
- **Cải thiện tốc độ API Response**: Thời gian phản hồi API giảm đáng kể nhờ các tối ưu hóa mới nhất của ASP.NET Core trên .NET 10.
- **Dễ bảo trì và mở rộng**: Cấu trúc Clean Architecture kết hợp với C# 13/.NET 10 giúp codebase gọn gàng, Type-safe và hạn chế lỗi runtime.
- **Real-time mượt mà**: ASP.NET Core SignalR tích hợp sẵn giúp triển khai cụm tính năng đấu trí PvP Mini-games Karuta, Shiritori một cách chuẩn xác và ít tốn tài nguyên.

### Tác động tiêu cực và thách thức:
- **Yêu cầu cập nhật công cụ phát triển**: Đội ngũ phát triển cần đảm bảo cài đặt .NET 10 SDK và sử dụng Visual Studio hoặc VS Code với C# Dev Kit tương thích.
- **Quản lý rủi ro tính tương thích thư viện**: Một số NuGet package bên thứ ba cũ có thể cần kiểm tra độ tương thích hoặc cập nhật lên phiên bản hỗ trợ .NET 10.

## 5. Xác nhận và kiểm chứng

- [x] Tạo dự án mẫu thử nghiệm với ASP.NET Core Web API trên .NET 10 SDK.
- [x] Kiểm tra việc tích hợp Clean Architecture, DDD template thành công.
- [x] Kiểm tra khả năng tương thích của Entity Framework Core và T-SQL SQL Server.