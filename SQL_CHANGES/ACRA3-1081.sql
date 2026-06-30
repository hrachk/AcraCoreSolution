USE ACRA3;
ALTER TABLE Sources
ADD COLUMN  GenerateAcraID varchar(8)
DEFAULT "Yes"
AFTER ContractDate;