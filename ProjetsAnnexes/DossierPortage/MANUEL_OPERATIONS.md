# MANUEL DES OPÉRATIONS - Infrastructure & Tests

> [!CAUTION]
> **RÈGLE D'OR :** Rien n'est validé tant que ça n'a pas été testé **IN-GAME** (connexion réelle, interaction). Les logs sont un diagnostic, le test terrain est la seule validation.

Dernière mise à jour : 22 Avril 2026
Documentation pratique pour lancer, migrer et valider le serveur Amtenaël.

---

## 🌿 Architecture de Sauvegarde (GitHub Fork)

L'ensemble des dépôts est désormais géré via un Fork GitHub unifié (`https://github.com/guill4ume/amtenael`), en remplacement des anciens `git stash` locaux qui provoquaient des pertes de données et des conflits de fusion.

1. **Serveur de Test (SPB) :**
    - Fonctionne sur la branche `spb`.
    - Pour sauvegarder vos scripts custom : `git add .`, puis `git commit -m "ma modification"`, puis `git push origin spb`.
    
2. **Serveur de Production (Core-master) :**
    - Fonctionne sur la branche `master`.
    - Pour tout mettre à jour (récupérer les MAJ officielles, commiter vos ajouts, et pousser sur votre GitHub), il suffit de lancer le script de la racine : `C:\OpenDAOC_server\Mettre_A_Jour_Prod.bat`.

---

## 🐳 Orchestration Docker

Commandes usuelles (à lancer depuis `C:\OpenDAOC_server\ProjetsAnnexes\OpenDAoC-SPB`) :

- **Démarrage complet** : `docker-compose up -d`
- **Reconstruction après modif code** : `docker-compose up -d --build gameserver`
- **Lecture des logs** : `docker-compose logs -f gameserver`
- **Accès base de données** : `docker exec -it openbots-db mysql -u root -pmy-secret-pw opendaoc`
- **Nettoyage Automatique Loot** :
  ```powershell
  Get-Content fix_missing_loot.sql | docker-compose exec -T db mysql -u root -pmy-secret-pw opendaoc
  ```

---

## 🗄️ Migration des Données (Export Breamor)

Pour exporter la base historique d'Amtenaël vers le nouveau serveur :

1.  **Export** (depuis le Docker Breamor) :
    ```powershell
    docker exec <nom_db_breamor> mysqldump -u root -p dol mob npctemplate itemtemplate account > breamor_export.sql
    ```
2.  **Import** (vers le Docker SPB) :
    ```powershell
    Get-Content breamor_export.sql | docker exec -i openbots-db mysql -u root -pmy-secret-pw opendaoc
    ```
3. **Migration Scripts (si nécessaire)** : 
    - En cas de perte de comportements (ex: menus textuels), migrer les scripts C# correspondants dans `GameServer/custom/` et s'assurer que le `ClassType` en base de données correspond bien au script (ex: `DOL.GS.Scripts.TextNPCMerchant`).

---

## 🧪 Plan de Recette (Procédures de Tests)

### T.1 Anonymat RP
1. Se connecter avec Joueur A et Joueur B (Royaumes différents).
2. Joueur B regarde Joueur A.
3. **Succès** : Le nom doit être remplacé par la race (Inconnu).

### T.2 Groupement Inter-Royaume
1. Albion et Midgard tentent `/invite`.
2. **Succès** : Le groupe est formé. Buffs/Soins partagés OK.

### T.3 Récompense RP
1. Parler en `/say` (Messire, Donjon, etc.).
2. Attendre 5 minutes.
3. **Succès** : Message système "Les dieux sont satisfaits" + Bonus RP.

### T.4 NPCs Avalon
1. Se rendre en Région 51 (Avalon).
2. **Succès** : Tous les PNJs historiquement présents sont visibles et interactifs.

### T.5 Marchands Zone 51 (Menus et Inventaires)
1. Parler à un marchand (ex: `Maelwyn` ou `Korentin`) via clic droit ou chuchotement.
2. **Succès** : Le marchand répond via la fenêtre de dialogue (script `TextNPCMerchant`).
3. Ouvrir la fenêtre de vente.
4. **Succès** : La fenêtre contient des objets à acheter (table `merchantitem` et `itemtemplate`).

---

## ⚙️ Configuration Moteur (Amtenael Hijack)

Le mode "Amtenael" utilise le type **PvP** de base d'OpenDAoC, mais avec les règles `AmtenaelRules.cs` forcées.

- **Nom du mode** : `PvP` (reconnu en tant que `GST_PvP = 2`).
- **Activation** : Assigner `GAME_TYPE: "PvP"` dans `docker-compose.yml`.
- **Priorité Ruleset** : Garanti par le commentaire de l'attribut dans `PvPServerRules.cs`.
- **Volume Config** : Le dossier `/app/config` est géré dans le `Dockerfile` pour assurer la présence de `invalidnames.txt`.

---

## 🔧 Maintenance & Debug
- **SQL Cleanup** : Utiliser `fix_missing_loot.sql` pour supprimer les références d'objets inexistants qui polluent les logs.
- **Logs Ruleset** : Chercher `AmtenaelRules` dans les logs Docker pour confirmer l'activation.
- **serverconfig.xml** : Géré par `entrypoint.sh`. Ne pas éditer manuellement.
