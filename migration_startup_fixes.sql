-- Migration: Startup Fixes (Missing Spells) - Corrected for SPB schema with TooltipId
-- Source: Breamor Legacy DB
-- Target: OpenDAoC-SPB (Docker)

-- Skin Spells (8982, 8983)
DELETE FROM `spell` WHERE `SpellID` IN (8982, 8983);
INSERT INTO `spell` (`Spell_ID`, `SpellID`, `Icon`, `Name`, `Description`, `Target`, `Range`, `Power`, `CastTime`, `Damage`, `DamageType`, `Type`, `Duration`, `Frequency`, `Pulse`, `PulsePower`, `Radius`, `RecastDelay`, `Value`, `Concentration`, `Message1`, `Message2`, `Message3`, `Message4`, `InstrumentRequirement`, `EffectGroup`, `TooltipId`, `LastTimeRowUpdated`) VALUES
('8982', 8982, 1547, 'Magican Skin', 'Increases resistance to all magic types.', 'Self', 0, 0, 0, 0, 0, 'AllMagicResistsBuff', 600, 0, 0, 0, 0, 0, 10, 0, 'You are more protected!', NULL, 'Your extra protection fades.', NULL, 0, 0, 5828, '2026-04-29 00:00:00'),
('8983', 8983, 1198, 'Pictish Skin', 'Adds a shield to recipient which will temporarily absorb some of the damage type specified.', 'Self', 0, 0, 0, 50, 0, 'AblativeArmor', 600, 0, 0, 0, 0, 0, 200, 0, NULL, NULL, NULL, NULL, 0, 0, 2970, '2026-04-29 00:00:00');

-- Consumable Spells (Dummy placeholders to stop warnings)
DELETE FROM `spell` WHERE `SpellID` IN (31024, 31028, 31032);
INSERT INTO `spell` (`Spell_ID`, `SpellID`, `Icon`, `Name`, `Description`, `Target`, `Range`, `Type`, `Duration`, `Value`, `TooltipId`, `LastTimeRowUpdated`) VALUES
('31024', 31024, 1, 'Soupe', 'Soupe buff', 'Self', 0, 'HealthRegenBuff', 3600, 5, 0, '2026-04-29 00:00:00'),
('31028', 31028, 1, 'Ciboulette', 'Ciboulette buff', 'Self', 0, 'HealthRegenBuff', 3600, 5, 0, '2026-04-29 00:00:00'),
('31032', 31032, 1, 'Abat', 'Abat buff', 'Self', 0, 'HealthRegenBuff', 3600, 5, 0, '2026-04-29 00:00:00');
