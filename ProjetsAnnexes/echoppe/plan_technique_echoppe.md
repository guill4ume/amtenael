# Plan d'Implémentation Technique (Échoppe)

Suite à notre étude, voici le plan détaillé et validé qui est mis en place :

## 1. La fondation (Entité PNJ Échoppe)
Création du fichier `GameServer\custom\Echoppe\EchoppeMerchant.cs` :
- Hérite de `GameNPC` et implémente `IGameInventoryObject`.
- Remplace la logique `HouseNumber` par l'`OwnerID` (ID du joueur).
- La table de marché est affichée visuellement par le marchand lui-même (Model 1494) qui est statique (MaxSpeed = 0).
- Utilise la base de données existante `DbHouseConsignmentMerchant` pour sécuriser l'argent sans nécessiter de maison.

## 2. Déploiement et Sécurité
Modification du coeur (`GameServer\gameobjects\GamePlayer.cs`) :
- Ajout d'une propriété `EchoppeMerchant ActiveEchoppe { get; set; }`.
- (En cours d'investigation) : Intégrer un système pour gérer la déconnexion inattendue et retirer l'échoppe du monde.

## 3. Commande Utilisateur
Création du fichier `GameServer\commands\playercommands\EchoppeCommand.cs` :
- Commande `/echoppe`.
- Vérifie que le joueur est en `SafeArea`, hors combat, et qu'il possède le talent "Trading".
- "Toggle" : Ouvre l'échoppe si elle est fermée, ou la ferme (la retire du monde).

## 4. Gestion de l'Inventaire (Décision Validée)
- **Conservation des Objets en DB** : À la fermeture de l'échoppe, les objets invendus **ne retournent pas** dans l'inventaire du joueur. Ils restent stockés dans le coffre virtuel lié au joueur (`OwnerID`) pour des raisons de sécurité, évitant toute perte en cas d'inventaire plein ou crash serveur.
