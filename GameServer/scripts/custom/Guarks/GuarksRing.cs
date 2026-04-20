using System;
using System.Collections.Generic;
using System.Linq;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts.Custom.Guarks
{
    /// <summary>
    /// Guarks Ring System - Rewritten to use OpenDAoC native Teleport table.
    /// 
    /// HOW IT WORKS:
    /// 1. Player equips item with Id_nb = "dre_guarks_anneau"
    /// 2. Player is immobilized and their health is drained over 9 seconds
    /// 3. During the ritual, player says a destination name (must match TeleportID in Teleport table with Type = "GUARKS")
    /// 4. At 9 seconds, player is teleported to that destination
    ///
    /// DB SETUP:
    ///   INSERT INTO Teleport (Type, TeleportID, Realm, RegionID, X, Y, Z, Heading) VALUES
    ///     ('GUARKS', 'Camelot', 0, 1, 37509, 30280, 8003, 0),
    ///     ('GUARKS', 'Tir Na Nog', 0, 200, 34000, 31000, 8000, 0);
    ///   (Add all desired destinations with Type = 'GUARKS')
    /// </summary>
    public class GuarksRing
    {
        private const string RING_TARGET_KEY = "GUARKS_RING_TARGET";
        private const string RING_CANCEL_KEY = "GUARKS_RING_CANCEL";
        private const string RING_TICKS_KEY = "GUARKS_RING_TICKS";
        private const string GUARKS_TELEPORT_TYPE = "GUARKS";

        private static readonly Dictionary<string, DbTeleport> _destinations = new();

        [GameServerStartedEvent]
        public static void Init(DOLEvent e, object sender, EventArgs args)
        {
            LoadDestinations();
            // GameEventMgr.AddHandler(PlayerInventoryEvent.ItemEquipped, new DOLEventHandler(OnItemEquipped));
            // GameEventMgr.AddHandler(PlayerInventoryEvent.ItemUnequipped, new DOLEventHandler(OnItemUnequipped));
            GameEventMgr.AddHandler(GameLivingEvent.Say, new DOLEventHandler(OnPlayerSay));
        }

        private static void LoadDestinations()
        {
            _destinations.Clear();
            var rows = DOLDB<DbTeleport>.SelectObjects(DB.Column("Type").IsEqualTo(GUARKS_TELEPORT_TYPE));
            foreach (var row in rows)
                _destinations[row.TeleportID.ToLower()] = row;
        }

        public static void OnPlayerSay(DOLEvent e, object sender, EventArgs args)
        {
            if (sender is not GamePlayer player) return;
            if (player.TempProperties.GetProperty<bool>(RING_CANCEL_KEY)) return; // not in ritual

            if (args is SayEventArgs say)
            {
                string text = say.Text.ToLower();
                DbTeleport match = _destinations.FirstOrDefault(d => d.Key.Contains(text)).Value;
                if (match != null && player.TempProperties.GetProperty<DbTeleport>(RING_TARGET_KEY, null) == null)
                {
                    player.TempProperties.SetProperty(RING_TARGET_KEY, match);
                    player.Out.SendMessage($"La magie entend votre demande pour {match.TeleportID}.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
                }
            }
        }

        public static void OnItemEquipped(DOLEvent e, object sender, EventArgs args)
        {
            /*
            if (args is not ItemEquippedArgs arg || arg.Item.Id_nb != "dre_guarks_anneau") return;
            if (sender is not GamePlayerInventory inv) return;
            GamePlayer player = inv.Player;
            if (player.Client.ClientState != GameClient.eClientState.Playing) return;

            if (!CheckCanUse(player)) return;

            player.Out.SendMessage("Une puissante magie vous immobilise.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
            player.MaxSpeedBase = 1;
            player.Out.SendUpdateMaxSpeed();

            int harm = player.MaxHealth / 10;
            if (player.Health <= harm)
            {
                player.Out.SendMessage("Vous n'avez pas assez de vitalité pour utiliser l'anneau.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
                player.MaxSpeedBase = 191;
                player.Out.SendUpdateMaxSpeed();
                return;
            }
            player.Health -= harm;

            // Mark as in ritual; cancel flag = false means currently active
            player.TempProperties.SetProperty(RING_CANCEL_KEY, false);
            player.TempProperties.SetProperty(RING_TICKS_KEY, 0);
            player.TempProperties.SetProperty("GUARKS_STARTX", player.X);
            player.TempProperties.SetProperty("GUARKS_STARTY", player.Y);

            SendEffect(player, 2661);
            ScheduleTick(player, 1000);
            */
        }

        public static void OnItemUnequipped(DOLEvent e, object sender, EventArgs args)
        {
            /*
            if (args is not ItemUnequippedArgs arg || arg.Item.Id_nb != "dre_guarks_anneau") return;
            if (sender is not GamePlayerInventory inv) return;
            CancelRitual(inv.Player, "Vous avez retiré l'anneau, la téléportation est annulée.");
            */
        }

        private static void ScheduleTick(GamePlayer player, int delayMs)
        {
            new ECSGameTimer(player, _ => { Tick(player); return 0; }, delayMs);
        }

        private static void Tick(GamePlayer player)
        {
            if (player.TempProperties.GetProperty<bool>(RING_CANCEL_KEY)) return;

            int startX = player.TempProperties.GetProperty<int>("GUARKS_STARTX");
            int startY = player.TempProperties.GetProperty<int>("GUARKS_STARTY");
            double distSq = Math.Pow(player.X - startX, 2) + Math.Pow(player.Y - startY, 2);

            if (player.InCombat || distSq > 100)
            {
                CancelRitual(player, player.InCombat ? "Vous êtes en combat, la téléportation est annulée." : "Vous avez bougé, la téléportation est annulée.");
                return;
            }

            int ticks = player.TempProperties.GetProperty<int>(RING_TICKS_KEY) + 1;
            player.TempProperties.SetProperty(RING_TICKS_KEY, ticks);
            int harm = player.MaxHealth / 10;

            switch (ticks)
            {
                case 1: case 3: case 5:
                    if (player.Health <= harm) { player.Health = 0; player.Die(player); CancelRitual(player, null); return; }
                    player.Health -= harm;
                    break;
                case 7:
                    if (player.Health <= harm / 2) { player.Health = 0; player.Die(player); CancelRitual(player, null); return; }
                    player.Health -= harm / 2;
                    break;
                case 2: SendEffect(player, 2661); break;
                case 4: SendEffect(player, 2661); SendEffect(player, 1677); break;
                case 6: SendEffect(player, 82); SendEffect(player, 276); break;
                case 8: SendEffect(player, 2569); break;
                case 9:
                    Teleport(player);
                    CancelRitual(player, null, resetSpeed: true);
                    return;
            }

            ScheduleTick(player, 1000);
        }

        private static void Teleport(GamePlayer player)
        {
            DbTeleport target = player.TempProperties.GetProperty<DbTeleport>(RING_TARGET_KEY, null);

            if (target == null || Util.Chance(5))
            {
                target = _destinations.Values.ToList() is { Count: > 0 } list ? list[Util.Random(0, list.Count - 1)] : null;
                if (target != null)
                    player.Out.SendMessage("La magie a choisi votre destination.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
            }

            if (target == null)
            {
                player.Out.SendMessage("La téléportation a échoué : aucune destination trouvée.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
                return;
            }

            SendEffect(player, 276);
            player.MoveTo((ushort)target.RegionID, target.X, target.Y, target.Z, (ushort)target.Heading);
        }

        private static void CancelRitual(GamePlayer player, string message, bool resetSpeed = true)
        {
            player.TempProperties.SetProperty(RING_CANCEL_KEY, true);
            player.TempProperties.RemoveProperty(RING_TARGET_KEY);
            player.TempProperties.RemoveProperty(RING_TICKS_KEY);
            player.TempProperties.RemoveProperty("GUARKS_STARTX");
            player.TempProperties.RemoveProperty("GUARKS_STARTY");

            if (resetSpeed && player.MaxSpeedBase == 1)
            {
                player.MaxSpeedBase = 191;
                player.Out.SendUpdateMaxSpeed();
            }

            if (!string.IsNullOrEmpty(message))
                player.Out.SendMessage(message, eChatType.CT_Important, eChatLoc.CL_SystemWindow);
        }

        private static bool CheckCanUse(GamePlayer player)
        {
            if (player.InCombat) { player.Out.SendMessage("Vous ne pouvez utiliser l'anneau en combat !", eChatType.CT_Important, eChatLoc.CL_SystemWindow); return false; }
            if (player.IsRiding) { player.Out.SendMessage("Vous ne pouvez utiliser l'anneau à cheval !", eChatType.CT_Important, eChatLoc.CL_SystemWindow); return false; }
            return true;
        }

        private static void SendEffect(GamePlayer player, ushort effectId)
        {
            foreach (GamePlayer p in player.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                p.Out.SendSpellEffectAnimation(player, player, effectId, 0, false, 1);
        }
    }
}
