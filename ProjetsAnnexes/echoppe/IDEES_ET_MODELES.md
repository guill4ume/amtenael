# Idées et Modèles Visuels

Voici une liste d'IDs de modèles (Models) DAoC que tu peux tester pour ton échoppe. Tu peux les voir en jeu avec `/mob model <ID>`.

## Modèles de Tables et Établis
- **Table de vente standard** : `1494` (C'est la table classique des marchands).
- **Table d'Alchimie** : `3271` ou `3140` (Pour un look d'herboriste).
- **Poste d'Empennage** : `3141` (Idéal pour un marchand de flèches/arcs).
- **Forge portative** : `2110` (Pour un forgeron itinérant).
- **Présentoir d'armes** : `1044`.
- **Petite Table / Bureau** : `1125`.

## Idées d'Évolutions
1. **Skins variés** : Permettre au joueur de choisir son modèle de table selon son métier principal.
2. **Enseigne personnalisée** : Utiliser le système de `name` pour afficher non seulement le nom du joueur mais aussi sa spécialité.
3. **Logs de vente** : Utiliser les logs de `GameConsignmentMerchant` pour envoyer un message au joueur lorsqu'il se reconnecte ("Vous avez vendu X objets pour Y pièces").
4. **Taxe dynamique** : Utiliser la `TRADING_TAX` globale, mais peut-être prévoir des bonus selon la zone (ex: taxe réduite dans la capitale).
