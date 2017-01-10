USE [mme]
GO

/****** Object:  Table [dbo].[useraccount]    Script Date: 9/19/2016 10:59:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[useraccount](
	[id] [uniqueidentifier] NOT NULL,
	[created] [datetime] NOT NULL,
	[username] [varchar](20) NOT NULL,
	[email] [varchar](500) NOT NULL,
	[passhash] [varchar](50) NOT NULL,
	[emailconfirmed] [int] NULL,
	[lockout] [int] NULL,
	[failedattempt] [int] NULL,
	[lockoutdt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[useraccount] ADD  DEFAULT (newsequentialid()) FOR [id]
GO

ALTER TABLE [dbo].[useraccount] ADD  DEFAULT (getdate()) FOR [created]
GO


