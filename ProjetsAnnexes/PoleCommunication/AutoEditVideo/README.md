# Projet AutoEditVideo : Guide de l'IA (Agent-First)

Bienvenue dans l'espace de travail dédié au montage vidéo par détection de parole.

## Accès Rapide
- **Outils (FFmpeg)** : Les binaires sont regroupés dans [tools/](file:///C:/OpenDAOC_server/ProjetsAnnexes/PoleCommunication/AutoEditVideo/tools/).
- **Vidéos éditées** : Les résultats sont déplacés dans le sous-dossier [processed/](file:///C:/OpenDAOC_server/ProjetsAnnexes/PoleCommunication/AutoEditVideo/processed/).
- **Fichiers temporaires** : Les logs et filtres sont dans [temp/](file:///C:/OpenDAOC_server/ProjetsAnnexes/PoleCommunication/AutoEditVideo/temp/).
- **Contexte projet** : [AI_CONTEXT.md](file:///C:/OpenDAOC_server/ProjetsAnnexes/PoleCommunication/AutoEditVideo/AI_CONTEXT.md)

## Tâches d'IA Courantes

### 10. Traiter de nouvelles vidéos
1.  **Vérification** : Assurez-vous que les vidéos sources sont dans `C:\Users\Guillaume\Videos\`.
2.  **Action** : Exécutez le script avec l'option `ExecutionPolicy Bypass`.
    ```powershell
    powershell -ExecutionPolicy Bypass -File "C:\OpenDAOC_server\ProjetsAnnexes\PoleCommunication\AutoEditVideo\process_silence.ps1"
    ```
3.  **Surveillance** : Le script affiche le nom de la vidéo en cours, le nombre de segments et le statut de l'exportation.

### 20. Ajuster la détection
Si l'utilisateur trouve que les coupures sont trop brusques ou que trop de silences sont conservés :
-   **Pour les coupures brusques** : Augmentez `$padding` (valeur actuelle : **0.5**).
-   **Pour les silences conservés** : Augmentez le seuil (ex : `-40dB` au lieu de `-30dB`) ou réduisez la durée `-d` (ex : `d=0.3`).

### 30. Dépannage
-   **Erreur : Invalid argument** : Souvent lié à un caractère spécial dans le chemin du fichier ou au format de `filter.txt` (vérifiez l'encodage du fichier en UTF8 sans BOM).
-   **Vidéo noire/audio décalé** : Le filtre `concat` est sensible à la synchronisation. Assurez-vous d'utiliser `setpts=PTS-STARTPTS` et `asetpts=PTS-STARTPTS`.

## Sécurité et Nettoyage
Le script **supprime l'original** après le succès. Ne relancez jamais une tâche dont vous n'êtes pas sûr sans avoir fait une sauvegarde manuelle au préalable si l'utilisateur l'exige.
