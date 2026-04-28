# Analyse des Scripts Amtenaël (Breamor)

Le dossier `AmteScripts` contient plus d'une centaine de scripts implémentant les mécaniques spécifiques et historiques du serveur Amtenaël pour l'émulateur Dawn of Light. Étant donné l'immensité du dossier, l'analyse a été structurée par catégorie/dossier principal pour en expliquer l'architecture logique :

> [!NOTE]
> **Précision sur le code natif vs Amtenaël :**
> Vous aviez raison ! Une seconde passe sur le code a permis de confirmer que certaines mécaniques étaient déjà gérées nativement par le cœur OpenDAoC.
> - **Ce qui a été retiré de cette liste car natif à OpenDAoC :** Les pierres de Bind (`bind.cs`), les téléporteurs (`GameTeleporter.cs`) et les banquiers/coffres de guilde.
> - **Ce qui reste exclusif à Amtenaël :** Le système de Prison, le Casier, les TextNPC, et le système RP de permission des noms (`DBNamePermission`). L'anonymat d'OpenDAoC se limite en effet seulement à la commande `/anon` et au RvR.
> La liste ci-dessous a été entièrement nettoyée pour ne garder que vos exclusivités Amtenaël !

## 1. Systèmes de Base & Règles (Managers & Management)
*Cœur des mécaniques altérées du jeu.*
- **`Managers/AmtenaelRules.cs`** : C'est la pierre angulaire du serveur (!). Ce script redéfinit de A à Z les règles de PvP, qui peut grouper avec qui, qui gagne de l'expérience, les règles d'utilisation de l'équipement (selon la classe et l'affinité), et gère surtout **le Roleplay de l'anonymat** (un joueur ne voit pas le nom d'un autre tant qu'il n'en a pas la permission !).
- **`Managers/Loot...`** : Gestion personnalisée du butin et générateurs d'argent.
- **`Management/AmtenaelLauncher.cs` et `DeathLog.cs`** : Scripts liés au démarrage du serveur et à la journalisation des lancements/décès en base de données.

## 2. Commandes (Commands)
*Les commandes textuelles tapées en jeu.*
- **Dossier `GM`** : Une vingtaine de commandes réservées au staff. On y trouve par exemple `/casier` (pour insérer des notes disciplinaires secrètes ou publiques sur le compte d'un joueur), ainsi que des commandes pour gérer le RvR, cloner des NPCs en masse (`MassCopy`), ou se rendre invisible.
- **Dossier `Player`** : Commandes pour les joueurs normaux comme `/banque`, lire un livre, visualiser son Karma ou gérer les permissions des noms (cf. le RP mentionné ci-haut).

## 3. Factions & Guildes (Factions, GvG)
*Le conflit unique d'Amtenaël.*
- **`Factions/BreamorFactionMgr.cs`** : Gère l'alignement des joueurs. Au lieu du classique Albion/Midgard/Hibernia, les joueurs s'alignent entre **Constructeurs** (> 10 000 points) et **Destructeurs** (< -10 000 points). Des grades RP sont attribués (ex: Diwaller, Bâtisseur, Bourreau de la destruction). Gagne ou perd des points en PvP selon son camp.
- **`GvG` (Guild vs Guild)** : Mécaniques, gardes, et commandes pour les batailles contrôlées entre guildes.

## 4. Personnages Non-Joueurs & Objets (GameObjects)
*C'est la catégorie la plus dense, elle contient tout ce qui "vit" sur le serveur.*
- **`AmteMob` / `NightMob`** : Des monstres standards aux monstres n'apparaissant que la nuit.
- **`TextNPC`** : Le système de dialogue iconique d'Amtenaël ! Des PNJ dont les discussions, conditions de quêtes, et échanges commerciaux sont entièrement pilotés par les textes rangés en base de données plutôt qu'en dur dans le code.

## 5. Zones de Guerre (PvP, PvP2, RvR)
*Tout ce qui permet de se taper dessus.*
- **`PvP` et `PvP2`** : Gère les téléportations de groupe, et les scripts des zones où tout le monde peut s'attaquer librement.
- **`RvR/LordRvR.cs`** : Un système qui gère des "Seigneurs" de RvR octroyant des bonus d'xp et de royaume massif lorsqu'on se bat près d'eux.

## 6. L'immersion (AutomatedEvents, Books, Jail)
*Pour rythmer la vie en jeu.*
- **`AutomatedEvents/BossEvent.cs`** : Scripts pour lancer périodiquement de gros raids scénarisés sans intervention d'un MJ. (Lié au dossier `AmteBoss` qui définit l'IA des boss).
- **`Books`** : Système de Copiste et Libraire (écrire un livre, le sauvegarder en DB, le recopier et se l'échanger).
- **`Jail`** : Le système carcéral. Géré par le `Geolier`, le joueur enfermé ne peut plus utiliser la plupart de ses capacités.

## 7. Divers (SpecialItems, Spells, DB)
- **`SpecialItems`** : Objets amusants qui ont des propres scripts cliquables (Feux d'artifice, Anneau des Guarks, Bière).
- **`DB`** : Les modèles de données (Tables) pour sauvegarder en SQL le casier, le fret, les permissions d'affichage des noms et le registre des morts de chaque objet.
