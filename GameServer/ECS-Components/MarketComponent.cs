using DOL.Database;
using DOL.GS;

namespace DOL.GS
{
    public class MarketComponent : IManagedEntity
    {
        public EntityManagerId EntityManagerId { get; set; } = new(EntityManager.EntityType.MarketComponent, false);

        public GameNPC OwnerNPC { get; private set; }
        
        /// <summary>
        /// ID of the player who owns this market.
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// Prevents multiple players from buying the same item at the exact same millisecond.
        /// Slot -> IsLocked mapping
        /// </summary>
        public System.Collections.Concurrent.ConcurrentDictionary<eInventorySlot, bool> LockedSlots { get; } = new();

        public MarketComponent(GameNPC owner)
        {
            OwnerNPC = owner;
        }

        public bool TryLockSlot(eInventorySlot slot)
        {
            return LockedSlots.TryAdd(slot, true);
        }

        public void UnlockSlot(eInventorySlot slot)
        {
            LockedSlots.TryRemove(slot, out _);
        }
    }
}
