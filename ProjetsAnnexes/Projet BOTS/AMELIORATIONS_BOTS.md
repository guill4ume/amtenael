# Réflexions et Pistes d'Amélioration : Système de Bots Amtenaël

**Date** : 30 Avril 2026
**Objectif** : Identifier les leviers pour augmenter l'immersion, la performance et l'intelligence des trois tiers de bots.

---

## 1. Optimisations Performance (Niveau Moteur)

### A. Spatial Hashing (Indexation Spatiale)
*   **Problème** : Actuellement, chaque bot scanne son rayon d'aggro via `GetPlayersInRadius` ou `GetNPCsInRadius`. Sur 500 bots, cela représente des milliers de calculs de distance par seconde.
*   **Solution** : Implémenter une grille virtuelle. Chaque entité s'enregistre dans une "case" de la grille. Un bot ne scanne que sa case et les cases adjacentes.
*   **Impact** : Réduction drastique de la charge CPU, permettant de doubler ou tripler la population sans lag.

### B. Offloading de l'IA (Asynchronisme)
*   **Problème** : L'IA tourne sur le thread principal du `GameLoop`. Un pic d'activité (grosse bataille) peut faire chuter les TPS (Ticks Per Second).
*   **Solution** : Déporter le "Think" des bots sur des `Task.Run` ou un pool de threads dédié. Le thread principal ne fait que l'application des résultats (mouvement, dégâts).
*   **Impact** : Meilleure fluidité globale du serveur.

---

## 2. Intelligence et Coordination (Tier 1 & 2)

### A. Priorisation des Cibles (Focus Smart)
*   **Casters** : Prioriser l'interruption des soigneurs ennemis ou le nuke des tissus (armures légères).
*   **Healers** : IA prédictive qui soigne la cible ayant le plus bas pourcentage de vie dans le groupe, avec gestion de la sur-guérison (overheal).
*   **Tanks** : Utilisation plus intelligente de la garde (`/guard`) sur les membres fragiles du groupe.

### B. Formations et Tactiques de Groupe
*   **Mouvements coordonnés** : Les bots d'un même groupe ne se suivent pas en "chenille" mais adoptent des formations (V, ligne de front, protection du centre).
*   **Repli Tactique** : Si la santé du groupe tombe sous 30%, le groupe tente de battre en retraite vers le fort le plus proche au lieu de mourir sur place.

---

## 3. Immersion et Roleplay

### A. Dialogue et Réactions
*   **Bark System** : Les bots lancent des phrases courtes en `Say` lors de certains événements (victoire, mort d'un allié, incantation d'un gros sort).
*   **Emotes** : Utilisation d'emotes de victoire (`/cheer`, `/victory`) après avoir tué un joueur réel.

### B. Mimétisme de Classe
*   **Gestion du Mana/Endu** : Faire en sorte que les bots doivent gérer leurs ressources (s'asseoir pour regen) de manière visible, créant des opportunités d'embuscade pour les joueurs.
*   **Utilisation de Potions** : Permettre aux bots Lourds d'utiliser des potions de soin/mana en cas d'urgence.

---

## 4. Évolutivité du Monde (Tier 3)

### A. Scaling Dynamique
*   **Principe** : Ajuster automatiquement le nombre de SwarmBots en fonction du nombre de joueurs réels connectés.
*   **But** : Si la zone est vide de joueurs, on réduit la population Swarm pour économiser du CPU. Si 50 joueurs arrivent, on "pop" massivement des renforts pour créer une ambiance de guerre.

### B. Front de Guerre Mobile
*   **Objectif** : Les SwarmBots ne se contentent pas de marcher vers le centre. Ils peuvent capturer des points mineurs (tentes, avant-postes) qui servent de nouveaux points de spawn, créant une ligne de front qui bouge réellement sur la carte.

---

## 5. Roadmap Technique Suggérée

1.  **Court Terme** : Implémenter le **Spatial Hashing** pour sécuriser la montée en charge.
2.  **Moyen Terme** : Ajouter les **Formations de groupe** pour rendre les combats moins chaotiques.
3.  **Long Terme** : Système de **Front de Guerre** avec capture d'objectifs par les SwarmBots.
