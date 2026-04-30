using System;
using System.Collections.Generic;
using System.Reflection;
using DOL.GS;
using DOL.GS.Realm;
using DOL.GS.Utils;
using log4net;

namespace DOL.GS.Scripts
{
    public static class SwarmManager
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void InitializeThidranki()
        {
            ushort region = 252;
            byte level = 50;
            int countPerRealm = 100; // Mass population!

            log.Info("SwarmManager: Initializing Thidranki battalions...");

            // Albion
            SpawnBattalion(new Point3D(37200, 51200, 3950), region, countPerRealm, eRealm.Albion, level);
            // Hibernia
            SpawnBattalion(new Point3D(19820, 19305, 4050), region, countPerRealm, eRealm.Hibernia, level);
            // Midgard
            SpawnBattalion(new Point3D(53300, 26100, 4270), region, countPerRealm, eRealm.Midgard, level);

            log.Info($"SwarmManager: Spawned {countPerRealm * 3} Ultra-Light bots in Thidranki.");
        }

        private static void SpawnBattalion(Point3D center, ushort region, int count, eRealm realm, byte level)
        {
            for (int i = 0; i < count; i++)
            {
                int offsetX = Util.Random(-800, 800);
                int offsetY = Util.Random(-800, 800);
                
                UltraLightMimicNPC swarmBot = new UltraLightMimicNPC(level, realm);
                swarmBot.X = center.X + offsetX;
                swarmBot.Y = center.Y + offsetY;
                swarmBot.Z = center.Z;
                swarmBot.CurrentRegionID = region;
                swarmBot.Heading = (ushort)Util.Random(0, 4095);

                swarmBot.AddToWorld();
            }
        }
    }
}
