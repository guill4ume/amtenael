using System;
using System.Reflection;
using log4net;

namespace DOL.GS
{
    /// <summary>
    /// Composant ECS attaché à un joueur pour gérer ses capacités de vol.
    /// </summary>
    public class StealingComponent : IManagedEntity
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Le joueur propriétaire de ce composant.
        /// </summary>
        public GamePlayer Owner { get; }

        /// <summary>
        /// Timestamp du dernier vol (GameLoop.GetCurrentTime()).
        /// </summary>
        public long LastStealTick { get; set; }

        /// <summary>
        /// Délai de rechargement entre deux tentatives (en ms).
        /// </summary>
        public int StealCooldown { get; set; } = 60000; // 60 secondes par défaut

        /// <summary>
        /// ID pour le gestionnaire d'entités ECS.
        /// </summary>
        public EntityManagerId EntityManagerId { get; set; } = new(EntityManager.EntityType.StealingComponent, false);

        public StealingComponent(GamePlayer owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Vérifie si le vol est disponible (cooldown expiré).
        /// </summary>
        public bool IsStealAvailable()
        {
            return GameLoop.GetCurrentTime() - LastStealTick >= StealCooldown;
        }

        /// <summary>
        /// Réinitialise le cooldown après un vol réussi.
        /// </summary>
        public void ResetCooldown()
        {
            LastStealTick = GameLoop.GetCurrentTime();
        }
    }
}
