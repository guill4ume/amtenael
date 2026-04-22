-- Création de la faction Gardes
INSERT INTO faction (ID, Name, BaseAggroLevel, LastTimeRowUpdated, Faction_ID) 
VALUES (1000, 'Gardes', 100, NOW(), 'gardes_51');

-- Mise à jour des gardes dans la région 51
UPDATE mob 
SET FactionID = 1000 
WHERE Region = 51 
AND (ClassType LIKE '%GuardNPC%' OR ClassType LIKE '%SimpleGvGGuard%' OR Name LIKE '%Garde%' OR Name LIKE '%Guard%');

-- Vérification
SELECT Mob_ID, Name, FactionID FROM mob WHERE Region = 51 AND FactionID = 1000;
