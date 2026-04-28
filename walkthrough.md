# Walkthrough : Restauration des Services et Migration du Mobilier

Ce document rÃ©sume les actions effectuÃ©es pour restaurer l'intÃ©gritÃ© de la RÃ©gion 51 (Avalon) et les services associÃ©s.

## 1. Restauration de l' "HÃ©raut des dieux"
- **CrÃ©ation du script** : [HerautDesDieux.cs](file:///C:/OpenDAOC_server/OpenDAoC-Core-master/GameServer/scripts/customnpc/HerautDesDieux.cs) implÃ©mentant les fonctions demandÃ©es :
    - `/whisper Niveau 50`
    - `/whisper Richesse` (100 platines)
    - `/whisper artisanat` (1100 partout)
- **Injection DB** : Le PNJ a Ã©tÃ© placÃ© Ã  la position exacte demandÃ©e (localisation du joueur **Edithind**).
- **GitHub** : Le script a Ã©tÃ© "pushÃ©" sur la branche `master` du dÃ©pÃ´t Core.

## 2. Migration du Mobilier (Lot B)
- **Extraction** : Plus de 6000 lignes extraites du dump Breamor (`worldobject` et `door`).
- **Transformation (Robuste)** : Utilisation d'un script Python (via Docker) avec une machine Ã  Ã©tats pour gÃ©rer les caractÃ¨res spÃ©ciaux complexes et les redirections de colonnes.
- **RÃ©sultat de l'injection** :
    - **7031 objets** dans `worldobject`.
    - **149 portes** dans `door` (avec colonnes `IsPostern` et `State` initialisÃ©es).
- **Nettoyage** : La RÃ©gion 51 a Ã©tÃ© vidÃ©e de son ancien mobilier avant l'importation pour Ã©viter les doublons.

## 3. Synchronisation Technique
- Tous les scripts d'extraction et de remapping ont Ã©tÃ© archivÃ©s sur le dÃ©pÃ´t [OpenDAoC-SPB](file:///C:/OpenDAOC_server/ProjetsAnnexes/OpenDAoC-SPB) (branche `spb`) pour rÃ©fÃ©rence future.

## 4. Maintenance (Completed)
- [x] Optimize server startup (Fixed equipment and zone warnings).
- [ ] Optimize server performance (Address `Long TimerService.Tick` warnings).
- [ ] Regular database indexing to speed up player and object lookups.

> [!TIP]
> **VÃ©rification In-Game** : Vous pouvez maintenant vous connecter en Avalon. Le HÃ©raut devrait se trouver devant vous, et les Portes/Forges/Coffres de Breamor devraient Ãªtre visibles et utilisables.

- **Optimize server startup**: Fixed equipment and zone warnings (GuardCorpseSummoner null inventory, OFAssistants DB lookup).
