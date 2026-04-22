# CONTEXTE ARCHITECTURE AutoPostYT - POUR IA

**Note à l'IA traitant ce dossier :** Ce dossier est dédié à l'automatisation de la publication des vidéos éditées sur YouTube.

## 1. Objectif du Projet
Récupérer automatiquement la vidéo la plus ancienne du dossier `AutoEditVideo/processed/` et la poster sur la chaîne YouTube de l'utilisateur avec les métadonnées SEO optimisées.

## 2. Intégration API YouTube
- **Service** : YouTube Data API v3.
- **Authentification** : OAuth2 (Application de bureau).
- **Scopes requis** : `https://www.googleapis.com/auth/youtube.upload`.

## 3. Paramètres de Publication validés
- **Visibilité** : Public.
- **Catégorie** : Gaming (ID 20).
- **Keywords/SEO** : `Amtenael`, `DAoC`, `Dark Age of Camelot`, `OpenDAOC`, `Server Development`.

## 4. État du projet (Alpha)
Les scripts sont actuellement en attente des identifiants `Client ID` et `Client Secret`. Ne tentez pas d'exécuter l'authentification sans ces clés.
