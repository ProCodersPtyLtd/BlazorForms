IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[flow_id_sequence]') AND type = 'SO')
CREATE SEQUENCE [dbo].[flow_id_sequence] 
    AS [bigint]
    START WITH 100000
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE  5000 
GO

IF NOT EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[flow_run]'))
BEGIN
	CREATE TABLE [dbo].[flow_run](
		[id] [int] NOT NULL,
		[flow_json] [nvarchar](MAX) NOT NULL,
	 CONSTRAINT [PK_flow_run] PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[flow_run]
		ADD CONSTRAINT [flow_json record should be formatted as JSON]
					   CHECK (ISJSON(flow_json)=1)
END

GO