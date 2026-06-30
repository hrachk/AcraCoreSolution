#be sure UserActivityId isn't exists in ACRA3.Pek_Journal
USE ACRA3;
ALTER TABLE Pek_Journal
ADD COLUMN  UserActivityId bigint(20)
AFTER       ID;