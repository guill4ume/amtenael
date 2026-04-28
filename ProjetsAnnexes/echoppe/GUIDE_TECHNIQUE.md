# Guide Technique : Échoppe Personnelle (OpenDAOC)

Ce dossier centralise la stratégie pour implémenter un système d'échoppe (marché itinérant) sur OpenDAOC-SPB en utilisant l'architecture native du serveur.

## 1. Approche Native vs DOL
Nous avons abandonné le portage direct du code DOL pour éviter les conflits et l'obsolescence. La nouvelle stratégie repose sur le détournement du système de **Housing** existant.

### Composants Clés à utiliser :
- **GameConsignmentMerchant** : La classe de base pour les marchands de maison. Elle gère déjà l'inventaire, les prix de vente et les transactions.
- **MarketComponent** : Le composant ECS qui sécurise les transactions (anti-dupe).
- **TemporaryConsignmentMerchant** : Propriété déjà ajoutée à `GamePlayer.cs` pour stocker l'instance active du joueur.

## 2. Plan d'Action
1. **Classe Custom** : Créer `MarketMerchant` héritant de `GameConsignmentMerchant`.
2. **Liaison Joueur** : Remplacer la liaison à la `House` par une liaison directe au `GamePlayer`.
3. **Commande `/market`** :
    - `open` : Déploiement du PNJ (modèle 1494) à la position du joueur.
    - `close` : Retrait du PNJ.
    - `name` : Personnalisation du titre de l'échoppe.
4. **Sécurité** :
    - Vérification des zones (Safe Zones uniquement).
    - Auto-fermeture si le joueur s'éloigne trop.
    - Utilisation du `MarketComponent` pour les verrous d'inventaire.

## 3. Éléments déjà en place dans le Core
- Aptitude `Trading` ajoutée.
- Propriété serveur `TRADING_TAX`.
- Codes de dialogue `OpenMarket` / `CloseMarket`.
- Interaction `PickupObject` mise à jour pour permettre la fermeture en cliquant sur la table.
