USE [KafkaLogParser4jDB]
GO
/****** Object:  User [NT AUTHORITY\SYSTEM]    Script Date: 23-12-2024 11:51:37 ******/
CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM] WITH DEFAULT_SCHEMA=[db_datareader]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
ALTER ROLE [db_datareader] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
/****** Object:  Table [dbo].[AppLog]    Script Date: 23-12-2024 11:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppLog](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[ThreadId] [varchar](100) NOT NULL,
	[ServiceName] [varchar](200) NOT NULL,
	[RequestDateTime] [datetime] NOT NULL,
	[ResponseDateTime] [datetime] NOT NULL,
	[ServiceTime] [time](7) NOT NULL,
	[HttpCode] [varchar](50) NOT NULL,
 CONSTRAINT [PK_App_Log] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileProcessingStatus]    Script Date: 23-12-2024 11:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileProcessingStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileNameWithExtension] [varchar](100) NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NULL,
	[CurrentLineReadPosition] [bigint] NOT NULL,
	[FileSize] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TopicTrace]    Script Date: 23-12-2024 11:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TopicTrace](
	[FirstTopicName] [varchar](100) NULL,
	[SecondTopicName] [varchar](100) NULL,
	[isFirstTopicCreated] [int] NOT NULL,
	[isSecondTopicCreated] [int] NOT NULL
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AppLog] ON 

INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22835, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22836, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22837, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22838, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22839, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22840, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22841, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:01:36.8710000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22842, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22843, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:01:36.8710000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22844, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22845, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22846, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22847, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22848, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22849, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22850, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:01:36.8710000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22851, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22852, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22853, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22854, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22855, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22856, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22857, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22858, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:01:36.8710000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22859, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:01:36.8710000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22860, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22861, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22862, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22863, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22864, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:01:36.8710000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22865, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22866, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22867, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:01:39.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22868, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22869, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22870, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22871, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22872, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22873, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22874, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22875, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22876, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22877, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22878, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22879, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22880, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22881, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22882, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22883, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22884, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22885, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22886, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22887, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22888, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22889, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22890, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22891, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22892, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22893, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22894, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22895, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22896, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22897, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22898, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22899, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22900, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22901, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22902, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22903, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22904, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22905, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22906, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22907, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22908, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22909, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22910, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22911, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22912, N'https-jsse-nio-8443-exec-19', N'userAuthentication', CAST(N'2022-10-02T11:14:16.103' AS DateTime), CAST(N'2022-10-02T11:22:29.037' AS DateTime), CAST(N'00:08:12.9320000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22913, N'https-jsse-nio-8443-exec-29', N'userAuthentication', CAST(N'2022-10-02T11:23:36.103' AS DateTime), CAST(N'2022-10-02T11:23:37.403' AS DateTime), CAST(N'00:00:01.3000000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22914, N'https-jsse-nio-8443-exec-12', N'fetchCustomerProfile', CAST(N'2022-10-02T11:23:37.103' AS DateTime), CAST(N'2022-10-02T11:23:38.523' AS DateTime), CAST(N'00:00:01.4210000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22915, N'https-jsse-nio-8443-exec-32', N'fetchCustomerAccounts', CAST(N'2022-10-02T11:24:06.103' AS DateTime), CAST(N'2022-10-02T11:24:08.457' AS DateTime), CAST(N'00:00:02.3540000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22916, N'https-jsse-nio-8443-exec-15', N'fetchPayeeGroups', CAST(N'2022-10-02T11:24:12.103' AS DateTime), CAST(N'2022-10-02T11:24:13.060' AS DateTime), CAST(N'00:00:00.9570000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22917, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:13.103' AS DateTime), CAST(N'2022-10-02T11:24:14.933' AS DateTime), CAST(N'00:00:01.8290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22918, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:15.103' AS DateTime), CAST(N'2022-10-02T11:24:15.993' AS DateTime), CAST(N'00:00:00.8910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22919, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:16.103' AS DateTime), CAST(N'2022-10-02T11:24:17.030' AS DateTime), CAST(N'00:00:00.9280000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22920, N'https-jsse-nio-8443-exec-15', N'fetchTransferReasons', CAST(N'2022-10-02T11:24:17.103' AS DateTime), CAST(N'2022-10-02T11:24:17.683' AS DateTime), CAST(N'00:00:00.5790000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22921, N'https-jsse-nio-8443-exec-12', N'domesticTransfer', CAST(N'2022-10-02T11:24:51.103' AS DateTime), CAST(N'2022-10-02T11:24:55.553' AS DateTime), CAST(N'00:00:04.4490000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22922, N'https-jsse-nio-8443-exec-12', N'fetchExchangeRates', CAST(N'2022-10-02T11:24:55.103' AS DateTime), CAST(N'2022-10-02T11:24:56.017' AS DateTime), CAST(N'00:00:00.9140000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22923, N'https-jsse-nio-8443-exec-12', N'paymentChargesInquiry', CAST(N'2022-10-02T11:24:56.103' AS DateTime), CAST(N'2022-10-02T11:24:56.603' AS DateTime), CAST(N'00:00:00.4990000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22924, N'https-jsse-nio-8443-exec-15', N'executeTransfer_demostic', CAST(N'2022-10-02T11:25:07.103' AS DateTime), CAST(N'2022-10-02T11:25:14.003' AS DateTime), CAST(N'00:00:06.8990000' AS Time), N'400')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22925, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22926, N'https-jsse-nio-8443-exec-18', N'fetchCustomerProfile', CAST(N'2022-10-02T11:25:28.103' AS DateTime), CAST(N'2022-10-02T11:25:29.117' AS DateTime), CAST(N'00:00:01.0150000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22927, N'https-jsse-nio-8443-exec-17', N'fetchAccountStatement', CAST(N'2022-10-02T11:26:09.103' AS DateTime), CAST(N'2022-10-02T11:26:11.123' AS DateTime), CAST(N'00:00:02.0200000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22928, N'https-jsse-nio-8443-exec-29', N'userAuthentication', CAST(N'2022-10-02T11:26:29.103' AS DateTime), CAST(N'2022-10-02T11:26:30.677' AS DateTime), CAST(N'00:00:01.5720000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22929, N'https-jsse-nio-8443-exec-17', N'fetchCustomerAccounts', CAST(N'2022-10-02T11:26:36.103' AS DateTime), CAST(N'2022-10-02T11:26:38.147' AS DateTime), CAST(N'00:00:02.0450000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22930, N'https-jsse-nio-8443-exec-18', N'selfTransfer', CAST(N'2022-10-02T11:26:48.103' AS DateTime), CAST(N'2022-10-02T11:26:51.943' AS DateTime), CAST(N'00:00:03.8400000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22931, N'https-jsse-nio-8443-exec-18', N'fetchExchangeRates', CAST(N'2022-10-02T11:26:52.103' AS DateTime), CAST(N'2022-10-02T11:26:52.367' AS DateTime), CAST(N'00:00:00.2640000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22932, N'https-jsse-nio-8443-exec-18', N'paymentChargesInquiry', CAST(N'2022-10-02T11:26:52.103' AS DateTime), CAST(N'2022-10-02T11:26:53.020' AS DateTime), CAST(N'00:00:00.9160000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22933, N'https-jsse-nio-8443-exec-15', N'executeTransfer_self', CAST(N'2022-10-02T11:26:55.103' AS DateTime), CAST(N'2022-10-02T11:26:58.767' AS DateTime), CAST(N'00:00:03.6630000' AS Time), N'200')
GO
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22934, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22935, N'https-jsse-nio-8443-exec-31', N'fetchCustomerProfile', CAST(N'2022-10-02T11:27:06.103' AS DateTime), CAST(N'2022-10-02T11:27:07.560' AS DateTime), CAST(N'00:00:01.4580000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22936, N'https-jsse-nio-8443-exec-17', N'userAuthentication', CAST(N'2022-10-02T11:27:53.103' AS DateTime), CAST(N'2022-10-02T11:35:57.493' AS DateTime), CAST(N'00:08:04.3910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22937, N'https-jsse-nio-8443-exec-17', N'userAuthentication', CAST(N'2022-10-02T11:41:25.103' AS DateTime), CAST(N'2022-10-02T11:49:21.623' AS DateTime), CAST(N'00:07:56.5190000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22938, N'https-jsse-nio-8443-exec-19', N'userAuthentication', CAST(N'2022-10-02T11:14:16.103' AS DateTime), CAST(N'2022-10-02T11:22:29.037' AS DateTime), CAST(N'00:08:12.9320000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22939, N'https-jsse-nio-8443-exec-29', N'userAuthentication', CAST(N'2022-10-02T11:23:36.103' AS DateTime), CAST(N'2022-10-02T11:23:37.403' AS DateTime), CAST(N'00:00:01.3000000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22940, N'https-jsse-nio-8443-exec-12', N'fetchCustomerProfile', CAST(N'2022-10-02T11:23:37.103' AS DateTime), CAST(N'2022-10-02T11:23:38.523' AS DateTime), CAST(N'00:00:01.4210000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22941, N'https-jsse-nio-8443-exec-32', N'fetchCustomerAccounts', CAST(N'2022-10-02T11:24:06.103' AS DateTime), CAST(N'2022-10-02T11:24:08.457' AS DateTime), CAST(N'00:00:02.3540000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22942, N'https-jsse-nio-8443-exec-15', N'fetchPayeeGroups', CAST(N'2022-10-02T11:24:12.103' AS DateTime), CAST(N'2022-10-02T11:24:13.060' AS DateTime), CAST(N'00:00:00.9570000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22943, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:13.103' AS DateTime), CAST(N'2022-10-02T11:24:14.933' AS DateTime), CAST(N'00:00:01.8290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22944, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:15.103' AS DateTime), CAST(N'2022-10-02T11:24:15.993' AS DateTime), CAST(N'00:00:00.8910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22945, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:16.103' AS DateTime), CAST(N'2022-10-02T11:24:17.030' AS DateTime), CAST(N'00:00:00.9280000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22946, N'https-jsse-nio-8443-exec-15', N'fetchTransferReasons', CAST(N'2022-10-02T11:24:17.103' AS DateTime), CAST(N'2022-10-02T11:24:17.683' AS DateTime), CAST(N'00:00:00.5790000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22947, N'https-jsse-nio-8443-exec-12', N'domesticTransfer', CAST(N'2022-10-02T11:24:51.103' AS DateTime), CAST(N'2022-10-02T11:24:55.553' AS DateTime), CAST(N'00:00:04.4490000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22948, N'https-jsse-nio-8443-exec-12', N'fetchExchangeRates', CAST(N'2022-10-02T11:24:55.103' AS DateTime), CAST(N'2022-10-02T11:24:56.017' AS DateTime), CAST(N'00:00:00.9140000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22949, N'https-jsse-nio-8443-exec-12', N'paymentChargesInquiry', CAST(N'2022-10-02T11:24:56.103' AS DateTime), CAST(N'2022-10-02T11:24:56.603' AS DateTime), CAST(N'00:00:00.4990000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22950, N'https-jsse-nio-8443-exec-15', N'executeTransfer_demostic', CAST(N'2022-10-02T11:25:07.103' AS DateTime), CAST(N'2022-10-02T11:25:14.003' AS DateTime), CAST(N'00:00:06.8990000' AS Time), N'400')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22951, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22952, N'https-jsse-nio-8443-exec-18', N'fetchCustomerProfile', CAST(N'2022-10-02T11:25:28.103' AS DateTime), CAST(N'2022-10-02T11:25:29.117' AS DateTime), CAST(N'00:00:01.0150000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22953, N'https-jsse-nio-8443-exec-17', N'fetchAccountStatement', CAST(N'2022-10-02T11:26:09.103' AS DateTime), CAST(N'2022-10-02T11:26:11.123' AS DateTime), CAST(N'00:00:02.0200000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22954, N'https-jsse-nio-8443-exec-29', N'userAuthentication', CAST(N'2022-10-02T11:26:29.103' AS DateTime), CAST(N'2022-10-02T11:26:30.677' AS DateTime), CAST(N'00:00:01.5720000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22955, N'https-jsse-nio-8443-exec-15', N'fetchCustomerProfile', CAST(N'2022-10-02T11:26:30.103' AS DateTime), CAST(N'2022-10-02T11:26:31.523' AS DateTime), CAST(N'00:00:01.4190000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22956, N'https-jsse-nio-8443-exec-17', N'fetchCustomerAccounts', CAST(N'2022-10-02T11:26:36.103' AS DateTime), CAST(N'2022-10-02T11:26:38.147' AS DateTime), CAST(N'00:00:02.0450000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22957, N'https-jsse-nio-8443-exec-18', N'selfTransfer', CAST(N'2022-10-02T11:26:48.103' AS DateTime), CAST(N'2022-10-02T11:26:51.943' AS DateTime), CAST(N'00:00:03.8400000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22958, N'https-jsse-nio-8443-exec-18', N'fetchExchangeRates', CAST(N'2022-10-02T11:26:52.103' AS DateTime), CAST(N'2022-10-02T11:26:52.367' AS DateTime), CAST(N'00:00:00.2640000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22959, N'https-jsse-nio-8443-exec-18', N'paymentChargesInquiry', CAST(N'2022-10-02T11:26:52.103' AS DateTime), CAST(N'2022-10-02T11:26:53.020' AS DateTime), CAST(N'00:00:00.9160000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22960, N'https-jsse-nio-8443-exec-15', N'executeTransfer_self', CAST(N'2022-10-02T11:26:55.103' AS DateTime), CAST(N'2022-10-02T11:26:58.767' AS DateTime), CAST(N'00:00:03.6630000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22961, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:27:05.103' AS DateTime), CAST(N'2022-10-02T11:27:06.693' AS DateTime), CAST(N'00:00:01.5910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22962, N'https-jsse-nio-8443-exec-31', N'fetchCustomerProfile', CAST(N'2022-10-02T11:27:06.103' AS DateTime), CAST(N'2022-10-02T11:27:07.560' AS DateTime), CAST(N'00:00:01.4580000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22963, N'https-jsse-nio-8443-exec-17', N'userAuthentication', CAST(N'2022-10-02T11:27:53.103' AS DateTime), CAST(N'2022-10-02T11:35:57.493' AS DateTime), CAST(N'00:08:04.3910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22964, N'https-jsse-nio-8443-exec-17', N'userAuthentication', CAST(N'2022-10-02T11:41:25.103' AS DateTime), CAST(N'2022-10-02T11:49:21.623' AS DateTime), CAST(N'00:07:56.5190000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22965, N'https-jsse-nio-8443-exec-19', N'userAuthentication', CAST(N'2022-10-02T11:14:16.103' AS DateTime), CAST(N'2022-10-02T11:22:29.037' AS DateTime), CAST(N'00:08:12.9320000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22966, N'https-jsse-nio-8443-exec-29', N'userAuthentication', CAST(N'2022-10-02T11:23:36.103' AS DateTime), CAST(N'2022-10-02T11:23:37.403' AS DateTime), CAST(N'00:00:01.3000000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22967, N'https-jsse-nio-8443-exec-12', N'fetchCustomerProfile', CAST(N'2022-10-02T11:23:37.103' AS DateTime), CAST(N'2022-10-02T11:23:38.523' AS DateTime), CAST(N'00:00:01.4210000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22968, N'https-jsse-nio-8443-exec-32', N'fetchCustomerAccounts', CAST(N'2022-10-02T11:24:06.103' AS DateTime), CAST(N'2022-10-02T11:24:08.457' AS DateTime), CAST(N'00:00:02.3540000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22969, N'https-jsse-nio-8443-exec-15', N'fetchPayeeGroups', CAST(N'2022-10-02T11:24:12.103' AS DateTime), CAST(N'2022-10-02T11:24:13.060' AS DateTime), CAST(N'00:00:00.9570000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22970, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:13.103' AS DateTime), CAST(N'2022-10-02T11:24:14.933' AS DateTime), CAST(N'00:00:01.8290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22971, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:15.103' AS DateTime), CAST(N'2022-10-02T11:24:15.993' AS DateTime), CAST(N'00:00:00.8910000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22972, N'https-jsse-nio-8443-exec-15', N'fetchPayeesUnderPayeeGroup', CAST(N'2022-10-02T11:24:16.103' AS DateTime), CAST(N'2022-10-02T11:24:17.030' AS DateTime), CAST(N'00:00:00.9280000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22973, N'https-jsse-nio-8443-exec-15', N'fetchTransferReasons', CAST(N'2022-10-02T11:24:17.103' AS DateTime), CAST(N'2022-10-02T11:24:17.683' AS DateTime), CAST(N'00:00:00.5790000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22974, N'https-jsse-nio-8443-exec-12', N'domesticTransfer', CAST(N'2022-10-02T11:24:51.103' AS DateTime), CAST(N'2022-10-02T11:24:55.553' AS DateTime), CAST(N'00:00:04.4490000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22975, N'https-jsse-nio-8443-exec-12', N'fetchExchangeRates', CAST(N'2022-10-02T11:24:55.103' AS DateTime), CAST(N'2022-10-02T11:24:56.017' AS DateTime), CAST(N'00:00:00.9140000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22976, N'https-jsse-nio-8443-exec-12', N'paymentChargesInquiry', CAST(N'2022-10-02T11:24:56.103' AS DateTime), CAST(N'2022-10-02T11:24:56.603' AS DateTime), CAST(N'00:00:00.4990000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22977, N'https-jsse-nio-8443-exec-15', N'executeTransfer_demostic', CAST(N'2022-10-02T11:25:07.103' AS DateTime), CAST(N'2022-10-02T11:25:14.003' AS DateTime), CAST(N'00:00:06.8990000' AS Time), N'400')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22978, N'https-jsse-nio-8443-exec-13', N'userAuthentication', CAST(N'2022-10-02T11:25:27.103' AS DateTime), CAST(N'2022-10-02T11:25:28.233' AS DateTime), CAST(N'00:00:01.1290000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22979, N'https-jsse-nio-8443-exec-18', N'fetchCustomerProfile', CAST(N'2022-10-02T11:25:28.103' AS DateTime), CAST(N'2022-10-02T11:25:29.117' AS DateTime), CAST(N'00:00:01.0150000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22980, N'https-jsse-nio-8443-exec-17', N'fetchAccountStatement', CAST(N'2022-10-02T11:26:09.103' AS DateTime), CAST(N'2022-10-02T11:26:11.123' AS DateTime), CAST(N'00:00:02.0200000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22981, N'https-jsse-nio-8443-exec-29', N'userAuthentication', CAST(N'2022-10-02T11:26:29.103' AS DateTime), CAST(N'2022-10-02T11:26:30.677' AS DateTime), CAST(N'00:00:01.5720000' AS Time), N'200')
INSERT [dbo].[AppLog] ([LogID], [ThreadId], [ServiceName], [RequestDateTime], [ResponseDateTime], [ServiceTime], [HttpCode]) VALUES (22982, N'https-jsse-nio-8443-exec-15', N'fetchCustomerProfile', CAST(N'2022-10-02T11:26:30.103' AS DateTime), CAST(N'2022-10-02T11:26:31.523' AS DateTime), CAST(N'00:00:01.4190000' AS Time), N'200')
SET IDENTITY_INSERT [dbo].[AppLog] OFF
GO
SET IDENTITY_INSERT [dbo].[FileProcessingStatus] ON 

INSERT [dbo].[FileProcessingStatus] ([Id], [FileNameWithExtension], [Status], [UpdateDate], [CreateDate], [CurrentLineReadPosition], [FileSize]) VALUES (1123, N'TransactionLog.2022-10-02-11.log', N'CP', CAST(N'2024-04-02T18:04:45.983' AS DateTime), CAST(N'2024-04-02T18:04:45.983' AS DateTime), 1081676, 1081692)
INSERT [dbo].[FileProcessingStatus] ([Id], [FileNameWithExtension], [Status], [UpdateDate], [CreateDate], [CurrentLineReadPosition], [FileSize]) VALUES (1124, N'TransactionLog.2022-10-02-11 - Copy.log', N'CP', CAST(N'2024-04-02T18:06:27.910' AS DateTime), CAST(N'2024-04-02T18:06:27.910' AS DateTime), 1081676, 1081692)
INSERT [dbo].[FileProcessingStatus] ([Id], [FileNameWithExtension], [Status], [UpdateDate], [CreateDate], [CurrentLineReadPosition], [FileSize]) VALUES (1125, N'TransactionLog.2022-10-02-11 - Copy - Copy.log', N'CP', CAST(N'2024-04-02T18:07:35.033' AS DateTime), CAST(N'2024-04-02T18:07:35.033' AS DateTime), 1081676, 1081692)
INSERT [dbo].[FileProcessingStatus] ([Id], [FileNameWithExtension], [Status], [UpdateDate], [CreateDate], [CurrentLineReadPosition], [FileSize]) VALUES (1126, N'TransactionLog.2022-10-02-11 - Copy - Copy - Copy.log', N'IP', CAST(N'2024-04-02T18:12:36.333' AS DateTime), CAST(N'2024-04-02T18:12:36.333' AS DateTime), 1074943, 1081676)
INSERT [dbo].[FileProcessingStatus] ([Id], [FileNameWithExtension], [Status], [UpdateDate], [CreateDate], [CurrentLineReadPosition], [FileSize]) VALUES (1127, N'TransactionLog.2022-10-02-11 - Copy - Copy - Copy - Copy.log', N'IP', CAST(N'2024-04-02T18:15:11.237' AS DateTime), CAST(N'2024-04-02T18:15:11.237' AS DateTime), 1074709, 1081676)
SET IDENTITY_INSERT [dbo].[FileProcessingStatus] OFF
GO
INSERT [dbo].[TopicTrace] ([FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated]) VALUES (N'First-Log-Parser-Topic', N'Second-Log-Parser-Topic', 1, 1)
GO
ALTER TABLE [dbo].[FileProcessingStatus] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[FileProcessingStatus] ADD  DEFAULT ((0)) FOR [CurrentLineReadPosition]
GO
ALTER TABLE [dbo].[FileProcessingStatus] ADD  DEFAULT ((0)) FOR [FileSize]
GO
/****** Object:  StoredProcedure [dbo].[uspAddServiceLog]    Script Date: 23-12-2024 11:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Add Service Log to AppLog Table
-- =============================================
CREATE PROCEDURE [dbo].[uspAddServiceLog]
-- Add the parameters for the stored procedure here
	@ThreadId VARCHAR(100),
	@ServiceName VARCHAR(200),
	@RequestDateTime DATETIME,
	@ResponseDateTime DATETIME,
	@ServiceTime TIME,
	@HttpCode VARCHAR(50)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRY
		-- Start the transaction
        BEGIN TRANSACTION;

		-- Add Service Log
		INSERT INTO AppLog (ThreadId, ServiceName, RequestDateTime, ResponseDateTime, ServiceTime, HttpCode)
		VALUES (@ThreadId, @ServiceName, @RequestDateTime, @ResponseDateTime, @ServiceTime, @HttpCode);

		-- If everything is successful, commit the transaction
        COMMIT TRANSACTION;

	END TRY

	BEGIN CATCH
		-- If an error occurs, rollback the transaction
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

		-- Handle exceptions
		SELECT
			ERROR_NUMBER() AS ErrorNumber
		   ,ERROR_MESSAGE() AS ErrorMessage
		   ,ERROR_SEVERITY() AS ErrorSeverity
		   ,ERROR_STATE() AS ErrorState
		   ,ERROR_LINE() AS ErrorLine
		   ,ERROR_PROCEDURE() AS ErrorProcedure;

	END CATCH;

END
GO
/****** Object:  StoredProcedure [dbo].[uspFileProcessingStatus]    Script Date: 23-12-2024 11:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Data fetch, Insert and Updates to Table - [dbo].[FileProcessingStatus]
-- =============================================
CREATE PROCEDURE [dbo].[uspFileProcessingStatus]
-- Add the parameters for the stored procedure here
	@State INT = 0,
    @FileNameWithExtension NVARCHAR(100) = NULL,
    @Status NVARCHAR(2) = NULL,
    @CurrentLineReadPosition BIGINT = NULL,
    @FileSize BIGINT = NULL,
    @UpdatedFileSize BIGINT = NULL
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRY
		-- Start transaction for write operations
		IF @State IN (5, 6, 7, 8)
		BEGIN
			BEGIN TRANSACTION;
		END
		IF @State = 1
		BEGIN 
			-- CheckExistence
			SELECT COUNT(1) AS TOTAL
			FROM FileProcessingStatus WITH (NOLOCK)
			WHERE FileNameWithExtension = @FileNameWithExtension;
		END

		ELSE IF @State = 2
		BEGIN
			-- GetCurrentLineReadPosition
			SELECT CurrentLineReadPosition
			FROM FileProcessingStatus WITH (NOLOCK)
			WHERE FileNameWithExtension = @FileNameWithExtension;
		END

		ELSE IF @State = 3
		BEGIN
			-- GetStatus
			SELECT [Status]
			FROM FileProcessingStatus WITH (NOLOCK)
			WHERE FileNameWithExtension = @FileNameWithExtension;
		END

		ELSE IF @State = 4
		BEGIN
			-- GetFilesToProcess
			SELECT FileNameWithExtension, [Status]
			FROM FileProcessingStatus WITH (NOLOCK)
			WHERE [Status] IN ('NS','IP');
		END

		ELSE IF @State = 5
		BEGIN
			-- InsertRecord
			INSERT INTO FileProcessingStatus
			(FileNameWithExtension, [Status], UpdateDate, CreateDate, CurrentLineReadPosition, FileSize)
			VALUES
			(@FileNameWithExtension, @Status, GETDATE(), GETDATE(), @CurrentLineReadPosition, @FileSize);
		END

		ELSE IF @State = 6
		BEGIN
			-- UpdateStatusAndPosition
			UPDATE FileProcessingStatus
			SET [Status] = @Status, CurrentLineReadPosition = @CurrentLineReadPosition
			WHERE FileNameWithExtension = @FileNameWithExtension;
		END

		ELSE IF @State = 7
		BEGIN
			-- UpdatePositionAndFileSize
			UPDATE FileProcessingStatus
			SET CurrentLineReadPosition = @CurrentLineReadPosition, FileSize = @UpdatedFileSize
			WHERE FileNameWithExtension = @FileNameWithExtension
		END

		ELSE IF @State = 8
		BEGIN
			-- UpdatePositionOnly
			UPDATE FileProcessingStatus
			SET CurrentLineReadPosition = @CurrentLineReadPosition
			WHERE FileNameWithExtension = @FileNameWithExtension;
		END

		ELSE IF @State = 9
		BEGIN
			-- GetFileDetailsCurrentState
			SELECT [Id]
			  ,[FileNameWithExtension]
			  ,[Status]
			  ,[UpdateDate]
			  ,[CreateDate]
			  ,[CurrentLineReadPosition]
			  ,[FileSize]
			FROM [KafkaLogParser4jDB].[dbo].[FileProcessingStatus] WITH (NOLOCK)
			WHERE FileNameWithExtension = @FileNameWithExtension;
		END

		-- Commit transaction for write operations
		IF @State IN (5, 6, 7, 8)
		BEGIN
			COMMIT TRANSACTION;
		END

	END TRY

	BEGIN CATCH
		-- Rollback transaction if an error occurs during write operations
		IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

		-- Handle exceptions
		SELECT
			ERROR_NUMBER() AS ErrorNumber
		   ,ERROR_MESSAGE() AS ErrorMessage
		   ,ERROR_SEVERITY() AS ErrorSeverity
		   ,ERROR_STATE() AS ErrorState
		   ,ERROR_LINE() AS ErrorLine
		   ,ERROR_PROCEDURE() AS ErrorProcedure;

	END CATCH;

END
GO
/****** Object:  StoredProcedure [dbo].[uspTopicTrace]    Script Date: 23-12-2024 11:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Data fetch, Insert and Updates to Table - [dbo].[uspTopicTrace]
-- =============================================
CREATE PROCEDURE [dbo].[uspTopicTrace]
    -- Add the parameters for the stored procedure here
    @State INT = 0,
    @FirstTopicName NVARCHAR(100) = NULL,
    @SecondTopicName NVARCHAR(100) = NULL,
    @isFirstTopicCreated INT = NULL,
    @isSecondTopicCreated INT = NULL
AS
BEGIN

    SET NOCOUNT ON;

    BEGIN TRY
		-- Start transaction
        BEGIN TRANSACTION;

        -- CheckExistence
        IF @State = 1
        BEGIN
            SELECT [FirstTopicName],
                   [SecondTopicName],
                   [isFirstTopicCreated],
                   [isSecondTopicCreated]
            FROM [TopicTrace] WITH (NOLOCK);
        END

        -- InsertData
        ELSE IF @State = 2
        BEGIN
            -- Check if a row not exists and perform the operation
            IF NOT EXISTS (SELECT 1 FROM [KafkaLogParser4jDB].[dbo].[TopicTrace] WITH (NOLOCK))
            BEGIN
                INSERT INTO [KafkaLogParser4jDB].[dbo].[TopicTrace]
                (
                    [FirstTopicName],
                    [SecondTopicName],
                    [isFirstTopicCreated],
                    [isSecondTopicCreated]
                )
                VALUES
                (@FirstTopicName, @SecondTopicName, @isFirstTopicCreated, @isSecondTopicCreated);
            END
        END

        -- UpdateData
        ELSE IF @State = 3
        BEGIN
            -- Check if a row exists and perform update operation
            IF EXISTS (SELECT 1 FROM [KafkaLogParser4jDB].[dbo].[TopicTrace] WITH (NOLOCK))
            BEGIN
                UPDATE [KafkaLogParser4jDB].[dbo].[TopicTrace]
                SET [FirstTopicName] = @FirstTopicName,
                    [SecondTopicName] = @SecondTopicName,
                    [isFirstTopicCreated] = @isFirstTopicCreated,
                    [isSecondTopicCreated] = @isSecondTopicCreated;
            END
        END

        -- DeleteData
        ELSE IF @State = 4
        BEGIN
            -- Check if a row exists and perform delete operation
            IF EXISTS (SELECT 1 FROM [TopicTrace] WITH (NOLOCK))
            BEGIN
                DELETE FROM TopicTrace;
            END
        END

		-- Commit transaction
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
		-- Handle exceptions and rollback transaction if needed
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Handle exceptions
        SELECT ERROR_NUMBER() AS ErrorNumber,
               ERROR_MESSAGE() AS ErrorMessage,
               ERROR_SEVERITY() AS ErrorSeverity,
               ERROR_STATE() AS ErrorState,
               ERROR_LINE() AS ErrorLine,
               ERROR_PROCEDURE() AS ErrorProcedure;

    END CATCH;

END
GO
