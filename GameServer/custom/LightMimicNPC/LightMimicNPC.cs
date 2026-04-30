using System;
using DOL.AI.Brain;
using DOL.GS;
using DOL.GS.PlayerClass; // For eCharacterClass
using DOL.GS.Utils; // For Util if needed
using DOL.Language;
using log4net;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using DOL.GS.Realm;

namespace DOL.GS.Scripts
{
    public class LightMimicNPC : GameNPC
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public eCharacterClass MimicClass { get; protected set; }

        public LightMimicNPC(eCharacterClass charClass, byte level, eRealm realm, string name, string equipmentTemplateId) : base()
        {
            this.Level = level;
            this.Realm = realm;
            this.MimicClass = charClass;
            this.Name = string.IsNullOrEmpty(name) ? charClass.ToString() : name;
            this.GuildName = "Light Mimic";

            // Default Race and Model based on Realm
            SetDefaultRaceAndModel();

            // Spawn point basic init
            this.SpawnPoint = new Point3D(X, Y, Z);
            this.RoamingRange = 500;
            this.MaxSpeedBase = 190; // Ensure they can walk

            // Assign the visual equipment
            if (!string.IsNullOrEmpty(equipmentTemplateId))
            {
                this.EquipmentTemplateID = equipmentTemplateId;
                this.LoadEquipmentTemplateFromDatabase(equipmentTemplateId);
            }
            
            // Assign custom lightweight brain
            this.SetOwnBrain(new LightMimicBrain());
        }

        private void SetDefaultRaceAndModel()
        {
            Gender = (eGender)Util.Random(0, 1);
            
            // Collect all races belonging to this realm
            var realmRaces = PlayerRace.AllRaces.Where(r => r.Realm == Realm).ToList();
            if (realmRaces.Count > 0)
            {
                PlayerRace randomRace = realmRaces[Util.Random(0, realmRaces.Count - 1)];
                Race = (short)randomRace.ID;
                Model = (ushort)randomRace.GetModel(Gender);
            }
            else
            {
                // Fallback
                Race = (short)eRace.Briton;
                Model = (ushort)((Gender == eGender.Male) ? 1 : 2);
            }
        }

        // Lightweight stat overrides - simple linear progression
        public override int MaxHealth => Level * 30 + 100;
        public override int MaxMana => Level * 20;
        public override int MaxEndurance => 100;

        public override short Strength => (short)(Level * 3);
        public override short Dexterity => (short)(Level * 3);
        public override short Constitution => (short)(Level * 3);
        public override short Quickness => (short)(Level * 3);
        public override short Intelligence => (short)(Level * 3);
        public override short Piety => (short)(Level * 3);
        public override short Empathy => (short)(Level * 3);
        public override short Charisma => (short)(Level * 3);
    }
}
