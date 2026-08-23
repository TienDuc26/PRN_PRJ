-- ==========================================================
-- TourManagementDb - Full database dump
-- Generated: 2026-08-24 00:57:43
-- Use: run this script on a fresh SQL Server instance
-- ==========================================================
USE [master];
GO
IF DB_ID(N'TourManagementDb') IS NOT NULL
BEGIN
    ALTER DATABASE [TourManagementDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TourManagementDb];
END;
GO
CREATE DATABASE [TourManagementDb];
GO
USE [TourManagementDb];
GO

-- =================== SCHEMA ===================

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

CREATE TABLE [AspNetRoles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Destinations] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [City] nvarchar(100) NOT NULL,
    [Country] nvarchar(100) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Image] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Destinations] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Guides] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(150) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Phone] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [Address] nvarchar(300) NULL,
    [ExperienceYears] int NULL,
    [Languages] nvarchar(300) NULL,
    [Bio] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Guides] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Promotions] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(30) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [DiscountType] int NOT NULL,
    [DiscountValue] decimal(18,2) NOT NULL,
    [MaxDiscount] decimal(18,2) NULL,
    [MinOrderValue] decimal(18,2) NOT NULL,
    [StartAt] datetime2 NOT NULL,
    [EndAt] datetime2 NOT NULL,
    [UsageLimit] int NOT NULL,
    [UsageCount] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(150) NOT NULL,
    [Avatar] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] int NULL,
    [Address] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(450) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Tours] (
    [Id] int NOT NULL IDENTITY,
    [DestinationId] int NOT NULL,
    [Code] nvarchar(30) NOT NULL,
    [Name] nvarchar(250) NOT NULL,
    [Description] nvarchar(max) NULL,
    [DurationDays] int NOT NULL,
    [DurationNights] int NOT NULL,
    [BasePrice] decimal(18,2) NOT NULL,
    [Thumbnail] nvarchar(max) NULL,
    [IncludedServices] nvarchar(max) NULL,
    [ExcludedServices] nvarchar(max) NULL,
    [Policy] nvarchar(max) NULL,
    [TourType] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [RowVersion] rowversion NULL,
    CONSTRAINT [PK_Tours] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tours_Destinations_DestinationId] FOREIGN KEY ([DestinationId]) REFERENCES [Destinations] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] int NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AuditLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [Action] nvarchar(100) NOT NULL,
    [EntityType] nvarchar(100) NULL,
    [EntityId] nvarchar(100) NULL,
    [OldValue] nvarchar(max) NULL,
    [NewValue] nvarchar(max) NULL,
    [IpAddress] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AuditLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Content] nvarchar(1000) NOT NULL,
    [Link] nvarchar(300) NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [TourItineraries] (
    [Id] int NOT NULL IDENTITY,
    [TourId] int NOT NULL,
    [DayNumber] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Location] nvarchar(250) NULL,
    [TimeInfo] nvarchar(max) NULL,
    [Meals] nvarchar(100) NULL,
    [Hotel] nvarchar(200) NULL,
    [Notes] nvarchar(max) NULL,
    [Image] nvarchar(max) NULL,
    CONSTRAINT [PK_TourItineraries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TourItineraries_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [TourSchedules] (
    [Id] int NOT NULL IDENTITY,
    [TourId] int NOT NULL,
    [Code] nvarchar(30) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [MeetingTime] time NOT NULL,
    [MeetingPoint] nvarchar(300) NOT NULL,
    [MaxGuests] int NOT NULL,
    [BookedGuests] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [RowVersion] rowversion NULL,
    CONSTRAINT [PK_TourSchedules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TourSchedules_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Bookings] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ScheduleId] int NOT NULL,
    [BookingCode] nvarchar(30) NOT NULL,
    [Adults] int NOT NULL,
    [Children] int NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [Discount] decimal(18,2) NOT NULL,
    [Surcharge] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [PaidAmount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [PaymentStatus] int NOT NULL,
    [PromotionId] int NULL,
    [Note] nvarchar(500) NULL,
    [BookedAt] datetime2 NOT NULL,
    [ConfirmedAt] datetime2 NULL,
    [CancelledAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Bookings_Promotions_PromotionId] FOREIGN KEY ([PromotionId]) REFERENCES [Promotions] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Bookings_TourSchedules_ScheduleId] FOREIGN KEY ([ScheduleId]) REFERENCES [TourSchedules] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Bookings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [GuideAssignments] (
    [Id] int NOT NULL IDENTITY,
    [GuideId] int NOT NULL,
    [ScheduleId] int NOT NULL,
    [AssignedAt] datetime2 NOT NULL,
    [Note] nvarchar(300) NULL,
    CONSTRAINT [PK_GuideAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GuideAssignments_Guides_GuideId] FOREIGN KEY ([GuideId]) REFERENCES [Guides] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GuideAssignments_TourSchedules_ScheduleId] FOREIGN KEY ([ScheduleId]) REFERENCES [TourSchedules] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [BookingParticipants] (
    [Id] int NOT NULL IDENTITY,
    [BookingId] int NOT NULL,
    [FullName] nvarchar(150) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] int NULL,
    [IdentityNumber] nvarchar(30) NULL,
    [Phone] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [IsAdult] bit NOT NULL,
    [Note] nvarchar(300) NULL,
    CONSTRAINT [PK_BookingParticipants] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BookingParticipants_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Payments] (
    [Id] int NOT NULL IDENTITY,
    [BookingId] int NOT NULL,
    [TransactionCode] nvarchar(40) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Method] int NOT NULL,
    [Status] int NOT NULL,
    [PaidAt] datetime2 NULL,
    [Note] nvarchar(500) NULL,
    [ProcessedBy] nvarchar(100) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Reviews] (
    [Id] int NOT NULL IDENTITY,
    [BookingId] int NOT NULL,
    [UserId] int NOT NULL,
    [TourId] int NOT NULL,
    [Rating] int NOT NULL,
    [Content] nvarchar(1000) NOT NULL,
    [Image] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reviews_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [IX_AuditLogs_CreatedAt] ON [AuditLogs] ([CreatedAt]);
GO

CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);
GO

CREATE INDEX [IX_BookingParticipants_BookingId] ON [BookingParticipants] ([BookingId]);
GO

CREATE INDEX [IX_Bookings_BookedAt] ON [Bookings] ([BookedAt]);
GO

CREATE UNIQUE INDEX [IX_Bookings_BookingCode] ON [Bookings] ([BookingCode]);
GO

CREATE INDEX [IX_Bookings_PromotionId] ON [Bookings] ([PromotionId]);
GO

CREATE INDEX [IX_Bookings_ScheduleId] ON [Bookings] ([ScheduleId]);
GO

CREATE INDEX [IX_Bookings_UserId] ON [Bookings] ([UserId]);
GO

CREATE INDEX [IX_Destinations_Name] ON [Destinations] ([Name]);
GO

CREATE UNIQUE INDEX [IX_GuideAssignments_GuideId_ScheduleId] ON [GuideAssignments] ([GuideId], [ScheduleId]);
GO

CREATE INDEX [IX_GuideAssignments_ScheduleId] ON [GuideAssignments] ([ScheduleId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_Payments_BookingId] ON [Payments] ([BookingId]);
GO

CREATE UNIQUE INDEX [IX_Payments_TransactionCode] ON [Payments] ([TransactionCode]);
GO

CREATE UNIQUE INDEX [IX_Promotions_Code] ON [Promotions] ([Code]);
GO

CREATE UNIQUE INDEX [IX_Reviews_BookingId] ON [Reviews] ([BookingId]);
GO

CREATE INDEX [IX_Reviews_TourId] ON [Reviews] ([TourId]);
GO

CREATE INDEX [IX_Reviews_UserId] ON [Reviews] ([UserId]);
GO

CREATE INDEX [IX_TourItineraries_TourId_DayNumber] ON [TourItineraries] ([TourId], [DayNumber]);
GO

CREATE UNIQUE INDEX [IX_Tours_Code] ON [Tours] ([Code]);
GO

CREATE INDEX [IX_Tours_DestinationId] ON [Tours] ([DestinationId]);
GO

CREATE INDEX [IX_Tours_Name] ON [Tours] ([Name]);
GO

CREATE UNIQUE INDEX [IX_TourSchedules_Code] ON [TourSchedules] ([Code]);
GO

CREATE INDEX [IX_TourSchedules_StartDate] ON [TourSchedules] ([StartDate]);
GO

CREATE INDEX [IX_TourSchedules_TourId] ON [TourSchedules] ([TourId]);
GO

CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]) WHERE [Email] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Users_PhoneNumber] ON [Users] ([PhoneNumber]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260822181333_InitialCreate', N'8.0.0');
GO

COMMIT;
GO



-- =================== DATA ===================
-- Note: __EFMigrationsHistory row is already inserted by the schema script

SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (1, N'ADMIN', N'ADMIN', NULL);
INSERT [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (2, N'STAFF', N'STAFF', NULL);
INSERT [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (3, N'CUSTOMER', N'CUSTOMER', NULL);
SET IDENTITY_INSERT [AspNetRoles] OFF;
GO
SET IDENTITY_INSERT [Users] ON;
INSERT [Users] ([Id], [FullName], [Avatar], [DateOfBirth], [Gender], [Address], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (1, N'Quản trị viên', NULL, NULL, NULL, NULL, 1, '2026-08-22 18:28:39.407', '2026-08-22 18:28:39.407', N'admin@tour.com', N'ADMIN@TOUR.COM', N'admin@tour.com', N'ADMIN@TOUR.COM', 1, N'AQAAAAIAAYagAAAAEKz/tClr5MvXZvicjT18xOOaA8NqXIni6lHp5gZrZ5tc6qUJdw/IKi2EJMOLB3ynOQ==', N'SJ4OCQZHMHSFEEL2DFW6L232Y4ZTPEJ7', N'77296f1b-b9c0-4a0f-8e11-0f4ebc3ac6a3', N'0900000000', 0, 0, NULL, 1, 0);
INSERT [Users] ([Id], [FullName], [Avatar], [DateOfBirth], [Gender], [Address], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (2, N'Nguyễn Văn Nhân Viên', NULL, NULL, NULL, NULL, 1, '2026-08-22 18:28:39.600', '2026-08-22 18:28:39.600', N'staff@tour.com', N'STAFF@TOUR.COM', N'staff@tour.com', N'STAFF@TOUR.COM', 1, N'AQAAAAIAAYagAAAAEKo1ITpWCVr6+RyC71lf2evV4JwR5VoNf4b1tvG8YcQBckRP/GE2xOuxzKzp6q3hPw==', N'GOJ2O7GTIHJBCO52Z35U7OFWEAFJZV4O', N'4848203a-03ff-4e87-bff0-b87f68702395', N'0900000001', 0, 0, NULL, 1, 0);
INSERT [Users] ([Id], [FullName], [Avatar], [DateOfBirth], [Gender], [Address], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (3, N'Trần Thị Khách Hàng', NULL, NULL, NULL, NULL, 1, '2026-08-22 18:28:39.658', '2026-08-22 18:28:39.658', N'customer@tour.com', N'CUSTOMER@TOUR.COM', N'customer@tour.com', N'CUSTOMER@TOUR.COM', 1, N'AQAAAAIAAYagAAAAEOXzJO96FiB/UtVV71kEHGx3p9x68oWQFRL3yPQK+8tA/dhwAN8wLSw3oGY/b6m1gQ==', N'CG7O3GMOQCZWXKQTNUQ2V6ZHOK4NWWEW', N'864f2e35-1aa6-401e-9c08-e9d6982a34e7', N'0900000002', 0, 0, NULL, 1, 0);
INSERT [Users] ([Id], [FullName], [Avatar], [DateOfBirth], [Gender], [Address], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (4, N'Test Customer 2', NULL, NULL, NULL, NULL, 1, '2026-08-22 18:52:02.245', '2026-08-22 18:52:02.245', N'customer2@tour.com', N'CUSTOMER2@TOUR.COM', N'customer2@tour.com', N'CUSTOMER2@TOUR.COM', 0, N'AQAAAAIAAYagAAAAEBb1SWTVVjzJQYyf9c9C/HNIOKvgCC/ijpy7FvGMREzA/JX3t9th+bQNADrWfwymoA==', N'6XJV275ZWIMAG2T2Q46JLY4FUEKKZFTK', N'beb082c8-3768-4dff-b498-d589e82062fb', N'0988777666', 0, 0, NULL, 1, 0);
INSERT [Users] ([Id], [FullName], [Avatar], [DateOfBirth], [Gender], [Address], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (1004, N'Duc Ta', NULL, NULL, NULL, NULL, 1, '2026-08-23 17:13:45.812', '2026-08-23 17:13:45.812', N'tatienduc263@gmail.com', N'TATIENDUC263@GMAIL.COM', N'tatienduc263@gmail.com', N'TATIENDUC263@GMAIL.COM', 0, N'AQAAAAIAAYagAAAAELOoIJx1tfHXsgcLx7Zr1sG238iuCv4ltVyLSg+oY5whYlMXgD8adQvMQBEIXAcHeA==', N'QNRRQ2OUG64KFSMQI4EZLB7HYOTJ6VRV', N'd3fe417d-aa12-4cf6-9fb2-e5bd99f2e466', N'0392869501', 0, 0, NULL, 1, 0);
SET IDENTITY_INSERT [Users] OFF;
GO
INSERT [AspNetUserRoles] ([UserId], [RoleId]) VALUES (1, 1);
INSERT [AspNetUserRoles] ([UserId], [RoleId]) VALUES (1004, 1);
INSERT [AspNetUserRoles] ([UserId], [RoleId]) VALUES (2, 2);
INSERT [AspNetUserRoles] ([UserId], [RoleId]) VALUES (3, 3);
INSERT [AspNetUserRoles] ([UserId], [RoleId]) VALUES (4, 3);
SET IDENTITY_INSERT [Destinations] ON;
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (1, N'Hạ Long', N'Quảng Ninh', N'Việt Nam', N'Vịnh Hạ Long - kỳ quan thiên nhiên thế giới với hàng nghìn đảo đá vôi', NULL, 1, '2026-08-22 18:28:39.726', '2026-08-22 18:28:39.726');
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (2, N'Đà Lạt', N'Lâm Đồng', N'Việt Nam', N'Thành phố ngàn hoa với khí hậu mát mẻ quanh năm', NULL, 1, '2026-08-22 18:28:39.726', '2026-08-22 18:28:39.726');
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (3, N'Phú Quốc', N'Kiên Giang', N'Việt Nam', N'Đảo ngọc với những bãi biển tuyệt đẹp', NULL, 1, '2026-08-22 18:28:39.726', '2026-08-22 18:28:39.726');
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (4, N'Sapa', N'Lào Cai', N'Việt Nam', N'Thị trấn vùng cao với ruộng bậc thang và văn hóa dân tộc', NULL, 1, '2026-08-22 18:28:39.726', '2026-08-22 18:28:39.726');
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (5, N'Nha Trang', N'Khánh Hòa', N'Việt Nam', N'Thành phố biển nổi tiếng với các resort cao cấp', NULL, 1, '2026-08-22 18:28:39.726', '2026-08-22 18:28:39.726');
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (6, N'Hội An', N'Quảng Nam', N'Việt Nam', N'Phố cổ đèn lồng - di sản văn hóa thế giới', NULL, 1, '2026-08-22 18:28:39.726', '2026-08-22 18:28:39.726');
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (7, N'Đà Nẵng', N'Đà Nẵng', N'Việt Nam', N'Thành phố đáng sống với biển Mỹ Khê tuyệt đẹp', NULL, 1, '2026-08-22 18:28:39.726', '2026-08-22 18:28:39.726');
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (8, N'Bangkok', N'Bangkok', N'Thái Lan', N'Thủ đô sôi động của Thái Lan', N'/uploads/tours/c283371e-e205-4120-9e51-07f7f5032a93.jpg', 1, '2026-08-22 18:28:39.726', '2026-08-22 18:34:43.192');
INSERT [Destinations] ([Id], [Name], [City], [Country], [Description], [Image], [Status], [CreatedAt], [UpdatedAt]) VALUES (9, N'fsdafsdf', N'fdasfasdf', N'ấdfasfa', N'fdasfdasfas', N'/uploads/tours/502b6e8f-c15b-402a-bf25-c17c8c675c05.jpg', 1, '2026-08-23 17:15:22.319', '2026-08-23 17:15:22.319');
SET IDENTITY_INSERT [Destinations] OFF;
GO
SET IDENTITY_INSERT [Tours] ON;
INSERT [Tours] ([Id], [DestinationId], [Code], [Name], [Description], [DurationDays], [DurationNights], [BasePrice], [Thumbnail], [IncludedServices], [ExcludedServices], [Policy], [TourType], [Status], [CreatedAt], [UpdatedAt]) VALUES (1, 1, N'T-01-001', N'Tour Hạ Long 2N1Đ - Du thuyền cao cấp', N'Khám phá vịnh Hạ Long với du thuyền 5 sao, thăm hang Sửng Sốt, đảo Ti Tốp', 2, 1, 2500000.00, NULL, N'Xe đưa đón, du thuyền, ăn 3 bữa, vé tham quan, hướng dẫn viên', N'Đồ uống, chi phí cá nhân, tip', N'Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%', 1, 1, '2026-08-22 18:28:39.760', '2026-08-22 18:28:39.760');
INSERT [Tours] ([Id], [DestinationId], [Code], [Name], [Description], [DurationDays], [DurationNights], [BasePrice], [Thumbnail], [IncludedServices], [ExcludedServices], [Policy], [TourType], [Status], [CreatedAt], [UpdatedAt]) VALUES (2, 2, N'T-02-001', N'Tour Đà Lạt 3N2Đ - Thành phố mộng mơ', N'Tham quan các điểm nổi tiếng: Hồ Xuân Hương, Thung lũng Tình Yêu, Langbiang', 3, 2, 3200000.00, NULL, N'Khách sạn 3*, xe đưa đón, ăn sáng, vé tham quan', N'Vé máy bay, ăn trưa/tối', N'Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%', 1, 1, '2026-08-22 18:28:39.760', '2026-08-22 18:28:39.760');
INSERT [Tours] ([Id], [DestinationId], [Code], [Name], [Description], [DurationDays], [DurationNights], [BasePrice], [Thumbnail], [IncludedServices], [ExcludedServices], [Policy], [TourType], [Status], [CreatedAt], [UpdatedAt]) VALUES (3, 3, N'T-03-001', N'Tour Phú Quốc 4N3Đ - Thiên đường biển đảo', N'Khám phá đảo ngọc với Bãi Sao, Hòn Thơm, VinWonders', 4, 3, 5800000.00, NULL, N'Resort 4*, xe đưa đón, ăn sáng, vé VinWonders', N'Vé máy bay, ăn trưa/tối', N'Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%', 2, 1, '2026-08-22 18:28:39.760', '2026-08-22 18:28:39.760');
INSERT [Tours] ([Id], [DestinationId], [Code], [Name], [Description], [DurationDays], [DurationNights], [BasePrice], [Thumbnail], [IncludedServices], [ExcludedServices], [Policy], [TourType], [Status], [CreatedAt], [UpdatedAt]) VALUES (4, 4, N'T-04-001', N'Tour Sapa 3N2Đ - Ruộng bậc thang', N'Trekking bản Cát Cát, Fansipan, chợ tình Sapa', 3, 2, 3500000.00, NULL, N'Khách sạn, xe đưa đón, ăn sáng, HDV địa phương', N'Cáp treo Fansipan, ăn trưa/tối', N'Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%', 1, 1, '2026-08-22 18:28:39.760', '2026-08-22 18:28:39.760');
INSERT [Tours] ([Id], [DestinationId], [Code], [Name], [Description], [DurationDays], [DurationNights], [BasePrice], [Thumbnail], [IncludedServices], [ExcludedServices], [Policy], [TourType], [Status], [CreatedAt], [UpdatedAt]) VALUES (5, 6, N'T-06-001', N'Tour Hội An - Đà Nẵng 4N3Đ', N'Phố cổ Hội An, Bà Nà Hills, Ngũ Hành Sơn', 4, 3, 4500000.00, NULL, N'Khách sạn 4*, xe, ăn sáng, vé Bà Nà', N'Vé máy bay, ăn trưa/tối', N'Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%', 1, 1, '2026-08-22 18:28:39.760', '2026-08-22 18:28:39.760');
INSERT [Tours] ([Id], [DestinationId], [Code], [Name], [Description], [DurationDays], [DurationNights], [BasePrice], [Thumbnail], [IncludedServices], [ExcludedServices], [Policy], [TourType], [Status], [CreatedAt], [UpdatedAt]) VALUES (6, 8, N'T-08-001', N'Tour Bangkok - Pattaya 5N4Đ', N'Khám phá Thái Lan với Bangkok sôi động và Pattaya biển xanh', 5, 4, 8500000.00, NULL, N'Khách sạn 4*, vé máy bay, HDV, ăn sáng', N'Hộ chiếu, ăn trưa/tối', N'Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%', 2, 1, '2026-08-22 18:28:39.760', '2026-08-22 18:28:39.760');
SET IDENTITY_INSERT [Tours] OFF;
GO
SET IDENTITY_INSERT [TourItineraries] ON;
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (1, 1, 1, N'Ngày 1', N'Hoạt động trong ngày 1 của tour', N'Tour Hạ Long 2N1Đ - Du thuyền cao cấp', N'07:00 - 21:00', N'', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (2, 1, 2, N'Ngày 2', N'Hoạt động trong ngày 2 của tour', N'Tour Hạ Long 2N1Đ - Du thuyền cao cấp', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (3, 2, 1, N'Ngày 1', N'Hoạt động trong ngày 1 của tour', N'Tour Đà Lạt 3N2Đ - Thành phố mộng mơ', N'07:00 - 21:00', N'', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (4, 2, 2, N'Ngày 2', N'Hoạt động trong ngày 2 của tour', N'Tour Đà Lạt 3N2Đ - Thành phố mộng mơ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (5, 2, 3, N'Ngày 3', N'Hoạt động trong ngày 3 của tour', N'Tour Đà Lạt 3N2Đ - Thành phố mộng mơ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (6, 3, 1, N'Ngày 1', N'Hoạt động trong ngày 1 của tour', N'Tour Phú Quốc 4N3Đ - Thiên đường biển đảo', N'07:00 - 21:00', N'', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (7, 3, 2, N'Ngày 2', N'Hoạt động trong ngày 2 của tour', N'Tour Phú Quốc 4N3Đ - Thiên đường biển đảo', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (8, 3, 3, N'Ngày 3', N'Hoạt động trong ngày 3 của tour', N'Tour Phú Quốc 4N3Đ - Thiên đường biển đảo', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (9, 3, 4, N'Ngày 4', N'Hoạt động trong ngày 4 của tour', N'Tour Phú Quốc 4N3Đ - Thiên đường biển đảo', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (10, 4, 1, N'Ngày 1', N'Hoạt động trong ngày 1 của tour', N'Tour Sapa 3N2Đ - Ruộng bậc thang', N'07:00 - 21:00', N'', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (11, 4, 2, N'Ngày 2', N'Hoạt động trong ngày 2 của tour', N'Tour Sapa 3N2Đ - Ruộng bậc thang', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (12, 4, 3, N'Ngày 3', N'Hoạt động trong ngày 3 của tour', N'Tour Sapa 3N2Đ - Ruộng bậc thang', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (13, 5, 1, N'Ngày 1', N'Hoạt động trong ngày 1 của tour', N'Tour Hội An - Đà Nẵng 4N3Đ', N'07:00 - 21:00', N'', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (14, 5, 2, N'Ngày 2', N'Hoạt động trong ngày 2 của tour', N'Tour Hội An - Đà Nẵng 4N3Đ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (15, 5, 3, N'Ngày 3', N'Hoạt động trong ngày 3 của tour', N'Tour Hội An - Đà Nẵng 4N3Đ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (16, 5, 4, N'Ngày 4', N'Hoạt động trong ngày 4 của tour', N'Tour Hội An - Đà Nẵng 4N3Đ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (17, 6, 1, N'Ngày 1', N'Hoạt động trong ngày 1 của tour', N'Tour Bangkok - Pattaya 5N4Đ', N'07:00 - 21:00', N'', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (18, 6, 2, N'Ngày 2', N'Hoạt động trong ngày 2 của tour', N'Tour Bangkok - Pattaya 5N4Đ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (19, 6, 3, N'Ngày 3', N'Hoạt động trong ngày 3 của tour', N'Tour Bangkok - Pattaya 5N4Đ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (20, 6, 4, N'Ngày 4', N'Hoạt động trong ngày 4 của tour', N'Tour Bangkok - Pattaya 5N4Đ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'Khách sạn 3-4*', NULL, NULL);
INSERT [TourItineraries] ([Id], [TourId], [DayNumber], [Title], [Description], [Location], [TimeInfo], [Meals], [Hotel], [Notes], [Image]) VALUES (21, 6, 5, N'Ngày 5', N'Hoạt động trong ngày 5 của tour', N'Tour Bangkok - Pattaya 5N4Đ', N'07:00 - 21:00', N'Sáng, Trưa, Tối', N'', NULL, NULL);
SET IDENTITY_INSERT [TourItineraries] OFF;
GO
SET IDENTITY_INSERT [TourSchedules] ON;
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (1, 1, N'SCH-1-001', '2026-08-29 00:00:00.000', '2026-08-30 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 15, 5, 2996351.00, 1, '2026-08-22 18:28:39.841', '2026-08-22 18:28:39.841');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (2, 1, N'SCH-1-002', '2026-09-12 00:00:00.000', '2026-09-13 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 19, 4, 2532034.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (3, 1, N'SCH-1-003', '2026-09-26 00:00:00.000', '2026-09-27 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 29, 4, 2523512.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (4, 1, N'SCH-1-004', '2026-10-10 00:00:00.000', '2026-10-11 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 25, 3, 2365081.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (5, 2, N'SCH-2-001', '2026-08-29 00:00:00.000', '2026-08-31 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 27, 1, 3608569.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (6, 2, N'SCH-2-002', '2026-09-12 00:00:00.000', '2026-09-14 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 24, 1, 3391474.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (7, 2, N'SCH-2-003', '2026-09-26 00:00:00.000', '2026-09-28 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 20, 1, 3369392.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (8, 2, N'SCH-2-004', '2026-10-10 00:00:00.000', '2026-10-12 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 27, 2, 3327406.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (9, 3, N'SCH-3-001', '2026-08-29 00:00:00.000', '2026-09-01 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 18, 1, 5887320.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (10, 3, N'SCH-3-002', '2026-09-12 00:00:00.000', '2026-09-15 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 20, 1, 5735797.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (11, 3, N'SCH-3-003', '2026-09-26 00:00:00.000', '2026-09-29 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 20, 1, 5620760.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (12, 3, N'SCH-3-004', '2026-10-10 00:00:00.000', '2026-10-13 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 16, 3, 5915871.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (13, 4, N'SCH-4-001', '2026-08-29 00:00:00.000', '2026-08-31 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 26, 2, 3389144.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (14, 4, N'SCH-4-002', '2026-09-12 00:00:00.000', '2026-09-14 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 18, 3, 3418653.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (15, 4, N'SCH-4-003', '2026-09-26 00:00:00.000', '2026-09-28 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 28, 4, 3529839.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (16, 4, N'SCH-4-004', '2026-10-10 00:00:00.000', '2026-10-12 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 25, 1, 3488848.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (17, 5, N'SCH-5-001', '2026-08-29 00:00:00.000', '2026-09-01 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 19, 4, 4501504.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (18, 5, N'SCH-5-002', '2026-09-12 00:00:00.000', '2026-09-15 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 29, 3, 4838871.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (19, 5, N'SCH-5-003', '2026-09-26 00:00:00.000', '2026-09-29 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 23, 3, 4920944.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (20, 5, N'SCH-5-004', '2026-10-10 00:00:00.000', '2026-10-13 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 25, 3, 4386994.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (21, 6, N'SCH-6-001', '2026-08-29 00:00:00.000', '2026-09-02 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 27, 3, 8315415.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (22, 6, N'SCH-6-002', '2026-09-12 00:00:00.000', '2026-09-16 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 26, 1, 8424453.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (23, 6, N'SCH-6-003', '2026-09-26 00:00:00.000', '2026-09-30 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 20, 2, 8395643.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
INSERT [TourSchedules] ([Id], [TourId], [Code], [StartDate], [EndDate], [MeetingTime], [MeetingPoint], [MaxGuests], [BookedGuests], [Price], [Status], [CreatedAt], [UpdatedAt]) VALUES (24, 6, N'SCH-6-004', '2026-10-10 00:00:00.000', '2026-10-14 00:00:00.000', N'07:30:00', N'Sân bay Tân Sơn Nhất - Cổng D1', 22, 4, 8982907.00, 1, '2026-08-22 18:28:39.857', '2026-08-22 18:28:39.857');
SET IDENTITY_INSERT [TourSchedules] OFF;
GO
SET IDENTITY_INSERT [Promotions] ON;
INSERT [Promotions] ([Id], [Code], [Name], [Description], [DiscountType], [DiscountValue], [MaxDiscount], [MinOrderValue], [StartAt], [EndAt], [UsageLimit], [UsageCount], [Status], [CreatedAt], [UpdatedAt]) VALUES (1, N'SUMMER2026', N'Khuyến mãi hè 2026', N'Giảm 15% cho tất cả tour mùa hè', 1, 15.00, 1000000.00, 2000000.00, '2026-08-12 18:28:39.915', '2026-11-22 18:28:39.915', 100, 0, 1, '2026-08-22 18:28:39.915', '2026-08-22 18:28:39.915');
INSERT [Promotions] ([Id], [Code], [Name], [Description], [DiscountType], [DiscountValue], [MaxDiscount], [MinOrderValue], [StartAt], [EndAt], [UsageLimit], [UsageCount], [Status], [CreatedAt], [UpdatedAt]) VALUES (2, N'NEW500K', N'Giảm 500K cho khách mới', N'Giảm cố định 500.000 VNĐ cho đơn từ 3 triệu', 2, 500000.00, NULL, 3000000.00, '2026-08-17 18:28:39.915', '2027-02-22 18:28:39.915', 200, 0, 1, '2026-08-22 18:28:39.915', '2026-08-22 18:28:39.915');
INSERT [Promotions] ([Id], [Code], [Name], [Description], [DiscountType], [DiscountValue], [MaxDiscount], [MinOrderValue], [StartAt], [EndAt], [UsageLimit], [UsageCount], [Status], [CreatedAt], [UpdatedAt]) VALUES (3, N'VIP30', N'VIP giảm 30%', N'Giảm 30% cho đơn từ 5 triệu, tối đa 2 triệu', 1, 30.00, 2000000.00, 5000000.00, '2026-07-23 18:28:39.915', '2026-09-22 18:28:39.915', 50, 5, 1, '2026-08-22 18:28:39.915', '2026-08-22 18:28:39.915');
SET IDENTITY_INSERT [Promotions] OFF;
GO
SET IDENTITY_INSERT [Guides] ON;
INSERT [Guides] ([Id], [FullName], [DateOfBirth], [Phone], [Email], [Address], [ExperienceYears], [Languages], [Bio], [Status], [CreatedAt], [UpdatedAt]) VALUES (1, N'Lê Văn Hùng', NULL, N'0912345678', N'hung@tour.com', N'Hà Nội', 8, N'Tiếng Việt, Tiếng Anh', NULL, 1, '2026-08-22 18:28:39.887', '2026-08-22 18:28:39.887');
INSERT [Guides] ([Id], [FullName], [DateOfBirth], [Phone], [Email], [Address], [ExperienceYears], [Languages], [Bio], [Status], [CreatedAt], [UpdatedAt]) VALUES (2, N'Nguyễn Thị Mai', NULL, N'0912345679', N'mai@tour.com', N'TP.HCM', 5, N'Tiếng Việt, Tiếng Anh, Tiếng Pháp', NULL, 1, '2026-08-22 18:28:39.888', '2026-08-22 18:28:39.888');
INSERT [Guides] ([Id], [FullName], [DateOfBirth], [Phone], [Email], [Address], [ExperienceYears], [Languages], [Bio], [Status], [CreatedAt], [UpdatedAt]) VALUES (3, N'Trần Văn Nam', NULL, N'0912345680', N'nam@tour.com', N'Đà Nẵng', 10, N'Tiếng Việt, Tiếng Anh, Tiếng Trung', NULL, 1, '2026-08-22 18:28:39.888', '2026-08-22 18:28:39.888');
INSERT [Guides] ([Id], [FullName], [DateOfBirth], [Phone], [Email], [Address], [ExperienceYears], [Languages], [Bio], [Status], [CreatedAt], [UpdatedAt]) VALUES (4, N'Phạm Thị Hoa', NULL, N'0912345681', N'hoa@tour.com', N'Nha Trang', 6, N'Tiếng Việt, Tiếng Anh', NULL, 1, '2026-08-22 18:28:39.888', '2026-08-22 18:28:39.888');
SET IDENTITY_INSERT [Guides] OFF;
GO
SET IDENTITY_INSERT [Bookings] ON;
INSERT [Bookings] ([Id], [UserId], [ScheduleId], [BookingCode], [Adults], [Children], [Subtotal], [Discount], [Surcharge], [TotalAmount], [PaidAmount], [Status], [PaymentStatus], [PromotionId], [Note], [BookedAt], [ConfirmedAt], [CancelledAt], [CompletedAt], [CreatedAt], [UpdatedAt]) VALUES (1, 3, 24, N'BK202608224314', 1, 0, 8982907.00, 0.00, 0.00, 8982907.00, 0.00, 4, 1, NULL, N'fwdfasdfad ád fasd', '2026-08-22 18:33:03.764', NULL, '2026-08-22 18:37:20.461', NULL, '2026-08-22 18:33:03.757', '2026-08-22 18:33:03.757');
INSERT [Bookings] ([Id], [UserId], [ScheduleId], [BookingCode], [Adults], [Children], [Subtotal], [Discount], [Surcharge], [TotalAmount], [PaidAmount], [Status], [PaymentStatus], [PromotionId], [Note], [BookedAt], [ConfirmedAt], [CancelledAt], [CompletedAt], [CreatedAt], [UpdatedAt]) VALUES (2, 3, 18, N'BK202608221217', 1, 0, 4838871.00, 0.00, 0.00, 4838871.00, 4838871.00, 3, 3, NULL, NULL, '2026-08-22 18:37:55.396', NULL, NULL, NULL, '2026-08-22 18:37:55.394', '2026-08-22 19:24:37.460');
INSERT [Bookings] ([Id], [UserId], [ScheduleId], [BookingCode], [Adults], [Children], [Subtotal], [Discount], [Surcharge], [TotalAmount], [PaidAmount], [Status], [PaymentStatus], [PromotionId], [Note], [BookedAt], [ConfirmedAt], [CancelledAt], [CompletedAt], [CreatedAt], [UpdatedAt]) VALUES (3, 3, 1, N'BK202608222240', 2, 1, 8090147.70, 0.00, 0.00, 8090147.70, 8090000.00, 2, 2, NULL, NULL, '2026-08-22 18:40:27.001', NULL, NULL, NULL, '2026-08-22 18:40:26.995', '2026-08-22 18:43:54.052');
SET IDENTITY_INSERT [Bookings] OFF;
GO
SET IDENTITY_INSERT [BookingParticipants] ON;
INSERT [BookingParticipants] ([Id], [BookingId], [FullName], [DateOfBirth], [Gender], [IdentityNumber], [Phone], [Email], [IsAdult], [Note]) VALUES (1, 1, N'Duc Ta', '2004-03-26 00:00:00.000', 1, N'0111232131231231231', N'0392859401', NULL, 1, NULL);
INSERT [BookingParticipants] ([Id], [BookingId], [FullName], [DateOfBirth], [Gender], [IdentityNumber], [Phone], [Email], [IsAdult], [Note]) VALUES (2, 2, N'Duc Ta', '2004-03-26 00:00:00.000', 1, N'0111232131231231231', N'0392859401', NULL, 1, NULL);
INSERT [BookingParticipants] ([Id], [BookingId], [FullName], [DateOfBirth], [Gender], [IdentityNumber], [Phone], [Email], [IsAdult], [Note]) VALUES (3, 3, N'Nguyen Van A', '1990-01-01 00:00:00.000', 1, N'123456789', N'0901234567', NULL, 1, NULL);
INSERT [BookingParticipants] ([Id], [BookingId], [FullName], [DateOfBirth], [Gender], [IdentityNumber], [Phone], [Email], [IsAdult], [Note]) VALUES (4, 3, N'Tran Thi B', '1992-05-01 00:00:00.000', 2, N'987654321', N'0901234568', NULL, 1, NULL);
INSERT [BookingParticipants] ([Id], [BookingId], [FullName], [DateOfBirth], [Gender], [IdentityNumber], [Phone], [Email], [IsAdult], [Note]) VALUES (5, 3, N'Be C', '2020-03-01 00:00:00.000', 2, NULL, NULL, NULL, 0, NULL);
SET IDENTITY_INSERT [BookingParticipants] OFF;
GO
SET IDENTITY_INSERT [Payments] ON;
INSERT [Payments] ([Id], [BookingId], [TransactionCode], [Amount], [Method], [Status], [PaidAt], [Note], [ProcessedBy], [CreatedAt]) VALUES (1, 3, N'TX20260822184102383', 2000000.00, 2, 3, '2026-08-22 18:41:02.384', N'Chuyen khoan VCB', N'customer@tour.com', '2026-08-22 18:41:02.383');
INSERT [Payments] ([Id], [BookingId], [TransactionCode], [Amount], [Method], [Status], [PaidAt], [Note], [ProcessedBy], [CreatedAt]) VALUES (2, 3, N'TX20260822184354020', 6090000.00, 1, 3, '2026-08-22 18:43:54.022', NULL, N'customer@tour.com', '2026-08-22 18:43:54.019');
INSERT [Payments] ([Id], [BookingId], [TransactionCode], [Amount], [Method], [Status], [PaidAt], [Note], [ProcessedBy], [CreatedAt]) VALUES (3, 2, N'TX20260822192437454', 4838871.00, 2, 3, '2026-08-22 19:24:37.454', N'Thanh toan het tu test', N'customer@tour.com', '2026-08-22 19:24:37.454');
SET IDENTITY_INSERT [Payments] OFF;
GO
SET IDENTITY_INSERT [Notifications] ON;
INSERT [Notifications] ([Id], [UserId], [Title], [Content], [Link], [Status], [CreatedAt]) VALUES (1, 3, N'Đặt tour thành công', N'Đơn hàng BK202608224314 đã được tạo. Vui lòng thanh toán để hoàn tất.', N'/Booking/Details/1', 2, '2026-08-22 18:33:03.887');
INSERT [Notifications] ([Id], [UserId], [Title], [Content], [Link], [Status], [CreatedAt]) VALUES (2, 3, N'Đơn hàng đã bị hủy', N'Đơn BK202608224314 đã được hủy. Hoàn tiền: 0 VNĐ', N'/Booking/Details/1', 1, '2026-08-22 18:37:20.466');
INSERT [Notifications] ([Id], [UserId], [Title], [Content], [Link], [Status], [CreatedAt]) VALUES (3, 3, N'Đặt tour thành công', N'Đơn hàng BK202608221217 đã được tạo. Vui lòng thanh toán để hoàn tất.', N'/Booking/Details/2', 1, '2026-08-22 18:37:55.408');
INSERT [Notifications] ([Id], [UserId], [Title], [Content], [Link], [Status], [CreatedAt]) VALUES (4, 3, N'Đặt tour thành công', N'Đơn hàng BK202608222240 đã được tạo. Vui lòng thanh toán để hoàn tất.', N'/Booking/Details/3', 1, '2026-08-22 18:40:27.058');
INSERT [Notifications] ([Id], [UserId], [Title], [Content], [Link], [Status], [CreatedAt]) VALUES (5, 3, N'Thanh toán thành công', N'Đã nhận 2,000,000 VNĐ cho đơn BK202608222240.', N'/Booking/Details/3', 1, '2026-08-22 18:41:02.418');
INSERT [Notifications] ([Id], [UserId], [Title], [Content], [Link], [Status], [CreatedAt]) VALUES (6, 3, N'Thanh toán thành công', N'Đã nhận 6,090,000 VNĐ cho đơn BK202608222240.', N'/Booking/Details/3', 2, '2026-08-22 18:43:54.365');
INSERT [Notifications] ([Id], [UserId], [Title], [Content], [Link], [Status], [CreatedAt]) VALUES (7, 3, N'Thanh toán thành công', N'Đã nhận 4,838,871 VNĐ cho đơn BK202608221217.', N'/Booking/Details/2', 1, '2026-08-22 19:24:37.520');
SET IDENTITY_INSERT [Notifications] OFF;
GO
SET IDENTITY_INSERT [AuditLogs] ON;
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (1, 1, N'LOGIN', N'User', N'1', NULL, N'admin@tour.com', N'::1', '2026-08-22 18:29:25.738');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (2, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:31:55.069');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3, 3, N'CREATE_BOOKING', N'Booking', N'1', NULL, N'BK202608224314', NULL, '2026-08-22 18:33:03.898');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (4, 3, N'LOGOUT', N'User', N'3', NULL, NULL, N'::1', '2026-08-22 18:33:35.355');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (5, 2, N'LOGIN', N'User', N'2', NULL, N'staff@tour.com', N'::1', '2026-08-22 18:33:50.285');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (6, 2, N'UPDATE_DESTINATION', N'Destination', N'8', NULL, N'Bangkok', N'::1', '2026-08-22 18:34:43.204');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (7, 2, N'LOGOUT', N'User', N'2', NULL, NULL, N'::1', '2026-08-22 18:36:05.985');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (8, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:36:19.321');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (9, 3, N'CANCEL_BOOKING', N'Booking', N'1', NULL, N'Refund: 0.000', NULL, '2026-08-22 18:37:20.469');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (10, 3, N'CREATE_BOOKING', N'Booking', N'2', NULL, N'BK202608221217', NULL, '2026-08-22 18:37:55.410');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (11, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:39:54.586');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (12, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:40:26.822');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (13, 3, N'CREATE_BOOKING', N'Booking', N'3', NULL, N'BK202608222240', NULL, '2026-08-22 18:40:27.072');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (14, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:40:47.568');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (15, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:41:02.244');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (16, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:41:13.410');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (17, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:51:00.291');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (18, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 18:51:38.493');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (19, 4, N'REGISTER', N'User', N'4', NULL, N'customer2@tour.com', N'::1', '2026-08-22 18:52:02.386');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (20, 4, N'LOGIN', N'User', N'4', NULL, N'customer2@tour.com', N'::1', '2026-08-22 18:52:02.473');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (21, 1, N'LOGIN', N'User', N'1', NULL, N'admin@tour.com', N'::1', '2026-08-22 18:52:41.087');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (22, 2, N'LOGIN', N'User', N'2', NULL, N'staff@tour.com', N'::1', '2026-08-22 18:52:49.651');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (1017, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 19:04:27.252');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (1018, 1, N'LOGIN', N'User', N'1', NULL, N'admin@tour.com', N'::1', '2026-08-22 19:06:22.933');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (2017, 1, N'LOGIN', N'User', N'1', NULL, N'admin@tour.com', N'::1', '2026-08-22 19:09:46.637');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3017, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 19:23:21.999');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3018, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 19:23:39.723');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3019, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 19:23:55.299');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3020, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 19:24:12.352');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3021, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 19:24:37.299');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3022, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 19:25:48.351');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3023, 3, N'LOGIN', N'User', N'3', NULL, N'customer@tour.com', N'::1', '2026-08-22 19:26:24.229');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3024, 1, N'LOGIN', N'User', N'1', NULL, N'admin@tour.com', N'::1', '2026-08-23 17:12:33.492');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3025, 1, N'LOGOUT', N'User', N'1', NULL, NULL, N'::1', '2026-08-23 17:13:33.950');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3026, 1004, N'REGISTER', N'User', N'1004', NULL, N'tatienduc263@gmail.com', N'::1', '2026-08-23 17:13:46.096');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3027, 1004, N'LOGOUT', N'User', N'1004', NULL, NULL, N'::1', '2026-08-23 17:13:53.008');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3028, 1, N'LOGIN', N'User', N'1', NULL, N'admin@tour.com', N'::1', '2026-08-23 17:14:06.698');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3029, 1, N'CHANGE_ROLE', N'User', N'1004', N'CUSTOMER', N'STAFF', N'::1', '2026-08-23 17:14:13.622');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3030, 1, N'CHANGE_ROLE', N'User', N'1004', N'STAFF', N'ADMIN', N'::1', '2026-08-23 17:14:16.387');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3031, 1, N'CHANGE_ROLE', N'User', N'1004', N'ADMIN', N'ADMIN', N'::1', '2026-08-23 17:14:26.858');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3032, 1, N'LOCK_USER', N'User', N'1', NULL, NULL, N'::1', '2026-08-23 17:14:46.269');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3033, 1, N'UNLOCK_USER', N'User', N'1', NULL, NULL, N'::1', '2026-08-23 17:14:48.761');
INSERT [AuditLogs] ([Id], [UserId], [Action], [EntityType], [EntityId], [OldValue], [NewValue], [IpAddress], [CreatedAt]) VALUES (3034, 1, N'CREATE_DESTINATION', N'Destination', N'9', NULL, N'fsdafsdf', N'::1', '2026-08-23 17:15:22.339');



SET IDENTITY_INSERT [AuditLogs] OFF;
GO
