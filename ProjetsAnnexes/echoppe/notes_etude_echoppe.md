# Étude du Système d'Échoppe (Marché Itinérant)

Ce document rassemble les notes de l'étude concernant l'intégration d'un système d'échoppe pour les joueurs, en comparant l'ancien système Breamor (Dawn of Light) et les possibilités natives du serveur OpenDAOC-SPB.

## 1. Analyse de la logique de l'ancien système Breamor (DOL)

Dans le système d'origine (fichiers de `datas diverses\Breamor\shop`), la logique était la suivante :

1. **Déploiement (`market.cs`)** :
   - Le joueur utilise la commande `/market open`.
   - Des vérifications strictes sont faites : il faut la compétence `Trading`, être dans une **Safe Area**, ne pas être en combat, sur une monture, AFK ou dans un donjon.
   - Une boîte de dialogue de confirmation (Yes/No) est envoyée au joueur.

2. **L'Entité Échoppe (`TemporaryConsignmentMerchant.cs` & `GameTradingTable.cs`)** :
   - À la confirmation, deux entités apparaissent au sol :
     - `GameTradingTable` : Un objet statique visuel (Modèle 1494 - la fameuse table en bois).
     - `TemporaryConsignmentMerchant` : Le PNJ invisible ou superposé (Modèle 667) avec lequel on interagit.
   - Le marchand hérite du `GameConsignmentMerchant` classique.

3. **Sécurité et Durée de vie (`TempConsignmentBrain`)** :
   - Le PNJ est équipé d'un *Brain* (IA) qui vérifie en boucle que le joueur propriétaire est toujours en ligne, dans la même région, et **à moins de 2000 unités de distance**.
   - Si l'une de ces conditions n'est plus remplie, l'échoppe est supprimée de l'univers (Despawn).

---

## 2. Piste OpenDAOC : Détourner le Vendeur de Maison

L'idée de détourner le marchand d'objets du housing (vendeur de consignation) est la **meilleure solution technique**. Les bases de code d'OpenDAOC disposent déjà de systèmes robustes pour ça.

### Pourquoi c'est le bon choix :
- **L'inventaire est déjà géré** : `ConsignmentMerchant.cs` (`GameServer\gameobjects\CustomNPC\ConsignmentMerchant.cs`) sait comment stocker des objets, définir des prix et faire payer un acheteur.
- **Base de données prête** : `DbHouseConsignmentMerchant` gère déjà l'enregistrement de l'argent récolté.
- **Sécurité** : OpenDAOC utilise `MarketComponent.cs` pour verrouiller les slots d'inventaire et empêcher la duplication d'objets (ce qui manquait ou était instable sur les vieux serveurs DOL).

### Ce qui diffère entre le Housing et l'Échoppe :
- **L'Ancrage** : Un vendeur de maison classique est lié à un `HouseNumber`. L'échoppe doit être liée à un `OwnerID` (le joueur) avec un `HouseNumber = 0` (ce qui était d'ailleurs la technique utilisée dans le code Breamor).
- **La Persistance** : Le vendeur de maison est permanent. L'échoppe est **temporaire** (liée à la présence physique du joueur).

---

## 3. Plan d'Action Théorique pour l'Intégration OpenDAOC

Voici les éléments qu'il faudrait développer ou adapter dans le core OpenDAOC pour finaliser ce système proprement :

1. **Création d'un PNJ Mobile / Temporaire** :
   - Créer une classe (ex: `GameMarketMerchant`) héritant de `ConsignmentMerchant` ou l'adaptant pour qu'il n'exige pas de maison (`houseRequired = false`).
   - S'assurer que le constructeur lie correctement le marchand au composant ECS (`MarketComponent`).

2. **Gestion Visuelle** :
   - Au lieu de gérer deux objets (PNJ + Table) qui peuvent se désynchroniser, on peut donner au PNJ directement le modèle visuel de la table (ex: 1494, 3271, 3141) et modifier sa `MaxSpeed` à 0. 

3. **Système de "Despawn" (Le lien Joueur-Échoppe)** :
   - Au lieu d'utiliser un vieux `Brain` de mob (gourmand en ressources), utiliser un `GameTimer` ou un `ECS System` qui vérifie la distance entre le joueur et son échoppe.
   - Si le joueur s'éloigne trop (rayon de 2000) ou se déconnecte, on déclenche une méthode `CloseMarket()` qui détruit l'objet en jeu (l'argent et les objets restent stockés en DB liés à l'OwnerID).

4. **Commande et UI** :
   - Câbler la commande `/market` (ou similaire) pour vérifier les zones sûres (`IsSafeArea`).
   - Utiliser les événements réseau (ex: `DialogResponseHandler.cs`) pour l'ouverture/fermeture propre.

## Conclusion

L'idée de se baser sur le vendeur de maison est la voie à suivre. Cela évitera de réinventer la roue (gestion de l'or, transfert d'objets) et de conserver l'architecture moderne ECS et anti-triche d'OpenDAOC-SPB.
