-- ======================================================
-- SCRIPT DE NETTOYAGE DES LOOTS (OpenDAoC)
-- Identifie les ItemTemplates manquants dans loottemplate
-- ======================================================

-- 1. IDENTIFIER : Voir la liste des objets inexistants qui polluent les logs
SELECT 
    lt.TemplateName AS 'Nom de la LootTemplate',
    lt.ItemTemplateID AS 'ID Objet Manquant'
FROM 
    loottemplate lt
LEFT JOIN 
    itemtemplate it ON lt.ItemTemplateID = it.Id_nb
WHERE 
    it.Id_nb IS NULL 
    AND lt.ItemTemplateID IS NOT NULL AND lt.ItemTemplateID <> ''
ORDER BY 
    lt.TemplateName;

-- 2. NETTOYER (OPTIONNEL) : Supprimer les entrées de loot invalides
-- Attention : Cela supprimera les chances de loot pour ces objets inexistants.

DELETE lt FROM loottemplate lt
LEFT JOIN itemtemplate it ON lt.ItemTemplateID = it.Id_nb
WHERE it.Id_nb IS NULL AND lt.ItemTemplateID IS NOT NULL AND lt.ItemTemplateID <> '';

SELECT 'Nettoyage terminé. Relancez le serveur pour valider.' AS Resultat;
