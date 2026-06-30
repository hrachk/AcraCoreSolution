CREATE TABLE `Pek_ActivityLogs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userActivityId` bigint(20) DEFAULT NULL,
  `message` varchar(255) DEFAULT NULL,
  `date` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;