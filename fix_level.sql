USE opendaoc;
UPDATE serverproperty SET Value='50' WHERE `Key`='slash_level_target';
UPDATE serverproperty SET Value='0' WHERE `Key`='slash_level_requirement';
UPDATE serverproperty SET Value='True' WHERE `Key`='allow_cata_slash_level';
SELECT `Key`, Value FROM serverproperty WHERE `Key` IN ('slash_level_target', 'slash_level_requirement', 'allow_cata_slash_level');
