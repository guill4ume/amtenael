-- Script de correction des classes d'Abilities pour MariaDB
-- On remplace les noms standards par les versions 'X' ou 'AtlasOF' présentes dans le code

UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.IgnorePainAbility', 'DOL.GS.RealmAbilities.XIgnorePainAbility') WHERE `Implementation` LIKE '%IgnorePainAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.LifterAbility', 'DOL.GS.RealmAbilities.XLifterAbility') WHERE `Implementation` LIKE '%LifterAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.LongWindAbility', 'DOL.GS.RealmAbilities.XLongWindAbility') WHERE `Implementation` LIKE '%LongWindAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.FirstAidAbility', 'DOL.GS.RealmAbilities.XFirstAidAbility') WHERE `Implementation` LIKE '%FirstAidAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.SecondWindAbility', 'DOL.GS.RealmAbilities.XSecondWindAbility') WHERE `Implementation` LIKE '%SecondWindAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.SerenityAbility', 'DOL.GS.RealmAbilities.XSerenityAbility') WHERE `Implementation` LIKE '%SerenityAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.ToughnessAbility', 'DOL.GS.RealmAbilities.XToughnessAbility') WHERE `Implementation` LIKE '%ToughnessAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.SpeedOfSoundAbility', 'DOL.GS.RealmAbilities.XSpeedOfSoundAbility') WHERE `Implementation` LIKE '%SpeedOfSoundAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.EtherealBondAbility', 'DOL.GS.RealmAbilities.XEtherealBondAbility') WHERE `Implementation` LIKE '%EtherealBondAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.VolcanicPillarAbility', 'DOL.GS.RealmAbilities.XVolcanicPillarAbility') WHERE `Implementation` LIKE '%VolcanicPillarAbility%';
UPDATE `Ability` SET `Implementation` = REPLACE(`Implementation`, 'DOL.GS.RealmAbilities.WildMinionAbility', 'DOL.GS.RealmAbilities.XWildMinionAbility') WHERE `Implementation` LIKE '%WildMinionAbility%';
UPDATE `Ability` SET `Implementation` = 'DOL.GS.RealmAbilities.AtlasOF_ReflexAttack' WHERE `Implementation` LIKE '%ReflexAttackAbility%';
