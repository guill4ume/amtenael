using System;
using DOL.GS;
using DOL.GS.Commands;
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts
{
    [Cmd(
        "&spawnlightmimics",
        ePrivLevel.GM,
        "Spawn a battalion of LightMimicNPCs",
        "/spawnlightmimics <count> <realm 1/2/3> <level> <equipmentTemplate>")]
    public class SpawnLightMimicsCommand : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (args.Length < 5)
            {
                client.Out.SendMessage("Usage: /spawnlightmimics <count> <realm 1/2/3> <level> <equipmentTemplate>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            if (!int.TryParse(args[1], out int count)) count = 10;
            if (!int.TryParse(args[2], out int realmId)) realmId = 1;
            if (!byte.TryParse(args[3], out byte level)) level = 50;
            
            string equipmentTemplate = args[4];

            eRealm realm = (eRealm)realmId;

            LightMimicManager.SpawnBataillon(client.Player, count, realm, level, equipmentTemplate);
        }
    }
}
