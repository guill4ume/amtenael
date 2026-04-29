# Plan d'Audit et Importation des Loots Avalon (Région 51)

Ce document trace l'ensemble des recherches et actions pour restaurer le système de loots (tokens et équipements) d'Avalon.

## Objectifs
- Identifier tous les tokens d'échange XP de Breamor.
- Recenser l'intégralité des loots de la Région 51 sur Breamor.
- Mapper ces loots sur les mobs déjà intégrés dans SPB.
- Documenter les PNJs échangeurs associés.

## État des Lieux
- Dossier de travail : `C:\OpenDAOC_server\ProjetsAnnexes\LootsAvalon`
- Source : Base de données Breamor (via exports SQL).
- Cible : Base de données SPB (openbots-db).

## Étapes de Recherche
1. `[ ]` Chercher les ItemTemplates de type "Token" dans l'export Breamor.
2. `[ ]` Lister les mobs de la Région 51 sur Breamor et leurs LootTemplates.
3. `[ ]` Comparer avec les mobs présents dans SPB (Région 51).
4. `[ ]` Préparer le script SQL d'importation sélectif.

## Questions Ouvertes
- Est-ce qu'un export complet de la base Breamor est disponible localement (ex: `breamor_export.sql`) ?
- Existe-t-il un préfixe spécifique dans le nom des tokens (ex: "Jeton", "Ecu", "Marque") ?
