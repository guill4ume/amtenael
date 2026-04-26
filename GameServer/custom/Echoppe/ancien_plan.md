# Ancien Plan d'Exécution (Phase DOL)

Ce plan était celui utilisé lors de la tentative de portage DOL. Il contient des détails sur les fichiers modifiés dans le Core.

## Fichiers Core Modifiés
- **SkillConstants.cs** : Ajout de `Abilities.Trading`.
- **ServerProperties.cs** : Ajout de `TRADING_TAX`.
- **IPacketLib.cs** : Ajout de `eDialogCode.OpenMarket` et `CloseMarket`.
- **GamePlayer.cs** : 
    - Ajout de la propriété `TemporaryConsignmentMerchant`.
    - Modification de `PickupObject` pour gérer l'interaction avec la table de marché.
- **DialogResponseHandler.cs** : Ajout des cas `OpenMarket` et `CloseMarket`.

## Logiciels Personnalisés (Abandonnés)
- `market.cs` (Commande)
- `TemporaryConsignmentMerchant.cs` (PNJ)
- `ChiefMerchant.cs` (Gestionnaire)
- `GameTradingTable.cs` (Modèle 3D)
