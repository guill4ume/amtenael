# Contexte des Factions - Amtenaël

Ce document sert de guide pour la mise à jour et la gestion des factions sur le serveur.

## Objectif
Maintenir une liste cohérente de factions pour la région 51 (Avalon Isle) et assurer que les interactions (gains/pertes de réputation) sont correctement implémentées.

## Instructions pour l'IA
Lors de l'ajout ou de la modification de PNJs ou de Monstres :
1. Vérifier si l'entité appartient à une micro-faction définie dans l'audit d'Avalon.
2. S'assurer que le script de mort déclenche les variations de réputation appropriées.
3. Mettre à jour la documentation des factions sur le wiki si de nouvelles interactions sont créées.

## Liste des Factions (À compléter via l'Audit)
- [ ] Druides de la Forêt
- [ ] Champignons Vénéneux
- [ ] Milice de Wearyall
- [ ] Bandits d'Avalon
- ...

## Contexte Technique Migration Avalon (Région 51)
- **PNJs Échangeurs** : Utiliser la classe `AvalonExchangerNPC` pour tout nouvel échangeur. Les données sont pilotées par `C:\OpenDAOC_server\ProjetsAnnexes\DossierPortage\Archives\LootsAvalon\echangeur_final.txt`.
- **Création de Guilde** : Le PNJ `GuildRegistrarNPC` permet la création solo et le choix de tous les emblèmes. Propriété `GUILD_NUM` fixée à 1.
- **SkillBase Fallback** : Le système gère les capacités avec préfixes `X` ou `AtlasOF_` automatiquement. Ne pas recréer de doublons de classes si le préfixe est la seule différence.

Pour plus de détails, voir le [Récapitulatif Complet de la Migration Avalon](file:///C:/OpenDAOC_server/ProjetsAnnexes/LootsAvalon/RECAPITULATIF_MIGRATION_AVALON.md).
