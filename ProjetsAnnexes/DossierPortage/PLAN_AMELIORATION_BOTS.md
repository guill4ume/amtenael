# Plan d'Amélioration : Navigation et Siège des Bots (Mimics)

**Contexte :**
Dans la version actuelle, les bots respectent les collisions et la géométrie du monde (portes, murs). En arrivant au fort de Thidranki, ils battent les gardes extérieurs, puis se retrouvent bloqués par la porte du fort ennemi qu'ils ne savent pas ouvrir ni détruire (la méthode `GameKeepDoor.Open()` bloque ce comportement pour les ennemis). De plus, leur gestion de l'eau (bien qu'ils ne se noient plus) reste hasardeuse s'ils entrent en combat aquatique et cherchent une cible inatteignable.

**Améliorations proposées pour l'avenir :**
1. **Destruction magique des portes ennemies** : Dans `MimicBrain.cs` (méthode `DetectDoor()`), actuellement les bots ignorent les portes ennemies (`if (Body.Realm != door.Realm) return;`). Il est proposé de modifier cela pour qu'un bot Mimic inflige un montant de dégâts fatal à la porte lorsqu'il s'en approche, ce qui simulera le fait qu'ils aient fait un "siège" rapide.
2. **Déblocage intelligent en combat** : Si un bot est bloqué trop longtemps en essayant d'atteindre une cible (derrière un mur ou dans l'eau), ajouter un mécanisme de téléportation ("Blink") à côté de la cible, ou bien un script pour forcer l'abandon de l'agressivité envers les cibles inatteignables.
3. **Correction de l'élévation (Z)** dans l'eau : Forcer la mise à jour de leur axe Z (hauteur) lorsqu'ils naviguent dans l'eau ou désactiver temporairement les checks de collision Z pour éviter qu'ils flottent en l'air sans pouvoir avancer.

**Avantages attendus par rapport à un mode sans collision (Ghost) :**
- Immersion préservée : Ils interagissent avec les portes (destruction visible) au lieu de simplement passer à travers les murs.
- Bataille de château : Ils envahissent le fort par la porte brisée et combattent le Lord à l'intérieur de manière bien plus réaliste et gérable par le moteur de collision.
