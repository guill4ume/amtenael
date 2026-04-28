-- Script de correction pour MariaDB (opendaoc)
-- Correction de l'ordre des colonnes pour correspondre au schéma actif

INSERT INTO `Spell` 
(`Spell_ID`, `SpellID`, `ClientEffect`, `Icon`, `Name`, `Description`, `Target`, `Range`, `Power`, `CastTime`, `Damage`, `DamageType`, `Type`, `Duration`, `Frequency`, `Pulse`, `PulsePower`, `Radius`, `RecastDelay`, `ResurrectHealth`, `ResurrectMana`, `Value`, `Concentration`, `LifeDrainReturn`, `AmnesiaChance`, `Message1`, `Message2`, `Message3`, `Message4`, `InstrumentRequirement`, `SpellGroup`, `EffectGroup`, `SubSpellID`, `MoveCast`, `Uninterruptible`, `IsPrimary`, `IsSecondary`, `AllowBolt`, `SharedTimerGroup`, `PackageID`, `IsFocus`, `TooltipId`, `LastTimeRowUpdated`) 
VALUES
('PictslayerMagicResistence', 8982, 1547, 1547, 'Magican Skin', 'Increases resistance to all magic types.', 'Self', 0, 0, 0, 0, 0, 'AllMagicResistsBuff', 600, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 'You are more protected!', NULL, 'Your extra protection fades.', NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'AstralSpells', 0, 5828, '2000-01-01 00:00:00'),
('PictslayerAbsorb', 8983, 1198, 1198, 'Pictish Skin', 'Adds a shield to recipient which will temporarily absorb some of the damage type specified.', 'Self', 0, 0, 0, 50, 0, 'AblativeArmor', 600, 0, 0, 0, 0, 0, 0, 0, 200, 0, 0, 0, NULL, NULL, NULL, NULL, 0, 0, 1001, 0, 0, 0, 0, 0, 0, 50, NULL, 0, 2970, '2000-01-01 00:00:00'),
('Potion_Heal_230', 8052, 8051, 8052, 'Elixir of Healing', 'Heals the target.', 'Self', 0, 0, 3, 0, 0, 'Heal', 0, 0, 0, 0, 0, 0, 0, 0, 130, 0, 0, 0, '', '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 'HealPotion', 0, 5765, '2000-01-01 00:00:00');
