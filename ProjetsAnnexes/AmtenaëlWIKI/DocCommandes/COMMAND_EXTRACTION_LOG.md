# Log d'Extraction des Commandes OpenDAOC

## Date : 2026-03-31 - 2026-04-01

### Objectif
Extraire ~400 commandes du code source C# (`OpenDAOC-SPB`), résoudre les traductions (FR/EN) et les publier sur le wiki Amtenaël en 3 catégories (Joueur, GM, Admin).

### Chronologie des Travaux

1.  **Extraction (OK)** : Script PowerShell `GenerateWikiCommands.ps1` écrit pour parser les attributs `[CmdAttribute]`.
2.  **Traduction (OK)** : Gestion des clés de traduction avec fallback anglais automatique.
3.  **Mise en forme (Blocage)** : Tentative initiale d'utiliser des tableaux DokuWiki complexes (`^ Header ^` et `| Cell |`).
4.  **Upload (Échec Prolongé - 5h)** :
    *   Problème 1 : Les descriptions contenant des pipes `|` brisaient le rendu des tableaux.
    *   Problème 2 : `browser_press_key` échouait sur les caractères accentués (`é`, `à`), interrompant la saisie.
    *   Problème 3 : Les timeouts Playwright (120s) empêchaient l'envoi de tableaux massifs (>200 lignes).
5.  **Changement de Méthode (Pivot)** : Basculement vers un format **Liste à puces simplifiée** (`* **Commande** : Description`). Suppression des accents pour l'ASCII propre.
6.  **Finalisation** : Utilisation du format simplifié pour un upload rapide et fiable.

### Leçons Apprises

*   **Complexité vs Vitesse** : Les tableaux DokuWiki sont esthétiques mais extrêmement fragiles lors d'une injection automatisée par IA (sensibilité aux pipes et retours à la ligne). Une liste simple est 10x plus robuste.
*   **Documentation Projet** : Le `WIKI_GUIDE.md` existant contenait la solution au problème des accents dès le début. **Toujours lire les guides de contribution avant de démarrer un workflow navigateur.**
*   **Time-boxing** : Si un outil de navigation (Playwright) boucle sur une erreur de caractère ou de timeout plus de 2-3 fois, la méthode d'injection (saisie clavier) doit être abandonnée au profit d'une injection de valeur brute (`input.value = ...`) ou d'une simplification radicale du contenu.

### État de l'Upload
*   **Admin** : Complété (Format simplifié).
*   **GM** : Incomplet (Partiel).
*   **Joueur** : Incomplet (Partiel).

*Prochaine étape : Terminer l'upload des listes GM et Joueur sous forme de listes à puces.*
