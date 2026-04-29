# Récapitulatif Complet : Migration Avalon (Région 51)

Ce document consigne l'ensemble des modifications techniques et fonctionnelles apportées lors de la migration des services d'Avalon.

## 1. Stabilité & Core Fixes
### Patch SkillBase (`SkillBase.cs`)
- **Problème** : Crashs au démarrage car les noms de classes d'Abilities en DB ne correspondaient pas au code (ex: `Lifter` vs `XLifterAbility`).
- **Solution** : Implémentation d'un système de "Fallback". Le serveur tente désormais les variantes suivantes si le nom primaire échoue :
  - Préfixes : `X`, `AtlasOF_`
  - Suffixe : `Ability`
- **Résultat** : Démarrage fluide sans intervention manuelle sur la base de données pour les capacités.

### Spells manquants
Injection de 5 sorts essentiels pour arrêter les warnings de démarrage et restaurer les effets visuels/buffs des consommables :
- `8982` : Magican Skin
- `8983` : Pictish Skin
- `31024` : Soupe (Regen)
- `31028` : Ciboulette (Regen)
- `31032` : Abat (Regen)

## 2. PNJ & Services
### Registraire de Guilde (`GuildRegistrarNPC.cs`)
- **Localisation** : `GameServer/scripts/customnpc/GuildRegistrarNPC.cs`
- **Fonctionnalités** :
  - Création solo (fixé à 1 membre minimum via `ServerProperties.GUILD_NUM`).
  - Définition du nom par **Whisper**.
  - **Bypass Emblèmes** : Débloque tous les emblèmes de tous les royaumes pour tous les joueurs (via un passage temporaire du royaume à `None` pendant le dialogue).

### Échangeurs d'Items (`AvalonExchangerNPC.cs`)
- **Fonctionnement** : Classe générique pilotée par fichier de données (`LootsAvalon\echangeur_final.txt`).
- **Récompenses** : Gère automatiquement l'XP et l'Or lors du Drag&Drop d'un item sur le PNJ.
- **Liste des PNJs convertis (80+)** :
  - Loras, Savant K, Skin2, Tywinn, John, Joric, Stellan, Rhaegar, Azzura, Skin4, Leag Ulvir, Borin Thekesd, Skin3, Zog, Alienor, Orn, Savant G, Savant A, Capitaine Némo, Eldrin, Helga, Lineat, Stanislas, Korgan, feu vif, Savant S, Grond, Savant W, Fendrel, Henry, Ingrid Lemouton, feu doux, Kilote, Orik, Selrien, Lorian, Savant Z, Krunk, Stèle du Souvenir, Snarg, Dori, Marmitte chaude, Valgard, Ame torturée, Torvald, Nabucar, Anspic, Savant L, Erion, Mischa, Laerea, Isira, Thorek Sek, Source d\, Mardona, Clovis Leclou, Savant I, Pellys, Stengrim, Ulrik, Kliomo, Corbin, Dunar, Marguerite, Sapharin, Vorion, Savant R, Phyl, Gauvain, Radius, feu moyen, Arazae, Theron, Meliop, Asbeth, Odor Thedoor, Sybil, Gunnar, Raelson.

## 3. Données de Loot & Échange
- **Source de vérité** : `C:\OpenDAOC_server\ProjetsAnnexes\DossierPortage\Archives\LootsAvalon\echangeur_final.txt`
- **Contenu** : 750+ lignes associant des IDs d'items (Tokens) à des récompenses spécifiques (XP/Or).
- **Structure** : `Nom PNJ|GUID|Nom Item|ID Item|Quantité|XP|Or`

## 4. Scripts de Migration associés
- `migration_startup_fixes.sql` : Injection des sorts.
- `update_npc_classes.sql` : Mise à jour de la table `mob` pour assigner la classe `AvalonExchangerNPC`.
- `generate_update_sql.ps1` : Générateur PowerShell pour reconstruire le SQL d'update si nécessaire.
