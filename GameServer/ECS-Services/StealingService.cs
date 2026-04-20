using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.Language;
using log4net;
using ECS.Debug;

namespace DOL.GS
{
    public static class StealingService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string SERVICE_NAME = nameof(StealingService);
        private static List<StealingComponent> _list;

        // Configuration du vol
        private const int MAX_STEAL_DISTANCE = 200; // Unités
        private const int MIN_GOLD_PERCENT = 1;     // 1%
        private const int MAX_GOLD_PERCENT = 5;     // 5%

        public static void Tick()
        {
            GameLoop.CurrentServiceTick = SERVICE_NAME;
            Diagnostics.StartPerfCounter(SERVICE_NAME);
            
            // On récupère les composants pour un éventuel traitement actif par tick (ex: timer visuel)
            // Pour l'instant, le vol est purement événementiel via la commande /vol.
            _list = EntityManager.UpdateAndGetAll<StealingComponent>(EntityManager.EntityType.StealingComponent, out int lastValidIndex);
            
            Diagnostics.StopPerfCounter(SERVICE_NAME);
        }

        /// <summary>
        /// Tente d'effectuer un vol sur la cible.
        /// </summary>
        public static void TrySteal(GamePlayer thief, GameObject targetObj)
        {
            if (thief == null || !thief.IsAlive) return;

            // 1. Vérification du mode Furtif
            if (!thief.IsStealthed)
            {
                thief.Out.SendMessage("Vous devez être furtif pour pouvoir voler quelqu'un.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // 2. Vérification de la cible
            if (targetObj == null || targetObj == thief)
            {
                thief.Out.SendMessage("Vous n'avez pas de cible valide.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            var target = targetObj as GameLiving;
            if (target == null || !target.IsAlive)
            {
                thief.Out.SendMessage("Vous ne pouvez voler que des cibles vivantes.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // 3. Vérification de la distance
            if (!thief.IsWithinRadius(target, MAX_STEAL_DISTANCE))
            {
                thief.Out.SendMessage("Vous êtes trop loin pour voler cette cible.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // 4. Vérification du Cooldown
            var stealingComp = thief.TempProperties.GetProperty<StealingComponent>("StealingComponent");
            if (stealingComp == null)
            {
                stealingComp = new StealingComponent(thief);
                thief.TempProperties.SetProperty("StealingComponent", stealingComp);
                EntityManager.Add(stealingComp);
            }

            if (!stealingComp.IsStealAvailable())
            {
                long remainingSeconds = (stealingComp.StealCooldown - (GameLoop.GetCurrentTime() - stealingComp.LastStealTick)) / 1000;
                thief.Out.SendMessage($"Vous devez attendre encore {remainingSeconds} secondes avant de pouvoir voler à nouveau.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // 5. Exécution du vol (Le joueur cible ou NPC a-t-il de l'argent ?)
            long targetMoney = GetTargetMoney(target);
            if (targetMoney <= 0)
            {
                thief.Out.SendMessage($"Les poches de {target.Name} sont vides.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                stealingComp.ResetCooldown(); // On déclenche quand même le cooldown sur un échec
                RevealThief(thief);
                return;
            }

            // Calcul du butin (1% à 5% de la bourse, plafonné par le niveau)
            long maxStealAmount = target.Level * 100; // 100 copper par niveau max par exemple
            int percent = Util.Random(MIN_GOLD_PERCENT, MAX_GOLD_PERCENT);
            long stealAmount = Math.Min(targetMoney * percent / 100, maxStealAmount);

            if (stealAmount <= 0)
            {
                thief.Out.SendMessage($"Vous n'avez rien réussi à voler à {target.Name}.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                stealingComp.ResetCooldown();
                RevealThief(thief);
                return;
            }

            // 6. Transfert de l'argent
            RemoveTargetMoney(target, stealAmount);
            thief.AddMoney(stealAmount);

            // 7. Mise à jour de l'état
            stealingComp.ResetCooldown();
            RevealThief(thief);

            // 8. Notifications
            string moneyStr = Money.GetString(stealAmount);
            thief.Out.SendMessage($"Vous avez volé avec succès {moneyStr} à {target.Name} !", eChatType.CT_System, eChatLoc.CL_SystemWindow);
            
            if (target is GamePlayer targetPlayer)
            {
                // En option, chance d'être découvert
                if (Util.Chance(50)) 
                {
                    targetPlayer.Out.SendMessage($"Vous sentez une main dans votre poche... Vous venez de vous faire voler de l'argent !", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                }
            }
        }

        private static void RevealThief(GamePlayer thief)
        {
            // Le voleur perd son aspect furtif après l'action
            thief.Stealth(false);
        }

        private static long GetTargetMoney(GameLiving target)
        {
            if (target is GamePlayer targetPlayer)
            {
                return targetPlayer.GetCurrentMoney();
            }
            // Exemple simple pour un NPC, pseudo-aléatoire basé sur son niveau
            return (long)Util.Random(10, 50) * target.Level;
        }

        private static void RemoveTargetMoney(GameLiving target, long amount)
        {
            if (target is GamePlayer targetPlayer)
            {
                targetPlayer.RemoveMoney(amount);
            }
        }
    }
}
