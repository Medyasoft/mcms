CREATE TABLE [dbo].[SiteRedirect](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContentId] [uniqueidentifier] NOT NULL,
	[SiteLanguageId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](256) NULL,
	[Slug] [nvarchar](256) NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdatedBy] [uniqueidentifier] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[LocalAddress] [nvarchar](400) NULL,
	[RouteAddress] [nvarchar](550) NULL,
	[TransferQuerystring] [bit] NULL,
	[OrderId] [int] NULL,
	[IsTemporary] [bit] NULL,
	[PageId] [int] NOT NULL,
 CONSTRAINT [PK_SiteRedirect] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[SiteRedirectHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContentId] [uniqueidentifier] NOT NULL,
	[SiteLanguageId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](256) NULL,
	[Slug] [nvarchar](256) NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdatedBy] [uniqueidentifier] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[LocalAddress] [nvarchar](400) NULL,
	[RouteAddress] [nvarchar](550) NULL,
	[TransferQuerystring] [bit] NULL,
	[OrderId] [int] NULL,
	[IsTemporary] [bit] NULL,
	[PageId] [int] NOT NULL,
 CONSTRAINT [PK_SiteRedirect] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


--CREATE TRIGGER [dbo].[SiteRedirectChangeTrigger]
--   ON  [dbo].[SiteRedirect]
--   AFTER INSERT,DELETE,UPDATE
--AS 
--BEGIN
--	-- SET NOCOUNT ON added to prevent extra result sets from
--	-- interfering with SELECT statements.
--	SET NOCOUNT ON;
--	UPDATE [dbo].[SiteMapLastUpdate]
--	SET [SiteRedirectLastUpdateDate] = GETDATE()
--    -- Insert statements for trigger here

--END
--GO


DECLARE @triggerName VARCHAR(100)
SET @triggerName= (select top 1 name from sys.triggers where parent_id=(select object_Id from sys.tables where name ='SiteRedirect'))

IF @triggerName IS NOT NULL
	EXEC('DROP TRIGGER '+ @triggerName)

