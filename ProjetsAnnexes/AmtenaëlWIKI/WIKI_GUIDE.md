# Guide de Contribution pour l'IA (Wiki Amtenaël)

Ce guide est destiné à l'assistant IA en charge de la maintenance du wiki **amtenaelwiki.fr**.

## Moteur de Wiki
Le site utilise **DokuWiki**. Utilisez la barre latérale pour naviguer dans les "Namespaces".

## Flux de Travail Recommandé

1.  **Préparation** : Placez les nouveaux fichiers `.txt` dans le dossier `reception/`.
2.  **Mapping des Pages** :
    *   Utilisez le format `categorie:nom_page`.
    *   Exemple : `systemes_prison.txt` -> `systemes:prison`.
3.  **Traitement des Textes (Playwright Workaround)** :
    *   **ATTENTION** : L'outil `browser_press_key` peut échouer sur les caractères accentués (`é`, `à`, `ç`).
    *   **Action** : Remplacez les accents par leurs équivalents simples avant l'upload pour garantir l'enregistrement.
4.  **Batching** : Ne traitez pas plus de 3-4 pages par appel de subagent navigateur pour éviter les surcharges de DOM.
5.  **Enregistrement** : Utilisez le tracker `wiki_upload_tracker.json` pour marquer les tâches comme terminées.

## Time-boxing et Changement de Stratégie

> [!CAUTION]
> Ne restez pas bloqués sur un formatage complexe (Tableaux, Injections JS lourdes) si l'upload échoue à plusieurs reprises.

*   **Règle des 2 tentatives** : Si un upload échoue 2 fois consécutivement (timeout, erreur de caractères), basculez **immédiatement** sur un format texte simplifié (listes à puces `* `).
*   **Limitation Temporelle** : Ne pas passer plus de 30 minutes sur un seul fichier sans mise à jour majeure visible du tracker. Si le blocage persiste, demandez l'avis de l'utilisateur ou simplifiez.
*   **Encodage** : Favorisez toujours l'ASCII pur pour les uploads automatisés via Playwright pour éviter les interruptions d'`Unknown key`.

## Exemple de commande de création
URL directe pour l'édition : `http://amtenaelwiki.fr/[namespace]:[page]?do=edit`
