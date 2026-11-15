IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RenmeiDb')
BEGIN
    CREATE DATABASE [RenmeiDb];
END
GO
USE [RenmeiDb];
GO

-- XÓA BẢNG THEO THỨ TỰ PHỤ THUỘC KHÓA NGOẠI (CON TRƯỚC, CHA SAU)
DROP TABLE IF EXISTS [audit_logs];
DROP TABLE IF EXISTS [feedbacks];
DROP TABLE IF EXISTS [daily_metrics];
DROP TABLE IF EXISTS [chat_messages];
DROP TABLE IF EXISTS [chat_sessions];
DROP TABLE IF EXISTS [chat_scenarios];
DROP TABLE IF EXISTS [placement_attempt_answers];
DROP TABLE IF EXISTS [placement_attempts];
DROP TABLE IF EXISTS [placement_options];
DROP TABLE IF EXISTS [placement_questions];
DROP TABLE IF EXISTS [daily_queues];
DROP TABLE IF EXISTS [review_logs];
DROP TABLE IF EXISTS [user_kanji_progress];
DROP TABLE IF EXISTS [kanji_revisions];
DROP TABLE IF EXISTS [kanji_media];
DROP TABLE IF EXISTS [kanji];
DROP TABLE IF EXISTS [content_sources];
DROP TABLE IF EXISTS [user_streaks];
DROP TABLE IF EXISTS [refresh_tokens];
DROP TABLE IF EXISTS [user_roles];
DROP TABLE IF EXISTS [roles];
DROP TABLE IF EXISTS [users];
GO

-- ADR-003, ADR-004, learner.yaml
CREATE TABLE [users] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [email] nvarchar(255) NOT NULL,
  [email_confirmed] bit NOT NULL DEFAULT 0,
  [password_hash] nvarchar(255) NOT NULL,
  [full_name] nvarchar(255),
  [current_level] nvarchar(10) NOT NULL DEFAULT 'N5',
  [daily_new_card_goal] int NOT NULL DEFAULT 10,
  [time_zone] nvarchar(100) NOT NULL DEFAULT 'Asia/Ho_Chi_Minh',
  [preferred_study_time] time NULL,
  [status] nvarchar(50) NOT NULL DEFAULT 'Active',
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  [updated_at] datetime2 NULL,
  [deleted_at] datetime2 NULL,
  [row_version] rowversion NOT NULL,
  CONSTRAINT [CK_users_current_level] CHECK ([current_level] IN ('N5', 'N4', 'N3', 'N2', 'N1')),
  CONSTRAINT [CK_users_daily_new_card_goal] CHECK ([daily_new_card_goal] BETWEEN 1 AND 100)
);
GO

CREATE UNIQUE INDEX [UX_users_email_active]
ON [users]([email])
WHERE [deleted_at] IS NULL;
GO

CREATE TABLE [roles] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [name] nvarchar(50) NOT NULL UNIQUE
);
GO

CREATE TABLE [user_roles] (
  [user_id] uniqueidentifier NOT NULL,
  [role_id] int NOT NULL,
  PRIMARY KEY ([user_id], [role_id]),
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE,
  FOREIGN KEY ([role_id]) REFERENCES [roles]([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [refresh_tokens] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [token_hash] nvarchar(128) NOT NULL,
  [token_family_id] uniqueidentifier NOT NULL DEFAULT NEWID(),
  [replaced_by_token_hash] nvarchar(128) NULL,
  [expires_at] datetime2 NOT NULL,
  [revoked_at] datetime2 NULL,
  [revoked_reason] nvarchar(100) NULL,
  [created_by_ip] nvarchar(50) NULL,
  [revoked_by_ip] nvarchar(50) NULL,
  [device_info] nvarchar(255) NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [UX_refresh_tokens_token_hash_active]
ON [refresh_tokens]([token_hash])
WHERE [revoked_at] IS NULL;
GO

CREATE INDEX [IX_refresh_tokens_family_id]
ON [refresh_tokens]([token_family_id]);
GO

CREATE TABLE [user_streaks] (
  [user_id] uniqueidentifier PRIMARY KEY,
  [current_streak] int NOT NULL DEFAULT 0,
  [longest_streak] int NOT NULL DEFAULT 0,
  [freeze_item_count] int NOT NULL DEFAULT 0,
  [last_learned_date] date NULL,
  [updated_at] datetime2 NULL,
  [row_version] rowversion NOT NULL,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE
);
GO

-- ADR-003, ADR-006
CREATE TABLE [content_sources] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [name] nvarchar(200) NOT NULL UNIQUE,
  [usage] nvarchar(200) NOT NULL,
  [license] nvarchar(100) NOT NULL,
  [license_url] nvarchar(500) NULL,
  [attribution_text] nvarchar(max) NOT NULL,
  [url] nvarchar(500) NULL,
  [license_checked_at] date NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE [kanji] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [character] nvarchar(10) NOT NULL,
  [han_viet] nvarchar(100) NOT NULL,
  [jlpt_level] tinyint NOT NULL CHECK ([jlpt_level] BETWEEN 1 AND 5),
  [stroke_count] int NULL,
  [radical] nvarchar(50) NULL,
  [radical_number] int NULL,
  [grade] int NULL,
  [freq] int NULL,
  [meanings_json] nvarchar(max) NULL,
  [onyomi_json] nvarchar(max) NULL,
  [kunyomi_json] nvarchar(max) NULL,
  [examples_json] nvarchar(max) NULL,
  [mnemonic_story] nvarchar(max) NULL,
  [source_id] int NULL,
  [source_ref] nvarchar(200) NULL,
  [status] nvarchar(50) NOT NULL DEFAULT 'Draft',
  [created_by] uniqueidentifier NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  [updated_at] datetime2 NULL,
  [deleted_at] datetime2 NULL,
  [row_version] rowversion NOT NULL,
  FOREIGN KEY ([source_id]) REFERENCES [content_sources]([id]) ON DELETE SET NULL,
  FOREIGN KEY ([created_by]) REFERENCES [users]([id]),
  CONSTRAINT [CK_kanji_status] CHECK ([status] IN ('Draft', 'InReview', 'Published', 'Archived'))
);
GO

CREATE UNIQUE INDEX [UX_kanji_character_active]
ON [kanji]([character])
WHERE [deleted_at] IS NULL;
GO

CREATE TABLE [kanji_media] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [kanji_id] int NOT NULL UNIQUE,
  [mnemonic_image_url] nvarchar(500) NULL,
  [stroke_order_animation_url] nvarchar(500) NULL,
  [audio_url] nvarchar(500) NULL,
  [updated_at] datetime2 NULL,
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [kanji_examples] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [kanji_id] int NOT NULL,
  [sentence_ja] nvarchar(500) NOT NULL,
  [translation] nvarchar(500) NOT NULL,
  [source_id] int NULL,
  [tatoeba_sentence_id] int NULL,
  [tatoeba_owner] nvarchar(100) NULL,
  [tatoeba_license] nvarchar(50) NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]) ON DELETE CASCADE,
  FOREIGN KEY ([source_id]) REFERENCES [content_sources]([id]) ON DELETE SET NULL,
  CONSTRAINT [CK_kanji_examples_tatoeba_license] CHECK (
    [tatoeba_license] IS NULL OR [tatoeba_license] IN ('CC BY 2.0 FR', 'CC0')
  )
);
GO

CREATE TABLE [kanji_revisions] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [kanji_id] int NOT NULL,
  [version] int NOT NULL,
  [content_json] nvarchar(max) NOT NULL,
  [created_by] uniqueidentifier NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]) ON DELETE CASCADE,
  FOREIGN KEY ([created_by]) REFERENCES [users]([id])
);
GO

-- ADR-007, learner
CREATE TABLE [user_kanji_progress] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [kanji_id] int NOT NULL,
  [state] nvarchar(20) NOT NULL DEFAULT 'New',
  [ease_factor] float NOT NULL DEFAULT 2.5,
  [interval] int NOT NULL DEFAULT 0,
  [repetitions] int NOT NULL DEFAULT 0,
  [lapses] int NOT NULL DEFAULT 0,
  [due_date] date NOT NULL,
  [last_reviewed_at] datetime2 NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  [updated_at] datetime2 NULL,
  [row_version] rowversion NOT NULL,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE,
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]) ON DELETE CASCADE,
  CONSTRAINT [CK_user_kanji_progress_state] CHECK ([state] IN ('New', 'Learning', 'Review', 'Relearning', 'Mastered'))
);
GO

CREATE UNIQUE INDEX [UX_user_kanji_progress_user_kanji]
ON [user_kanji_progress]([user_id], [kanji_id]);
GO

CREATE TABLE [review_logs] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [kanji_id] int NOT NULL,
  [client_event_id] uniqueidentifier NOT NULL,
  [rating] int NOT NULL,
  [time_taken_ms] int NULL,
  [interval_before] int NULL,
  [interval_after] int NULL,
  [ease_factor_before] float NULL,
  [ease_factor_after] float NULL,
  [reviewed_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]),
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]),
  CONSTRAINT [CK_review_logs_rating] CHECK ([rating] BETWEEN 0 AND 5),
  CONSTRAINT [CK_review_logs_time_taken_ms] CHECK ([time_taken_ms] IS NULL OR [time_taken_ms] >= 0)
);
GO

CREATE UNIQUE INDEX [UX_review_logs_user_client_event]
ON [review_logs]([user_id], [client_event_id])
WHERE [client_event_id] IS NOT NULL;
GO

CREATE TABLE [daily_queues] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [date] date NOT NULL,
  [new_cards_count] int NOT NULL DEFAULT 0,
  [review_cards_count] int NOT NULL DEFAULT 0,
  [completed_count] int NOT NULL DEFAULT 0,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [UX_daily_queues_user_date]
ON [daily_queues]([user_id], [date]);
GO

-- PLACEMENT TEST, ROLEPLAY CHAT
CREATE TABLE [placement_questions] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [kanji_id] int NULL,
  [content] nvarchar(max) NOT NULL,
  [jlpt_level] tinyint NOT NULL CHECK ([jlpt_level] BETWEEN 1 AND 5),
  [is_active] bit NOT NULL DEFAULT 1,
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]) ON DELETE SET NULL
);
GO

CREATE TABLE [placement_options] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [question_id] int NOT NULL,
  [option_text] nvarchar(max) NOT NULL,
  [is_correct] bit NOT NULL DEFAULT 0,
  FOREIGN KEY ([question_id]) REFERENCES [placement_questions]([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [placement_attempts] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [status] nvarchar(20) NOT NULL DEFAULT 'InProgress',
  [total_questions] int NOT NULL DEFAULT 0,
  [correct_count] int NOT NULL DEFAULT 0,
  [score_percentage] float NULL,
  [recommended_level] nvarchar(10) NULL,
  [started_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  [completed_at] datetime2 NULL,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE,
  CONSTRAINT [CK_placement_attempts_status] CHECK ([status] IN ('InProgress', 'Completed', 'Abandoned'))
);
GO

CREATE TABLE [placement_attempt_answers] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [attempt_id] uniqueidentifier NOT NULL,
  [question_id] int NOT NULL,
  [selected_option_id] int NULL,
  [is_correct] bit NOT NULL DEFAULT 0,
  [answered_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FOREIGN KEY ([attempt_id]) REFERENCES [placement_attempts]([id]) ON DELETE CASCADE,
  FOREIGN KEY ([question_id]) REFERENCES [placement_questions]([id]),
  FOREIGN KEY ([selected_option_id]) REFERENCES [placement_options]([id])
);
GO

CREATE TABLE [chat_scenarios] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [title] nvarchar(255) NOT NULL,
  [jlpt_level] tinyint NOT NULL DEFAULT 5 CHECK ([jlpt_level] BETWEEN 1 AND 5),
  [roleplay_context] nvarchar(max) NOT NULL,
  [suggested_kanji_ids_json] nvarchar(max) NULL,
  [is_active] bit NOT NULL DEFAULT 1
);
GO

CREATE TABLE [chat_sessions] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [scenario_id] int NOT NULL,
  [status] nvarchar(20) NOT NULL DEFAULT 'Active',
  [target_kanji_ids_json] nvarchar(max) NULL,
  [used_kanji_ids_json] nvarchar(max) NULL,
  [feedback_summary_json] nvarchar(max) NULL,
  [started_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  [ended_at] datetime2 NULL,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE,
  FOREIGN KEY ([scenario_id]) REFERENCES [chat_scenarios]([id]),
  CONSTRAINT [CK_chat_sessions_status] CHECK ([status] IN ('Active', 'Completed', 'Abandoned'))
);
GO

CREATE TABLE [chat_messages] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [session_id] uniqueidentifier NOT NULL,
  [sender_role] nvarchar(20) NOT NULL,
  [content] nvarchar(max) NOT NULL,
  [furigana_content] nvarchar(max) NULL,
  [detected_kanji_json] nvarchar(max) NULL,
  [grammar_correction_json] nvarchar(max) NULL,
  [token_count] int NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FOREIGN KEY ([session_id]) REFERENCES [chat_sessions]([id]) ON DELETE CASCADE,
  CONSTRAINT [CK_chat_messages_sender_role] CHECK ([sender_role] IN ('User', 'Assistant', 'System'))
);
GO

--ADMIN SYSTEM, ANALYTICS
CREATE TABLE [audit_logs] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [actor_user_id] uniqueidentifier NULL,
  [action] nvarchar(100) NOT NULL,
  [entity_type] nvarchar(100) NOT NULL,
  [entity_id] nvarchar(100) NOT NULL,
  [details_json] nvarchar(max) NULL,
  [ip_address] nvarchar(50) NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FOREIGN KEY ([actor_user_id]) REFERENCES [users]([id])
);
GO

CREATE TABLE [feedbacks] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [kanji_id] int NULL,
  [type] nvarchar(50) NOT NULL,
  [description] nvarchar(max) NOT NULL,
  [status] nvarchar(50) NOT NULL DEFAULT 'Open',
  [admin_note] nvarchar(max) NULL,
  [resolved_by] uniqueidentifier NULL,
  [created_at] datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
  [updated_at] datetime2 NULL,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]),
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]),
  FOREIGN KEY ([resolved_by]) REFERENCES [users]([id])
);
GO

CREATE TABLE [daily_metrics] (
  [date] date PRIMARY KEY,
  [dau] int NOT NULL DEFAULT 0,
  [wau] int NOT NULL DEFAULT 0,
  [mau] int NOT NULL DEFAULT 0,
  [total_reviews] int NOT NULL DEFAULT 0,
  [d1_retention] float NULL,
  [d7_retention] float NULL,
  [d30_retention] float NULL
);
GO