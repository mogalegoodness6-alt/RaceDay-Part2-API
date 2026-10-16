/* OPTIONAL LOCAL TEST SETUP ONLY.
   1. Register a test account through POST /api/Auth/register first.
   2. Replace the email below with that account's email.
   3. Run this script against RaceDayDB, then log in again to obtain a fresh JWT.
   Do not run against a production database. */
USE RaceDayDB;
GO
DECLARE @TestEmail NVARCHAR(100) = 'replace-with-your-test-email@example.com';
UPDATE dbo.Users
SET Role = 'Organiser'
WHERE Email = @TestEmail;
SELECT UserID, Email, Role
FROM dbo.Users
WHERE Email = @TestEmail;
GO
