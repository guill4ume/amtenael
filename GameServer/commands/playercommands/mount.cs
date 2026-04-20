using System;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Commands
{
    [CmdAttribute("&mount", ePrivLevel.Player, "Mount a flying mount", "/mount <griffin|dragon|dismount>")]
    public class MountCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (client.Player == null)
                return;

            if (args.Length < 2)
            {
                DisplaySyntax(client);
                return;
            }

            string subCommand = args[1].ToLower();

            if (subCommand == "dismount")
            {
                if (client.Player.IsOnFlyingMount)
                {
                    client.Player.IsOnFlyingMount = false;
                    client.Out.SendMessage("You dismount your flying mount.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                }
                else
                {
                    client.Out.SendMessage("You are not on a flying mount.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                }
                return;
            }

            if (client.Player.IsOnFlyingMount || client.Player.IsOnHorse || client.Player.IsRiding)
            {
                client.Out.SendMessage("You are already mounted!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            if (client.Player.InCombat)
            {
                client.Out.SendMessage("You cannot mount while in combat!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            eFlyingMountType type = eFlyingMountType.None;

            switch (subCommand)
            {
                case "griffin":
                case "griffon":
                    type = eFlyingMountType.Griffin;
                    break;
                case "dragon":
                    type = eFlyingMountType.Dragon;
                    break;
                default:
                    DisplaySyntax(client);
                    return;
            }

            // Set the mount type and activate
            client.Player.FlyingMountType = type;
            client.Player.IsOnFlyingMount = true;
            
            client.Out.SendMessage($"You successfully mount your {type}!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
            client.Out.SendMessage("Use Jump to go up, and X (or your down key) to descend.", eChatType.CT_Help, eChatLoc.CL_SystemWindow);
        }
    }
}
