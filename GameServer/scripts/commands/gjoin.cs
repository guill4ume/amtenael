using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.ServerRules;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&gjoin",
		ePrivLevel.Player,
		"Allows you to join another player's group directly (Cross-realm bypass).",
		"/gjoin <playername>")]
	public class GJoinCommand : ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			GamePlayer player = client.Player;
			if (player == null) return;

			if (args.Length < 2)
			{
				player.Out.SendMessage("Usage: /gjoin <playername>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			string targetName = args[1];
			GamePlayer target = ClientService.GetPlayerByExactName(targetName);

			if (target == null)
			{
				player.Out.SendMessage("Player " + targetName + " not found or not online.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			if (target == player)
			{
				player.Out.SendMessage("You cannot join yourself.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			if (player.GetDistance(target) > 2000)
			{
				player.Out.SendMessage("You are too far from " + target.Name + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			AmtenaelRules.ForceJoin(player, target);
		}
	}
}
