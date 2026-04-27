-- 1. Suppression des équipements avec un slot invalide (0)
-- Supprime les entrées corrompues qui polluent les logs (Error adding NPC equipment ... slot=0).
DELETE FROM npcequipment WHERE Slot = 0;

-- 2. Suppression des doublons exacts 
-- On ne garde qu'une seule instance si Template/Slot/Model/Color/etc sont identiques.
-- Cela évite les avertissements inutiles sur des templates comme AlbMerchantShield.
DELETE n1 FROM npcequipment n1
INNER JOIN npcequipment n2 
WHERE 
    n1.NPCEquipment_ID > n2.NPCEquipment_ID AND 
    n1.TemplateID = n2.TemplateID AND 
    n1.Slot = n2.Slot AND 
    n1.Model = n2.Model AND 
    n1.Color = n2.Color AND 
    n1.Effect = n2.Effect AND 
    n1.Extension = n2.Extension AND 
    n1.Emblem = n2.Emblem;
