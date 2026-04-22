# CONTEXTE ARCHITECTURE PoleCommunication - POUR IA

**Note à l'IA traitant ce dossier :** Ce dossier contient les outils d'automatisation pour le montage vidéo simplifié (suppression des silences).

## 1. Objectif du Projet
Le projet `AutoEditVideo` permet de transformer des enregistrements bruts (type OBS) en vidéos condensées en supprimant toutes les zones de silence où l'utilisateur ne parle pas.

## 2. Structure des Dossiers
- **Outils** : `ffmpeg.exe` et `ffprobe.exe` sont regroupés dans `tools/`.
- **Fichiers temporaires** : Les logs et scripts de filtrage sont dans `temp/`.
- **Dossier Source** : `C:\Users\Guillaume\Videos\`
- **Dossier Destination** : `C:\OpenDAOC_server\ProjetsAnnexes\PoleCommunication\AutoEditVideo\processed\`

## 3. Workflow d'Automatisation
1.  **Détection** : Analyse audio via le filtre `silencedetect` de FFmpeg (Seuil : -30dB, Durée min : 0.5s).
2.  **Calcul** : Le script PowerShell `process_silence.ps1` parse les logs et génère des segments de "parole" avec un padding de sécurité.
3.  **Montage** : Utilisation d'un `filter_complex_script` (`filter.txt`) pour concaténer les segments sans perte de synchronisation audio/vidéo.
4.  **Nettoyage** : Si l'export réussit, le fichier source est automatiquement supprimé du dossier `Videos/`.

## 4. Paramètres de Qualité
- **Padding** : Actuellement fixé à **0.5s** (avant et après chaque segment parlé).
- **Codec Vidéo** : `libx264` avec le preset `ultrafast` pour minimiser le temps de calcul.
- **Codec Audio** : `aac` à 128k.

## 5. Maintenance
Si le script échoue sur une vidéo de très grande taille, vérifiez le fichier `filter.txt`. Une trop grande quantité de segments peut parfois saturer la mémoire si FFmpeg n'est pas utilisé avec `-filter_complex_script`.
