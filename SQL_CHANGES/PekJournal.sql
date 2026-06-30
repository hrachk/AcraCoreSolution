CREATE TABLE `Pek_Journal` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserActivityId` bigint(20) DEFAULT NULL,
  `Request` text,
  `Response` text,
  `IsDeserialized` bit(1) NOT NULL DEFAULT b'1',
  `ErrorText` text,
  `ResponseDateTime` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8;
