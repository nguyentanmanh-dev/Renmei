# RTM: Requirements Traceability Matrix

| Phiên bản : 1.0 | Ngày : 01-11-2026 | Trạng thái : Frozen | Cổng : R1-G1 | Người duyệt : Nguyễn Tấn Mạnh |

## Ma trận truy vết yêu cầu chức năng

| Mã FR		 | Tên Yêu cầu Chức năng									| Độ ưu tiên | Mô-đun / SDD													 | Bảng Dữ liệu Database																				    | Test Case ID								| Trạng thái	 |
|---|---|---|---|---|---|
| **FR-001** | Đăng ký và đăng nhập tài khoản							| P0		 | AuthController, AuthService, JwtProvider					 	 | `Users`, `RefreshTokens`																				    | `TC-AUTH-001`, `TC-AUTH-002`				| Pending Design |
| **FR-002** | Kiểm tra trình độ đầu vào và chọn trình độ				| P1		 | PlacementController, PlacementService						 | `PlacementQuestions`, `PlacementAttempts`, `PlacementAttemptAnswers`									    | `TC-PLACE-001`, `TC-PLACE-002`			| Pending Design |
| **FR-003** | Ôn tập Kanji theo thuật toán SM-2 và đánh giá năng lực   | P0	  	 | ReviewController, Sm2EngineService							 | `UserKanjiProgress`, `ReviewLogs`																		| `TC-SM2-001`, `TC-SM2-002`, `TC-SM2-003`  | Pending Design |
| **FR-004** | Hàng đợi học hằng ngày và thống kê tiến độ				| P0		 | QueueController, DailyQueueService							 | `DailyQueues`, `UserKanjiProgress`																		| `TC-QUEUE-001`, `TC-QUEUE-002`			| Pending Design |
| **FR-005** | Học từ vựng có hình ảnh minh họa và giọng đọc chuẩn	    | P0		 | FlashcardView , MediaService									 | `Kanji`, `KanjiMedia`, `KanjiExamples`																	| `TC-FLASH-001`, `TC-FLASH-002`			| Pending Design |
| **FR-006** | Duy trì chuỗi ngày Học									| P0		 | StreakService, GamificationEngine							 | `UserStreaks`																							| `TC-STRK-001`, `TC-STRK-002`				| Pending Design |
| **FR-007** | Hội thoại tình huống Ngắn và giao tiếp Freestyle			| P1		 | ChatController, RoleplayChatEngine							 | `ChatScenarios`,`ChatSessions`, `ChatMessages`															| `TC-CHAT-001`, `TC-CHAT-002`				| Pending Design |
| **FR-010** | Quản lý Nội dung Kanji và lịch sử phiên bản				| P0		 | AdminKanjiController, RevisionService						 | `Kanji`, `KanjiRevisions`																				| `TC-ADM-010-01`, `TC-ADM-010-02`			| Pending Design |
| **FR-011** | Quản lý tài khoản user và an toàn						| P0 / P1	 | AdminUserController, IdentityService							 | `Users`, `UserRoles`, `RefreshTokens`                                                                    | `TC-ADM-011-01`, `TC-ADM-011-02`			| Pending Design |
| **FR-012** | Tiếp nhận và xử lý báo lỗi								| P0		 | AdminFeedbackController, FeedbackService						 | `Feedbacks`																								| `TC-ADM-012-01`							| Pending Design |
| **FR-013** | Truy vết thao tác quản trị							    | P0		 | AuditLogMiddleware, AuditService								 | `AuditLogs`																								| `TC-ADM-013-01`, `TC-ADM-013-02`			| Pending Design |
| **FR-014** | Quản lý Bài kiểm tra Đầu vào							    | P1		 | AdminPlacementController										 | `PlacementQuestions`, `PlacementOptions`																	| `TC-ADM-014-01`							| Pending Design |
| **FR-015** | Báo cáo chỉ số và Analytics sản phẩm						| P0 / P1	 | AnalyticsController, MetricsWorkerJob						 | `DailyMetrics`, `ReviewLogs`																				| `TC-ADM-015-01`							| Pending Design |
| **FR-016** | Quản lý nội dung và cấu hình trò chơi					| P1		 | GameContentAdminController, GameContentAdminService			 | `GameModes`, `KanjiGroups`, `KanjiGroupItems`, `Bosses`, `CaseFiles`, `CaseChapters`, `ShadowingClips`   | `TC-ADM-016-01`, `TC-ADM-016-02`			| Pending Design |
| **FR-017** | Kiểm duyệt và chống gian lận trò chơi					| P1		 | GameModerationController, GameModerationService				 | `Reports`, `GameSessions`, `GameProfiles`, `AuditLogs`													| `TC-ADM-017-01`							| Pending Design |
| **FR-018** | Kiểm duyệt nội dung xã hội								| P0		 | SocialModerationAdminController, SocialModerationAdminService | `Reports`, `Users`, `AuditLogs`																			| `TC-ADM-018-01`							| Pending Design |
| **FR-020** | Sảnh Đấu trường và chế độ chơi							| P1		 | GameCatalogController, GameCatalogService, GameSessionService | `GameSessions`, `GameProfiles`																			| `TC-GAME-020-01`							| Pending Design |
| **FR-021** | Đấu bất đồng bộ			  								| P1		 | MatchmakingController, MatchmakingService, RatingService		 | `Matches`, `MatchPlayers`, `GameProfiles`, `GameSessions`												| `TC-GAME-021-01`, `TC-GAME-021-02`		| Pending Design |
| **FR-022** | Xếp hạng, mùa giải và hồ sơ người chơi					| P1		 | LeaderboardController, ProfileService, LeaderboardService	 | `GameProfiles`, `LeaderboardEntries`, `Seasons`, `MatchPlayers`											| `TC-GAME-022-01`							| Pending Design |
| **FR-023** | Karuta Kanji												| P1		 | KarutaGameEngine, GameSessionService							 | `GameSessions`, `Kanji`																					| `TC-GAME-023-01`							| Pending Design |
| **FR-024** | Shiritori Kanji											| P1		 | ShiritoriGameEngine, GameSessionService						 | `GameSessions`, `Kanji`																					| `TC-GAME-024-01`							| Pending Design |
| **FR-025** | Menko Kanji												| P2		 | MenkoGameEngine, GameSessionService							 | `GameSessions`, `Kanji`																					| `TC-GAME-025-01`							| Pending Design |
| **FR-026** | Hanafuda Kanji											| P2		 | HanafudaGameEngine, GameSessionService						 | `GameSessions`, `Kanji`																					| `TC-GAME-026-01` 							| Pending Design |
| **FR-027** | Kingyo-sukui Kanji										| P2		 | KingyoGameEngine, GameSessionService							 | `GameSessions`, `Kanji`																					| `TC-GAME-027-01` 							| Pending Design |
| **FR-028** | Tobe! Geta												| P2		 | TobeGetaGameEngine, GameSessionService						 | `GameSessions`, `Kanji`																					| `TC-GAME-028-01` 							| Pending Design |
| **FR-029** | Kendama Kanji Combo										| P2		 | KendamaGameEngine, GameSessionService						 | `GameSessions`, `Kanji`																					| `TC-GAME-029-01` 							| Pending Design |
| **FR-030** | Kanji Boss Raid											| P1		 | RaidController, RaidService									 | `Bosses`, `BossRaidAttempts`, `GameSessions`																| `TC-GAME-030-01` 							| Pending Design |
| **FR-031** | Vụ án Kanji												| P2		 | CaseController, CaseService, RoomCaseService					 | `Cases`, `CaseChapters`, `RoomCaseProgress`																| `TC-GAME-031-01` 							| Pending Design |
| **FR-032** | Shadowing Voice Battle									| P2		 | ShadowingController, ShadowingScorer, RoomShadowingService	 | `ShadowingClips`, `ShadowingSubmissions`, `GameSessions`													| `TC-GAME-032-01` 							| Pending Design |
| **FR-033** | Hồ sơ người chơi và cài đặt riêng tư xã hội				| P1		 | SocialSettingsController, ProfileService						 | `GameProfiles`, `SocialSettings`, `Users`																| `TC-SOC-033-01` 							| Pending Design |
| **FR-034** | Kết bạn													| P1		 | FriendController, FriendService								 | `Friends`, `FriendRequests`, `BlockedUsers`																| `TC-SOC-034-01`, `TC-SOC-034-02`			| Pending Design |
| **FR-035** | Nhắn tin riêng giữa bạn bè								| P1		 | MessagingController, MessagingService						 | `Conversations`, `Messages`																				| `TC-SOC-035-01` 							| Pending Design |
| **FR-036** | Lời mời thi đấu 1 vs 1									| P1		 | GameInvitationController, InvitationService					 | `GameInvitations`, `Matches`, `MatchPlayers`																| `TC-SOC-036-01` 							| Pending Design |
| **FR-037** | Phòng chơi nhiều người									| P2		 | RoomController, RoomService									 | `GameRooms`, `RoomPlayers`, `RoomQuickMessages`, `RoomCaseProgress`										| `TC-SOC-037-01`, `TC-SOC-037-02`			| Pending Design |
| **FR-038** | Thông báo xã hội											| P1		 | NotificationController, NotificationService, PushDispatcherJob| `Notifications`, `UserDevices`																			| `TC-SOC-038-01` 							| Pending Design |
| **FR-039** | An toàn người dùng: chặn, báo cáo và hạn chế				| P0		 | ModerationController, FriendService, ModerationService		 | `BlockedUsers`, `Reports`, `SocialSettings`																| `TC-SOC-039-01` 							| Pending Design |

## 3. Ma trận truy vết yêu cầu phi chức năng 

| Mã NFR	  | Phân loại Yêu cầu			| Mục tiêu																		| Giải pháp kiến trúc													| Kiểm thử												| Trạng thái	 |
|---|---|---|---|---|---|
| **NFR-001** | Hiệu năng					| API Latency < 300ms,  App load < 1.5s											| Redis Caching, Asynchronous Query, DB Indexing						| k6 Load Testing, BenchmarkDotNet						| Pending Design |
| **NFR-002** | Bảo mật						| Hash BCrypt/Argon2, JWT Bearer, RBAC Policies									| ASP.NET Core Identity, JWT Bearer Middleware, Policy-based Auth		| OWASP ZAP Scan, Unit Test Policies					| Pending Design |
| **NFR-003** | Khả năng ngoại tuyến		| hàng đợi cục bộ, tự động đồng bộ khi có mạng									| SQLite, Offline Sync Queue Service									| Mobile Integration Test								| Pending Design |
| **NFR-004** | Truy vết an ninh			| 100% thao tác Admin được lưu vết, Append-only DB								| AuditLog ActionFilter / Middleware, DB Constraint						| Integration Test Audit Interceptor					| Pending Design |
| **NFR-005** | Trải nghiệm tự do			| Cho phép Skip Placement, không ép buộc luồng cứng								| Loose Coupling UI Flow, Optional API Parameters						| UX Functional Testing									| Pending Design |
| **NFR-006** | Bảo mật & Chống gian lận	| Chống gian lận điểm số và event trò chơi real-time							| Server-side Game Engine Verification, GameSession Validation			| Unit Test Scoring Engine, Event Fuzzing				| Pending Design |
| **NFR-007** | Hiệu năng trò chơi			| đọc bảng xếp hạng p95 <300ms, tạo ván p95 <500ms								| bảng LeaderboardEntries + index, Redis là tùy chọn					| k6 Leaderboard Concurrency Test						| Pending Design |
| **NFR-008** | Quyền riêng tư				| Tuân thủ an toàn dữ liệu cá nhân & dữ liệu giọng nói							| Data Encryption at Rest/Transit, Privacy Filter, GDPR/JPPA Compliance | Privacy Audit Check, Static Security Scan				| Pending Design |
| **NFR-009** | Chơi ngoại tuyến			| Hỗ trợ chơi Offline cho mini-games 1 người									| Local Question Generator Engine, SQLite Storage						| Mobile Offline Game Test								| Pending Design |
| **NFR-010** | Hiệu năng mạng				| 95% tin nhắn hiển thị ≤3 giây khi cả hai online, API danh sách p95 <300ms		| WebSocket / SignalR Core Hub, FCM Push Notification Dispatcher		| SignalR Stress Testing, Push Delivery Test			| Pending Design |
| **NFR-011** | An toàn nội dung			| Kiểm duyệt nội dung do người dùng tải lên/nhắn tin							| Auto-filtering Bad Words, Moderation Queue Service					| Content Abuse Test, Integration Test Safety Filter	| Pending Design |
| **NFR-012** | Ghi công nguồn dữ liệu		| Màn hình Giới thiệu và ghi công + `ATTRIBUTION.md`							| TC-NFR-012 |


## 4. AC → Test Case

| Mã yêu cầu | Mã AC | Nội dung rút gọn																												| Mã test dự kiến | Loại test | Trạng thái | Link bằng chứng |
| :--- | :---: | :--- | :--- | :---: | :---: | :---: |
| **FR-001** | AC-01 | Đăng nhập đúng Email & Mật khẩu trả về JWT Access Token hợp lệ và vào màn hình chính											| `TC-FR001-AC01` | Integration | Chưa viết | *G4* |
| **FR-001** | AC-02 | Đăng nhập sai mật khẩu trả về mã lỗi `401 Unauthorized` kèm thông báo phù hợp												| `TC-FR001-AC02` | Integration | Chưa viết | *G4* |
| **FR-002** | AC-01 | Hoàn thành chuỗi câu hỏi Placement Test, hệ thống tính điểm và đề xuất JLPT Level tương ứng									| `TC-FR002-AC01` | Integration | Chưa viết | *G4* |
| **FR-002** | AC-02 | Chọn "Bỏ qua / Bắt đầu từ đầu" thiết lập trình độ mặc định N5 và khởi tạo hàng đợi học N5									| `TC-FR002-AC02` | Integration | Chưa viết | *G4* |
| **FR-002** | AC-03 | Chọn "Tôi đã biết trình độ của mình" và chọn cấp độ thì lưu trình độ và mở khóa Kanji tương ứng								| `TC-FR002-AC03` | Integration | Chưa viết | *G4* |
| **FR-003** | AC-01 | Kanji trạng thái `Review` chọn "Nhớ" `Rating >= 3` tính `Interval` mới = `Interval` cũ × `EaseFactor`						| `TC-FR003-AC01` | Unit | Chưa viết | *G4* |
| **FR-003** | AC-02 | Kanji chọn "Quên" `Rating < 3` reset `Interval` về 1 ngày và đưa lại vào hàng đợi cùng ngày									| `TC-FR003-AC02` | Unit | Chưa viết | *G4* |
| **FR-003** | AC-03 | Hoàn thành 1 lượt chấm điểm ghi đúng 1 dòng vào `ReviewLogs` `Rating`, `Interval`, `EaseFactor`, `TimeTakenMs`				| `TC-FR003-AC03` | Integration | Chưa viết | *G4* |
| **FR-003** | AC-04 | Kanji có `Interval >= 21` ngày và chuỗi Nhớ liên tục `>= 3` lần được gắn trạng thái `Mastered`								| `TC-FR003-AC04` | Unit | Chưa viết | *G4* |
| **FR-003** | AC-05 | Bài ôn trễ N ngày vẫn bấm "Nhớ" `Rating >= 3` tính `Interval` tiếp theo theo `Interval` cũ + N × `EaseFactor`				| `TC-FR003-AC05` | Unit | Chưa viết | *G4* |
| **FR-004** | AC-01 | Màn hình chính hiển thị chính xác `NewCardsCount` và `ReviewCardsCount` theo mục tiêu ngày của người dùng					| `TC-FR004-AC01` | Integration | Chưa viết | *G4* |
| **FR-004** | AC-02 | Hoàn thành 1 thẻ học/ôn tập thì số thẻ hàng đợi ngày giảm đi 1 và thanh tiến độ mục tiêu cập nhật							| `TC-FR004-AC02` | UI | Chưa viết | *G4* |
| **FR-004** | AC-03 | Sang ngày mới nạp hàng đợi theo thứ tự: Quên/quá hạn ngày cũ → Ôn đúng hạn hôm nay → Từ mới theo hạn mức						| `TC-FR004-AC03` | Unit | Chưa viết | *G4* |
| **FR-005** | AC-01 | Mở thẻ Kanji hiển thị `MnemonicImageUrl`, `StrokeOrderAnimationUrl` và phiên âm Hiragana/Katakana chuẩn						| `TC-FR005-AC01` | UI | Chưa viết | *G4* |
| **FR-005** | AC-02 | Bấm biểu tượng loa trên thẻ phát đúng bản thu âm người bản ngữ đọc từ ghép ví dụ thông dụng									| `TC-FR005-AC02` | UI | Chưa viết | *G4* |
| **FR-006** | AC-01 | Hoàn thành ít nhất 1 phiên học/ôn tập trong ngày tăng `CurrentStreak` thêm 1 và hiện hiệu ứng chúc mừng						| `TC-FR006-AC01` | Integration | Chưa viết | *G4* |
| **FR-006** | AC-02 | Không hoàn thành phiên học/ôn tập nào trong 1 ngày thì sang ngày hôm sau `CurrentStreak` reset về 0							| `TC-FR006-AC02` | Unit | Chưa viết | *G4* |
| **FR-006** | AC-03 | Bỏ lỡ 1 ngày học khi có `StreakFreeze` thì tự động tiêu thụ 1 vật phẩm để giữ nguyên `CurrentStreak` cấp 3/tháng				| `TC-FR006-AC03` | Unit | Chưa viết | *G4* |
| **FR-007** | AC-01 | Chuyển sang "Luyện tập Giao tiếp" mở màn hình nhắn tin theo kịch bản tình huống có từ vựng vừa học							| `TC-FR007-AC01` | UI | Chưa viết | *G4* |
| **FR-007** | AC-02 | Nhập hoặc nói câu phản hồi ngẫu nhiên được phân tích ngữ pháp, phản hồi ngữ cảnh và gợi ý diễn đạt chuẩn						| `TC-FR007-AC02` | Integration | Chưa viết | *G4* |
| **FR-007** | AC-03 | Màn hình hội thoại có nút "Bỏ qua / Hoàn thành bài" để kết thúc bài học mà không bắt buộc chat								| `TC-FR007-AC03` | UI | Chưa viết | *G4* |
| **FR-010** | AC-01 | Admin nhập thông tin Kanji và bấm "Lưu" tạo mới hoặc cập nhật Kanji ở trạng thái `Draft`										| `TC-FR010-AC01` | Integration | Chưa viết | *G4* |
| **FR-010** | AC-02 | Kanji trạng thái `InReview` được Admin bấm "Publish" chuyển sang `Published` và hiển thị cho người học						| `TC-FR010-AC02` | Integration | Chưa viết | *G4* |
| **FR-010** | AC-03 | Cập nhật Kanji đã `Published` tự động lưu bản chụp JSON hiện tại vào `KanjiRevisions` kèm `Version` tăng tiến				| `TC-FR010-AC03` | Integration | Chưa viết | *G4* |
| **FR-011** | AC-01 | Admin khóa tài khoản vi phạm chuyển `Status` sang `Locked` và thu hồi toàn bộ Refresh Token active							| `TC-FR011-AC01` | Integration | Chưa viết | *G4* |
| **FR-011** | AC-02 | Admin gán vai trò `Editor` hoặc `Admin` cập nhật `UserRoles` và áp dụng policy ở lần cấp Token tiếp theo						| `TC-FR011-AC02` | Integration | Chưa viết | *G4* |
| **FR-011** | AC-03 | Admin tự khóa, tự xóa hoặc tự hạ quyền Admin của chính mình bị hệ thống từ chối và báo lỗi nghiệp vụ							| `TC-FR011-AC03` | Integration | Chưa viết | *G4* |
| **FR-012** | AC-01 | Admin xem danh sách Feedback hỗ trợ lọc theo loại `ContentError`, `Bug`, `Suggestion`, trạng thái và liên kết Kanji			| `TC-FR012-AC01` | Integration | Chưa viết | *G4* |
| **FR-012** | AC-02 | Admin nhập `AdminNote` và chuyển Feedback sang `Resolved`/`Rejected` lưu `ResolvedBy` và thời gian cập nhật					| `TC-FR012-AC02` | Integration | Chưa viết | *G4* |
| **FR-013** | AC-01 | Mọi thao tác Create/Update/Delete/Publish/Lock thành công của Admin tự động ghi 1 dòng vào `AuditLogs`						| `TC-FR013-AC01` | Integration | Chưa viết | *G4* |
| **FR-013** | AC-02 | API hệ thống không cung cấp bất kỳ endpoint nào hỗ trợ sửa `PUT`/`PATCH` hoặc xóa `DELETE` `AuditLogs`						| `TC-FR013-AC02` | Integration | Chưa viết | *G4* |
| **FR-014** | AC-01 | Admin tạo câu hỏi Placement với 1 đáp án `IsCorrect` lưu vào `PlacementQuestions` và `PlacementOptions`						| `TC-FR014-AC01` | Integration | Chưa viết | *G4* |
| **FR-014** | AC-02 | Tắt câu hỏi `IsActive = false` làm số câu active của cấp độ xuống dưới mức tối thiểu bị ngăn chặn và báo lỗi					| `TC-FR014-AC02` | Integration | Chưa viết | *G4* |
| **FR-015** | AC-01 | Báo cáo Kanji khó nhớ tính toán từ `ReviewLogs`/`UserKanjiProgress` trả về chữ có tỷ lệ "Quên" cao nhất theo JLPT			| `TC-FR015-AC01` | Integration | Chưa viết | *G4* |
| **FR-015** | AC-02 | Dashboard phân tích truy vấn từ bảng tổng hợp `DailyMetrics` hiển thị DAU và tỷ lệ giữ chân D1/D7/D30						| `TC-FR015-AC02` | Integration | Chưa viết | *G4* |
| **FR-016** | AC-01 | Admin bật/tắt trò chơi áp dụng ngay cho ứng dụng người học mà không cần phát hành lại và ghi `AuditLogs`						| `TC-FR016-AC01` | Integration | Chưa viết | *G4* |
| **FR-016** | AC-02 | Chỉ cho Publish Boss khi cấp độ JLPT đó có đủ số lượng Kanji `Published` có media theo mức tối thiểu							| `TC-FR016-AC02` | Integration | Chưa viết | *G4* |
| **FR-016** | AC-03 | Chỉ cho Publish vụ án Kanji khi mỗi chương đều có đáp án đúng và ít nhất 1 Kanji liên quan									| `TC-FR016-AC03` | Integration | Chưa viết | *G4* |
| **FR-017** | AC-01 | Mục Kiểm duyệt trò chơi liệt kê các ván bị gắn cờ kèm lý do bất thường, hỗ trợ lọc theo trò chơi và người chơi				| `TC-FR017-AC01` | Integration | Chưa viết | *G4* |
| **FR-017** | AC-02 | Loại ván hoặc cấm người chơi khỏi BXH tính lại `RankPoints`, ghi `AuditLogs`, người bị cấm vẫn học bình thường				| `TC-FR017-AC02` | Integration | Chưa viết | *G4* |
| **FR-017** | AC-03 | Xử lý báo cáo biệt danh vi phạm cho phép buộc đổi biệt danh hoặc khóa tài khoản theo FR-011 và lưu vết						| `TC-FR017-AC03` | Integration | Chưa viết | *G4* |
| **FR-018** | AC-01 | Báo cáo xã hội hiển thị bằng chứng chụp lúc báo cáo tin bị báo cáo + tin lân cận, không lộ toàn bộ hội thoại					| `TC-FR018-AC01` | Integration | Chưa viết | *G4* |
| **FR-018** | AC-02 | Xử lý báo cáo tạo `UserSanctions` khi hạn chế, ghi `AuditLogs`, lưu người xử lý/thời gian và gửi thông báo theo mẫu			| `TC-FR018-AC02` | Integration | Chưa viết | *G4* |
| **FR-018** | AC-03 | Admin gỡ hạn chế còn hiệu lực áp dụng ngay lập tức, người dùng gửi được tin nhắn trở lại và ghi `AuditLogs`					| `TC-FR018-AC03` | Integration | Chưa viết | *G4* |
| **FR-020** | AC-01 | Tab Đấu trường hiển thị danh sách trò chơi đang bật chia theo 4 nhóm kèm lựa chọn cấp độ N5–N1								| `TC-FR020-AC01` | UI | Chưa viết | *G4* |
| **FR-020** | AC-02 | Chọn trò chơi ở cấp độ thiếu Kanji `Published` đủ media không cho bắt đầu, gợi ý cấp độ khác, không chặn học					| `TC-FR020-AC02` | Integration | Chưa viết | *G4* |
| **FR-020** | AC-03 | Chế độ "Luyện tập" chạy solo offline không tính hạng, chế độ "Đấu hạng" bắt buộc có kết nối mạng								| `TC-FR020-AC03` | Integration | Chưa viết | *G4* |
| **FR-020** | AC-04 | Kết thúc ván gợi ý đưa các chữ sai `>= 2` lần vào hàng đợi ôn hôm sau, không ghi `ReviewLogs` hay đổi SM-2					| `TC-FR020-AC04` | Integration | Chưa viết | *G4* |
| **FR-021** | AC-01 | Ghép đấu hạng trả về `MatchId` và bộ câu hỏi cố định theo `Seed`, hai người chơi nhận đúng cùng một bộ câu hỏi				| `TC-FR021-AC01` | Integration | Chưa viết | *G4* |
| **FR-021** | AC-02 | Hoàn thành trước đặt trận `WaitingOpponent`, quá thời hạn ghép với bản ghi ván ghost cùng bộ câu hỏi							| `TC-FR021-AC02` | Integration | Chưa viết | *G4* |
| **FR-021** | AC-03 | Cả hai hoàn thành thì so điểm và cập nhật `RankPoints` người chơi thật trong 1 Transaction, ghost không đổi hạng				| `TC-FR021-AC03` | Integration | Chưa viết | *G4* |
| **FR-021** | AC-04 | Thoát ván giữa chừng quá thời gian quy định thì đóng ván trạng thái `Abandoned`, xử thua và không treo trận					| `TC-FR021-AC04` | Integration | Chưa viết | *G4* |
| **FR-021** | AC-05 | Đấu với bạn bè dùng lời mời theo FR-036 tạo trận giao hữu dùng chung `Seed` bộ câu hỏi cho cả hai người						| `TC-FR021-AC05` | Integration | Chưa viết | *G4* |
| **FR-022** | AC-01 | Bảng xếp hạng hiển thị Top 100 và vị trí học viên đọc từ bảng tổng hợp `LeaderboardEntries` theo mùa/cấp độ					| `TC-FR022-AC01` | Integration | Chưa viết | *G4* |
| **FR-022** | AC-02 | Kết thúc mùa giải theo `Seasons.EndsAt` lưu kết quả cuối mùa, trao huy hiệu và mở mùa mới với hạng khởi tạo					| `TC-FR022-AC02` | Integration | Chưa viết | *G4* |
| **FR-022** | AC-03 | Lần đầu đấu hạng yêu cầu đặt biệt danh duy nhất qua bộ lọc từ cấm, người khác chỉ thấy biệt danh và hạng						| `TC-FR022-AC03` | Integration | Chưa viết | *G4* |
| **FR-022** | AC-04 | Bật tùy chọn ẩn khỏi BXH vẫn cho thi đấu và tính hạng nhưng không hiển thị học viên trên bảng công khai						| `TC-FR022-AC04` | Integration | Chưa viết | *G4* |
| **FR-023** | AC-01 | Karuta bày N thẻ Kanji lên bàn và đưa gợi ý theo độ khó để học viên chạm chọn thẻ khớp gợi ý									| `TC-FR023-AC01` | UI | Chưa viết | *G4* |
| **FR-023** | AC-02 | Chạm đúng cộng điểm + thưởng tốc độ theo giờ Server và rút thẻ, chạm sai trừ điểm và khóa thẻ 1 giây							| `TC-FR023-AC02` | Unit | Chưa viết | *G4* |
| **FR-023** | AC-03 | Hết thẻ/hết giờ tổng kết số lượt đúng, thời gian phản ứng TB, chữ sai và gửi kết quả theo FR-021 nếu đấu hạng				| `TC-FR023-AC03` | Integration | Chưa viết | *G4* |
| **FR-024** | AC-01 | Shiritori hiển thị từ hiện tại và các thẻ lựa chọn bắt đầu bằng kana cuối của âm đọc từ hiện tại								| `TC-FR024-AC01` | UI | Chưa viết | *G4* |
| **FR-024** | AC-02 | Chọn thẻ sai luật trừ 1 mạng mặc định 3 mạng và cho chọn lại, hết mạng kết thúc ván											| `TC-FR024-AC02` | Unit | Chưa viết | *G4* |
| **FR-024** | AC-03 | Chuẩn hóa âm đọc khi so khớp trên Server và tính điểm bằng độ dài chuỗi nối hợp lệ											| `TC-FR024-AC03` | Unit | Chưa viết | *G4* |
| **FR-025** | AC-01 | Menko bày lưới thẻ úp gồm các cặp và cho lật từng thẻ bằng thao tác vuốt nhanh có hiệu ứng lật								| `TC-FR025-AC01` | UI | Chưa viết | *G4* |
| **FR-025** | AC-02 | Lật 2 thẻ khớp cặp giữ ngửa, cộng điểm và tăng combo, không khớp úp lại sau 1 giây và reset combo về 0						| `TC-FR025-AC02` | Unit | Chưa viết | *G4* |
| **FR-025** | AC-03 | Lật hết cặp/hết giờ tổng kết điểm, số lượt lật, chữ nhầm và gửi kết quả theo FR-021 nếu đấu hạng								| `TC-FR025-AC03` | Integration | Chưa viết | *G4* |
| **FR-026** | AC-01 | Hanafuda chia bài trên tay và mặt bàn theo `Seed`, mỗi thẻ thuộc 1 trong 12 nhóm do Admin định nghĩa							| `TC-FR026-AC01` | Unit | Chưa viết | *G4* |
| **FR-026** | AC-02 | Ra thẻ cùng nhóm với thẻ trên bàn thu cả hai về kho điểm, không cùng nhóm thì đặt thẻ lên mặt bàn							| `TC-FR026-AC02` | Unit | Chưa viết | *G4* |
| **FR-026** | AC-03 | Thu đủ bộ cộng điểm thưởng bộ, hết bài tổng kết điểm và đối chiếu kết quả theo FR-021 không đồng thời						| `TC-FR026-AC03` | Integration | Chưa viết | *G4* |
| **FR-027** | AC-01 | Kingyo-sukui hiển thị bể cá mang Kanji bơi chậm kèm yêu cầu để học viên kéo vợt vớt cá đúng									| `TC-FR027-AC01` | UI | Chưa viết | *G4* |
| **FR-027** | AC-02 | Vớt đúng cá cộng điểm và đưa vào chén, vớt sai giảm độ bền vợt, về 0 thì rách và mất 1 chiếc vợt								| `TC-FR027-AC02` | Unit | Chưa viết | *G4* |
| **FR-027** | AC-03 | Hết giờ hoặc hết vợt kết thúc ván, tổng kết cá đúng/sai và gửi kết quả theo FR-021 nếu đấu hạng								| `TC-FR027-AC03` | Integration | Chưa viết | *G4* |
| **FR-028** | AC-01 | Tobe! Geta cho nhân vật chạy tự động, hiển thị yêu cầu và 2–3 đôi Geta để chạm nhảy vào chữ đúng								| `TC-FR028-AC01` | UI | Chưa viết | *G4* |
| **FR-028** | AC-02 | Nhảy đúng Geta cộng điểm và tăng quãng đường, nhảy sai hoặc không kịp rơi mất 1 mạng mặc định 3 mạng							| `TC-FR028-AC02` | Unit | Chưa viết | *G4* |
| **FR-028** | AC-03 | Tốc độ tăng dần theo quãng đường, hết mạng kết thúc ván và tính điểm từ nhật ký sự kiện Server theo NFR-006					| `TC-FR028-AC03` | Integration | Chưa viết | *G4* |
| **FR-029** | AC-01 | Kendama hiển thị dãy chữ Kanji và gợi ý để học viên chọn 2 chữ ghép thành từ ghép đúng										| `TC-FR029-AC01` | UI | Chưa viết | *G4* |
| **FR-029** | AC-02 | Ghép đúng từ ghép trong dữ liệu tăng combo và nhân hệ số điểm, ghép sai rơi bóng và reset combo về 0							| `TC-FR029-AC02` | Unit | Chưa viết | *G4* |
| **FR-029** | AC-03 | Hết giờ hoặc rơi bóng tối đa 3 lần tổng kết combo dài nhất, điểm và gửi kết quả theo FR-021 nếu đấu hạng						| `TC-FR029-AC03` | Integration | Chưa viết | *G4* |
| **FR-030** | AC-01 | Boss Raid hiển thị tên Boss, `CurrentHp`, `MaxHp`, thời gian còn lại và số vé tấn công còn lại theo cấp độ					| `TC-FR030-AC01` | Integration | Chưa viết | *G4* |
| **FR-030** | AC-02 | Dùng 1 vé hoàn thành lượt đánh tính sát thương trên Server theo câu đúng + tốc độ và trừ vào `CurrentHp`						| `TC-FR030-AC02` | Integration | Chưa viết | *G4* |
| **FR-030** | AC-03 | Nhiều học viên đánh đồng thời cập nhật máu Boss nguyên tử, không âm máu và không mất sát thương								| `TC-FR030-AC03` | Integration | Chưa viết | *G4* |
| **FR-030** | AC-04 | Boss bị hạ hoặc hết hạn chốt trạng thái và trao thưởng đúng 1 lần cho người đạt ngưỡng sát thương tối thiểu					| `TC-FR030-AC04` | Integration | Chưa viết | *G4* |
| **FR-030** | AC-05 | Tab Bạn bè của Boss Raid xếp hạng sát thương giảm dần của bản thân và bạn bè, không lộ người lạ								| `TC-FR030-AC05` | Integration | Chưa viết | *G4* |
| **FR-031** | AC-01 | Vụ án Kanji mở khóa tuần tự từng chương và chỉ cho sang chương tiếp theo khi giải đúng chương hiện tại						| `TC-FR031-AC01` | Integration | Chưa viết | *G4* |
| **FR-031** | AC-02 | Giải manh mối kiểm tra đáp án phía Server, báo đúng/sai và chuyển diễn biến câu chuyện khi đúng								| `TC-FR031-AC02` | Integration | Chưa viết | *G4* |
| **FR-031** | AC-03 | Dùng tối đa 3 gợi ý/chương, hoàn thành vụ án tính số sao theo gợi ý & thời gian và xếp hạng trên BXH vụ án					| `TC-FR031-AC03` | Unit | Chưa viết | *G4* |
| **FR-031** | AC-04 | Chơi đồng đội chuyển sang phòng nhiều người theo FR-037 dùng chung nội dung vụ án, chơi đơn giữ nguyên AC-01..03				| `TC-FR031-AC04` | Integration | Chưa viết | *G4* |
| **FR-032** | AC-01 | Lần đầu vào Shadowing xin quyền micro kèm giải thích, từ chối vẫn dùng chức năng khác và hiện hướng dẫn bật lại				| `TC-FR032-AC01` | UI | Chưa viết | *G4* |
| **FR-032** | AC-02 | Đọc nhại theo âm thanh chuẩn trả về điểm phát âm 0–100 kèm các chữ đọc sai so với văn bản và phiên âm chuẩn					| `TC-FR032-AC02` | Integration | Chưa viết | *G4* |
| **FR-032** | AC-03 | Sau khi chấm xong không lưu trữ bản ghi âm, chỉ lưu điểm, văn bản nhận dạng và phiên bản bộ nhận dạng						| `TC-FR032-AC03` | Integration | Chưa viết | *G4* |
| **FR-032** | AC-04 | Đấu hạng Shadowing áp dụng FR-021 với cùng bộ đoạn âm thanh cho hai người chơi và so tổng điểm phát âm						| `TC-FR032-AC04` | Integration | Chưa viết | *G4* |
| **FR-032** | AC-05 | Chơi cùng bạn tạo phòng Shadowing theo FR-037 với cùng bộ đoạn âm thanh, giao hữu và không đổi `RankPoints`					| `TC-FR032-AC05` | Integration | Chưa viết | *G4* |
| **FR-033** | AC-01 | Hiển thị `FriendCode` duy nhất và cho sao chép/chia sẻ, người chưa đặt biệt danh được hướng dẫn đặt trước					| `TC-FR033-AC01` | UI | Chưa viết | *G4* |
| **FR-033** | AC-02 | Đổi cài đặt nhận kết bạn, tin nhắn, lời mời thi đấu lưu và áp dụng ngay cho các yêu cầu phát sinh sau đó						| `TC-FR033-AC02` | Integration | Chưa viết | *G4* |
| **FR-033** | AC-03 | Lần đầu dùng tính năng xã hội hiển thị quy tắc ứng xử & chính sách riêng tư, chỉ tiếp tục khi đồng ý `SocialTermsAcceptedAt` | `TC-FR033-AC03` | Integration | Chưa viết | *G4* |
| **FR-033** | AC-04 | Hồ sơ công khai chỉ trả biệt danh, hạng, huy hiệu và trạng thái quan hệ, không trả email hay tên thật						| `TC-FR033-AC04` | Integration | Chưa viết | *G4* |
| **FR-034** | AC-01 | Tìm bạn theo biệt danh chính xác hoặc `FriendCode` có rate-limit, không hỗ trợ liệt kê hay tìm gần đúng hàng loạt			| `TC-FR034-AC01` | Integration | Chưa viết | *G4* |
| **FR-034** | AC-02 | Gửi lời mời kết bạn tạo trạng thái `Pending` và thông báo, từ chối nếu trùng, bị chặn, tắt nhận lời mời hoặc quá 20/ngày		| `TC-FR034-AC02` | Integration | Chưa viết | *G4* |
| **FR-034** | AC-03 | Chấp nhận tạo quan hệ bạn bè 2 chiều trong 1 Transaction, từ chối/hủy/quá hạn 14 ngày đóng lời mời không tạo bạn				| `TC-FR034-AC03` | Integration | Chưa viết | *G4* |
| **FR-034** | AC-04 | Xóa bạn kết thúc quan hệ, chặn gửi tin/thách đấu mới và tự động hủy các lời mời thi đấu đang chờ giữa hai người				| `TC-FR034-AC04` | Integration | Chưa viết | *G4* |
| **FR-034** | AC-05 | Danh sách bạn hiển thị biệt danh, hạng, huy hiệu, số tin chưa đọc, nút nhắn tin/thách đấu có phân trang và giới hạn tối đa	| `TC-FR034-AC05` | Integration | Chưa viết | *G4* |
| **FR-035** | AC-01 | Gửi tin nhắn văn bản lưu với `ClientMessageId` duy nhất idempotent không trùng tin và hiển thị theo NFR-010					| `TC-FR035-AC01` | Integration | Chưa viết | *G4* |
| **FR-035** | AC-02 | Từ chối gửi tin cho người lạ, người chặn/bị chặn, người tắt nhận tin hoặc khi bản thân đang bị hạn chế nhắn tin				| `TC-FR035-AC02` | Integration | Chưa viết | *G4* |
| **FR-035** | AC-03 | Mở hội thoại trả lịch sử tin nhắn phân trang theo thời gian, cập nhật vị trí đã đọc và số tin chưa đọc						| `TC-FR035-AC03` | Integration | Chưa viết | *G4* |
| **FR-035** | AC-04 | Tin nhắn qua bộ lọc từ cấm cơ bản và rate-limit, vượt ngưỡng trả về mã lỗi `429 Too Many Requests`							| `TC-FR035-AC04` | Integration | Chưa viết | *G4* |
| **FR-035** | AC-05 | Xóa tin do chính mình gửi ẩn tin với cả hai bên, tin bị báo cáo trước đó vẫn được giữ làm bằng chứng							| `TC-FR035-AC05` | Integration | Chưa viết | *G4* |
| **FR-036** | AC-01 | Gửi lời mời thi đấu 1v1 kiểm tra trò chơi bật, đủ `MinKanjiRequired`, người nhận cho phép và tạo lời mời `Pending`			| `TC-FR036-AC01` | Integration | Chưa viết | *G4* |
| **FR-036** | AC-02 | Chấp nhận tạo trận giao hữu cùng `Seed` cho hai người chơi bất đồng bộ, từ chối/hủy/hết hạn không tạo trận					| `TC-FR036-AC02` | Integration | Chưa viết | *G4* |
| **FR-036** | AC-03 | Hoàn thành hoặc hết hạn so điểm Server, xử thua bên chưa chơi theo, cập nhật tỷ số bạn bè, không đổi `RankPoints`			| `TC-FR036-AC03` | Integration | Chưa viết | *G4* |
| **FR-036** | AC-04 | Giới hạn số lời mời đang chờ tới cùng 1 người và tổng số lời mời gửi mỗi ngày, vượt ngưỡng từ chối							| `TC-FR036-AC04` | Integration | Chưa viết | *G4* |
| **FR-036** | AC-05 | Bấm "Đấu lại" trên màn hình kết quả trận giao hữu tạo lời mời mới với cùng trò chơi, cấp độ và người bạn đó					| `TC-FR036-AC05` | Integration | Chưa viết | *G4* |
| **FR-037** | AC-01 | Tạo phòng Vụ án 2–4 người hoặc Shadowing 2–8 người trạng thái `Lobby` kèm mã phòng, mặc định chỉ bạn bè vào được				| `TC-FR037-AC01` | Integration | Chưa viết | *G4* |
| **FR-037** | AC-02 | Vào phòng kiểm tra đầy/bắt đầu/hết hạn/chặn, rời phòng giải phóng chỗ và chuyển quyền chủ phòng khi chủ phòng thoát			| `TC-FR037-AC02` | Integration | Chưa viết | *G4* |
| **FR-037** | AC-03 | Phòng Vụ án đồng bộ chương chung, tối đa 3 gợi ý chung/chương, ghi người giải đầu tiên và tính sao + đóng góp cả nhóm		| `TC-FR037-AC03` | Integration | Chưa viết | *G4* |
| **FR-037** | AC-04 | Phòng Shadowing dùng chung bộ âm thanh theo `Seed`, chốt BXH phòng khi nộp đủ/hết hạn, không lưu ghi âm hay đổi `RankPoints` | `TC-FR037-AC04` | Integration | Chưa viết | *G4* |
| **FR-037** | AC-05 | Phòng không đủ người hoặc không hoạt động quá hạn tự đóng sang `Expired`, giữ phần đã chơi và không treo vô hạn				| `TC-FR037-AC05` | Integration | Chưa viết | *G4* |
| **FR-037** | AC-06 | Trong phòng chỉ dùng được danh sách lời nhắn nhanh cố định, không có ô nhập tin nhắn tự do									| `TC-FR037-AC06` | UI | Chưa viết | *G4* |
| **FR-038** | AC-01 | Tạo thông báo cho các sự kiện xã hội, hiển thị trung tâm thông báo với số chưa đọc và hỗ trợ đánh dấu đã đọc					| `TC-FR038-AC01` | Integration | Chưa viết | *G4* |
| **FR-038** | AC-02 | App đang mở hiện thông báo đúng hạn NFR-010, app đóng gửi Push Notification nếu cấp quyền, từ chối quyền vẫn xem in-app		| `TC-FR038-AC02` | Integration | Chưa viết | *G4* |
| **FR-038** | AC-03 | Tôn trọng cài đặt tắt từng loại thông báo và không gửi thông báo từ hoặc về người đã bị chặn									| `TC-FR038-AC03` | Integration | Chưa viết | *G4* |
| **FR-038** | AC-04 | Thông báo đẩy mặc định chỉ ghi "Bạn có tin nhắn mới", không hiển thị nội dung tin nhắn trên màn hình khóa					| `TC-FR038-AC04` | Integration | Chưa viết | *G4* |
| **FR-039** | AC-01 | Chặn người dùng hủy bạn bè, hủy lời mời chờ và chặn tương tác 2 chiều âm thầm, bỏ chặn không tự khôi phục bạn bè				| `TC-FR039-AC01` | Integration | Chưa viết | *G4* |
| **FR-039** | AC-02 | Báo cáo tin nhắn/người dùng/trận lưu kèm bằng chứng chụp tại thời điểm báo cáo và chống báo cáo trùng lặp					| `TC-FR039-AC02` | Integration | Chưa viết | *G4* |
| **FR-039** | AC-03 | Bị báo cáo từ nhiều người vượt ngưỡng tự động tạo hạn chế tạm thời `UserSanctions` chờ Admin duyệt và ghi `AuditLogs`		| `TC-FR039-AC03` | Integration | Chưa viết | *G4* |
| **FR-039** | AC-04 | Biệt danh và tin nhắn qua bộ lọc từ cấm, tái phạm nhiều lần trong khoảng thời gian cấu hình bị hạn chế nhắn tin tạm thời		| `TC-FR039-AC04` | Integration | Chưa viết | *G4* |
| **FR-039** | AC-05 | Từ chối lời mời kết bạn, tin nhắn hoặc thách đấu khi người nhận đã tắt mà không tiết lộ cài đặt riêng tư cụ thể				| `TC-FR039-AC05` | Integration | Chưa viết | *G4* |
| **NFR-001** | NFR-001.1 | Thời gian phản hồi API đọc/truy vấn đạt p95 `< 300ms` trong điều kiện tải bình thường									| `TC-NFR001-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-001** | NFR-001.2 | Thời gian xử lý chấm điểm SM-2 và ghi nhận tiến độ không quá `500ms`													| `TC-NFR001-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-001** | NFR-001.3 | Thời gian tải trang màn hình chính trên ứng dụng Mobile không quá `1.5 giây`											| `TC-NFR001-AC03` | Phi chức năng | Chưa viết | *G4* |
| **NFR-002** | NFR-002.1 | Mật khẩu người dùng bắt buộc mã hóa bằng thuật toán băm an toàn trước khi lưu DB, không lưu plain text					| `TC-NFR002-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-002** | NFR-002.2 | Xác thực API dùng JWT Bearer Token thời hạn ngắn, Refresh Token lưu băm `token_hash` và thu hồi khi khóa tài khoản		| `TC-NFR002-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-002** | NFR-002.3 | Các endpoint `/api/v1/admin/...` bắt buộc kiểm tra Policy `EditorOrAdmin` hoặc `AdminOnly`, trả về `403 Forbidden` nếu trái phép | `TC-NFR002-AC03` | Phi chức năng | Chưa viết | *G4* |
| **NFR-003** | NFR-003.1 | Ứng dụng Mobile lưu bộ đệm hàng đợi bài học trong ngày vào CSDL cục bộ để học offline									| `TC-NFR003-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-003** | NFR-003.2 | Nhật ký ôn tập `ReviewLogs` offline tự động đồng bộ idempotent lên Server khi có kết nối Internet trở lại				| `TC-NFR003-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-004** | NFR-004.1 | 100% thao tác Create/Update/Delete/Publish/Lock trên khu vực Admin ghi 1 bản ghi `AuditLogs` trong cùng Transaction		| `TC-NFR004-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-004** | NFR-004.2 | Bảng `AuditLogs` thiết lập cơ chế chỉ thêm append-only, không cung cấp API sửa hoặc xóa nhật ký audit					| `TC-NFR004-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-005** | NFR-005.1 | Cho phép người học tự do chọn cấp độ bắt đầu hoặc bỏ qua bài test đầu vào mà không bị ép buộc							| `TC-NFR005-AC01` | UI | Chưa viết | *G4* |
| **NFR-005** | NFR-005.2 | Màn hình luyện tập giao tiếp cho phép nhập văn bản tự do và có nút "Bỏ qua" để kết thúc bài học nhanh					| `TC-NFR005-AC02` | UI | Chưa viết | *G4* |
| **NFR-006** | NFR-006.1 | Điểm của mọi ván do Server tính từ nhật ký sự kiện, bỏ qua điểm số do ứng dụng client tự gửi lên						| `TC-NFR006-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-006** | NFR-006.2 | Server đo thời gian phản ứng giữa mốc gửi câu hỏi và nhận đáp án, gắn cờ các ván có thời gian phản ứng bất thường		| `TC-NFR006-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-006** | NFR-006.3 | Mỗi sự kiện có `ClientEventId` duy nhất, gửi lặp lại cùng một sự kiện không cộng điểm hai lần							| `TC-NFR006-AC03` | Phi chức năng | Chưa viết | *G4* |
| **NFR-006** | NFR-006.4 | Giới hạn tần suất tạo ván và tạo trận mỗi người dùng, vượt ngưỡng trả về `429 Too Many Requests`						| `TC-NFR006-AC04` | Phi chức năng | Chưa viết | *G4* |
| **NFR-007** | NFR-007.1 | Thời gian phản hồi API tạo ván và lấy bộ câu hỏi đạt p95 `< 500ms`														| `TC-NFR007-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-007** | NFR-007.2 | Thời gian phản hồi API đọc bảng xếp hạng đạt p95 `< 300ms`																| `TC-NFR007-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-007** | NFR-007.3 | Trò chơi chuyển động Kingyo-sukui, Tobe! Geta, Kendama duy trì tối thiểu `30 FPS` trên thiết bị Android tầm trung		| `TC-NFR007-AC03` | Phi chức năng | Chưa viết | *G4* |
| **NFR-008** | NFR-008.1 | Người không phải bạn bè chỉ thấy biệt danh, hạng, huy hiệu, không API nào trả email hoặc tên thật người chơi khác		| `TC-NFR008-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-008** | NFR-008.2 | Chỉ cho nhắn tin văn bản thuần giữa bạn bè không chặn nhau, không chat công cộng, bình luận hay gửi ảnh/liên kết		| `TC-NFR008-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-008** | NFR-008.3 | Âm thanh ghi âm của Shadowing không được lưu trữ hay ghi nhật ký, chỉ gửi văn bản nhận dạng hoặc điểm					| `TC-NFR008-AC03` | Phi chức năng | Chưa viết | *G4* |
| **NFR-008** | NFR-008.4 | Biệt danh và tin nhắn được kiểm tra qua bộ lọc từ cấm cơ bản, có chức năng chặn và báo cáo								| `TC-NFR008-AC04` | Phi chức năng | Chưa viết | *G4* |
| **NFR-008** | NFR-008.5 | Job nền xóa tin nhắn hết hạn giữ tin bị báo cáo đến khi xử lý xong và xóa/ẩn danh tin khi xóa tài khoản					| `TC-NFR008-AC05` | Phi chức năng | Chưa viết | *G4* |
| **NFR-008** | NFR-008.6 | Nội dung tin nhắn không được ghi vào nhật ký ứng dụng hay gửi tới dịch vụ phân tích/báo lỗi tự động						| `TC-NFR008-AC06` | Phi chức năng | Chưa viết | *G4* |
| **NFR-008** | NFR-008.7 | Thông báo đẩy mặc định không chứa nội dung tin nhắn																		| `TC-NFR008-AC07` | Phi chức năng | Chưa viết | *G4* |
| **NFR-009** | NFR-009.1 | Chế độ Luyện tập của các trò chơi chạy được ngoại tuyến bằng bộ nội dung đã tải xuống									| `TC-NFR009-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-009** | NFR-009.2 | Ván Luyện tập không tính BXH, ván Đấu hạng mất mạng giữ tạm trong thời hạn rồi gửi lại, quá hạn đóng theo FR-021 AC-04	| `TC-NFR009-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-010** | NFR-010.1 | Gửi lại cùng `ClientMessageId` không tạo thêm tin, thứ tự tin trong hội thoại sắp theo thời gian Server					| `TC-NFR010-AC01` | Phi chức năng | Chưa viết | *G4* |
| **NFR-010** | NFR-010.2 | Khi cả hai người đang mở ứng dụng, 95% tin nhắn hiển thị cho người nhận trong vòng `3 giây`								| `TC-NFR010-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-010** | NFR-010.3 | Mất mạng khi gửi đặt tin ở trạng thái chờ gửi trên thiết bị, tự gửi lại khi có mạng và hiển thị rõ trạng thái			| `TC-NFR010-AC03` | Phi chức năng | Chưa viết | *G4* |
| **NFR-010** | NFR-010.4 | Thời gian phản hồi API lấy danh sách hội thoại, lịch sử tin và danh sách bạn đạt p95 `< 300ms`							| `TC-NFR010-AC04` | Phi chức năng | Chưa viết | *G4* |
| **NFR-010** | NFR-010.5 | Giới hạn tần suất gửi tin, gửi lời mời và tìm kiếm người dùng theo mỗi người dùng, vượt ngưỡng trả `429`				| `TC-NFR010-AC05` | Phi chức năng | Chưa viết | *G4* |
| **NFR-011** | NFR-011.1 | Nút Chặn và Báo cáo đặt ngay trong hội thoại và hồ sơ người chơi, tối đa 2 thao tác chạm từ màn hình đó					| `TC-NFR011-AC01` | UI | Chưa viết | *G4* |
| **NFR-011** | NFR-011.2 | Báo cáo ghi nhận kèm bằng chứng cho Admin, mục tiêu xử lý tối thiểu 90% báo cáo trong 72 giờ FR-018 AC-02				| `TC-NFR011-AC02` | Phi chức năng | Chưa viết | *G4* |
| **NFR-011** | NFR-011.3 | Quy tắc ứng xử và thông báo riêng tư về tin nhắn hiển thị và được đồng ý trước lần dùng chức năng xã hội đầu tiên		| `TC-NFR011-AC03` | UI | Chưa viết | *G4* |
| **NFR-011** | NFR-011.4 | Chính sách bảo mật cập nhật nêu rõ dữ liệu tin nhắn, danh sách bạn và thông báo trước khi bật tính năng					| `TC-NFR011-AC04` | Phi chức năng | Chưa viết | *G4* |
| **NFR-011** | NFR-011.5 | Đối chiếu yêu cầu nền tảng phân phối về nội dung người dùng tạo ra & độ tuổi và lưu hồ sơ cổng phát hành				| `TC-NFR011-AC05` | Phi chức năng | Chưa viết | *G4* |
| **NFR-012** | AC-01 | Mở màn hình "Giới thiệu và ghi công" từ menu chính trong tối đa 2 thao tác chạm, nội dung khớp `data/ATTRIBUTION.md`		| `TC-NFR012-AC01` | UI | Chưa viết | *G4* |