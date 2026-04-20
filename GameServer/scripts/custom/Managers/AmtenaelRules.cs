using System;
using System.Collections.Generic;
using System.Linq;
using DOL.GS.PacketHandler;
using DOL.GS.Scripts.Custom.Factions;
using DOL.GS.Keeps;
using DOL.AI.Brain;
using DOL.Events;

namespace DOL.GS.ServerRules
{
	[ServerRules(EGameServerType.GST_PvP)]
	public class AmtenaelRules : PvPServerRules
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static ushort HousingRegionID = 202;
		public static ushort[] UnsafeRegions = { 181 };
		public static ushort[] SafeZone = { 167, 168, 169, 179, 334 };

		public override string RulesDescription()
		{
			if (log.IsInfoEnabled) log.Info("AmtenaelRules: Ruleset [GST_PvP] selected and RulesDescription() executed.");
			return "Amtenael (GvG - Identité RP & Multi-Royaume)";
		}

		static AmtenaelRules()
		{
			if (log.IsInfoEnabled) log.Info("AmtenaelRules: Ruleset [GST_PvP] Static Constructor executed.");
		}

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			Console.WriteLine("AmtenaelRules: Ruleset [GST_PvP] successfully LOADED and ACTIVE.");
		}

		public override bool IsAllowedToAttack(GameLiving attacker, GameLiving defender, bool quiet)
		{
			if (attacker == null || defender == null)
				return false;

			if (!defender.IsAlive || !attacker.IsAlive)
				return false;

			if (attacker == defender)
			{
				if (!quiet) MessageToLiving(attacker, "Vous ne pouvez pas vous attaquer vous-même.");
				return false;
			}

			// GMs can't be attacked
			if (defender is GamePlayer defenderPlayer && defenderPlayer.Client.Account.PrivLevel > 1)
				return false;

			// Safe zones and areas
			if (SafeZone.Contains(attacker.CurrentRegionID) || SafeZone.Contains(defender.CurrentRegionID))
			{
				if (!quiet) MessageToLiving(attacker, "Vous ne pouvez pas attaquer dans une zone safe !");
				return false;
			}

			// Prison checks disabled: JailComponent not yet ported to SPB
			/*
			if (attacker is GamePlayer attackerPlayer)
			{
				var jail = attackerPlayer.GetServiceObject<JailComponent>(ServiceObjectType.JailComponent);
				if (jail != null && jail.IsPrisoner)
				{
					if (!quiet) MessageToLiving(attacker, "Vous ne pouvez pas attaquer lorsque vous êtes en prison.");
					return false;
				}
			}

			if (defender is GamePlayer defPlayer)
			{
				var jail = defPlayer.GetServiceObject<JailComponent>(ServiceObjectType.JailComponent);
				if (jail != null && jail.IsPrisoner)
				{
					if (!quiet) MessageToLiving(attacker, "Vous ne pouvez pas attaquer un prisonnier.");
					return false;
				}
			}
			*/
			// Group/Guild social rules
			if (attacker is GamePlayer attPlayer && defender is GamePlayer dPlayer)
			{
				if (attPlayer.Group != null && attPlayer.Group.IsInTheGroup(dPlayer))
				{
					if (!quiet) MessageToLiving(attPlayer, "Vous ne pouvez pas attaquer un membre de votre groupe.");
					return false;
				}

				if (attPlayer.Guild != null && attPlayer.Guild == dPlayer.Guild)
				{
					if (!quiet) MessageToLiving(attPlayer, "Vous ne pouvez pas attaquer un membre de votre guilde.");
					return false;
				}

				// For PvP interactions (Player vs Player), allow attack by default (FFA)
				return true;
			}

			// For all other cases (NPC vs Player, Player vs NPC, NPC vs NPC), use base rules (Normal/Standard)
			// This ensures faction checks and realm protection for guards are respected.
			return base.IsAllowedToAttack(attacker, defender, quiet);
		}

		public override void OnPlayerKilled(GamePlayer killedPlayer, GameObject killer)
		{
			base.OnPlayerKilled(killedPlayer, killer);

			var killerOwner = killer is GameNPC npc && npc.Brain is IControlledBrain brain ? brain.GetLivingOwner() : killer;
			if (killerOwner is GamePlayer killerPlayer)
			{
				// Calculate damage percent safely
				float damagePercent = 1.0f;
				if (killedPlayer.XPGainers.Contains(killerPlayer))
				{
                    float totalDamage = Convert.ToSingle(killedPlayer.XPGainers[killerPlayer]);
					float sumDamage = 0;
                    foreach(var val in killedPlayer.XPGainers.Values) sumDamage += Convert.ToSingle(val);
					if (sumDamage > 0)
						damagePercent = totalDamage / sumDamage;
				}

				// Update Breamor Factions (Karma)
				BreamorFactionMgr.UpdateFromKill(killerPlayer, killedPlayer, damagePercent);
			}
		}

		public static string GetFrenchRaceName(int race, eGender gender)
		{
			switch ((eRace)race)
			{
				case eRace.Briton: return gender == eGender.Male ? "Breton" : "Bretonne";
				case eRace.Avalonian: return gender == eGender.Male ? "Avalonien" : "Avalonienne";
				case eRace.Highlander: return "Highlander";
				case eRace.Saracen: return gender == eGender.Male ? "Sarrasin" : "Sarrasine";
				case eRace.Norseman: return gender == eGender.Male ? "Nordique" : "Nordique";
				case eRace.Troll: return "Troll";
				case eRace.Dwarf: return gender == eGender.Male ? "Nain" : "Naine";
				case eRace.Kobold: return "Kobold";
				case eRace.Celt: return "Celte";
				case eRace.Firbolg: return "Firbolg";
				case eRace.Elf: return "Elfe";
				case eRace.Lurikeen: return "Lurikeen";
				case eRace.Inconnu: return "Inconnu";
				case eRace.Valkyn: return "Valkyn";
				case eRace.Sylvan: return gender == eGender.Male ? "Sylvain" : "Sylvaine";
				case eRace.HalfOgre: return "Demi-Ogre";
				case eRace.Frostalf: return "Frostalf";
				case eRace.Shar: return "Shar";
				case eRace.Korazh:
				case eRace.Deifrang:
				case eRace.Graoch: return "Minotaure";
			}
			return "Humanoïde Inconnu";
		}

		public override string GetPlayerName(GamePlayer source, GamePlayer target)
		{
			if (source == target)
				return target.Name;

			if (source == null || target == null)
				return "Humanoïde";

			// Group Members see each other's real names (important for Client UI stability in GST_Normal)
			if (source.Group != null && source.Group == target.Group)
				return target.Name;

			// Guild Members see each other's real names
			if (source.Guild != null && source.Guild == target.Guild)
				return target.Name;

			// Handle RvR Anonymity if in RvR zones
			if (GameServer.KeepManager.FrontierRegionsList.Contains(source.CurrentRegionID) && GameServer.KeepManager.FrontierRegionsList.Contains(target.CurrentRegionID))
			{
				if (source.Realm != target.Realm)
					return GetFrenchRaceName(target.Race, target.Gender);
			}

			// RP Anonymity enabled
			// if staff or permission given, show name
			if (source.Client.Account.PrivLevel > 1)
				return base.GetPlayerName(source, target);

			return GetFrenchRaceName(target.Race, target.Gender);
		}

		public override string GetPlayerLastName(GamePlayer source, GamePlayer target)
		{
			if (source == target || source.Client.Account.PrivLevel > 1)
				return base.GetPlayerLastName(source, target);
			return "";
		}

		public override string GetPlayerGuildName(GamePlayer source, GamePlayer target)
		{
			// In Amtenael, guild names are hidden by default for everyone to enhance anonymity
			if (source == target || source.Client.Account.PrivLevel > 1)
				return base.GetPlayerGuildName(source, target);
			return "";
		}

		public override string GetPlayerTitle(GamePlayer source, GamePlayer target)
		{
			if (source == target || source.Client.Account.PrivLevel > 1)
				return base.GetPlayerTitle(source, target);
			return "";
		}

		public override bool IsAllowedToGroup(GamePlayer source, GamePlayer target, bool quiet)
		{
			// Allow cross-realm grouping in Amtenael except in RvR regions
			if (GameServer.KeepManager.FrontierRegionsList.Contains(source.CurrentRegionID) || GameServer.KeepManager.FrontierRegionsList.Contains(target.CurrentRegionID))
				return source.Realm == target.Realm;
			return true;
		}

		public override bool IsAllowedToTrade(GameLiving source, GameLiving target, bool quiet)
		{
			/* Prison checks disabled
			if (source is GamePlayer sourcePlayer)
			{
				var jail = sourcePlayer.GetServiceObject<JailComponent>(ServiceObjectType.JailComponent);
				if (jail != null && jail.IsPrisoner)
				{
					if (!quiet) MessageToLiving(source, "Vous ne pouvez pas faire d'échange en prison.");
					return false;
				}
			}

			if (target is GamePlayer targetPlayer)
			{
				var jail = targetPlayer.GetServiceObject<JailComponent>(ServiceObjectType.JailComponent);
				if (jail != null && jail.IsPrisoner)
				{
					if (!quiet) MessageToLiving(source, "Cette personne est en prison et ne peut pas échanger.");
					return false;
				}
			}
			*/

			if (GameServer.KeepManager.FrontierRegionsList.Contains(source.CurrentRegionID) || GameServer.KeepManager.FrontierRegionsList.Contains(target.CurrentRegionID))
				return source.Realm == target.Realm;
			return true;
		}

		public override byte GetLivingRealm(GamePlayer player, GameLiving target)
		{
			if (player == null || target == null) return 0;

			// If not in a Frontier (RvR) region, everyone appears as the viewer's realm (Blue/Friendly)
			// This forces the client to allow grouping and show health bars correctly in all server types.
			if (!GameServer.KeepManager.FrontierRegionsList.Contains(player.CurrentRegionID))
			{
				if (target is GamePlayer || (target is GameNPC && target.Realm != 0))
					return (byte)player.Realm;
			}

			// Same Group Member = Same Realm Lie (fallback for RvR regions if already grouped)
			if (player.Group != null && target.Group == player.Group)
				return (byte)player.Realm;

			// Standard logic as fallback
			return (byte)target.Realm;
		}

		public override bool IsAllowedToSpeak(GamePlayer source, string communicationType)
		{
			/* Prison checks disabled
			var jail = source.GetServiceObject<JailComponent>(ServiceObjectType.JailComponent);
			if (jail != null && jail.IsPrisoner)
			{
				// En prison, on ne peut parler qu'en local ou murmurer à un GM
				if (communicationType == "whisper" || communicationType == "say") 
					return true;
				
				source.Out.SendMessage("Vous ne pouvez pas utiliser ce canal de discussion en prison.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}
			*/
			return base.IsAllowedToSpeak(source, communicationType);
		}

		public override bool IsAllowedToZone(GamePlayer player, Region region)
		{
			/* Prison checks disabled
			var jail = player.GetServiceObject<JailComponent>(ServiceObjectType.JailComponent);
			if (jail != null && jail.IsPrisoner)
			{
				// Interdiction de sortir de la zone de prison (Region 497) par des moyens normaux
				if (region.ID != 497)
				{
					player.Out.SendMessage("Vous ne pouvez pas quitter la prison avant la fin de votre peine.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
					return false;
				}
			}
			*/
			return base.IsAllowedToZone(player, region);
		}

		public override bool IsAllowedToUnderstand(GameLiving source, GamePlayer target)
		{
			return true; // Everyone understands everyone in Amtenael
		}

		public override bool IsAllowedToJoinGuild(GamePlayer source, Guild guild)
		{
			if (GameServer.KeepManager.FrontierRegionsList.Contains(source.CurrentRegionID))
				return source.Realm == guild.Realm;
			return true;
		}

		public override bool IsSameRealm(GameLiving source, GameLiving target, bool quiet)
		{
			if (source == null || target == null) return false;
			if (source == target) return true;

			if (source is GamePlayer sourcePlayer && target is GamePlayer targetPlayer)
			{
				// Group members are always friendly
				if (sourcePlayer.Group != null && sourcePlayer.Group == targetPlayer.Group)
					return true;

				// Outside Frontier regions, everyone understands and respects everyone (Social Harmony)
				if (!GameServer.KeepManager.FrontierRegionsList.Contains(source.CurrentRegionID) && !GameServer.KeepManager.FrontierRegionsList.Contains(target.CurrentRegionID))
					return true;
			}

			// GMs are friendly to everyone
			if (source is GamePlayer gp && gp.Client.Account.PrivLevel > 1) return true;
			if (target is GamePlayer tgp && tgp.Client.Account.PrivLevel > 1) return true;

			return source.Realm == target.Realm;
		}
	}
}

