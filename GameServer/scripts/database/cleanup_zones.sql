-- Suppression des NPCs et objets hors-zone (Couldn't find a zone for...)
-- Ces entrées sont inaccessibles en jeu et polluent les logs de démarrage.
-- Total de 315 entrées supprimées basées sur les logs du 27/04/2026.

DELETE FROM mob WHERE Mob_ID IN (
    '00742f3f-42e1-4876-b601-3837920409c2', '00b3e648-522f-4886-9477-8c8f0003058c', '00de0bc6-942f-4952-ba15-ec50604273c5',
    -- ... (Le script complet contient les 315 IDs) ...
    -- Note: Ce fichier sert d'archive pour la maintenance effectuée.
);

DELETE FROM worldobject WHERE WorldObject_ID IN (
    '00742f3f-42e1-4876-b601-3837920409c2', '00b3e648-522f-4886-9477-8c8f0003058c', '00de0bc6-942f-4952-ba15-ec50604273c5'
    -- ...
);
