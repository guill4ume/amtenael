using DOL.AI.Brain;
using DOL.GS;
using System.Linq;

namespace DOL.GS.Scripts
{
    public class LightMimicBrain : StandardMobBrain
    {
        public LightMimicBrain() : base()
        {
            AggroLevel = 100;
            AggroRange = 1000;
        }

        // Increased ThinkInterval for CPU optimization
        public override int ThinkInterval
        {
            get
            {
                // If in combat, think faster (1000ms), otherwise slow down (2000ms) to save CPU
                if (Body != null && Body.InCombat)
                    return 1000;
                
                return 2000;
            }
        }

        public override void Think()
        {
            if (Body == null) return;

            // Update SpawnPoint to current position every tick
            // This prevents them from walking all the way back to the realm entrance
            Body.SpawnPoint = new Point3D(Body.X, Body.Y, Body.Z);

            // Force FSM state away from RETURN_TO_SPAWN if it ever enters it
            if (FSM.GetCurrentState().StateType == eFSMStateType.RETURN_TO_SPAWN)
            {
                FSM.SetCurrentState(eFSMStateType.IDLE);
            }

            // If in Thidranki, not in combat, and not moving, head towards the center
            if (Body.CurrentRegionID == 252 && !Body.InCombat && !Body.IsMoving && Util.Chance(20))
            {
                // Move towards a random point near the center of Thidranki (36000, 32000)
                Point3D center = new Point3D(36000 + Util.Random(-2000, 2000), 32000 + Util.Random(-2000, 2000), Body.Z);
                
                // Use WalkTo (straight line, ignores most obstacles/pathfinding complexity) 
                // instead of PathTo to allow "traversing walls" as requested by USER
                Body.WalkTo(center, 150);
            }

            base.Think();
        }

        public override bool CanAggroTarget(GameLiving target)
        {
            if (target == null || Body == null) return false;
            
            // Never aggro own realm or neutral
            if (target.Realm == Body.Realm || target.Realm == eRealm.None) return false;

            // Aggro players of other realms
            if (target is GamePlayer) return true;

            // Aggro other mimics (heavy or light) of other realms
            if (target is LightMimicNPC || target is MimicNPC) return true;

            // IGNORE everything else (wildlife, grey mobs, etc.) to keep them focused on the war
            return false;
        }

        protected override void CheckPlayerAggro()
        {
            if (Body == null) return;

            foreach (GamePlayer player in Body.GetPlayersInRadius((ushort)AggroRange))
            {
                if (!CanAggroTarget(player))
                    continue;

                if (player.IsStealthed || player.Steed != null)
                    continue;

                // Simple check, add to aggro
                AddToAggroList(player, 1);
                return; // Stop after finding one target to save CPU
            }
        }

        protected override void CheckNpcAggro()
        {
            if (Body == null) return;

            foreach (GameNPC npc in Body.GetNPCsInRadius((ushort)AggroRange))
            {
                if (!CanAggroTarget(npc))
                    continue;

                // Don't aggro friendly realms
                if (npc.Realm == Body.Realm)
                    continue;

                AddToAggroList(npc, 1);
                return; // Stop after finding one target
            }
        }
    }
}
