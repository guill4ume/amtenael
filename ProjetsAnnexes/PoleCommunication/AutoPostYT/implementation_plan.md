# Plan : Automatisation Post YouTube (AutoPostYT)

Ce projet vise à automatiser l'upload de la vidéo la plus ancienne du dossier `processed` vers votre chaîne YouTube.

## User Review Required

> [!IMPORTANT]
> L'API YouTube nécessite une authentification **OAuth2**. Vous devrez effectuer une configuration unique dans la console Google Cloud :
> 1.  Créer un projet Google Cloud.
> 2.  Activer **YouTube Data API v3**.
> 3.  Créer des identifiants **Client OAuth 2.0** (Type : Application de bureau).
> 4.  Me fournir le `Client ID` et le `Client Secret`.

> [!WARNING]
> Le premier lancement demandera une interaction manuelle pour autoriser l'application et générer un `Refresh Token`. Une fois fait, l'automatisation pourra tourner sans intervention.

## Proposed Changes

Le projet sera situé dans `C:\OpenDAOC_server\ProjetsAnnexes\PoleCommunication\AutoPostYT\`.

### 1. Script d'Authentification (`auth.ps1`)
- Script PowerShell pour gérer l'échange de jetons Google (OAuth2).
- Stockage sécurisé du `refresh_token` dans un fichier local.

### 2. Script de Publication (`post_to_yt.ps1`)
- **Workflow** :
    - Scan de `C:\OpenDAOC_server\ProjetsAnnexes\PoleCommunication\AutoEditVideo\processed\`.
    - Identification de la vidéo la plus ancienne.
    - Rafraîchissement du jeton d'accès.
    - Upload "Résumable".
    - **Métadonnées** : 
        - **Visibilité** : `public`.
        - **Catégorie** : Gaming (ID 20).
        - **Mots-clés (SEO)** : `Amtenael`, `DAoC`, `Dark Age of Camelot`, `OpenDAOC`, `Server Development`.
        - **Description** : Inclusion automatique d'une signature "Serveur Amtenaël - Développement OpenDAoC".

### 3. Documentation Agent-First
- **`README.md`** : Instructions pour les futurs assistants IA.
- **`AI_CONTEXT.md`** : Architecture de l'API et scopes utilisés.

## Config. Youtube validée
1.  **Visibilité** : Public.
2.  **SEO** : Focus sur "Amtenaël" et "DAoC" (mots-clés intégrés).
3.  **Client ID / Secret** : À fournir par l'utilisateur pour finaliser le script `auth.ps1`.

## Verification Plan

### Manual Verification
- Test d'upload avec une petite vidéo de test.
- Vérification de l'apparition de la vidéo dans le **YouTube Studio**.
