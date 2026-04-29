# Documentation Structurelle de la Base de Données Breamor

Ce document décrit l'organisation du dump SQL `BDDamte030326.sql` (240 Mo) utilisé pour le portage des données Avalon vers OpenDAoC-SPB.

## Informations Générales
- **Fichier** : `C:\OpenDAOC_server\ProjetsAnnexes\datas diverses\Breamor\BDDamte030326.sql`
- **Taille** : ~240 Mo
- **Lignes totales** : ~1 440 000

## Index des Tables Stratégiques

| Table | Ligne de début (approx) | Description |
| :--- | :--- | :--- |
| `droptemplatexitemtemplate` | 1 085 203 | Correspondance entre templates de loot et objets. |
| `echangeur` | 1 088 518 | Système d'échange de tokens (PNJ, Objet reçu, XP/Argent donné). |
| `inventory` | 1 091 387 | Inventaires des personnages et PNJs (Attention : très volumineux). |
| `itemtemplate` | 1 147 452 | Définition de tous les objets (Stats, Nom, Modèle, etc.). |
| `lootchangertemplate` | 1 214 950 | Ancienne table d'échange (remplacée par `echangeur` sur certains points). |
| `loottemplate` | 1 215 119 | Définition des groupes de loot. |
| `merchantitem` | 1 215 298 | Liste des objets vendus par les marchands. |
| `mob` | 1 221 816 | Liste de tous les PNJs et Monstres (Position, Stats, Brain, etc.). |
| `mobdroptemplate` | 1 235 620 | Lien direct entre un Mob et son Template de Loot. |
| `mobxloottemplate` | 1 236 348 | Autre structure de lien Mob-Loot. |
| `npctemplate` | 1 244 485 | Modèles génériques de PNJs. |
| `textnpc` | 1 370 000 | Dialogues et textes spécifiques aux PNJs. |

## Notes Techniques
- **Format** : MySQL Dump.
- **Encodage** : Les caractères spéciaux (accents) peuvent apparaître mal encodés (`Ǹ` pour `é`, `Ǯ` pour `ë`, etc.).
- **Recherche** : Utiliser `sls` (Select-String) avec les plages de lignes ci-dessus pour accélérer les recherches.
- **Tokens** : La plupart des tokens personnalisés d'Amtenael/Breamor commencent par `Pitch_`, `brea_qxp` ou `Hapo_`.
