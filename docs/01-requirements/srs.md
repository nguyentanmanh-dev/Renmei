# SRS: Software Requirements Specification

| Phiên bản : 1.0 | Ngày : 01-11-2026 | Trạng thái : Frozen | Cổng : R1-G1 | Người duyệt : Nguyễn Tấn Mạnh |

## 1. Giới thiệu
- **Mục đích**: Tài liệu xác định toàn bộ yêu cầu chức năng và phi chức năng cho ứng dụng học Kanji Renmei (Release 1).
- **Phạm vi**: Hệ thống Backend Web API (.NET 10) và ứng dụng di động Android ( .NET MAUI).

## 2. Mô tả Tổng quan
- **Persona**: Học viên tiếng Nhật trình độ N5-N1 cần công cụ ôn tập lặp lại ngắt quãng có hệ thống.
- **Ràng buộc kĩ thuật**:
  - Backend: Clean Architecture, .NET 10 Web API.
  - Mobile: .NET MAUI.

## 3. Functional Requirements

## 3.1 Chức năng user

## FR-001: Đăng nhập JWT
- **User Story:** tôi muốn đăng ký và đăng nhập tài khoản bằng Email/Mật khẩu để có tài khoản cá nhân lưu trữ tiến độ học.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** người dùng điền đúng Email và Mật khẩu, nhấn "Đăng nhập", hệ thống trả về JWT Access Token hợp lệ và chuyển vào màn hình chính.
  - [x] **AC-02:** người dùng nhập sai mật khẩu, nhấn "Đăng nhập", hệ thống trả về mã lỗi `401 Unauthorized` kèm thông báo phù hợp.

#### FR-002: Kiểm tra trình độ đầu vào
- **User Story:** Là người lần đầu dùng app, tôi muốn làm bài kiểm tra nhanh hoặc chủ động chọn trình độ Kanji bắt đầu để có thể vào học ngay theo đúng nhu cầu mà không bị ép buộc.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** học viên bắt đầu test, trả lời xong chuỗi câu hỏi placement, hệ thống tính điểm và đề xuất JLPT Level tương ứng.
  - [x] **AC-02:** học viên ở màn hình Placement Test, người dùng chọn "Bỏ qua / Bắt đầu từ đầu",hệ thống thiết lập trình độ mặc định là N5 và khởi tạo hàng đợi học N5.
  - [x] **AC-03:** học viên ở màn hình Placement Test, người dùng chọn "Tôi đã biết trình độ của mình" và chọn cấp độ, hệ thống lưu trình độ đã chọn và mở khóa danh sách Kanji tương ứng.

  #### FR-003: Ôn tập Kanji theo thuật toán SM-2
- **User Story:** tôi muốn ứng dụng tự động tính toán sắp xếp lịch ôn tập Kanji dựa trên các lần học của tôi để tối ưu khả năng ghi nhớ.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Kanji đang ở trạng thái `Review`, người dùng chọn "Nhớ" chấm điểm >= 3, Hệ thống tính số ngày chờ mới bằng công thức `Interval` mới = `Interval` cũ x `EaseFactor`, và lùi ngày ôn tiếp theo `NextReviewDate` tương ứng với `Interval` mới.
  - [x] **AC-02:** Kanji ở bất kỳ trạng thái nào, người dùng chọn "Quên" chấm điểm < 3, hệ thống reset `Interval` về 1 ngày, và đưa chữ đó quay lại hàng đợi ôn tập trong cùng ngày.
  - [x] **AC-03:** học viên hoàn thành 1 lượt chấm điểm, hệ thống lưu kết quả, tự động ghi nhận đúng 1 dòng vào `ReviewLogs` lưu các thông tin: điểm số `Rating`, thời gian chờ `Interval`, độ dễ `EaseFactor`, và thời gian suy nghĩ `TimeTakenMs`.
  - [x] **AC-04:** một chữ Kanji đang học, người dùng duy trì chuỗi chấm điểm >= 3 với số ngày chờ `Interval` >= 21 ngày, hệ thống gắn nhãn trạng thái Kanji đó đã được ghi nhớ sâu `Mastered`.
  - [x] **AC-05:** một bài bị ôn trễ N ngày so với lịch ôn, người dùng mở ra ôn vẫn bấm "Nhớ" chấm điểm >= 3, hệ thống tính toán `Interval` tiếp theo dựa trên khoảng thời gian thực tế người học ghi nhớ được (`Interval` cũ + N) x `EaseFactor`.

#### FR-004: Hàng đợi học hằng ngày và Thống kê
- **User Story:** tôi muốn thấy danh sách các từ Kanji cần học trong ngày để dễ dàng theo dõi mục tiêu học tập.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** học viên mở ứng dụng, màn hình chính được tải thành công, hệ thống hiển thị chính xác số thẻ mới (`NewCardsCount`) và số thẻ cần ôn tập (`ReviewCardsCount`) dựa trên cài đặt mục tiêu ngày của người dùng.
  - [x] **AC-02:** học viên đang trong màn hình chính, When hoàn thành 1 thẻ học hoặc 1 lượt ôn tập, Then số lượng thẻ trong hàng đợi ngày lập tức giảm đi 1 và thanh tiến độ mục tiêu ngày được cập nhật tương ứng.
  - [x] **AC-03:** sang ngày mới, học viên truy cập ứng dụng, hệ thống quét và nạp vào hàng đợi theo thứ tự ưu tiên: Toàn bộ từ Kanji bị "Quên" hoặc quá hạn từ ngày cũ --> Các bài ôn đúng hạn hôm nay --> Các từ Kanji mới theo hạn mức mục tiêu đã thiết lập

#### FR-005: Học từ vựng có hình ảnh minh họa và giọng đọc chuẩn
- **User Story:** tôi muốn nhìn thấy hình ảnh minh họa thực tế và giọng đọc chuẩn xác cho từng từ Kanji, để tăng khả năng ghi nhớ qua ngữ cảnh và phát âm chuẩn ngay từ đầu.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** học viên mở thẻ Kanji, màn hình thẻ hiển thị, hệ thống bắt buộc xuất hiện hình ảnh minh họa (`MnemonicImageUrl`), ảnh động nét viết (`StrokeOrderGifUrl`), và phiên âm Hiragana/Katakana chuẩn.
  - [x] **AC-02:** học viên bấm vào biểu tượng loa trên thẻ, kích hoạt, hệ thống phát đúng file âm thanh chuẩn của từ vựng đó.

#### FR-006: Duy trì chuỗi ngày học
- **User Story:** tôi muốn hệ thống ghi nhận và hiển thị chuỗi ngày học liên tục để tạo thói quen và giữ động lực quay lại ứng dụng mỗi ngày.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** học viên hoàn thành ít nhất 1 phiên học mới hoặc 1 phiên ôn tập trong ngày, kết thúc phiên, chỉ số `CurrentStreak` tăng thêm 1 và hiển thị hiệu ứng chúc mừng.
  - [x] **AC-02:** học viên không hoàn thành bất kỳ phiên học hoặc ôn tập nào trong 1 ngày, sang ngày hôm sau, hệ thống tự động reset `CurrentStreak` về 0.
  - [x] **AC-03:** học viên có sở hữu vật phẩm đóng băng chuỗi `StreakFreeze` và bỏ lỡ 1 ngày học, sang ngày mới, hệ thống tự động tiêu thụ 1 `StreakFreeze` để giữ nguyên `CurrentStreak` mà không bị reset, mỗi tháng sẽ được cấp 3 `StreakFreeze`.

#### FR-007: Hội thoại tình huống ngắn và giao tiếp tự do
- **User Story:** tôi muốn tham gia một cuộc hội thoại tình huống ngắn ở cuối mỗi bài học để ứng dụng ngay từ vựng vừa học vào giao tiếp thực tế.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** học viên hoàn thành các thẻ từ vựng của bài học, chuyển sang phần "Luyện tập Giao tiếp", hệ thống mở màn hình nhắn tin theo một kịch bản tình huống thực tế có chứa các từ vựng vừa học.
  - [x] **AC-02:** học viên nhập hoặc nói câu phản hồi ngẫu nhiên, gửi tin nhắn, hệ thống phân tích ngữ pháp, đưa ra phản hồi phù hợp với ngữ cảnh, và gợi ý cách diễn đạt chuẩn nếu người học dùng sai từ.
  - [x] **AC-03:** học viên chỉ muốn tập trung ôn từ vựng nhanh, ở màn hình hội thoại, hệ thống cung cấp nút "Bỏ qua / Hoàn thành bài" để học viên kết thúc bài học mà không bị bắt buộc hoàn thành chat.

### 3.2 Chức năng Admin

#### FR-010: Quản lý Nội dung Kanji và lịch sử phiên bản
- **User Story:** tạo, sửa, gửi duyệt, xuất bản và quản lý phiên bản của các thẻ Kanji, để đảm bảo tính chính xác và chất lượng của dữ liệu học.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin nhập thông tin Kanji (gồm Hán Việt, Onyomi, Kunyomi, Nghĩa, Câu chuyện, Nét viết), bấm "Lưu", hệ thống tạo mới hoặc cập nhật Kanji ở trạng thái `Draft`.
  - [x] **AC-02:** Kanji ở trạng thái `InReview`, Admin bấm "Publish", hệ thống chuyển trạng thái Kanji sang `Published` và hiển thị công khai trên ứng dụng người học.
  - [x] **AC-03:** một Kanji đã ở trạng thái `Published`, Admin thực hiện cập nhật nội dung, hệ thống tự động lưu bản chụp JSON hiện tại vào bảng `KanjiRevisions` kèm số phiên bản (`Version`) tăng tiến trước khi áp dụng thay đổi mới.

#### FR-011: Quản lý tài khoản user và an toàn hệ thống
- **User Story:** xem danh sách, phân quyền, khóa tài khoản hoặc thu hồi phiên đăng nhập của người dùng, để duy trì an ninh và kiểm soát quyền truy cập hệ thống.
- **Độ ưu tiên:** P0 (Khóa/Thu hồi) | P1 (Gán quyền)
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin thực hiện khóa 1 tài khoản vi phạm, xác nhận "Lock", hệ thống chuyển `Status` người dùng sang `Locked` và lập tức thu hồi toàn bộ Refresh Token active của tài khoản đó.
  - [x] **AC-02:** Admin muốn thay đổi quyền hạn của tài khoản, gán vai trò `Editor` hoặc `Admin` và lưu, Then hệ thống cập nhật bảng `UserRoles` và áp dụng policy phân quyền mới ngay từ lượt cấp Token tiếp theo.
  - [x] **AC-03:** Admin đang đăng nhập, cố gắng tự khóa tài khoản, tự xóa, hoặc tự hạ quyền Admin của chính mình, Then hệ thống từ chối thao tác và trả về thông báo lỗi vi phạm quy tắc nghiệp vụ.

#### FR-012: Tiếp nhận và xử lý báo lỗi từu user
- **User Story:** xem và xử lý các phản hồi/báo lỗi nội dung từ ứng dụng người học, để kịp thời chỉnh sửa các sai sót dữ liệu.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin truy cập mục Feedback, xem danh sách, hệ thống hiển thị danh sách phản hồi hỗ trợ lọc theo loại: `ContentError`, `Bug`, `Suggestion`, trạng thái: `Open`, `InProgress`, `Resolved`, `Rejected`, và liên kết trực tiếp tới Kanji bị báo lỗi.
  - [x] **AC-02:** Admin giải quyết xong một báo lỗi, nhập ghi chú admin (`AdminNote`) và đổi trạng thái sang `Resolved` hoặc `Rejected`, hệ thống lưu thông tin người xử lý (`ResolvedBy`) và thời gian cập nhật.

#### FR-013: Truy vết thao tác quản trị
- **User Story:** hệ thống tự động ghi chép nhật ký mọi hành động thay đổi dữ liệu của Admin để phục vụ việc kiểm tra và truy vết an ninh khi cần thiết.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin thực hiện bất kỳ thao tác Create, Update, Delete, Publish, Lock, API xử lý thành công, hệ thống tự động chèn 1 bản ghi vào bảng `AuditLogs` chứa: `ActorUserId`, `Action`, `EntityType`, `EntityId`, `DetailsJson`, và `IpAddress`.
  - [x] **AC-02:** dữ liệu trong bảng `AuditLogs`, gọi API hệ thống, không cung cấp bất kỳ endpoint nào hỗ trợ Sửa (`PUT/PATCH`) hoặc Xóa (`DELETE`) các bản ghi `AuditLogs`.

#### FR-014: Quản lý bài kiểm tra đầu vào
- **User Story:** tạo, cập nhật câu hỏi kiểm tra trình độ và xem thống kê độ khó, để đảm bảo bài placement phân loại chính xác trình độ học viên.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin tạo câu hỏi mới, nhập nội dung câu hỏi kèm các lựa chọn và đánh dấu đúng 1 đáp án `IsCorrect`, hệ thống lưu câu hỏi vào bảng `PlacementQuestions` và `PlacementOptions`.
  - [x] **AC-02:** Admin muốn tắt 1 câu hỏi (`IsActive = false`), số câu hỏi active còn lại của cấp độ đó xuống dưới mức tối thiểu, hệ thống ngăn chặn thao tác và báo lỗi yêu cầu bổ sung câu hỏi.

#### FR-015: Báo cáo chỉ số và Analytics Sản phẩm 
- **User Story:** các số liệu tổng quan về người dùng, tỷ lệ giữ chân và danh sách Kanji khó nhớ, để đưa ra quyết định cải tiến sản phẩm.
- **Độ ưu tiên:** P0 (Tổng quan/Kanji khó) | P1 (Cohort Retention/Funnel)
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin xem báo cáo học tập, truy vấn danh sách Kanji khó nhớ, hệ thống tính toán từ bảng `ReviewLogs`/`UserKanjiProgress` và trả về danh sách các chữ Kanji có tỷ lệ đánh giá "Quên" cao nhất theo từng cấp độ JLPT.
  - [x] **AC-02:** Admin xem dashboard phân tích, chọn khoảng thời gian, hệ thống truy vấn từ bảng tổng hợp `DailyMetrics` để hiển thị biểu đồ người dùng tích cực và tỷ lệ giữ chân D1/D7/D30 mà không quét trực tiếp trên toàn bộ bảng `ReviewLogs`.

## 4. Yêu cầu Phi chức năng

#### NFR-001: Hiệu năng và thời gian phản hồi
- **Mô tả:** Hệ thống phải phản hồi nhanh chóng để không làm gián đoạn trải nghiệm học tập và ôn tập Flashcard của người dùng.
- **Tiêu chí kiểm thử:**
  - [x] **NFR-001.1:** Thời gian phản hồi API cho 95% các yêu cầu đọc, truy vấn phải dưới `300ms` trong điều kiện tải bình thường.
  - [x] **NFR-001.2:** Thời gian xử lý chấm điểm SM-2 và ghi nhận tiến độ không quá `500ms`.
  - [x] **NFR-001.3:** Tải trang màn hình chính trên ứng dụng Mobile không quá `1.5 giây`.

#### NFR-002: Bảo mật và phân quyền
- **Mô tả:** Đảm bảo an toàn thông tin tài khoản, bảo vệ dữ liệu người dùng và phân quyền truy cập chặt chẽ giữa các vai trò.
- **Tiêu chí kiểm thử:**
  - [x] **NFR-002.1:** Mật khẩu người dùng bắt buộc phải mã hóa bằng thuật toán băm an toàn trước khi lưu vào Database; tuyệt đối không lưu mật khẩu dạng văn bản thô.
  - [x] **NFR-002.2:** Xác thực API sử dụng mô hình JWT Bearer Token. Access Token có thời gian ngắn, Refresh Token được lưu vết và thu hồi ngay khi tài khoản bị khóa.
  - [x] **NFR-002.3:** Áp dụng phân quyền theo tài nguyên, các endpoint khu vực `/api/admin/...` bắt buộc kiểm tra Policy `EditorOrAdmin` hoặc `AdminOnly`. Trả về mã lỗi `403 Forbidden` nếu truy cập trái phép.

#### NFR-003: Khả năng ngoại tuyến và đồng bộ
- **Mô tả:** Học viên có thể tiếp tục học và ôn tập ngay cả khi mất kết nối mạng internet tạm thời.
- **Tiêu chí kiểm thử:**
  - [x] **NFR-003.1:** Ứng dụng Mobile tự động lưu bộ đệm hàng đợi bài học trong ngày vào cơ sở dữ liệu cục bộ để học viên làm bài offline.
  - [x] **NFR-003.2:** Các nhật ký ôn tập `ReviewLogs` phát sinh ở chế độ offline được đẩy vào hàng đợi đồng bộ cục bộ và tự động gửi về Server ngay khi thiết bị có kết nối Internet trở lại.

#### NFR-004: Truy vết an ninh và kiểm toán
- **Mô tả:** Lưu vết toàn bộ các thao tác thay đổi dữ liệu của Admin để phục vụ kiểm tra và an toàn hệ thống.
- **Tiêu chí kiểm thử:**
  - [x] **NFR-004.1:** 100% các hành động Create, Update, Delete, Publish, Lock trên khu vực Admin bắt buộc tự động chèn 1 bản ghi vào bảng `AuditLogs` trong cùng một Database Transaction.
  - [x] **NFR-004.2:** Bảng `AuditLogs` thiết lập cơ chế chỉ thêm, không cung cấp bất kỳ API nào hỗ trợ chỉnh sửa hoặc xóa nhật ký audit.

#### NFR-005: Trải nghiệm tự do và không rào cản
- **Mô tả:** Giao diện ứng dụng phải linh hoạt, không khóa cứng quá nhiều tính năng cốt lõi hoặc ép buộc người học đi theo một con đường duy nhất.
- **Tiêu chí kiểm thử:**
  - [x] **NFR-005.1:** Cho phép người học tự do chọn cấp độ bắt đầu hoặc bỏ qua bài test đầu vào mà không bị bắt buộc.
  - [x] **NFR-005.2:** Trong màn hình luyện tập giao tiếp, hệ thống cho phép người dùng nhập văn bản tự do và có nút "Bỏ qua" để kết thúc bài học nhanh khi cần.

## 5. Ngoài phạm vi
- Chưa hỗ trợ nhận diện chữ viết tay Kanji qua Camera/AI.
- Chưa hỗ trợ thanh toán trực tuyến qua cổng Momo/ZaloPay.
- Hệ thống mạng xã hội VÀ tương tác cộng đồng.
- Đa ngôn ngữ giao diện.

## 6. Chỉ số Thành công
1. **Chỉ số 1: Tỷ lệ nhớ trong 7 ngày**
   - **Mô tả:** Tỷ lệ phần trăm các từ Kanji học viên trả lời đúng trong các phiên ôn tập sau 7 ngày kể từ lần học đầu tiên.
   - **Đánh giá:** Đạt chỉ số nếu tỷ lệ nhớ >= 85%. Đo lường dựa trên lịch sử đánh giá chất lượng phản hồi (phân điểm 0-5) của thuật toán SM-2 trong cơ sở dữ liệu.

2. **Chỉ số 2: Thói quen học và giờ giấc**
   - **Mô tả:** Mức độ duy trì chuỗi học tập và khung giờ học cố định hằng ngày của người dùng.
   - **Đánh giá:** Đạt chỉ số nếu người dùng duy trì chuỗi học >= 5 ngày/tuần và thực hiện phiên ôn tập trong khoảng khung giờ ưu tiên đã đăng ký với độ lệch 1 giờ.

3. **Chỉ số 3: Tỷ lệ giữ chân vào ngày thứ 7**
   - **Mô tả:** Tỷ lệ phần trăm người dùng mới tiếp tục quay lại ứng dụng và thực hiện ít nhất 1 phiên học, ôn tập vào ngày thứ 7 sau khi đăng ký tài khoản.
   - **Đánh giá:** Mục tiêu đạt >= 40% số lượng người dùng mới trong tuần. Đo lường tự động qua nhật ký truy cập.

## 7. Giả định và Rủi ro
- **Giả định**: .NET 10 SDK giữ tính ổn định tương thích ngược tốt với các thư viện NuGet core.
- **Rủi ro**: Thuật toán SM-2 có thể gây dồn lịch học nếu học viên bỏ dở quá 7 ngày.

