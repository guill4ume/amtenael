# Quick Start pour l'IA (Maintenance Wiki Amtenaël)

Si vous reprenez ce projet, voici les informations essentielles pour être opérationnel immédiatement :

## 1. Accès Wiki
- **URL** : [amtenaelwiki.fr](http://amtenaelwiki.fr)
- **Identifiants** : Voir le fichier [brainwiki.txt](file:///C:/OpenDAOC_server/ProjetsAnnexes/Amtena%C3%ABlWIKI/brainwiki.txt).

## 2. Extraction des Commandes (OpenDAOC)
Les commandes sont extraites directement du code source C#.
- **Script PowerShell** : [GenerateWikiCommands.ps1](file:///C:/OpenDAOC_server/ProjetsAnnexes/Amtena%C3%ABlWIKI/DocCommandes/GenerateWikiCommands.ps1).
- **Fonctionnement** : Le script scanne les attributs `[CmdAttribute]`, gère les traductions FR/EN et génère 3 fichiers `.txt` de sortie.

## 3. Workflow d'Upload (IMPORTANT)
Consultez impérativement le [WIKI_GUIDE.md](file:///C:/OpenDAOC_server/ProjetsAnnexes/Amtena%C3%ABlWIKI/WIKI_GUIDE.md).
- **REGLÉ D'OR** : Si Playwright échoue sur un format complexe (tableaux), passez immédiatement en format **liste à puces simplifiée** (`* **Cmd** : Desc`).
- **Encodage** : Utilisez impérativement de l'ASCII pur pour les saisies automatiques.

## 4. Suivi
- Consultez le [wiki_upload_tracker.json](file:///C:/OpenDAOC_server/ProjetsAnnexes/Amtena%C3%ABlWIKI/wiki_upload_tracker.json) pour savoir quelles pages ont été traitées et le temps passé.
- Consultez le [COMMAND_EXTRACTION_LOG.md](file:///C:/OpenDAOC_server/ProjetsAnnexes/Amtena%C3%ABlWIKI/DocCommandes/COMMAND_EXTRACTION_LOG.md) pour l'historique des méthodes testées.
