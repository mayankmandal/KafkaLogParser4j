USE [KafkaLogParser4jDB]
GO
/****** Object:  StoredProcedure [dbo].[uspFileProcessingStatus]    Script Date: 23-12-2024 11:59:26 ******/
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