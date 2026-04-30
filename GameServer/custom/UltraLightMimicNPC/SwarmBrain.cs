using System;
using System.Linq;
using DOL.AI.Brain;
using DOL.GS;
using DOL.GS.Utils;

namespace DOL.GS.Scripts
{
    public class SwarmBrain : StandardMobBrain
    {
        public SwarmBrain() : base()
        {
            AggroLevel = 100;
            AggroRange = 800;
        }

        // Extremely high think interval to save CPU
        public override int ThinkInterval => 3000; 

        public override void Think()
        {
            if (Body == null) return;

            // Hibernation: If no real player is in a large radius, do nothing
            if (!Body.GetPlayersInRadius(4000).Any())
            {
                return; 
            }

            // Simple "Attack center" logic if not in combat and not moving
            if (Body.CurrentRegionID == 252 && !Body.InCombat && !Body.IsMoving && Util.Chance(10))
            {
                Point3D center = new Point3D(33089 + Util.Random(-1000, 1000), 38271 + Util.Random(-1000, 1000), Body.Z);
                Body.WalkTo(center, 150);
            }

            base.Think();
        }

        public override bool CanAggroTarget(GameLiving target)
        {
            if (target == null || Body == null) return false;
            if (target.Realm == Body.Realm || target.Realm == eRealm.None) return false;
            
            // Only aggro players or other bots
            return target is GamePlayer || target is LightMimicNPC || target is MimicNPC || target is UltraLightMimicNPC;
        }

        // Simplified aggro checks
        protected override void CheckPlayerAggro()
        {
            if (Body == null) return;
            var player = Body.GetPlayersInRadius((ushort)AggroRange).FirstOrDefault(p => CanAggroTarget(p) && !p.IsStealthed);
            if (player != null) AddToAggroList(player, 1);
        }

        protected override void CheckNpcAggro()
        {
            if (Body == null) return;
            var npc = Body.GetNPCsInRadius((ushort)AggroRange).FirstOrDefault(n => CanAggroTarget(n));
            if (npc != null) AddToAggroList(npc, 1);
        }
    }
}
