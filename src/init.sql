--CREATE DATABASE [cachara-users];
--CREATE DATABASE [cachara-content];

USE [master];
GO

IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'cachara')
    BEGIN
        CREATE LOGIN [cachara] WITH PASSWORD = 'J%Kn%x6-_x~jQ-W(', CHECK_POLICY = OFF;
        ALTER SERVER ROLE [sysadmin] ADD MEMBER [cachara];
    END
GO
      
