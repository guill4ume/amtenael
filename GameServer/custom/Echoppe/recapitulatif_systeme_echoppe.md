# Récapitulatif : Système d'Échoppe pour OpenDAOC-SPB

Ce document sert de mémoire technique pour la transition du système de marché (échoppe) vers une architecture native OpenDAOC, suite à l'abandon du portage direct depuis Dawn of Light (DOL).

## 1. État des Lieux (Ressources Natives Identifiées)

### Core OpenDAOC
- **GameConsignmentMerchant.cs** : Situé dans `GameServer\gameobjects\CustomNPC\`. C'est le marchand de consignation standard (Housing). Il gère déjà l'inventaire, les achats, les ventes et les logs de transactions.
- **MarketComponent.cs** : Situé dans `GameServer\ECS-Components\`. Un composant léger utilisé pour verrouiller les slots d'inventaire lors d'un achat (anti-dupe) et lier un PNJ à un `OwnerID`.
- **DbHouseConsignmentMerchant.cs** : Table de base de données gérant l'argent stocké par les marchands.

### Éléments déjà intégrés (À conserver)
- **SkillConstants.cs** : L'aptitude `Trading` a été ajoutée.
- **ServerProperties.cs** : La propriété `TRADING_TAX` (taxe de vente) a été ajoutée.
- **IPacketLib.cs / DialogResponseHandler.cs** : Les codes de dialogue `OpenMarket` et `CloseMarket` sont prêts pour gérer les confirmations oui/non.

## 2. Pourquoi le portage DOL a été abandonné
- **Conflits de noms** : OpenDAOC possède déjà ses propres classes de marché, créant des erreurs de compilation massives.
- **Architecture divergente** : OpenDAOC utilise un système ECS (Entity Component System) pour certaines parties du commerce, alors que DOL repose sur des scripts monolithiques.
- **Obsolescence** : Le code DOL historique utilise des structures (comme `StandardMobBrain`) qui ne sont plus optimales par rapport aux `ECS-Services` d'OpenDAOC.

## 3. Stratégie pour la Nouvelle Conversation
L'objectif est de créer un **"Marché Itinérant"** (Échoppe) en se basant sur le code du `GameConsignmentMerchant` du Housing.

### Pistes Techniques :
1.  **Héritage ou Imitation** : Créer une classe `GameMarketMerchant` qui hérite de `GameConsignmentMerchant`.
2.  **Détournement du Propriétaire** : Modifier la logique pour que le marchand ne cherche pas une `House` (maison) mais un `GamePlayer` (le joueur vendeur).
3.  **Composant ECS** : Attacher un `MarketComponent` au PNJ échoppe pour bénéficier du système de verrouillage de slot natif.
4.  **Localisation** : Utiliser la commande `/market open` pour déployer ce PNJ uniquement en zone "Safe" (zones de villes ou zones RP).
5.  **Visuels (Modèles à tester en jeu) :**
    - Table de vente standard : `1494` (confirmé)
    - Table d'Alchimie : `3271` ou `3140`
    - Poste d'Empennage (Fletching) : `3141`
    - Petite Table / Bureau : `1125`
    - Forge portative : `2110`
    - Présentoir d'armes : `1044`

## 4. Fichiers à surveiller
- `GameServer\gameobjects\GamePlayer.cs` : Contient déjà la propriété `TemporaryConsignmentMerchant` pour le suivi du marché actif.
- `GameServer\gameobjects\CustomNPC\ConsignmentMerchant.cs` : Le fichier de référence à étudier.

---
*Dossier de travail : C:\OpenDAOC_server\ProjetsAnnexes\OpenDAoC-SPB\GameServer\custom\Echoppe*
