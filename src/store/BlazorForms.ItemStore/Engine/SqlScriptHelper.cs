using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public static class SqlScriptHelper
    {
        public const string ID_COLUMN = "id";
        public const string DATA_COLUMN = "data";
        public static string CreateSchema(string name)
        {
            var sql = @$"
IF NOT EXISTS ( SELECT  *
                FROM    sys.schemas
                WHERE   name = N'{name}' )
    EXEC('CREATE SCHEMA [{name}]');
";
            return sql;
        }

        public static string CreateSequence(string name, string schema)
        {
            var sql = @$"
IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{schema}].[{name}]') AND type = 'SO')
CREATE SEQUENCE [{schema}].[{name}] 
    AS [bigint]
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE  3;
";

            return sql;
        }

        public static string CreateJsonTable(string name, string schema)
        {
            var sql = @$"
IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{schema}].[{name}]') AND type = 'U')
BEGIN
CREATE TABLE [{schema}].[{name}] ({ID_COLUMN} bigint PRIMARY KEY, {DATA_COLUMN} nvarchar(max));

ALTER TABLE [{schema}].[{name}]
    ADD CONSTRAINT [{schema}_{name}_JSON]
                   CHECK (ISJSON({DATA_COLUMN})=1);
END
";
            return sql;
        }

        public static string SelectByIdJsonTable(string name, string schema, long id)
        {
            var sql = @$"
SELECT * FROM {schema}.{name}
WHERE {ID_COLUMN} = {id};
";
            return sql;
        }

        public static string InsertJsonTable(string name, string schema, long id, string idColumn, string jsonParamName)
        {
            var sql = @$"
DECLARE @id bigint = {id};
DECLARE @json_data nvarchar(max) = @{jsonParamName};

SET @json_data = JSON_MODIFY(@json_data, '$.{idColumn}', CAST(@id AS NVARCHAR(20)));

INSERT INTO {schema}.{name}
SELECT @id,  @json_data;

SELECT @id as id;
";
            return sql;
        }

        public static string InsertJsonTableAutoIncrement(string name, string schema, string sequenceName, string idColumn, string jsonParamName)
        {
            var sql = @$"
DECLARE @id bigint = NEXT VALUE FOR {schema}.{sequenceName};
DECLARE @json_data nvarchar(max) = @{jsonParamName};

SET @json_data = JSON_MODIFY(@json_data, '$.{idColumn}', CAST(@id AS NVARCHAR(20)));

INSERT INTO {schema}.{name}
SELECT @id,  @json_data;

SELECT @id as id;
";
            return sql;
        }

        public static string UpdateJsonTable(string name, string schema, long id, string jsonParamName)
        {
            var sql = @$"
UPDATE {schema}.{name} SET {DATA_COLUMN} = @{jsonParamName}
WHERE {ID_COLUMN} = {id};
";
            return sql;
        }

        public static string UpsertJsonTable(string name, string schema, string sequenceName, string idColumn, long id, string jsonParamName)
        {
            var sql = @$"
DECLARE @id bigint = {id};
DECLARE @json_data nvarchar(max) = @{jsonParamName};

IF @id = 0
BEGIN
    SET @id = NEXT VALUE FOR {schema}.{sequenceName};

    SET @json_data = JSON_MODIFY(@json_data, '$.{idColumn}', CAST(@id AS NVARCHAR(20)));

    INSERT INTO {schema}.{name}
    SELECT @id,  @json_data;
END
ELSE
BEGIN
    UPDATE {schema}.{name} SET {DATA_COLUMN} = @{jsonParamName}
    WHERE {ID_COLUMN} = @id;
END

SELECT @id as id;
";
            return sql;
        }

        public static string DropSchemaWithObjects(string schema)
        {
            var sql = @$"
DECLARE @Sql VARCHAR(MAX)
      , @Schema varchar(20)

SET @Schema = '{schema}' --put your schema name between these quotes

--tables
SELECT @Sql = COALESCE(@Sql,'') + 'DROP TABLE %SCHEMA%.' + QUOTENAME(TABLE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @Schema
    AND TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME


--views
SELECT @Sql = COALESCE(@Sql,'') + 'DROP VIEW %SCHEMA%.' + QUOTENAME(TABLE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @Schema
    AND TABLE_TYPE = 'VIEW'
ORDER BY TABLE_NAME

--Procedures
SELECT @Sql = COALESCE(@Sql,'') + 'DROP PROCEDURE %SCHEMA%.' + QUOTENAME(ROUTINE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = @Schema
    AND ROUTINE_TYPE = 'PROCEDURE'
ORDER BY ROUTINE_NAME

--Functions
SELECT @Sql = COALESCE(@Sql,'') + 'DROP FUNCTION %SCHEMA%.' + QUOTENAME(ROUTINE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = @Schema
    AND ROUTINE_TYPE = 'FUNCTION'
ORDER BY ROUTINE_NAME

-- sequences
SELECT @Sql = COALESCE(@Sql,'') + 'DROP SEQUENCE %SCHEMA%.' + QUOTENAME(obj.name) + ';' + CHAR(13)
FROM sys.objects obj inner join sys.schemas sch on obj.schema_id = sch.schema_id
WHERE sch.name = @Schema and obj.type = 'SO'

-- substitute name
SELECT @Sql = COALESCE(REPLACE(@Sql,'%SCHEMA%',@Schema), '')

EXEC(@Sql);

-- Schema
DROP SCHEMA  IF EXISTS {schema};
";
            return sql;
        }
    }
}
