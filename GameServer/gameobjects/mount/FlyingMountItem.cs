using System;
using DOL.Database;
using DOL.Language;

namespace DOL.GS
{
    /// <summary>
    /// Item that summons a flying mount (Dragon/Griffin).
    /// </summary>
    public class FlyingMountItem : GameInventoryItem
    {
        public FlyingMountItem() : base() { }
        public FlyingMountItem(DbItemTemplate template) : base(template) { }
        public FlyingMountItem(DbInventoryItem item) : base(item) { }

        /// <summary>
        /// Logic called when player clicks or uses the item.
        /// </summary>
        /// <param name="player">The player using the item.</param>
        /// <returns>True if handled.</returns>
        public override bool Use(GamePlayer player)
        {
            if (player == null || !player.IsAlive)
                return false;

            // Dismount if already on a flying mount
            if (player.IsOnFlyingMount)
            {
                player.IsOnFlyingMount = false;
                return true;
            }

            // Check if player is allowed to mount
            string reason = GameServer.ServerRules.ReasonForDisallowMounting(player);
            if (!string.IsNullOrEmpty(reason))
            {
                player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, reason), eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return true;
            }

            // Determine mount type from item (default to Dragon for MVP)
            eFlyingMountType mountType = eFlyingMountType.Dragon;

            // Optional: determine based on Model ID or Name
            if (this.Name.ToLower().Contains("griffin") || this.Name.ToLower().Contains("griffon"))
                mountType = eFlyingMountType.Griffin;

            player.FlyingMountType = mountType;
            player.IsOnFlyingMount = true;

            return true;
        }
    }
}
