# Audit des Loots et Échanges d'Avalon (Région 51)

Ce document récapitule les correspondances entre les objets à farm (tokens), les mobs qui les lâchent et les PNJs échangeurs sur Breamor, pour intégration dans OpenDAoC-SPB.

## 1. Objets à forte XP (Principaux Tokens)

| Objet | ID Objet | Mob Source | PNJ Échangeur | Récompense (XP) |
| :--- | :--- | :--- | :--- | :--- |
| Blason de la Justice Mutilée | `brea_qxp41` | paladin miraculé, moine miraculé, etc. | Alwen / Kilote | 900 000 000 |
| Viande sporique | `brea_qxp38` | spore | Laerea | 300 000 000 |
| Peau d'elfe noir | `brea_qxp35` | mage elfe noir | Laerea | 150 000 000 |
| Corne de Bashaag | `brea_qxp26` | wehu | Krunk / Asbeth | 13 250 000 |
| Écusson de Thormori | `brea_qxp20` | bleusaille de clan | Joric / Lineat | 2 100 000 |
| Armure de légionnaire | `brea_qxp17` | légionnaire | Dunar | 337 500 |

## 2. Objets du Wiki (Progression)

| Objet | ID Objet | PNJ Échangeur | Mob Source (Breamor) |
| :--- | :--- | :--- | :--- |
| Bout de bois | `brea_qxp5` | Borin Thekesd / Lineat | (Inventaire/Marchand?) |
| Carapace de brachyoure | `brea_qxp12` | Grond / Lineat | brachyoure |
| Peau de loup | `brea_qxp10` | Ulrik / Lineat | loup |
| Griffe de loup | `ottor_griffe_loup` | Dunar | loup |

## 3. Trophées de Chasse (Tokens Spéciaux)

Les trophées (`Pitch_tropheeXX`) sont échangés principalement par **John** et **Joric**.

| Trophée | ID Objet | PNJ | Récompense (XP) |
| :--- | :--- | :--- | :--- |
| Trophée 40 | `Pitch_trophee40` | John | 1 000 000 000 |
| Trophée 38 | `Pitch_trophee38` | John | 191 250 000 |
| Trophée 34 | `Pitch_trophee34` | Snarg | 120 000 000 |
| Trophée 20 | `Pitch_trophee20` | Joric | 3 950 000 |

## 4. Prochaines Étapes pour l'intégration SPB

1. **Vérification des Mobs** : Confirmer la présence des mobs sources dans `mobs_map51_amte.sql`.
2. **Génération SQL** : Créer les scripts `mobdroptemplate` et `droptemplatexitemtemplate` pour SPB.
3. **Scripts PNJ** : Les échangeurs (Alwen, Dunar, etc.) doivent être configurés pour utiliser la table `echangeur` ou un script équivalent sur SPB.
