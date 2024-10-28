CREATE DATABASE [cachara-users];
CREATE DATABASE [cachara-content];

USE [master];
CREATE LOGIN cachara WITH PASSWORD = 'J%Kn%x6-_x~jQ-W(';
       
USE [cachara-users];
CREATE USER cachara FOR LOGIN cachara;
ALTER ROLE db_owner ADD MEMBER cachara;
      
USE [cachara-content];
CREATE USER cachara FOR LOGIN cachara;
ALTER ROLE db_owner ADD MEMBER cachara;
      
