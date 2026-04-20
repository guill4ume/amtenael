using System;
using DOL.Database;
using DOL.GS.PacketHandler;
using log4net;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using DOL.GS.Scripts.Custom.Factions;

namespace DOL.GS.Scripts
{
	public class GuildCaptainGuard : GameNPC
	{
		public int BreamorFaction { get; set; }
		public Guild Guild { get; private set; }
		public List<string> safeGuildIds = new();
		
		public long LastClaimTick { get; set; }
		public long LastGuardCombatTick { get; set; }

		/*
		public override List<AmteCustomParam> GetCustomParam()
		{
			var list = base.GetCustomParam();
			list.Add(new AmteCustomParam(
				"safeGuildIds",
				() => string.Join(";", safeGuildIds),
				v => safeGuildIds = v.Split(';').ToList(),
				""));
			list.Add(new AmteCustomParam(
				"breamorFaction",
				() => BreamorFaction.ToString(),
				v => int.TryParse(v, out BreamorFaction),
				"0"));
			list.Add(new AmteCustomParam(
				"LastClaimTick",
				() => LastClaimTick.ToString(),
				v => { if (long.TryParse(v, out var tick)) LastClaimTick = tick; else LastClaimTick = 0; },
				"0"));
			list.Add(new AmteCustomParam(
				"LastGuardCombatTick",
				() => LastGuardCombatTick.ToString(),
				v => { if (long.TryParse(v, out var tick)) LastGuardCombatTick = tick; else LastGuardCombatTick = 0; },
				"0"));
			return list;
		}
		*/

		//Select template based on Captain level
        private static int GetTemplateIdForLevel(int level)
        {
            switch (level)
            {
                case 60:
                    return 11111;
                case 65:
                    return 22222;
                case 70:
                    return 33333;
                default:
                    return 44444; 
            }
        }

        public override bool AddToWorld()
		{
			var r = base.AddToWorld();
			Guild = GuildMgr.GetGuildByName(GuildName);
			GvGManager.allCaptains.Add(this);
			return r;
		}

		public override bool RemoveFromWorld()
		{
			GvGManager.allCaptains.Remove(this);
			return base.RemoveFromWorld();
		}

		public bool CanInteractWith(GamePlayer player)
		{
			if (player.Client?.Account?.PrivLevel >= (uint)ePrivLevel.GM)
			{
				return true;
			}
			if (Guild != null && Guild == player.Guild)
				return true;

			var plGuildId = player.Guild != null ? player.GuildID : "NOGUILD";
			if (safeGuildIds.Contains(plGuildId))
				return true;
			
			return false;
		}

		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
			{
				return false;
			}

			bool sameFaction = BreamorFactionMgr.IsSameFaction(this, player);
			bool hasGuildClaim = player.Guild != null && player.GuildRank.Claim;
			bool canGuildClaim = hasGuildClaim && Guild == player.Guild;
			var actions = new List<(bool, string)>
			{
				(hasGuildClaim && Guild != player.Guild, "capturer le territoire pour ma guilde"),
				(canGuildClaim, "modifier les alliances"),
                (canGuildClaim, "entrainez vos gardes"),

            };

			var title = player.GuildRank?.Title ?? BreamorFactionMgr.GetRank(player).Item1;
			var actionStr = string.Join('\n', actions.Where(act => act.Item1).Select(act => $"[{act.Item2}]"));
			player.Out.SendMessage($"Bonjour {player.Name}, que puis-je faire pour vous ?\n\n{actionStr}", eChatType.CT_System, eChatLoc.CL_PopupWindow);
			return true;
		}

		public override bool WhisperReceive(GameLiving source, string text)
		{

			/*if (!base.WhisperReceive(source, text) || source is not GamePlayer player)
			{
				return false;
			}*/

			if (source == null || text == null || source is not GamePlayer player)
			{
				return false;
			}

			long whisperdelay = player.TempProperties.GetProperty<long>("WHISPERDELAY", 0);
			if (whisperdelay > 0 && (CurrentRegion.Time - 1500) < whisperdelay && player.Client?.Account?.PrivLevel == 1)
			{
				player.Out.SendMessage("Speak slower!", eChatType.CT_ScreenCenter, eChatLoc.CL_SystemWindow);
				return false;
			}
			
			player.TempProperties.SetProperty("WHISPERDELAY", CurrentRegion.Time);

			var sameFaction = BreamorFactionMgr.IsSameFaction(this, player);
			var hasGuildClaim = player.Guild != null && player.GuildRank.Claim;
			var canGuildClaim = hasGuildClaim && Guild == player.Guild;

			switch (text)
			{
                case "entrainez vos gardes":
                    if (!canGuildClaim)
                    {
                        player.Out.SendMessage("Vous n'êtes pas autorisé à entrainer ces gardes.", eChatType.CT_System, eChatLoc.CL_PopupWindow);
                        return false;
                    }

                    player.Out.SendMessage(
                        "Comment souhaitez vous les entrainer ?\n" +
                        "[Level 60 (500g)]\n" +
                        "[Level 65 (1p)]\n" +
                        "[Level 70 (2p)]", eChatType.CT_System, eChatLoc.CL_PopupWindow);
                    return true;

                case "Level 60 (500g)":
                    UpgradeGuards(player, 60, (int)Money.GetMoney(0, 0, 500, 0, 0));
                    return true;

                case "Level 65 (1p)":
                    UpgradeGuards(player, 65, (int)Money.GetMoney(0, 1, 0, 0, 0));
                    return true;

                case "Level 70 (2p)":
                    UpgradeGuards(player, 70, (int)Money.GetMoney(0, 2, 0, 0, 0));
                    return true;

                case "capturer le territoire pour ma guilde":
				{
					if (hasGuildClaim)
					{
						Claim(player, player.Guild, player.BreamorFaction);
					}
					return true;
				}

				case "modifier les alliances":
					if (!canGuildClaim)
					{
						return false;
					}

					var guilds = GuildMgr.GetAllGuilds()
						.Where(g => !GvGManager.systemGuildIds.Contains(g.GuildID) && g.GuildID != Guild.GuildID)
						.OrderBy(g => g.Name)
						.Select(g =>
						{
							var safe = safeGuildIds.Contains(g.GuildID);
							if (safe)
							{
								return $"{g.Name}: [{g.ID}. attaquer à vue]";
							}
							return $"{g.Name}: [{g.ID}. ne plus attaquer à vue]";
						})
						.Aggregate((a, b) => $"{a}\n{b}");
					var safeNoGuild = safeGuildIds.Contains("NOGUILD");
					guilds += $"\nLes sans guildes: [256.{(safeNoGuild ? "" : " ne plus")} attaquer à vue]";
					player.Out.SendMessage($"Voici la liste des guildes et leurs paramètres :\n${guilds}", eChatType.CT_System, eChatLoc.CL_PopupWindow);
					return true;
			}

			// change guild allies
			if (!canGuildClaim)
			{
				return false;
			}
			var dotIdx = text.IndexOf('.');
			if (dotIdx <= 0 || !ushort.TryParse(text[..dotIdx], out var id))
			{
				return false;
			}

			var guild = GuildMgr.GetAllGuilds().FirstOrDefault(g => g.ID == id);
			if (guild == null && id != 256)
			{
				return false;
			}
			var guildID = guild == null ? "NOGUILD" : guild.GuildID;
			if (safeGuildIds.Contains(guildID))
			{
				safeGuildIds.Remove(guildID);
			}
			else
			{
				safeGuildIds.Add(guildID);
			}
			SaveIntoDatabase();
			return WhisperReceive(source, "modifier les alliances");
		}

		public IEnumerable<SimpleGvGGuard> GetGuardsInRadius(ushort radius = GvGManager.AREA_RADIUS)
		{
			return GetNPCsInRadius(radius).OfType<SimpleGvGGuard>().Where(npc => npc.Captain == this);
		}

		public void ResetArea(int newemblem)
		{
            int newTemplateId = GetTemplateIdForLevel(this.Level);

            INpcTemplate newTemplate = NpcTemplateMgr.GetTemplate(newTemplateId);

            if (newTemplate == null)
			{
				log.Error($"NPCTemplate '{newTemplateId}' not found for Captain at level {this.Level}.");
				return;
			}
        
            if (Inventory != null)
			{
				foreach (var item in Inventory.VisibleItems)
				{
					if (item.SlotPosition == (int)eInventorySlot.Cloak || item.Object_Type == (int)eObjectType.Shield)
					{
						item.Emblem = newemblem;
					}
				}
			}
			BroadcastLivingEquipmentUpdate();
			foreach (var guard in GetGuardsInRadius())
			{
                guard.UpdateTemplate(newTemplate);
				if (GuildName == "Le Clan des Brumes")
                {
                    guard.Model = 188;
                }
				guard.Captain = this;
			}
			foreach (var obj in GetItemsInRadius(GvGManager.AREA_RADIUS))
			{
				if (obj is GameStaticItem item)
				{
					item.Emblem = newemblem;
					item.SaveIntoDatabase();
				}
			}
		}

        private void UpgradeGuards(GamePlayer player, int newLevel, int cost)
        {
            if (newLevel < Level)
            {
                player.Out.SendMessage(
                    "Vous ne pouvez pas rétrograder les gardes.",
                    eChatType.CT_System,
                    eChatLoc.CL_PopupWindow
                );
                return;
            }

            if (player.Guild != null && !player.RemoveMoney(cost))
            {
                player.Out.SendMessage(
                    "Vous n'avez pas assez d'argent pour entrainer ces gardes.",
                    eChatType.CT_System,
                    eChatLoc.CL_PopupWindow
                );
                return;
            }

            Level = (byte)newLevel;
			this.Level = Level;
            SaveIntoDatabase();

            int emblem = player.Guild?.Emblem ?? GvGManager.NEUTRAL_EMBLEM;
            ResetArea(emblem);

            player.Out.SendMessage($"Vos soldats ont été entrainé au niveau {newLevel}!", eChatType.CT_System, eChatLoc.CL_PopupWindow);
        }

        public void Claim(GamePlayer player, Guild guild, int faction)
		{
			if (!Name.StartsWith("Capitaine") && !Name.StartsWith("Diwaller") && !Name.StartsWith("Lugan"))
			{
				player.Out.SendMessage(
					"Vous devez demander à un GM pour ce type de territoire.",
					eChatType.CT_System,
					eChatLoc.CL_PopupWindow
				);
				return;
			}

			if (!GvGManager.IsOpen(player))
			{
				return;
			}

			if (player.Client?.Account?.PrivLevel < (uint)ePrivLevel.GM && GetGuardsInRadius(GvGManager.AREA_RADIUS * 2).Any(g => g.IsAlive))
			{
				player.Out.SendMessage(
					"Vous devez tuer tous les gardes avant de pouvoir prendre possession du territoire.",
					eChatType.CT_System,
					eChatLoc.CL_PopupWindow
				);
				return;
			}

            if (player.Client?.Account?.PrivLevel < (uint)ePrivLevel.GM)
            {
                var now = DateTime.UtcNow.Ticks;
                var safeUntil = LastClaimTick + GvGManager.CLAIM_SAFE_DURATION.Ticks;
                if (now < safeUntil)
                {
                    var remaining = TimeSpan.FromTicks(safeUntil - now);
                    player.Out.SendMessage(
                        $"Le territoire est protégé pendant encore {remaining.Minutes}m {remaining.Seconds}s.",
                        eChatType.CT_System,
                        eChatLoc.CL_PopupWindow
                    );
                    return;
                }
            }

			if (guild != null && !player.RemoveMoney(GvGManager.CLAIM_COST))
			{
				player.Out.SendMessage(
					"Vous n'avez pas assez d'argent pour prendre possession du territoire.",
					eChatType.CT_System,
					eChatLoc.CL_PopupWindow
				);
				return;
			}

			int emblem;
			if (guild == null)
			{
                player.Out.SendMessage(
                    "Vous devez avoir une guilde pour prendre possession du territoire.",
                    eChatType.CT_System,
                    eChatLoc.CL_PopupWindow
                );
                return;
            }
			else
			{
				Level = 50;
				// BreamorFaction = 0;
				Guild = guild;
				GuildName = guild.Name;
				emblem = guild.Emblem;
			}

			SaveIntoDatabase();
			ResetArea(emblem);
			if (guild != null)
			{
				player.Out.SendMessage(
					"Le territoire appartient maintenant à votre guilde, que voulez-vous faire ?\n\n[modifier les alliances]\n",
					eChatType.CT_System,
					eChatLoc.CL_PopupWindow
				);
				LastClaimTick = DateTime.UtcNow.Ticks;
				SaveIntoDatabase();
			}
			else
			{
				player.Out.SendMessage(
					$"Le territoire appartient maintenant aux {GuildName} !",
					eChatType.CT_System,
					eChatLoc.CL_PopupWindow
				);
			}
		}
	}
}