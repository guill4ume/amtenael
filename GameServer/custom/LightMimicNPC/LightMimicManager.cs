using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS.PacketHandler;
using DOL.GS.Realm;
using DOL.GS.PlayerClass;
using DOL.GS.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DOL.GS.Scripts
{
    public static class LightMimicManager
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void InitializeThidranki()
        {
            ushort region = 252;
            byte level = 50;
            int count = 50;

            SpawnBataillonAt(new Point3D(37200, 51200, 3950), region, count, eRealm.Albion, level, "guard_albion_melee");
            SpawnBataillonAt(new Point3D(19820, 19305, 4050), region, count, eRealm.Hibernia, level, "guard_hibernia_melee");
            SpawnBataillonAt(new Point3D(53300, 26100, 4270), region, count, eRealm.Midgard, level, "guard_midgard_melee");
            
            log.Info($"Spawned {count} LightMimicNPCs per realm in Thidranki.");
        }

        public static void SpawnBataillonAt(Point3D center, ushort region, int count, eRealm realm, byte level, string equipmentTemplate)
        {
            int spawned = 0;
            for (int i = 0; i < count; i++)
            {
                eCharacterClass randomClass = GetRandomClass(realm);
                
                int offsetX = Util.Random(-500, 500);
                int offsetY = Util.Random(-500, 500);
                
                Point3D position = new Point3D(center.X + offsetX, center.Y + offsetY, center.Z);

                LightMimicNPC mimic = new LightMimicNPC(randomClass, level, realm, null, equipmentTemplate);
                
                mimic.X = position.X;
                mimic.Y = position.Y;
                mimic.Z = position.Z;
                mimic.CurrentRegionID = region;
                mimic.Heading = (ushort)Util.Random(0, 4095);

                if (mimic.AddToWorld())
                {
                    spawned++;
                }
            }
        }

        public static void SpawnBataillon(GamePlayer spawner, int count, eRealm realm, byte level, string equipmentTemplate)
        {
            if (spawner == null) return;
            SpawnBataillonAt(new Point3D(spawner.X, spawner.Y, spawner.Z), spawner.CurrentRegionID, count, realm, level, equipmentTemplate);
            spawner.Out.SendMessage($"Spawned {count} LightMimicNPCs of realm {realm}.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
        }

        private static eCharacterClass GetRandomClass(eRealm realm)
        {
            // Simplified, just returning a basic melee class for testing
            switch (realm)
            {
                case eRealm.Albion: return eCharacterClass.Armsman;
                case eRealm.Hibernia: return eCharacterClass.Hero;
                case eRealm.Midgard: return eCharacterClass.Warrior;
                default: return eCharacterClass.Armsman;
            }
        }
    }
}
