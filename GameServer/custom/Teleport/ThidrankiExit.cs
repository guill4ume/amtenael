using System;
using System.Reflection;
using DOL.GS;
using DOL.Events;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.Scripts
{
    /// <summary>
    /// Script pour les PNJs de sortie de Thidranki (Région 252) vers Avalon (Map 51).
    /// </summary>
    public class ThidrankiExit : GameNPC
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override bool Interact(GamePlayer player)
        {
            if (!base.Interact(player)) return false;

            TurnTo(player.X, player.Y);
            
            player.Out.SendMessage(Name + " vous regarde sereinement : \"La bataille a été rude. Souhaites-tu retourner en sécurité à Avalon ?\"\n\n" +
                "[Oui, ramène-moi à Avalon]", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
            
            return true;
        }

        public override bool WhisperReceive(GameLiving source, string str)
        {
            if (!base.WhisperReceive(source, str)) return false;
            if (!(source is GamePlayer)) return false;
            GamePlayer player = (GamePlayer)source;

            if (str.ToLower() == "oui, ramène-moi à avalon")
            {
                if (player.InCombat)
                {
                    player.Out.SendMessage("Vous ne pouvez pas voyager pendant un combat !", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                    return true;
                }

                // Téléportation vers Avalon (Région 51) au hub d'Aerto
                player.MoveTo(51, 482984, 481439, 3072, 3234);
                
                SayTo(player, "Que la paix vous accompagne.");
            }
            return true;
        }

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
        {
            if (!ServerProperties.Properties.LOAD_QUESTS)
                return;

            SpawnExitNPC("Aertis", eRealm.Albion, 37333, 51881, 3944, 4090);
            SpawnExitNPC("Aertis", eRealm.Midgard, 54309, 25234, 4319, 1744);
            SpawnExitNPC("Aertis", eRealm.Hibernia, 18708, 18710, 4320, 1424);
            
            log.Info("Thidranki Exit NPCs (Aertis) initialized.");
        }

        private static void SpawnExitNPC(string name, eRealm realm, int x, int y, int z, ushort heading)
        {
            // Vérifier si le PNJ existe déjà pour éviter les doublons au rechargement de script
            foreach (GameNPC npc in WorldMgr.GetNPCsByName(name, realm))
            {
                if (npc.CurrentRegionID == 252 && Math.Abs(npc.X - x) < 100 && Math.Abs(npc.Y - y) < 100)
                {
                    return;
                }
            }

            ThidrankiExit exitNpc = new ThidrankiExit();
            exitNpc.Name = name;
            exitNpc.GuildName = "Gardien du Retour";
            exitNpc.Model = 137; // Modèle identique à Aerto
            exitNpc.Realm = realm;
            exitNpc.CurrentRegionID = 252;
            exitNpc.Size = 50;
            exitNpc.Level = 50;
            exitNpc.X = x;
            exitNpc.Y = y;
            exitNpc.Z = z;
            exitNpc.Heading = heading;
            exitNpc.Flags |= eFlags.PEACE;
            
            exitNpc.AddToWorld();
        }
    }
}
