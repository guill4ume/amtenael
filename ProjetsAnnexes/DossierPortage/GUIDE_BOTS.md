# 🤖 Guide du Système de Bots (Mimics)

Ce document détaille le fonctionnement, la configuration et l'exploitation du système de bots "Mimics" sur le serveur OpenDAoC-SPB.

## 1. Vue d'ensemble
Le système Mimic simule des joueurs réels pour peupler les zones de combat (RvR) et permettre le groupement en PvE. Les bots utilisent une IA avancée (`MimicBrain`) capable de gérer les classes, les styles de combat, les sorts et le groupement.

## 2. Thidranki (Battleground)
À Thidranki (Région 252), les bots sont configurés pour simuler une bataille permanente.

- **Démarrage automatique** : Le système se lance à la fin du boot du serveur (Event `GameServerStarted`).
- **Population** : 60 bots au total, répartis de manière égale (20 Albion, 20 Hibernia, 20 Midgard).
- **Niveau** : Tous les bots sont forcés au **Niveau 50**.
- **Spawn progressif** : Pour éviter les pics de lag, les bots apparaissent au rythme de 1 bot par royaume par seconde après l'initialisation.

## 3. Commandes Administrateur
Ces commandes permettent de gérer le champ de bataille global.

- `/mbstats thid` : Affiche les statistiques de population actuelle (combien de bots par royaume).
- `/mbattle thid start` : Force le démarrage immédiat du cycle de spawn.
- `/mbattle thid stop` : Arrête le cycle de spawn (les bots en vie restent).
- `/mbattle thid clear` : Arrête le spawn et supprime instantanément tous les bots de la zone.

## 4. Commandes Joueur (Groupe PvE)
Les joueurs peuvent interagir avec les bots pour former des groupes.

- `/mlfg` : Liste les bots disponibles pour être recrutés.
- `/mbring [nom]` : Invoque un bot spécifique à vos côtés.
- `/mrole [tank|healer|puller]` : Assigne un rôle spécifique à un bot de votre groupe.
- `/mleave` : Ordonne au bot de quitter votre groupe.

## 5. Maintenance Technique
- **Code source** : `MimicManager.cs` (logique globale) et `MimicNPC.cs` (comportement individuel).
- **Stabilité PvP** : Un correctif a été appliqué dans `Group.cs` pour éviter les crashs (NullReferenceException) lors du groupement de Mimics en mode PvP.
- **Navigation Aquatique (Fix 28/04/2026)** :
    - Désactivation de la noyade pour les bots (`MimicNPC.cs`).
    - Interdiction de s'asseoir/se reposer dans l'eau (`MimicBrain.cs`).
    - Correction de la dérive du point de spawn en zone immergée (`MimicState.cs`).
    - Détection de blocage ("stuck") : réinitialisation du mouvement après 10s d'immobilité en état de déplacement (`MimicState.cs`).
- **Évolutions futures (Siège & Navigation)** : Un plan d'amélioration détaillé a été stocké pour permettre plus tard aux bots de détruire les portes ennemies et corriger définitivement les blocages aquatiques. Voir `ProjetsAnnexes/DossierPortage/PLAN_AMELIORATION_BOTS.md`.
- **Vérification** : Dans les logs Docker, cherchez `Thidranki Battleground bots started automatically` pour confirmer l'activation.
