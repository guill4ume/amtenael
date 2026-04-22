-- SQL pour restaurer le point de téléportation du portail rouge vers Albion
-- Utile si on veut remettre la fonctionnalité plus tard.
INSERT INTO zonepoint (Id, TargetX, TargetY, TargetZ, TargetRegion, TargetHeading, SourceX, SourceY, SourceZ, SourceRegion, Realm, ClassType, LastTimeRowUpdated, ZonePoint_ID)
VALUES (154, 462076, 633125, 1749, 1, 10, 525872, 542106, 3173, 51, 0, '', '2000-01-01 00:00:00', 'ALB SI EXIT');
