# RTM: Requirements Traceability Matrix

| Phiên bản : 1.0 | Ngày : 01-11-2026 | Trạng thái : Frozen | Cổng : R1-G1 | Người duyệt : Nguyễn Tấn Mạnh |

## Ma trận truy vết yêu cầu chức năng

| Mã FR		 | Tên Yêu cầu Chức năng									| Độ ưu tiên | Mô-đun / SDD								 | Bảng Dữ liệu (Database)					 | Test Case ID								| Trạng thái	 |
|---|---|---|---|---|---|
| **FR-001** | Đăng ký và đăng nhập tài khoản							| P0		 | AuthController, AuthService, JwtProvider  | `Users`, `RefreshTokens`					 | `TC-AUTH-001`, `TC-AUTH-002`				| Pending Design |
| **FR-002** | Kiểm tra trình độ đầu vào và chọn trình độ				| P1		 | PlacementController, PlacementService	 | `PlacementQuestions`, `PlacementAttempts` | `TC-PLACE-001`, `TC-PLACE-002`			| Pending Design |
| **FR-003** | Ôn tập Kanji theo thuật toán SM-2 và đánh giá năng lực   | P0	  	 | ReviewController, Sm2EngineService		 | `UserKanjiProgress`, `ReviewLogs`		 | `TC-SM2-001`, `TC-SM2-002`, `TC-SM2-003` | Pending Design |
| **FR-004** | Hàng đợi học hằng ngày và thống kê tiến độ				| P0		 | QueueController, DailyQueueService		 | `DailyQueues`, `UserKanjiProgress`		 | `TC-QUEUE-001`, `TC-QUEUE-002`			| Pending Design |
| **FR-005** | Học từ vựng có hình ảnh minh họa và giọng đọc chuẩn	    | P0		 | FlashcardView , MediaService				 | `Kanji`, `KanjiMedia`					 | `TC-FLASH-001`, `TC-FLASH-002`			| Pending Design |
| **FR-006** | Duy trì chuỗi ngày Học									| P0		 | StreakService, GamificationEngine		 | `UserStreaks`, `StreakHistory`		     | `TC-STRK-001`, `TC-STRK-002`				| Pending Design |
| **FR-007** | Hội thoại tình huống Ngắn và giao tiếp Freestyle			| P1		 | ChatController, RoleplayChatEngine		 | `ChatScenarios`, `ChatHistory`			 | `TC-CHAT-001`, `TC-CHAT-002`				| Pending Design |
| **FR-010** | Quản lý Nội dung Kanji và lịch sử phiên bản				| P0		 | AdminKanjiController, RevisionService	 | `Kanji`, `KanjiRevisions`				 | `TC-ADM-010-01`, `TC-ADM-010-02`			| Pending Design |
| **FR-011** | Quản lý tài khoản user và an toàn						| P0 / P1	 | AdminUserController, IdentityService		 | `Users`, `UserRoles`, `RefreshTokens`     | `TC-ADM-011-01`, `TC-ADM-011-02`			| Pending Design |
| **FR-012** | Tiếp nhận và xử lý báo lỗi								| P0		 | AdminFeedbackController, FeedbackService  | `Feedbacks`, `AdminNotes`				 | `TC-ADM-012-01`							| Pending Design |
| **FR-013** | Truy vết thao tác quản trị							    | P0		 | AuditLogMiddleware, AuditService			 | `AuditLogs`								 | `TC-ADM-013-01`, `TC-ADM-013-02`			| Pending Design |
| **FR-014** | Quản lý Bài kiểm tra Đầu vào							    | P1		 | AdminPlacementController					 | `PlacementQuestions`, `PlacementOptions`  | `TC-ADM-014-01`							| Pending Design |
| **FR-015** | Báo cáo chỉ số và Analytics sản phẩm						| P0 / P1	 | AnalyticsController, MetricsWorkerJob     | `DailyMetrics`, `ReviewLogs`				 | `TC-ADM-015-01`							| Pending Design |

## 3. Ma trận truy vết yêu cầu phi chức năng 

| Mã NFR	  | Phân loại Yêu cầu	 | Mục tiêu											 | Giải pháp kiến trúc												| Kiểm thử								| Trạng thái	 |
|---|---|---|---|---|---|
| **NFR-001** | Hiệu năng			 | API Latency < 300ms,  App load < 1.5s			 | Redis Caching, Asynchronous Query, DB Indexing					| k6 Load Testing, BenchmarkDotNet		| Pending Design |
| **NFR-002** | Bảo mật				 | Hash BCrypt/Argon2, JWT Bearer, RBAC Policies     | ASP.NET Core Identity, JWT Bearer Middleware, Policy-based Auth  | OWASP ZAP Scan, Unit Test Policies	| Pending Design |
| **NFR-003** | Khả năng ngoại tuyến | hàng đợi cục bộ, tự động đồng bộ khi có mạng		 | SQLite, Offline Sync Queue Service							    | Mobile Integration Test				| Pending Design |
| **NFR-004** | Truy vết an ninh	 | 100% thao tác Admin được lưu vết, Append-only DB  | AuditLog ActionFilter / Middleware, DB Constraint				| Integration Test Audit Interceptor	| Pending Design |
| **NFR-005** | Trải nghiệm tự do	 | Cho phép Skip Placement, không ép buộc luồng cứng | Loose Coupling UI Flow, Optional API Parameters					| UX Functional Testing					| Pending Design |
