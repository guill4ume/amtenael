# 🌌 Guide de Téléportation - Thidranki (Val de Quartz)

Ce document répertorie tous les moyens d'accéder à la zone de combat Thidranki sur le serveur OpenDAoC-SPB.

## 1. Commandes Admin (Rapide)
La méthode la plus directe pour un administrateur.
- **Téléportation directe** : `/teleport 252`
- **Stats du BG** : `/mbstats thid` (Affiche le score et la population actuelle des bots).

## 2. PNJ de Téléportation (Scripts)
### BG TELEPORTER (Générique)
Un PNJ universel présent sur le serveur (ou à créer).
- **Apparition** : `/mob create DOL.GS.Scripts.BGTeleporter`
- **Usage** : Clic droit -> Sélectionner la destination dans la fenêtre popup.

### Aerto (Custom Avalon - Map 51)
- **Mécanique** : "Global Hook" (S'applique à TOUS les PNJs nommés "Aerto").
- **Dialogue** : Propose de rejoindre le "Val de Quartz" (Thidranki) pour les joueurs de niveau 50.
- **Usage** : Clic droit sur Aerto -> Clic sur le lien [Val de Quartz] dans le dialogue.

## 3. Système d'Objets (Necklace)
Il existe un objet natif dans la base de données permettant le voyage vers le BG.
- **Nom** : `Necklace to Thidranki` (`thidranki_necklace`)
- **Fonctionnement** : 
    1. Se tenir sur un **Portal Pad** (Cercle de téléportation).
    2. Équiper le collier.
    3. La téléportation se déclenche automatiquement.
- **Note** : Cet objet est utile pour recréer des mécaniques similaires à l'Anneau des Guarks.

## 4. Informations sur la Zone
Pour plus de détails sur le fonctionnement des bots, les commandes de statistiques et la configuration de la population, consultez le [GUIDE_BOTS.md](file:///c:/OpenDAOC_server/ProjetsAnnexes/DossierPortage/GUIDE_BOTS.md).

## 5. Comportement des Gardes et PVP
Sur ce serveur (Ruleset Amtenael / PvP), les gardes des forts non revendiqués par une guilde sont paramétrés pour être **agressifs** par défaut envers tous les joueurs.
- **Agressivité** : Si vous vous approchez d'un fort ennemi ou neutre, les gardes vous attaqueront à vue.
- **Propriété** : Ce comportement est contrôlé par `PVP_UNCLAIMED_KEEPS_ENEMY` dans `ServerProperties.cs`.

## 6. Colliers de Téléportation Régionaux (Avalon - Map 51)
En plus de la téléportation vers Thidranki, il existe un système de transport entre les villages d'Avalon via des colliers spécifiques.

### Marchands de Colliers (NPCs)
Les PNJs suivants vendent des colliers pour les différentes villes de la région 51 :
- **Ansall** (Village de Wearyall)
- **Maskel**
- **Inshael**
- **Roric**
- **Booor**
- **Llaeom**
- **Lorian** (Spécialisé Avalon)
- **Ballkor**

### Fonctionnement (À porter)
- **Système** : Repose sur les classes `AutoTimedTeleporter` et `AutoTeleportDestination`.
- **Mécanique** : Équiper le collier sur un "Portal Pad" (les destinations sont mappées sur le nom exact de l'objet).
- **Statut** : Les PNJs sont présents en base de données (Lot B), mais le code C# doit être porté depuis le dossier `Breamor` pour être fonctionnel sur OpenDAoC-SPB.
