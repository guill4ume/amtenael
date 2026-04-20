/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using log4net;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.InviteToGroup, "Handle Invite to Group Request.", eClientStatus.PlayerInGame)]
	public class InviteToGroupHandler : IPacketHandler
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			new HandleGroupInviteAction(client.Player).Start(1);
		}

		/// <summary>
		/// Handles group invlite actions
		/// </summary>
		protected class HandleGroupInviteAction : ECSGameTimerWrapperBase
		{
			/// <summary>
			/// constructs a new HandleGroupInviteAction
			/// </summary>
			/// <param name="actionSource">The action source</param>
			public HandleGroupInviteAction(GamePlayer actionSource) : base(actionSource) { }

			/// <summary>
			/// Called on every timer tick
			/// </summary>
			protected override int OnTick(ECSGameTimer timer)
			{
				GamePlayer player = (GamePlayer) timer.Owner;

				if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] {(player?.Name ?? "Unknown")} is attempting to invite target.");

				if (player.TargetObject == null || player.TargetObject == player)
				{
					if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] {player.Name} failed: target is null or self.");
					ChatUtil.SendSystemMessage(player, "You have not selected a valid player as your target.");
					return 0;
				}

				if (!(player.TargetObject is GamePlayer))
				{
					if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] {player.Name} failed: target {player.TargetObject.Name} is not a GamePlayer.");
					ChatUtil.SendSystemMessage(player, "You have not selected a valid player as your target.");
					return 0;
				}

				var target = (GamePlayer) player.TargetObject;
				
				if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] {player.Name} (Realm:{player.Realm}) inviting {target.Name} (Realm:{target.Realm}).");

				if (player.Group != null && player.Group.Leader != player)
				{
					if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] {player.Name} failed: not the leader of their group.");
					ChatUtil.SendSystemMessage(player, "You are not the leader of your group.");
					return 0;
				}

				if (player.Group != null && player.Group.MemberCount >= ServerProperties.Properties.GROUP_MAX_MEMBER)
				{
					if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] {player.Name} failed: group is full.");
					ChatUtil.SendSystemMessage(player, "The group is full.");
					return 0;
				}

				if (!GameServer.ServerRules.IsAllowedToGroup(player, target, false))
				{
					if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] {player.Name} failed: IsAllowedToGroup returned false for {target.Name}.");
					return 0;
				}

				if (target.Group != null)
				{
					if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] {player.Name} failed: target {target.Name} is already in a group.");
					ChatUtil.SendSystemMessage(player, "The player is still in a group.");
					return 0;
				}

				if (log.IsDebugEnabled) log.Debug($"[InviteToGroup] All checks passed. Sending invite from {player.Name} to {target.Name}.");

				string targetNameForPlayer = GameServer.ServerRules.GetPlayerName(player, target);
				string playerNameForTarget = GameServer.ServerRules.GetPlayerName(target, player);

				ChatUtil.SendSystemMessage(player, "You have invited " + targetNameForPlayer + " to join your group.");
				target.Out.SendGroupInviteCommand(player,
				                                  playerNameForTarget + " has invited you to join\n" + player.GetPronoun(1, false) +
				                                  " group. Do you wish to join?");
				ChatUtil.SendSystemMessage(target,
				                           playerNameForTarget + " has invited you to join " + player.GetPronoun(1, false) + " group.");

				return 0;
			}
		}
	}
}
