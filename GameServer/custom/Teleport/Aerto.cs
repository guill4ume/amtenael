using System;
using DOL.GS;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;
using System.Reflection;

namespace DOL.GS.Scripts
{
    /// <summary>
    /// Script pour les PNJs nommés Aerto sur Avalon (Map 51).
    /// Permet la téléportation vers Thidranki pour les niveaux 20-24.
    /// </summary>
    public class Aerto : GameNPC
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override bool Interact(GamePlayer player)
        {
            if (!base.Interact(player)) return false;

            TurnTo(player.X, player.Y);
            
            // Plus de restriction de niveau pour Thidranki 50
            player.Out.SendMessage("Aerto vous regarde avec intensité : \"Le Val de Quartz (Thidranki) est désormais le terrain des champions. Souhaites-tu rejoindre la bataille de niveau 50 ?\"\n\n" +
                "[Oui, emmène-moi à Thidranki]", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
            
            return true;
        }

        public override bool WhisperReceive(GameLiving source, string str)
        {
            if (!base.WhisperReceive(source, str)) return false;
            if (!(source is GamePlayer)) return false;
            GamePlayer player = (GamePlayer)source;

            if (str.ToLower() == "oui, emmène-moi à thidranki")
            {
                if (player.InCombat)
                {
                    player.Out.SendMessage("Vous ne pouvez pas voyager pendant un combat !", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                    return true;
                }

                // Téléportation vers Thidranki (Région 252) selon le royaume du joueur
                switch (player.Realm)
                {
                    case eRealm.Albion:
                        player.MoveTo(252, 38113, 53507, 4160, 3268);
                        break;
                    case eRealm.Midgard:
                        player.MoveTo(252, 53568, 23643, 4530, 3268);
                        break;
                    case eRealm.Hibernia:
                        player.MoveTo(252, 17367, 18248, 4320, 3268);
                        break;
                    default:
                        // Si royaume inconnu, on envoie par défaut sur le spawn Albion
                        player.MoveTo(252, 38113, 53507, 4160, 3268);
                        break;
                }
                
                SayTo(player, "Bonne chance sur le champ de bataille !");
            }
            return true;
        }
    }

    /// <summary>
    /// Classe technique pour "hacker" n'importe quel PNJ nommé Aerto sans toucher à la DB.
    /// </summary>
    public class AertoSetup
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
        {
            GameEventMgr.AddHandler(GameLivingEvent.Interact, new DOLEventHandler(OnInteract));
            GameEventMgr.AddHandler(GameLivingEvent.WhisperReceive, new DOLEventHandler(OnWhisper));
            log.Info("Aerto Global Hook initialized (Global Interceptor).");
        }

        private static void OnInteract(DOLEvent e, object sender, EventArgs args)
        {
            GameNPC npc = sender as GameNPC;
            if (npc != null && npc.Name == "Aerto")
            {
                GamePlayer player = ((InteractEventArgs)args).Source;
                if (player == null) return;

                npc.TurnTo(player.X, player.Y);
                player.Out.SendMessage("Aerto vous regarde avec intensité : \"Le Val de Quartz (Thidranki) est désormais le terrain des champions. Souhaites-tu rejoindre la bataille de niveau 50 ?\"\n\n" +
                    "[Oui, emmène-moi à Thidranki]", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
            }
        }

        private static void OnWhisper(DOLEvent e, object sender, EventArgs args)
        {
            GameNPC npc = sender as GameNPC;
            WhisperReceiveEventArgs whisperArgs = args as WhisperReceiveEventArgs;
            
            if (npc != null && npc.Name == "Aerto" && whisperArgs.Text.ToLower() == "oui, emmène-moi à thidranki")
            {
                GamePlayer player = whisperArgs.Source as GamePlayer;
                if (player == null) return;

                if (player.InCombat)
                {
                    player.Out.SendMessage("Vous ne pouvez pas voyager pendant un combat !", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                    return;
                }

                switch (player.Realm)
                {
                    case eRealm.Albion: player.MoveTo(252, 38113, 53507, 4160, 3268); break;
                    case eRealm.Midgard: player.MoveTo(252, 53568, 23643, 4530, 3268); break;
                    case eRealm.Hibernia: player.MoveTo(252, 17367, 18248, 4320, 3268); break;
                    default: player.MoveTo(252, 38113, 53507, 4160, 3268); break;
                }
                npc.SayTo(player, "Bonne chance pour cette bataille finale !");
            }
        }
    }
}
