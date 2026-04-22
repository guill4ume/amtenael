# Projet AutoPostYT : Guide de l'IA (Agent-First)

Bienvenue dans l'espace de travail dédié à l'automatisation de la mise en ligne des vidéos sur YouTube.

## Accès Rapide
- **Plan d'implémentation** : [implementation_plan.md](file:///C:/OpenDAOC_server/ProjetsAnnexes/PoleCommunication/AutoPostYT/implementation_plan.md)
- **Contexte projet** : [AI_CONTEXT.md](file:///C:/OpenDAOC_server/ProjetsAnnexes/PoleCommunication/AutoPostYT/AI_CONTEXT.md)
- **Logique d'upload** : [post_to_yt.ps1](file:///C:/OpenDAOC_server/ProjetsAnnexes/PoleCommunication/AutoPostYT/post_to_yt.ps1) (En attente)

## Tâches d'IA Courantes

### 10. Initialisation (Credentials)
Si l'utilisateur fournit les clés `Client ID` et `Client Secret` :
1.  **Configuration** : Mettez à jour le fichier `config.json` (à créer) avec ces valeurs.
2.  **Authentification** : Exécutez `auth.ps1` pour obtenir le `refresh_token`. L'utilisateur devra cliquer sur un lien Google et copier le code.

### 20. Publication Auto
Une fois les jetons obtenus :
```powershell
powershell -ExecutionPolicy Bypass -File "C:\OpenDAOC_server\ProjetsAnnexes\PoleCommunication\AutoPostYT\post_to_yt.ps1"
```
Le script recherchera la vidéo la plus ancienne dans `AutoEditVideo/processed/` et la mettra en ligne.

### 30. Paramètres SEO
Les métadonnées sont optimisées pour le serveur **Amtenaël**. Ne modifiez pas les tags sans approbation explicite de l'utilisateur.

## Scopes API
- `youtube.upload` : Utilisé pour la mise en ligne.
- `youtube.readonly` : Utilisé pour vérifier le statut des uploads précédents.
