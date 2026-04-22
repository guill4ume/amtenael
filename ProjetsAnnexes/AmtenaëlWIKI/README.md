# Amtenaël Wiki Documentation Project

Ce projet a pour but de documenter les systèmes du serveur Amtenaël sur le site [amtenaelwiki.fr](http://amtenaelwiki.fr).

## Structure du Dossier

- **archive/** : Contient les fichiers `.txt` déjà intégrés au wiki.
- **reception/** : Emplacement pour les nouveaux fichiers à uploader.
- **WIKI_GUIDE.md** : Guide technique complet pour les futures IAs (moteur, méthodes, erreurs connues).
- **wiki_upload_tracker.json** : Suivi de l'état des uploads.
- **DocCommandes/** : Sous-projet d'extraction automatique des commandes depuis le code source C#.

## Projets en Cours

### Extraction des Commandes (OpenDAOC)
Extraction automatisée des attributs `[CmdAttribute]` pour générer une documentation multilingue (FR/EN) sur le wiki.
- **Script principal** : [/DocCommandes/GenerateWikiCommands.ps1](file:///C:/OpenDAOC_server/ProjetsAnnexes/Amtena%C3%ABlWIKI/DocCommandes/GenerateWikiCommands.ps1)
- **Historique et Leçons** : [/DocCommandes/COMMAND_EXTRACTION_LOG.md](file:///C:/OpenDAOC_server/ProjetsAnnexes/Amtena%C3%ABlWIKI/DocCommandes/COMMAND_EXTRACTION_LOG.md)

## Informations d'Accès

- **Lien** : [amtenaelwiki.fr](http://amtenaelwiki.fr)
- **Compte** : `brainwiki`
- **Moteur** : DokuWiki
