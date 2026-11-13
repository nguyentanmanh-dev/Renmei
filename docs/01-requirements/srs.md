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
  - [x] **AC-02:** học viên bấm vào biểu tượng loa trên thẻ, kích hoạt, hệ thống phát đúng bản thu âm người bản ngữ đọc **từ ghép ví dụ thông dụng** của từ kanji đó.

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

#### FR-016: Quản lý nội dung và cấu hình trò chơi
- **User Story:** quản lý các trò chơi thi đấu: bật/tắt trò chơi, tạo Boss, vụ án Kanji, nhóm thẻ Hanafuda, đoạn luyện nhại giọng và mùa giải, để nội dung trò chơi luôn đúng, đủ và kiểm soát được.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin vào mục Trò chơi, bật hoặc tắt một trò chơi và lưu, hệ thống áp dụng ngay cho ứng dụng người học mà không cần phát hành lại ứng dụng, và ghi 1 bản ghi AuditLogs.
  - [x] **AC-02:** Admin tạo một Boss cấp độ JLPT, máu tối đa MaxHp, thời gian bắt đầu, kết thúc ở trạng thái Draft, bấm Publish, hệ thống chỉ cho xuất bản khi cấp độ đó có đủ số Kanji Published có media theo mức tối thiểu của trò chơi.
  - [x] **AC-03:** Admin tạo một vụ án Kanji gồm các chương nội dung, Kanji liên quan, đáp án đúng, tối đa 3 gợi ý, bấm Publish, hệ thống kiểm tra mỗi chương có đáp án và ít nhất 1 Kanji liên quan trước khi chuyển sang Published.

#### FR-017: Kiểm duyệt và chống gian lận trò chơi
- **User Story:** xem các ván có dấu hiệu bất thường và xử lý biệt danh vi phạm, để bảng xếp hạng công bằng và cộng đồng an toàn.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin mở mục Kiểm duyệt trò chơi, hệ thống liệt kê các ván bị gắn cờ kèm lý do điểm hoặc thời gian phản ứng bất thường, trùng ClientEventId, sai lệch giờ, hỗ trợ lọc theo trò chơi và người chơi.
  - [x] **AC-02:** Admin loại một ván khỏi bảng xếp hạng hoặc cấm một người chơi khỏi bảng xếp hạng, hệ thống tính lại RankPoints bị ảnh hưởng, cập nhật bảng xếp hạng và ghi AuditLogs, người chơi bị cấm vẫn dùng được các chức năng học.
  - [x] **AC-03:** Admin xử lý một báo cáo biệt danh vi phạm, hệ thống cho phép buộc đổi biệt danh hoặc khóa tài khoản theo FR-011 và lưu người xử lý cùng thời gian.

#### FR-018: Kiểm duyệt nội dung xã hội
- **User Story:** xử lý các báo cáo về tin nhắn, lời mời và hành vi của người chơi, để cộng đồng bạn bè an toàn và quấy rối được xử lý kịp thời.
- **Độ ưu tiên:** P0
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Admin mở mục Báo cáo xã hội, hệ thống liệt kê các báo cáo theo trạng thái, lý do và người bị báo cáo, hỗ trợ lọc và phân trang; mỗi báo cáo chỉ hiển thị bằng chứng đã chụp lại khi báo cáo (tin nhắn bị báo cáo cùng vài tin lân cận), không cho Admin đọc toàn bộ hội thoại không liên quan.
  - [x] **AC-02:** Admin xử lý một báo cáo bác bỏ, cảnh cáo, hạn chế nhắn tin hoặc gửi lời mời có thời hạn, hoặc khóa tài khoản, hệ thống tạo UserSanctions khi có hạn chế, ghi 1 bản ghi AuditLogs, lưu người xử lý và thời gian, và thông báo cho người bị xử lý theo mẫu nội dung.
  - [x] **AC-03:** Admin gỡ một hạn chế còn hiệu lực, hệ thống áp dụng ngay, người dùng gửi được tin nhắn trở lại và ghi AuditLogs.

## 3.3 Đấu trường Kanji

#### FR-020: Sảnh Đấu trường và chế độ chơi
- **User Story:** Là học viên, tôi muốn có một khu "Đấu trường" tập hợp các trò chơi Kanji và chọn chơi luyện tập hoặc đấu hạng, để đổi không khí khi ôn tập mà vẫn luyện đúng những chữ mình đang học.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên mở tab Đấu trường, hệ thống hiển thị danh sách trò chơi đang bật chia theo nhóm Đấu trí truyền thống, Boss Raid, Vụ án, Giọng nói, kèm lựa chọn cấp độ N5-N1.
  - [x] **AC-02:** Học viên chọn một trò chơi ở cấp độ có số Kanji Published đủ media ít hơn mức tối thiểu của trò chơi đó, hệ thống không cho bắt đầu, hiển thị lý do và gợi ý chọn cấp độ khác, không chặn các chức năng học khác.
  - [x] **AC-03:** Học viên chọn "Luyện tập", hệ thống bắt đầu ván solo không tính hạng và cho chơi ngoại tuyến nếu bộ nội dung đã được tải; chọn "Đấu hạng", hệ thống yêu cầu kết nối mạng.
  - [x] **AC-04:** Học viên kết thúc một ván, hệ thống liệt kê các chữ trả lời sai từ 2 lần trở lên trong ván và đề xuất đưa vào hàng đợi ôn hôm sau; việc chơi game không ghi vào ReviewLogs và không thay đổi EaseFactor, Interval của SM-2.

#### FR-021: Đấu bất đồng bộ
- **User Story:** Là học viên, tôi muốn thi đấu với người chơi khác trên cùng một bộ câu hỏi vào thời điểm thuận tiện cho mỗi người, để cảm nhận sự cạnh tranh mà không phải chờ nhau.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên chọn "Đấu hạng" của một trò chơi và cấp độ, hệ thống ghép với đối thủ cùng trò chơi, cùng cấp độ, hạng gần nhau hoặc tạo trận chờ, rồi trả về MatchId và bộ câu hỏi cố định theo Seed; hai người chơi nhận đúng cùng một bộ câu hỏi.
  - [x] **AC-02:** Học viên hoàn thành ván trước khi có đối thủ, hệ thống lưu điểm và đặt trận ở trạng thái WaitingOpponent, nếu chưa có đối thủ sau thời hạn cấu hình, hệ thống ghép với bản ghi ván của một người chơi khác cùng bộ câu hỏi để có kết quả.
  - [x] **AC-03:** Cả hai bên đã hoàn thành, hệ thống so điểm, xác định thắng/hòa/thua và cập nhật RankPoints của người chơi thật trong cùng một Database Transaction, ghost không bị thay đổi hạng.
  - [x] **AC-04:** Học viên thoát ván giữa chừng và không quay lại trong thời gian quy định, hệ thống đóng ván với trạng thái Abandoned, tính thua theo quy tắc trận đấu, và không để trận ở trạng thái treo vô hạn.
  - [x] **AC-05:** Học viên muốn đấu với một người bạn cụ thể, hệ thống dùng lời mời thi đấu theo FR-036 thay cho ghép ngẫu nhiên, trận tạo ra là trận giao hữu và không dùng Seed khác với bộ câu hỏi của hai người.

#### FR-022: Xếp hạng, mùa giải và hồ sơ người chơi
- **User Story:** Là học viên, tôi muốn xem hạng của mình và bảng xếp hạng theo từng trò chơi và cấp độ, kèm mùa giải, để có mục tiêu thi đấu.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên mở bảng xếp hạng của một trò chơi, cấp độ và mùa giải, hệ thống hiển thị 100 người chơi đứng đầu và vị trí của chính học viên, đọc từ bảng tổng hợp LeaderboardEntries mà không quét toàn bộ bảng MatchPlayers.
  - [x] **AC-02:** Mùa giải kết thúc theo Seasons.EndsAt, hệ thống lưu kết quả cuối mùa, trao huy hiệu theo hạng và mở mùa mới với hạng khởi tạo theo quy tắc cấu hình.
  - [x] **AC-03:** Học viên lần đầu chọn "Đấu hạng", hệ thống yêu cầu đặt biệt danh duy nhất, kiểm tra bộ lọc từ cấm cơ bản, người chơi khác chỉ thấy biệt danh và hạng, không thấy email hay tên thật.
  - [x] **AC-04:** Học viên bật tùy chọn ẩn khỏi bảng xếp hạng, hệ thống vẫn cho thi đấu và tính hạng nhưng không hiển thị học viên trong bảng công khai.

#### FR-023: Karuta Kanji
- **User Story:** Là học viên, tôi muốn chơi Karuta với các thẻ Kanji: nghe hoặc đọc gợi ý rồi chạm thật nhanh vào thẻ đúng, để luyện phản xạ nhận mặt chữ, âm đọc và nghĩa.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Ván bắt đầu, hệ thống bày N thẻ Kanji lên bàn và lần lượt đưa ra gợi ý theo cấp độ khó, học viên chạm vào thẻ khớp với gợi ý.
  - [x] **AC-02:** Học viên chạm đúng thẻ, hệ thống cộng điểm cơ bản cộng thưởng tốc độ dựa trên thời gian phản ứng do Server đo, thẻ đó được lấy khỏi bàn, học viên chạm sai thẻ, hệ thống trừ điểm và khóa thẻ đó trong 1 giây.
  - [x] **AC-03:** Hết các thẻ hoặc hết thời gian, hệ thống tổng kết số lượt đúng, thời gian phản ứng trung bình và danh sách chữ sai, rồi gửi kết quả theo FR-021 nếu là Đấu hạng.

#### FR-024: Shiritori Kanji
- **User Story:** Là học viên, tôi muốn chơi Shiritori nối từ bằng các từ ghép Kanji, để luyện âm đọc của từ ghép và vốn từ gắn với chữ Kanji đã học.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Ván bắt đầu, hệ thống hiển thị một từ hiện tại cùng các thẻ từ lựa chọn; học viên chọn thẻ có âm đọc bắt đầu bằng kana cuối của âm đọc từ hiện tại.
  - [x] **AC-02:** Học viên chọn thẻ vi phạm luật, hệ thống trừ 1 mạng mặc định có 3 mạng và cho chọn lại, hết mạng thì kết thúc ván.
  - [x] **AC-03:** Hệ thống chuẩn hóa âm đọc khi so khớp, và điểm của ván bằng độ dài chuỗi nối hợp lệ, việc nối đúng hay sai do Server quyết định từ dữ liệu từ vựng.

#### FR-025: Menko Kanji
- **User Story:** Là học viên, tôi muốn chơi Menko với cặp thẻ Kanji và âm đọc, nghĩa, để luyện trí nhớ ghép chữ với âm và nghĩa.
- **Độ ưu tiên:** P2
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Ván bắt đầu, hệ thống bày lưới thẻ úp gồm các cặp, học viên lật từng thẻ bằng thao tác vuốt nhanh có hiệu ứng lật thẻ.
  - [x] **AC-02:** Học viên lật 2 thẻ khớp cặp, hệ thống giữ 2 thẻ ngửa, cộng điểm và tăng hệ số liên tiếp, lật 2 thẻ không khớp, hệ thống úp lại sau 1 giây và đặt lại combo về 0.
  - [x] **AC-03:** Học viên lật hết các cặp hoặc hết thời gian, hệ thống tổng kết điểm, số lượt lật và các chữ bị chọn nhầm, rồi gửi kết quả theo FR-021 nếu là Đấu hạng.

#### FR-026: Hanafuda Kanji
- **User Story:** Là học viên, tôi muốn chơi một ván bài Hanafuda với thẻ Kanji để hệ thống hóa các chữ Kanji liên quan với nhau.
- **Độ ưu tiên:** P2
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Ván bắt đầu, hệ thống chia bài theo Seed: học viên nhận số thẻ cố định trên tay và mặt bàn có số thẻ cố định, mỗi thẻ thuộc một trong 12 nhóm do Admin định nghĩa.
  - [x] **AC-02:** Học viên ra một thẻ cùng nhóm với thẻ trên mặt bàn, hệ thống cho thu cả hai thẻ về kho điểm, ra thẻ không cùng nhóm với thẻ nào, thẻ được đặt lại trên mặt bàn.
  - [x] **AC-03:** Học viên thu đủ bộ thẻ theo quy tắc ghép, hệ thống cộng điểm thưởng bộ, hết bài, hệ thống tổng kết điểm. Đối thủ trong ván là người chơi còn lại theo FR-021 hoặc bản ghi ván, không có lượt chơi đồng thời.

#### FR-027: Kingyo-sukui Kanji
- **User Story:** Là học viên, tôi muốn vớt đúng những con cá vàng mang chữ Kanji theo yêu cầu bằng chiếc vợt giấy, để luyện chọn nhanh chữ đúng dưới áp lực thời gian.
- **Độ ưu tiên:** P2
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Ván bắt đầu, hệ thống hiển thị bể cá có các con cá mang Kanji bơi chậm và một yêu cầu, học viên kéo vợt để vớt con cá có chữ đúng.
  - [x] **AC-02:** Học viên vớt đúng cá, hệ thống cộng điểm và đưa cá vào chén, vớt sai cá, độ bền của vợt giảm, vợt giảm về 0 thì rách và học viên mất 1 chiếc vợt.
  - [x] **AC-03:** Hết thời gian hoặc hết vợt, hệ thống kết thúc ván, tổng kết số cá đúng, số cá sai, rồi gửi kết quả theo FR-021 nếu là Đấu hạng.

#### FR-028: Tobe! Geta
- **User Story:** Là học viên, tôi muốn chạy và nhảy qua các đôi guốc Geta mang chữ Kanji để chọn đúng đáp án, để luyện nhận chữ trong một trò chơi vận động nhanh.
- **Độ ưu tiên:** P2
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Ván bắt đầu, nhân vật chạy tự động; mỗi đoạn hệ thống hiển thị một yêu cầu và 2 đến 3 đôi Geta mang các Kanji khác nhau; học viên chạm để nhảy vào đôi Geta mang chữ đúng.
  - [x] **AC-02:** Học viên nhảy vào Geta đúng, hệ thống cộng điểm và tăng quãng đường, nhảy vào Geta sai hoặc không nhảy kịp, nhân vật rơi và mất 1 mạng mặc định 3 mạng).
  - [x] **AC-03:** Tốc độ chạy tăng dần theo số đoạn đã vượt qua, hết mạng thì kết thúc ván; điểm tính từ nhật ký sự kiện theo NFR-006, không dùng điểm do ứng dụng tự gửi.

#### FR-029: Kendama Kanji Combo
- **User Story:** Là học viên, tôi muốn ghép các chữ Kanji thành từ ghép để bắt bóng Kendama và nối thành chuỗi combo, để luyện từ ghép và khả năng nhớ liên tiếp.
- **Độ ưu tiên:** P2
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Ván bắt đầu, hệ thống hiển thị một dãy chữ Kanji và gợi ý, học viên chọn 2 chữ để ghép thành từ ghép đúng với gợi ý.
  - [x] **AC-02:** Học viên ghép đúng từ ghép có trong dữ liệu, hệ thống "bắt bóng" thành công, tăng chuỗi combo và nhân hệ số điểm theo chuỗi, ghép sai, hệ thống làm rơi bóng và đặt lại combo về 0.
  - [x] **AC-03:** Hết thời gian hoặc hết số lần rơi bóng cho phép tối đa 3 lần rơi, hệ thống tổng kết combo dài nhất và điểm, rồi gửi kết quả theo FR-021 nếu là Đấu hạng.

#### FR-030: Kanji Boss Raid
- **User Story:** Là học viên, tôi muốn cùng cộng đồng đánh bại một Boss hằng tuần bằng cách trả lời câu hỏi Kanji, để có mục tiêu chung và phần thưởng khi Boss bị hạ.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên mở Boss Raid, hệ thống hiển thị Boss đang hoạt động theo cấp độ: tên, máu hiện tại CurrentHp, máu tối đa, thời gian còn lại và số vé tấn công còn lại của học viên.
  - [x] **AC-02:** Học viên dùng 1 vé và hoàn thành lượt tấn công, hệ thống tính sát thương trên Server theo số câu đúng và tốc độ, rồi trừ vào CurrentHp của Boss.
  - [x] **AC-03:** Nhiều học viên tấn công cùng lúc, hệ thống cập nhật máu Boss nguyên tử, không để máu âm hay mất sát thương, và hiển thị máu mới cho các học viên.
  - [x] **AC-04:** Boss bị hạ hoặc hết hạn, hệ thống chốt trạng thái, trao phần thưởng cho người có sát thương từ mức tối thiểu và chỉ trao một lần cho mỗi người dù yêu cầu bị gửi lại.
  - [x] **AC-05:** Học viên mở tab Bạn bè của Boss Raid, hệ thống hiển thị sát thương của bản thân và của bạn bè trên Boss đang hoạt động, xếp theo sát thương giảm dần, chỉ bạn bè mới xuất hiện trong danh sách này.

#### FR-031: Vụ án Kanji
- **User Story:** Là học viên, tôi muốn điều tra các vụ án bằng cách đọc, hiểu và giải mã manh mối chứa chữ Kanji, để học Kanji qua một câu chuyện thú vị.
- **Độ ưu tiên:** P2
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên mở một vụ án theo cấp độ, hệ thống mở khóa tuần tự từng chương và chỉ cho sang chương sau khi giải đúng chương hiện tại.
  - [x] **AC-02:** Học viên giải một manh mối bằng cách chọn hoặc nhập đáp án, hệ thống kiểm tra phía Server và báo đúng hoặc sai; đáp án đúng đưa câu chuyện sang diễn biến tiếp theo.
  - [x] **AC-03:** Học viên dùng gợi ý tối đa có 3, hệ thống hiển thị gợi ý tiếp theo và ghi nhận số gợi ý đã dùng, khi hoàn thành vụ án, hệ thống tính số sao theo số gợi ý và thời gian, rồi xếp hạng theo thời gian và số gợi ý trên bảng xếp hạng của vụ án.
  - [x] **AC-04 (CR-002):** Học viên chọn chơi đồng đội, hệ thống chuyển sang phòng chơi nhiều người theo FR-037 và dùng cùng nội dung vụ án; chế độ chơi một mình vẫn giữ nguyên các AC-01 đến AC-03.

#### FR-032: Shadowing Voice Battle
- **User Story:** Là học viên, tôi muốn nghe một câu hoặc từ rồi nhại lại theo giọng chuẩn và được chấm điểm phát âm, để luyện phát âm và so tài với người khác trên cùng đoạn âm thanh.
- **Độ ưu tiên:** P2
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên lần đầu vào Shadowing, hệ thống xin quyền micro kèm giải thích; học viên từ chối quyền, hệ thống vẫn cho dùng các chức năng khác của ứng dụng và hiển thị cách bật quyền sau.
  - [x] **AC-02:** Học viên nghe đoạn âm thanh chuẩn rồi đọc lại, hệ thống nhận dạng giọng nói, so với văn bản và phiên âm chuẩn, trả điểm phát âm từ 0 đến 100 kèm các chữ đọc sai.
  - [x] **AC-03:** Sau khi chấm xong, hệ thống không lưu trữ bản ghi âm; chỉ lưu điểm, văn bản nhận dạng và phiên bản bộ nhận dạng đã dùng.
  - [x] **AC-04:** Học viên chọn đấu hạng Shadowing, hệ thống áp dụng FR-021 với cùng bộ đoạn âm thanh cho hai người chơi và so tổng điểm phát âm.
  - [x] **AC-05:** học viên chọn chơi cùng bạn, hệ thống tạo phòng Shadowing theo FR-037 với cùng bộ đoạn âm thanh cho mọi thành viên, phòng là giao hữu và không đổi RankPoints.


## 3.4 Kết nối và thi đấu cùng bạn bè - CR-002

#### FR-033: Hồ sơ người chơi và cài đặt riêng tư xã hội
- **User Story:** Tôi muốn có mã bạn bè và tự quyết định ai được gửi lời kết bạn, nhắn tin hoặc mời tôi thi đấu, để chỉ tương tác với người tôi chọn.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên đã đặt biệt danh hợp lệ (FR-022) mở màn hình Bạn bè, hệ thống hiển thị mã bạn bè duy nhất của mình (FriendCode) và cho sao chép hoặc chia sẻ; học viên chưa đặt biệt danh được hướng dẫn đặt biệt danh trước khi dùng chức năng xã hội.
  - [x] **AC-02:** Học viên đổi cài đặt nhận lời mời kết bạn (mọi người hoặc không ai), nhận tin nhắn (bạn bè hoặc không ai) và nhận lời mời thi đấu (bạn bè hoặc không ai), hệ thống lưu và áp dụng ngay cho các yêu cầu phát sinh sau đó.
  - [x] **AC-03:** Học viên lần đầu dùng chức năng xã hội, hệ thống hiển thị quy tắc ứng xử và chính sách riêng tư về tin nhắn, chỉ cho tiếp tục khi học viên đồng ý và lưu thời điểm đồng ý (SocialTermsAcceptedAt).
  - [x] **AC-04:** Người dùng khác xem hồ sơ công khai của học viên, hệ thống chỉ trả biệt danh, hạng, huy hiệu và trạng thái quan hệ (đã là bạn, đã gửi lời mời, chưa liên quan); không trả email, tên thật hay thông tin đăng nhập.

#### FR-034: Kết bạn
- **User Story:** Tôi muốn tìm và kết bạn với người học khác bằng biệt danh hoặc mã bạn bè, để thi đấu và học cùng nhau.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên nhập biệt danh chính xác hoặc mã bạn bè, hệ thống trả hồ sơ công khai tương ứng và nút Kết bạn, hệ thống không có chức năng liệt kê hoặc tìm gần đúng hàng loạt người dùng, và tìm kiếm có giới hạn tần suất.
  - [x] **AC-02:** Học viên gửi lời mời kết bạn, hệ thống tạo lời mời ở trạng thái Pending và thông báo cho người nhận theo FR-038; hệ thống từ chối khi đã có lời mời đang chờ giữa hai người, khi một bên đã chặn bên kia, khi người nhận đã tắt nhận lời mời, hoặc khi vượt giới hạn lời mời mỗi ngày (mặc định 20).
  - [x] **AC-03:** Người nhận chấp nhận, hệ thống tạo quan hệ bạn bè hai chiều trong một Database Transaction và đóng lời mời, người nhận từ chối, người gửi hủy, hoặc lời mời hết hạn quá 14 ngày, hệ thống đóng lời mời với trạng thái tương ứng và không tạo quan hệ bạn bè.
  - [x] **AC-04:** Học viên xóa một người bạn, hệ thống kết thúc quan hệ, cả hai không còn gửi được tin nhắn hay lời mời thi đấu mới cho nhau, và các lời mời thi đấu đang chờ giữa hai người bị hủy.
  - [x] **AC-05:** Học viên mở danh sách bạn, hệ thống hiển thị biệt danh, hạng, huy hiệu, số tin chưa đọc và lối tắt nhắn tin hoặc thách đấu, có phân trang và giới hạn số bạn tối đa.

#### FR-035: Nhắn tin riêng giữa bạn bè
- **User Story:** Tôi muốn nhắn tin với bạn bè của mình trong ứng dụng, để rủ nhau thi đấu và trao đổi khi học.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên gửi một tin nhắn văn bản cho một người bạn, hệ thống lưu tin với ClientMessageId duy nhất, gửi lại cùng một tin không tạo tin trùng, và người nhận thấy tin theo NFR-010.
  - [x] **AC-02:** Học viên gửi tin cho người không phải bạn bè, người đã chặn mình hoặc bị mình chặn, người đã tắt nhận tin, hoặc khi chính học viên đang bị hạn chế nhắn tin, hệ thống từ chối với lý do nghiệp vụ rõ ràng và không lưu tin.
  - [x] **AC-03:** Học viên mở một hội thoại, hệ thống trả lịch sử tin nhắn theo thời gian có phân trang, đặt vị trí đã đọc và hiển thị số tin chưa đọc đúng trên danh sách hội thoại.
  - [x] **AC-04:** Tin nhắn đi qua bộ lọc từ cấm cơ bản và giới hạn tần suất, vượt giới hạn, hệ thống trả 429 Too Many Requests.
  - [x] **AC-05:** Học viên xóa một tin do chính mình gửi, hệ thống ẩn tin với cả hai bên và ghi thời điểm xóa, tin bị báo cáo trước đó vẫn được giữ làm bằng chứng.

#### FR-036: Lời mời thi đấu 1 vs 1
- **User Story:** Tôi muốn mời một người bạn thi đấu một trò chơi 1 vs 1 mà tôi chọn, để cùng chơi vào lúc thuận tiện cho mỗi người.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên chọn một người bạn, một trò chơi 1 vs 1 và cấp độ, rồi gửi lời mời, hệ thống kiểm tra trò chơi đang bật, cấp độ đủ MinKanjiRequired, người nhận cho phép nhận lời mời, rồi tạo lời mời Pending hết hạn theo cấu hình.
  - [x] **AC-02:** Người nhận chấp nhận, hệ thống tạo một trận giao hữu với cùng Seed và bộ câu hỏi cho cả hai; mỗi người chơi bất đồng bộ trong thời hạn cấu hình, người nhận từ chối, người gửi hủy hoặc lời mời hết hạn thì không tạo trận.
  - [x] **AC-03:** Cả hai hoàn thành, hoặc hết thời hạn, hệ thống so điểm do Server tính, xác định thắng, hòa, thua theo quy tắc FR-021 AC-04 cho bên chưa chơi, và cập nhật bộ đếm thắng thua giữa hai người, trận giao hữu mặc định không thay đổi RankPoints của ai.
  - [x] **AC-04:** mỗi học viên có tối đa số lời mời đang chờ tới cùng một người và tổng số lời mời gửi mỗi ngày, vượt giới hạn, hệ thống từ chối.
  - [x] **AC-05:** học viên bấm Đấu lại trên màn hình kết quả trận giao hữu, hệ thống tạo lời mời mới với cùng trò chơi, cấp độ và người bạn đó.

#### FR-037: Phòng chơi nhiều người
- **User Story:** Tôi muốn lập phòng cùng vài người bạn để cùng phá một vụ án hoặc cùng thi Shadowing trên một bộ đoạn âm thanh, để học vui hơn khi có bạn.
- **Độ ưu tiên:** P2
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** Học viên chọn Vụ án Kanji 2 đến 4 người hoặc Shadowing 2 đến 8 người, cấp độ và vụ án nếu có, hệ thống tạo phòng ở trạng thái Lobby với mã phòng, chủ phòng mời bạn bè hoặc chia sẻ mã phòng, và mặc định chỉ bạn bè của chủ phòng được vào.
  - [x] **AC-02:** Người được mời chấp nhận hoặc nhập mã phòng, hệ thống cho vào phòng; hệ thống từ chối kèm lý do khi phòng đầy, đã bắt đầu, hết hạn hoặc người chơi đang bị chặn bởi chủ phòng; người rời phòng ở trạng thái Lobby giải phóng chỗ, và khi chủ phòng rời thì quyền chủ phòng chuyển cho người vào sớm nhất hoặc phòng bị hủy nếu trống.
  - [x] **AC-03:** Vụ án: Chủ phòng bấm bắt đầu khi có ít nhất 2 người, cả nhóm cùng giải một vụ án với tiến độ chung chương hiện tại, số gợi ý đã dùng chung, tối đa 3 mỗi chương, mỗi chương có thể được giải bởi bất kỳ thành viên, hệ thống ghi người giải đúng đầu tiên và không tính hai người cho cùng một chương, hoàn thành thì hệ thống tính sao của nhóm theo gợi ý và thời gian, và ghi đóng góp từng người.
  - [x] **AC-04:** Shadowing: Chủ phòng bấm bắt đầu, mọi thành viên nhận cùng bộ đoạn âm thanh theo Seed, nộp điểm trong thời hạn, hệ thống chốt phòng khi mọi người đã nộp hoặc hết hạn và hiển thị bảng điểm trong phòng, không lưu bản ghi âm, phòng Shadowing là giao hữu và không đổi RankPoints.
  - [x] **AC-05:** phòng không đủ người để bắt đầu hoặc không hoạt động quá thời hạn cấu hình, hệ thống đóng phòng với trạng thái Expired, giữ kết quả phần đã chơi, và không để phòng treo vô hạn.
  - [x] **AC-06:** trong phòng, học viên chỉ dùng được lời nhắn nhanh định sẵn từ một danh sách cố định, không có ô nhập tin tự do, muốn trao đổi riêng thì dùng FR-035.

#### FR-038: Thông báo xã hội
- **User Story:** Tôi muốn biết khi có lời mời kết bạn, lời mời thi đấu, tin nhắn mới hoặc kết quả trận, để không bỏ lỡ khi bạn bè tìm tôi.
- **Độ ưu tiên:** P1
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** có một sự kiện xã hội: lời mời kết bạn, được chấp nhận kết bạn, lời mời thi đấu, lời mời phòng, tin nhắn mới, trận hoặc phòng có kết quả, hệ thống tạo thông báo cho người nhận; ứng dụng hiển thị trung tâm thông báo với số chưa đọc và cho đánh dấu đã đọc.
  - [x] **AC-02:** ứng dụng đang mở, thông báo hiển thị trong khoảng thời gian theo NFR-010, ứng dụng đã đóng thì gửi thông báo đẩy nếu người dùng đã bật và cấp quyền, người dùng từ chối quyền vẫn xem được thông báo trong ứng dụng.
  - [x] **AC-03:** học viên tắt từng loại thông báo trong cài đặt, hệ thống tôn trọng cài đặt và không gửi thông báo từ hoặc về người đã bị chặn.
  - [x] **AC-04:** thông báo đẩy mặc định chỉ ghi "Bạn có tin nhắn mới" và không hiển thị nội dung tin nhắn trên màn hình khóa.

#### FR-039: An toàn người dùng: chặn, báo cáo và hạn chế
- **User Story:** Tôi muốn chặn và báo cáo người làm phiền, và tắt nhận tương tác từ người khác, để cảm thấy an toàn khi dùng chức năng xã hội.
- **Độ ưu tiên:** P0 (điều kiện để bật FR-034 đến FR-037)
- **Tiêu chí chấp nhận (AC):**
  - [x] **AC-01:** học viên chặn một người dùng từ hồ sơ hoặc hội thoại, hệ thống kết thúc quan hệ bạn bè, hủy lời mời đang chờ giữa hai người, chặn hai chiều việc gửi tin nhắn, lời mời kết bạn, lời mời thi đấu và lời mời phòng, và không thông báo cho người bị chặn; học viên bỏ chặn được và việc bỏ chặn không khôi phục quan hệ bạn bè.
  - [x] **AC-02:** học viên báo cáo một tin nhắn, một người dùng hoặc một trận bằng cách chọn lý do, hệ thống tạo báo cáo kèm bằng chứng chụp lại tại thời điểm báo cáo và hiển thị xác nhận, hệ thống không nhận báo cáo trùng cùng một tin từ cùng một người.
  - [x] **AC-03:** một người dùng bị báo cáo từ số lượng người khác nhau vượt ngưỡng trong khoảng thời gian cấu hình, hệ thống tự động hạn chế tạm thời việc gửi tin nhắn và lời mời của người đó (UserSanctions) để chờ Admin xem xét theo FR-018, và ghi AuditLogs.
  - [x] **AC-04:** biệt danh và nội dung tin nhắn đi qua bộ lọc từ cấm, người dùng lặp lại vi phạm bộ lọc nhiều lần trong khoảng thời gian cấu hình bị hạn chế nhắn tin tạm thời.
  - [x] **AC-05:** học viên đã tắt nhận lời mời kết bạn, tin nhắn hoặc lời mời thi đấu, hệ thống từ chối các yêu cầu tương ứng từ người khác mà không tiết lộ cài đặt cụ thể của học viên.

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

#### NFR-006: Công bằng và chống gian lận trong trò chơi
- **Mô tả:** Kết quả thi đấu phải công bằng; ứng dụng người học không được tự quyết định điểm số.
- **Tiêu chí kiểm thử:**
  - [x] **NFR-006.1:** Điểm của mọi ván do Server tính từ nhật ký sự kiện, điểm do ứng dụng gửi lên bị bỏ qua.
  - [x] **NFR-006.2:** Thời gian phản ứng được Server đo bằng mốc thời gian gửi câu hỏi và nhận đáp án, có dung sai mạng được cấu hình, ván có thời gian phản ứng thấp hơn ngưỡng người chơi bình thường bị gắn cờ.
  - [x] **NFR-006.3:** Mỗi sự kiện có ClientEventId duy nhất, gửi lại cùng một sự kiện không làm tăng điểm hai lần.
  - [x] **NFR-006.4:** Giới hạn tần suất tạo ván và tạo trận mỗi người dùng, vượt ngưỡng trả 429 Too Many Requests.

#### NFR-007: Hiệu năng trò chơi
  - **Mô tả:** Trò chơi phải mượt và không làm chậm các chức năng học.
  - **Tiêu chí kiểm thử:**
    - [x] **NFR-007.1:** Thời gian phản hồi API tạo ván và lấy bộ câu hỏi cho 95% yêu cầu dưới 500ms.
    - [x] **NFR-007.2:** Thời gian phản hồi API đọc bảng xếp hạng cho 95% yêu cầu dưới 300ms.
    - [x] **NFR-007.3:** Các trò chơi có chuyển động như Kingyo-sukui, Tobe! Geta, Kendama duy trì tối thiểu 30 khung hình/giây trên thiết bị Android tầm trung dùng để kiểm thử.

#### NFR-008: Quyền riêng tư và an toàn trong trò chơi và kết nối xã hội
  - **Mô tả:** Người chơi chỉ lộ thông tin tối thiểu. Kênh tương tác giữa người dùng chỉ gồm nhắn tin riêng giữa bạn bè và lời nhắn nhanh định sẵn trong phòng.
  - **Tiêu chí kiểm thử:**
    - [x] **NFR-008.1:** Người không phải bạn bè chỉ nhìn thấy biệt danh, hạng và huy hiệu, không API nào trả email hoặc tên thật của người chơi khác.
    - [x] **NFR-008.2:** Hệ thống chỉ cho nhắn tin giữa hai người đã là bạn bè và không chặn nhau, không có chat công cộng, bình luận, nhóm chat tự do hay nhắn tin cho người lạ, nội dung chỉ là văn bản thuần (không ảnh, tệp, liên kết bấm được).
    - [x] **NFR-008.3:** Âm thanh ghi âm của Shadowing không được lưu trữ hay ghi vào nhật ký, ứng dụng chỉ gửi văn bản nhận dạng hoặc điểm theo thiết kế được duyệt.
    - [x] **NFR-008.4:** Biệt danh và tin nhắn được kiểm tra qua bộ lọc từ cấm cơ bản, có chức năng chặn và báo cáo.
    - [x] **NFR-008.5:** Tin nhắn được lưu trong thời hạn cấu hình rồi bị xóa bởi job nền, tin bị báo cáo được giữ làm bằng chứng đến khi báo cáo xử lý xong, khi người dùng xóa tài khoản, tin nhắn của họ bị xóa hoặc ẩn danh theo chính sách riêng tư.
    - [x] **NFR-008.6:** Nội dung tin nhắn không được ghi vào nhật ký ứng dụng và không được gửi tới dịch vụ phân tích hoặc báo lỗi tự động.
    - [x] **NFR-008.7:** Thông báo đẩy mặc định không chứa nội dung tin nhắn.

#### NFR-009: Chơi ngoại tuyến cho trò chơi
  - **Mô tả:** Học viên có thể luyện tập các trò chơi khi mất mạng; đấu hạng cần mạng.
  - **Tiêu chí kiểm thử:**
    - [x] **NFR-009.1:** Chế độ Luyện tập của các trò chơi chạy được ngoại tuyến bằng bộ nội dung đã tải xuống.
    - [x] **NFR-009.2:** Kết quả ván Luyện tập không tính vào bảng xếp hạng, ván Đấu hạng mất mạng giữa chừng được giữ tạm trong thời gian quy định rồi gửi lại, quá hạn thì đóng ván theo FR-021 AC-04.

#### NFR-010: Giao nhận tin nhắn và thông báo
  - **Mô tả:** Tin nhắn và thông báo đến đúng người, không trùng, không mất khi mạng chập chờn.
  - **Tiêu chí kiểm thử:**
    - [x] **NFR-010.1:** Gửi lại cùng một ClientMessageId không tạo thêm tin; thứ tự tin trong hội thoại theo thời gian Server.
    - [x] **NFR-010.2:** Khi cả hai người đang mở ứng dụng, 95% tin nhắn hiển thị cho người nhận trong vòng 3 giây.
    - [x] **NFR-010.3:** Mất mạng khi gửi, tin ở trạng thái chờ gửi trên thiết bị và tự gửi lại khi có mạng; người gửi thấy rõ trạng thái đã gửi, đang chờ hoặc lỗi.
    - [x] **NFR-010.4:** Thời gian phản hồi API lấy danh sách hội thoại, lịch sử tin và danh sách bạn cho 95% yêu cầu dưới 300ms.
    - [x] **NFR-010.5:** Giới hạn tần suất gửi tin, gửi lời mời và tìm kiếm người dùng theo mỗi người dùng, vượt ngưỡng trả 429 Too Many Requests.

#### NFR-011: An toàn nội dung người dùng tạo ra và tuân thủ 
  - **Mô tả:** Chức năng xã hội phải có biện pháp kiểm soát và thông tin rõ ràng cho người dùng.
  - **Tiêu chí kiểm thử:**
    - [x] **NFR-011.1:** Nút Chặn và Báo cáo có ngay trong hội thoại và hồ sơ người chơi, tối đa hai thao tác từ màn hình đó.
    - [x] **NFR-011.2:** Báo cáo được ghi nhận kèm bằng chứng và hiển thị cho Admin, mục tiêu đề xuất là xử lý tối thiểu 90% báo cáo trong 72 giờ, đo bằng số liệu của FR-018 AC-04.
    - [x] **NFR-011.3:** Quy tắc ứng xử và thông báo riêng tư về tin nhắn được hiển thị và được đồng ý trước lần dùng chức năng xã hội đầu tiên.
    - [x] **NFR-011.4:** Chính sách bảo mật được cập nhật nêu rõ dữ liệu tin nhắn, danh sách bạn và thông báo trước khi bật chức năng cho người dùng thật.
    - [x] **NFR-011.5:** Trước khi phát hành, đối chiếu yêu cầu hiện hành của nền tảng phân phối về nội dung do người dùng tạo ra và độ tuổi và ghi kết quả vào hồ sơ cổng phát hành.

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

