USE [KafkaLogParser4jDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddServiceLog]    Script Date: 23-12-2024 11:59:05 ******/
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