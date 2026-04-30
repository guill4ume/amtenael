using System;
using System.Collections.Generic;
using System.Linq;
using DOL.GS;
using DOL.GS.Utils;
using DOL.GS.Realm;
using DOL.AI.Brain;

namespace DOL.GS.Scripts
{
    /// <summary>
    /// Tier 3 Bot: Ultra-Lightweight "SwarmBot".
    /// Pure GameNPC with minimal memory footprint.
    /// </summary>
    public class UltraLightMimicNPC : GameNPC
    {
        public UltraLightMimicNPC(byte level, eRealm realm) : base()
        {
            this.Level = level;
            this.Realm = realm;
            this.Name = "Swarm Warrior";
            this.GuildName = "Swarm";

            SetDefaultRaceAndModel();

            // Very basic roaming range
            this.RoamingRange = 100;
            this.MaxSpeedBase = 191;

            // Simplified stats
            this.Strength = (short)(level * 2);
            this.Constitution = (short)(level * 2);
            
            // Assign custom swarm brain
            this.SetOwnBrain(new SwarmBrain());
        }

        private void SetDefaultRaceAndModel()
        {
            Gender = (eGender)Util.Random(0, 1);
            
            switch (Realm)
            {
                case eRealm.Albion:
                    Race = (short)eRace.Briton;
                    Model = (ushort)((Gender == eGender.Male) ? 1 : 2);
                    break;
                case eRealm.Hibernia:
                    Race = (short)eRace.Celt;
                    Model = (ushort)((Gender == eGender.Male) ? 171 : 172);
                    break;
                case eRealm.Midgard:
                    Race = (short)eRace.Norseman;
                    Model = (ushort)((Gender == eGender.Male) ? 51 : 52);
                    break;
            }
        }

        // Extremely simplified health/mana
        public override int MaxHealth => Level * 20 + 50;
        public override int MaxMana => 0;
        public override int MaxEndurance => 100;
    }
}
