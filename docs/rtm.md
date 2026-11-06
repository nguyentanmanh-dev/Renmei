# RTM: Requirements Traceability Matrix

| Phiên bản : 1.0 | Ngày : 01-11-2026 | Trạng thái : Frozen | Cổng : R1-G1 | Người duyệt : Nguyễn Tấn Mạnh |

## Ma trận truy vết yêu cầu chức năng

| Mã FR		 | Tên Yêu cầu Chức năng									| Độ ưu tiên | Mô-đun / SDD													 | Bảng Dữ liệu (Database)																				    | Test Case ID								| Trạng thái	 |
|---|---|---|---|---|---|
| **FR-001** | Đăng ký và đăng nhập tài khoản							| P0		 | AuthController, AuthService, JwtProvider					 	 | `Users`, `RefreshTokens`																				    | `TC-AUTH-001`, `TC-AUTH-002`				| Pending Design |
| **FR-002** | Kiểm tra trình độ đầu vào và chọn trình độ				| P1		 | PlacementController, PlacementService						 | `PlacementQuestions`, `PlacementAttempts`															    | `TC-PLACE-001`, `TC-PLACE-002`			| Pending Design |
| **FR-003** | Ôn tập Kanji theo thuật toán SM-2 và đánh giá năng lực   | P0	  	 | ReviewController, Sm2EngineService							 | `UserKanjiProgress`, `ReviewLogs`																		| `TC-SM2-001`, `TC-SM2-002`, `TC-SM2-003`  | Pending Design |
| **FR-004** | Hàng đợi học hằng ngày và thống kê tiến độ				| P0		 | QueueController, DailyQueueService							 | `DailyQueues`, `UserKanjiProgress`																		| `TC-QUEUE-001`, `TC-QUEUE-002`			| Pending Design |
| **FR-005** | Học từ vựng có hình ảnh minh họa và giọng đọc chuẩn	    | P0		 | FlashcardView , MediaService									 | `Kanji`, `KanjiMedia`																					| `TC-FLASH-001`, `TC-FLASH-002`			| Pending Design |
| **FR-006** | Duy trì chuỗi ngày Học									| P0		 | StreakService, GamificationEngine							 | `UserStreaks`, `StreakHistory`																			| `TC-STRK-001`, `TC-STRK-002`				| Pending Design |
| **FR-007** | Hội thoại tình huống Ngắn và giao tiếp Freestyle			| P1		 | ChatController, RoleplayChatEngine							 | `ChatScenarios`, `ChatHistory`																			| `TC-CHAT-001`, `TC-CHAT-002`				| Pending Design |
| **FR-010** | Quản lý Nội dung Kanji và lịch sử phiên bản				| P0		 | AdminKanjiController, RevisionService						 | `Kanji`, `KanjiRevisions`																				| `TC-ADM-010-01`, `TC-ADM-010-02`			| Pending Design |
| **FR-011** | Quản lý tài khoản user và an toàn						| P0 / P1	 | AdminUserController, IdentityService							 | `Users`, `UserRoles`, `RefreshTokens`                                                                    | `TC-ADM-011-01`, `TC-ADM-011-02`			| Pending Design |
| **FR-012** | Tiếp nhận và xử lý báo lỗi								| P0		 | AdminFeedbackController, FeedbackService						 | `Feedbacks`, `AdminNotes`																				| `TC-ADM-012-01`							| Pending Design |
| **FR-013** | Truy vết thao tác quản trị							    | P0		 | AuditLogMiddleware, AuditService								 | `AuditLogs`																								| `TC-ADM-013-01`, `TC-ADM-013-02`			| Pending Design |
| **FR-014** | Quản lý Bài kiểm tra Đầu vào							    | P1		 | AdminPlacementController										 | `PlacementQuestions`, `PlacementOptions`																	| `TC-ADM-014-01`							| Pending Design |
| **FR-015** | Báo cáo chỉ số và Analytics sản phẩm						| P0 / P1	 | AnalyticsController, MetricsWorkerJob						 | `DailyMetrics`, `ReviewLogs`																				| `TC-ADM-015-01`							| Pending Design |
| **FR-016** | Quản lý nội dung và cấu hình trò chơi					| P1		 | GameContentAdminController, GameContentAdminService			 | `GameModes`, `KanjiGroups`, `KanjiGroupItems`, `Bosses`, `CaseFiles`, `CaseChapters`, `ShadowingClips`   | `TC-ADM-016-01`, `TC-ADM-016-02`			| Pending Design |
| **FR-017** | Kiểm duyệt và chống gian lận trò chơi					| P1		 | GameModerationController, GameModerationService				 | `Reports`, `GameSessions`, `GameProfiles`, `AuditLogs`													| `TC-ADM-017-01`							| Pending Design |
| **FR-018** | Kiểm duyệt nội dung xã hội								| P0		 | SocialModerationAdminController, SocialModerationAdminService | `Reports`, `Users`, `AuditLogs`																			| `TC-ADM-018-01`							| Pending Design |
| **FR-020** | Sảnh Đấu trường và chế độ chơi							| P1		 | GameCatalogController, GameCatalogService, GameSessionService | `GameSessions`, `GameProfiles`																			| `TC-GAME-020-01`							| Pending Design |
| **FR-021** | Đấu bất đồng bộ			  								| P1		 | MatchmakingController, MatchmakingService, RatingService		 | `Matches`, `MatchPlayers`, `GameProfiles`, `GameSessions`												| `TC-GAME-021-01`, `TC-GAME-021-02`		| Pending Design |
| **FR-022** | Xếp hạng, mùa giải và hồ sơ người chơi					| P1		 | LeaderboardController, ProfileService, LeaderboardService	 | `GameProfiles`, `DailyMetrics`																			| `TC-GAME-022-01`							| Pending Design |
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

| Mã NFR	  | Phân loại Yêu cầu			| Mục tiêu												| Giải pháp kiến trúc													| Kiểm thử												| Trạng thái	 |
|---|---|---|---|---|---|
| **NFR-001** | Hiệu năng					| API Latency < 300ms,  App load < 1.5s					| Redis Caching, Asynchronous Query, DB Indexing						| k6 Load Testing, BenchmarkDotNet						| Pending Design |
| **NFR-002** | Bảo mật						| Hash BCrypt/Argon2, JWT Bearer, RBAC Policies			| ASP.NET Core Identity, JWT Bearer Middleware, Policy-based Auth		| OWASP ZAP Scan, Unit Test Policies					| Pending Design |
| **NFR-003** | Khả năng ngoại tuyến		| hàng đợi cục bộ, tự động đồng bộ khi có mạng			| SQLite, Offline Sync Queue Service									| Mobile Integration Test								| Pending Design |
| **NFR-004** | Truy vết an ninh			| 100% thao tác Admin được lưu vết, Append-only DB		| AuditLog ActionFilter / Middleware, DB Constraint						| Integration Test Audit Interceptor					| Pending Design |
| **NFR-005** | Trải nghiệm tự do			| Cho phép Skip Placement, không ép buộc luồng cứng		| Loose Coupling UI Flow, Optional API Parameters						| UX Functional Testing									| Pending Design |
| **NFR-006** | Bảo mật & Chống gian lận	| Chống gian lận điểm số và event trò chơi real-time	| Server-side Game Engine Verification, GameSession Validation			| Unit Test Scoring Engine, Event Fuzzing				| Pending Design |
| **NFR-007** | Hiệu năng trò chơi			| Cập nhật bảng xếp hạng real-time latency < 100ms		| Redis Sorted Sets for Leaderboards, Async Worker Jobs					| k6 Leaderboard Concurrency Test						| Pending Design |
| **NFR-008** | Quyền riêng tư				| Tuân thủ an toàn dữ liệu cá nhân & dữ liệu giọng nói	| Data Encryption at Rest/Transit, Privacy Filter, GDPR/JPPA Compliance | Privacy Audit Check, Static Security Scan				| Pending Design |
| **NFR-009** | Chơi ngoại tuyến			| Hỗ trợ chơi Offline cho mini-games 1 người			| Local Question Generator Engine, SQLite Storage						| Mobile Offline Game Test								| Pending Design |
| **NFR-010** | Hiệu năng mạng				| Giao nhận tin nhắn chat & thông báo Push < 500ms		| WebSocket / SignalR Core Hub, FCM Push Notification Dispatcher		| SignalR Stress Testing, Push Delivery Test			| Pending Design |
| **NFR-011** | An toàn nội dung			| Kiểm duyệt nội dung do người dùng tải lên/nhắn tin	| Auto-filtering Bad Words, Moderation Queue Service					| Content Abuse Test, Integration Test Safety Filter	| Pending Design |