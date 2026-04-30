# Étude et Idéation : Système de Bots Multi-Tiers

**Date** : 29 Avril 2026
**Objectif** : Conserver les bots "Lourds" actuels (haute qualité) tout en introduisant des architectures pour des bots "Légers" et "Ultra-Légers" afin de peupler massivement le serveur sans impacter les performances CPU/RAM.

---

## 1. Rappel : Le Bot Lourd (MimicNPC Actuel)
**Statut** : À conserver tel quel pour les interactions poussées.
*   **Caractéristiques** : Implémente `IGamePlayer`, inventaire complet généré aléatoirement, calcul des stats via bonus d'équipement, IA complexe (FSM à 10 états), ThinkInterval à 500ms.
*   **Rôle** : Membres de groupe pour les joueurs (PvE), Gardes du corps, Bosses RvR, Champions d'arène.
*   **Capacité max estimée** : ~50-100 bots selon le hardware.

---

## 2. Le Bot "Léger" (LightMimic)
**Statut** : [x] IMPLÉMENTÉ (Proof of Concept validé)
**Objectif** : Un bot capable de participer au RvR de manière crédible, d'utiliser quelques compétences, mais très économe en RAM et CPU.
*   **Rôle** : Troupes régulières de Thidranki, patrouilles de forts.
*   **Capacité max estimée** : ~300-500 bots. (Actuellement 150)

### Idées d'optimisation (Léger) :
1.  **Fake Inventory (Gain RAM)** : Supprimer `MimicNPCInventory`. Utiliser l'équipement visuel standard des PNJs (`EquipmentTemplateID`) qui ne charge qu'une liste d'IDs d'objets pour le rendu client.
2.  **Stats Pré-calculées (Gain CPU/RAM)** : Au lieu de calculer les stats (Force, Dex, AF) dynamiquement en lisant les objets, on assigne un bloc de stats fixes à la création du bot selon sa classe et son niveau.
3.  **Brain Simplifié (Gain CPU)** : 
    *   Augmenter le `ThinkInterval` à **1500ms** ou **2000ms**.
    *   Réduire la FSM à 3 états : `Idle/Patrol`, `Aggro`, `Dead`.
4.  **Ciblage Evénementiel** : Au lieu que chaque bot scanne autour de lui toutes les secondes (`GetPlayersInRadius`), on peut utiliser un système où les cibles potentielles "s'annoncent" aux bots proches, ou utiliser un système de grille spatiale (Spatial Hashing).
5.  **Sorts et Styles "Trompe-l'œil"** : Ne pas gérer la véritable liste de sorts de la classe. Donner au bot 2 ou 3 compétences hardcodées (ex: un root, un nuke, un heal) qu'il lance avec un cooldown fixe.

---

## 3. Le Bot "Ultra-Léger" (SwarmBot / ProxyMob)
**Statut** : [x] IMPLÉMENTÉ
**Objectif** : Peupler le monde massivement (guerres de grande ampleur, invasions) avec un coût quasi-nul par entité.
*   **Rôle** : Figurants RvR, armées d'invasion, chair à canon.
*   **Capacité max estimée** : 1000+ bots (similaire au chargement des 95k mobs). (Actuellement 300)

### Idées d'optimisation (Ultra-Léger) :
1.  **Architecture Pure `GameNPC` (Gain RAM massif)** : Ne pas hériter de composants joueurs. C'est un PNJ standard avec un modèle (Skin) de joueur humain/elfe/etc.
2.  **Swarm Intelligence (Gain CPU massif)** : 
    *   Au lieu que chaque bot ait un `Brain` qui réfléchit, on crée un **`SwarmManager`** global.
    *   Le `SwarmManager` déplace des groupes entiers (ex: 50 bots) comme une seule unité logique. Il calcule le pathfinding une seule fois pour le groupe. Les bots ne font que suivre le leader du groupe bêtement.
3.  **Combat Déterministe** : 
    *   Désactiver les calculs de pénétration d'armure, de résistances complexes ou d'évasion.
    *   Dégâts = `Niveau * Constante`. Si un Ultra-Léger frappe, c'est un calcul instantané.
4.  **Zonage Actif/Passif** : 
    *   Si aucun joueur réel n'est dans une zone de 5000 unités, les Ultra-Légers arrêtent totalement de penser et de bouger (ils passent en hibernation). Leurs combats entre royaumes sont simulés mathématiquement en arrière-plan sans rendu 3D.
5.  **Instanciation par Pool** : Pré-allouer 1000 objets bots au démarrage du serveur (Object Pooling) pour éviter les allocations/désallocations mémoire (`garbage collection`) lors des morts et respawns de ces armées.

---

## 4. Pistes d'évolution transverse (Architecture Globale)
*   **Spatial Hashing** : Le goulot d'étranglement principal des bots est la recherche de cibles (Aggro). Implémenter une grille de collision (Grid Spatial Index) permettrait aux bots de trouver les cibles instantanément sans scanner toute la zone.
*   **Offloading de l'IA** : Déporter le calcul de l'IA de l'armée sur un Thread séparé (ou un Task asynchrone) pour ne jamais ralentir le `GameLoop` principal du serveur.

*Note : La preuve de concept pour le bot "Léger" et le "SwarmBot" a été finalisée le 30 Avril 2026.*
