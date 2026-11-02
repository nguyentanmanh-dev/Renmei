IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RenmeiDb')
BEGIN
    CREATE DATABASE [RenmeiDb];
END
GO
USE [RenmeiDb];
GO
DROP TABLE IF EXISTS [audit_logs];
DROP TABLE IF EXISTS [feedbacks];
DROP TABLE IF EXISTS [daily_metrics];
DROP TABLE IF EXISTS [placement_options];
DROP TABLE IF EXISTS [placement_questions];
DROP TABLE IF EXISTS [chat_scenarios];
DROP TABLE IF EXISTS [daily_queues];
DROP TABLE IF EXISTS [review_logs];
DROP TABLE IF EXISTS [user_kanji_progress];
DROP TABLE IF EXISTS [kanji_revisions];
DROP TABLE IF EXISTS [kanji_media];
DROP TABLE IF EXISTS [kanji];
DROP TABLE IF EXISTS [user_streaks];
DROP TABLE IF EXISTS [refresh_tokens];
DROP TABLE IF EXISTS [user_roles];
DROP TABLE IF EXISTS [roles];
DROP TABLE IF EXISTS [users];
GO

-- PHÂN HỆ USER và AUTHENTICATION
CREATE TABLE [users] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [email] nvarchar(255) NOT NULL UNIQUE,
  [password_hash] nvarchar(255) NOT NULL,
  [full_name] nvarchar(255),
  [status] nvarchar(50) NOT NULL DEFAULT 'Active',
  [created_at] datetime2 DEFAULT SYSDATETIME(),
  [updated_at] datetime2
);
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
  [token] nvarchar(500) NOT NULL UNIQUE,
  [expires_at] datetime2 NOT NULL,
  [is_revoked] bit DEFAULT 0,
  [created_at] datetime2 DEFAULT SYSDATETIME(),
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [user_streaks] (
  [user_id] uniqueidentifier PRIMARY KEY,
  [current_streak] int DEFAULT 0,
  [longest_streak] int DEFAULT 0,
  [freeze_item_count] int DEFAULT 0,
  [last_learned_date] date,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE
);
GO

--PHÂN HỆ NỘI DUNG KANJI và MEDIA
CREATE TABLE [kanji] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [character] nvarchar(10) NOT NULL UNIQUE,
  [han_viet] nvarchar(100) NOT NULL,
  [jlpt_level] nvarchar(10) NOT NULL,
  [stroke_count] int,
  [meanings_json] nvarchar(max),
  [onyomi_json] nvarchar(max),
  [kunyomi_json] nvarchar(max),
  [mnemonic_story] nvarchar(max),
  [status] nvarchar(50) NOT NULL DEFAULT 'Draft',
  [version] int DEFAULT 1,
  [created_by] uniqueidentifier,
  [created_at] datetime2 DEFAULT SYSDATETIME(),
  FOREIGN KEY ([created_by]) REFERENCES [users]([id])
);
GO

CREATE TABLE [kanji_media] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [kanji_id] uniqueidentifier NOT NULL UNIQUE,
  [mnemonic_image_url] nvarchar(500),
  [stroke_order_gif_url] nvarchar(500),
  [audio_url] nvarchar(500),
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [kanji_revisions] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [kanji_id] uniqueidentifier NOT NULL,
  [version] int NOT NULL,
  [content_json] nvarchar(max) NOT NULL,
  [created_by] uniqueidentifier,
  [created_at] datetime2 DEFAULT SYSDATETIME(),
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]) ON DELETE CASCADE,
  FOREIGN KEY ([created_by]) REFERENCES [users]([id])
);
GO

--PHÂN HỆ TIẾN ĐỘ và THUẬT TOÁN SM-2
CREATE TABLE [user_kanji_progress] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [kanji_id] uniqueidentifier NOT NULL,
  [ease_factor] float DEFAULT 2.5,
  [interval] int DEFAULT 0,
  [repetitions] int DEFAULT 0,
  [due_date] date NOT NULL,
  [last_reviewed_at] datetime2,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE,
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [review_logs] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [kanji_id] uniqueidentifier NOT NULL,
  [score] int NOT NULL,
  [interval_before] int,
  [interval_after] int,
  [ease_factor_before] float,
  [ease_factor_after] float,
  [reviewed_at] datetime2 DEFAULT SYSDATETIME(),
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]),
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id])
);
GO

CREATE TABLE [daily_queues] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [date] date NOT NULL,
  [new_cards_count] int DEFAULT 0,
  [review_cards_count] int DEFAULT 0,
  [completed_count] int DEFAULT 0,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]) ON DELETE CASCADE
);
GO

-- PLACEMENT TEST và ROLEPLAY CHAT
CREATE TABLE [placement_questions] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [content] nvarchar(max) NOT NULL,
  [jlpt_level] nvarchar(10) NOT NULL,
  [is_active] bit DEFAULT 1
);
GO

CREATE TABLE [placement_options] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [question_id] uniqueidentifier NOT NULL,
  [option_text] nvarchar(max) NOT NULL,
  [is_correct] bit DEFAULT 0,
  FOREIGN KEY ([question_id]) REFERENCES [placement_questions]([id]) ON DELETE CASCADE
);
GO

CREATE TABLE [chat_scenarios] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [title] nvarchar(255) NOT NULL,
  [roleplay_context] nvarchar(max) NOT NULL,
  [suggested_kanji_ids_json] nvarchar(max)
);
GO

--ADMIN SYSTEM)
CREATE TABLE [audit_logs] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [actor_user_id] uniqueidentifier,
  [action] nvarchar(100) NOT NULL,
  [entity_type] nvarchar(100) NOT NULL,
  [entity_id] nvarchar(100) NOT NULL,
  [details_json] nvarchar(max),
  [ip_address] nvarchar(50),
  [created_at] datetime2 DEFAULT SYSDATETIME(),
  FOREIGN KEY ([actor_user_id]) REFERENCES [users]([id])
);
GO

CREATE TABLE [feedbacks] (
  [id] uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
  [user_id] uniqueidentifier NOT NULL,
  [kanji_id] uniqueidentifier,
  [type] nvarchar(50) NOT NULL,
  [description] nvarchar(max) NOT NULL,
  [status] nvarchar(50) NOT NULL DEFAULT 'Open',
  [admin_note] nvarchar(max),
  [resolved_by] uniqueidentifier,
  [created_at] datetime2 DEFAULT SYSDATETIME(),
  [updated_at] datetime2,
  FOREIGN KEY ([user_id]) REFERENCES [users]([id]),
  FOREIGN KEY ([kanji_id]) REFERENCES [kanji]([id]),
  FOREIGN KEY ([resolved_by]) REFERENCES [users]([id])
);
GO

CREATE TABLE [daily_metrics] (
  [date] date PRIMARY KEY,
  [dau] int DEFAULT 0,
  [wau] int DEFAULT 0,
  [mau] int DEFAULT 0,
  [total_reviews] int DEFAULT 0,
  [d1_retention] float,
  [d7_retention] float,
  [d30_retention] float
);
GO