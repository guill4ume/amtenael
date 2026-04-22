# CONTEXTE ARCHITECTURE OPENDAOC - POUR IA

**Note à l'IA traitant ce fichier :** Lis ce fichier en priorité absolue avant de modifier des fichiers ou de proposer des commandes.

## 1. Environnements
Le projet est scindé en deux serveurs distincts :
- **SERVEUR PROD** (`C:\OpenDAOC_server\OpenDAoC-Core-master`) : Le serveur principal basé sur la branche officielle d'OpenDAoC. Il est configuré nativement en `GAME_TYPE: "PvP"` (dans le `docker-compose.yml`).
- **SERVEUR TEST / SPB** (`C:\OpenDAOC_server\ProjetsAnnexes\OpenDAoC-SPB`) : Serveur de test basé sur SinglePlayerBots. Également configuré en `GAME_TYPE: "PvP"`.
> **Note sur le mode PvP :** Ce mode (actif sur les deux serveurs) autorise par défaut de **grouper avec n'importe qui (cross-realm)**. Il permet aussi d'**attaquer n'importe quel joueur** de n'importe quel royaume, *sauf* s'il fait partie de votre propre groupe ou guilde.
- **Base de Données** : Clone de OpenDAoC-Database. Une interface phpMyAdmin est disponible localement.

## 2. Architecture de Compilation (Critique)
- **Le problème historique** : Le paramètre d'origine `ENABLE_COMPILATION: "True"` (compilation des scripts C# au démarrage par le conteneur Linux .NET 8) **NE FONCTIONNE PAS FIABLEMENT**. Il cause des crashs `Bad IL Format` en production à cause de dépendances système introuvables.
- **La solution implémentée** : 
  - La Prod a été modifiée dans `docker-compose.yml` : l'instruction `image:` a été remplacée par `build: .`
  - Le paramètre `ENABLE_COMPILATION` a été passé à `"False"`.
  - **Conséquence** : Les scripts C# custom sont compilés par le SDK Microsoft local lors du `docker compose build`. C'est robuste et ça empêche le lancement du serveur si le code C# contient la moindre faute de syntaxe.

## 3. Règles C# pour les Scripts Personnalisés
- **Emplacement** : Tous les scripts maison DOIVENT être placés dans le dossier `GameServer/custom/` ou `GameServer/scripts/customnpc/`.
- **Librairies obsolètes** : NE PAS UTILISER `log4net` (ex: `ILog log = ...`). Le moteur moderne tourne sous `NLog`. Utiliser `Console.WriteLine` ou les adaptateurs NLog.
- **Requêtes Joueurs** : 
  - En Prod, pour itérer sur tous les joueurs connectés, utiliser : `foreach (GamePlayer player in ClientService.Instance.GetPlayers())`.

## 4. Stratégie de Mise à jour (DevOps) & GitHub Fork
Les deux serveurs sont unifiés sur un dépôt "Fork" GitHub unique (ex: `https://github.com/guill4ume/amtenael`), avec deux branches distinctes :
- La branche `master` pour la Production (Core).
- La branche `spb` pour le server de Test (SPB).
- La branche `global` pour la racine (`C:\OpenDAOC_server`) : Documentation, TODO et scripts de pilotage.
- **Mises à jour Officielles** : Les deux serveurs peuvent synchroniser le code officiel via un `git pull upstream master` (OpenDAoC).
- **Mise à jour en Production** : Le fichier `C:\OpenDAOC_server\Mettre_A_Jour_Prod.bat` gère la mise à jour (git add, git commit local automatique des modifs, pull de l'upstream, push sur GitHub origin, et rebuild Docker). **Finis les `git stash` fragiles qui perdaient les données.**

## 5. Workflow de Développement et Sauvegardes
- **Développement** : Les scripts maison sont créés et testés sur TEST (SPB). 
- **Sauvegarde SPB** : Se fait dorénavant via un simple `git commit` et `git push origin spb`.
- **Mise en Production** : L'utilisateur copie-colle les fichiers validés depuis le dossier `custom` de SPB vers celui de Prod. Ensuite, il lance simplement `Mettre_A_Jour_Prod.bat`. Le script s'occupe de faire le `commit` de sauvegarde locale et d'envoyer (push) le tout de manière sécurisée sur GitHub.

## 6. Précautions et Dépannage (Restauration SPB)
- **Fichiers Dupliqués C#** : Ne **JAMAIS** laisser de fichiers copies Windows (ex : `MonScript(1).cs` ou `MonScript - Copie.cs`) dans le dossier projet. Le compilateur `.NET` va lire tous les `.cs` et plantera au démarrage avec une erreur `CS0111` (Duplication de classe).
- **Encodage de la Base de Données** : Attention aux sauvegardes générées via PowerShell avec `>`. Elles sont souvent encodées en UTF-16, ce qui empêchera l'import dans MariaDB (Erreur `ASCII '\0'`). Il faut forcer l'export en UTF-8 (ex: `| Out-File -Encoding utf8`).