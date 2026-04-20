using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DOL.AI.Brain;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Scripts;
using DOL.Language;

namespace DOL.GS.Scripts
{
	public class SimpleGvGGuard : GameNPC
	{
		public int BreamorFaction { get; set; }
		public GuildCaptainGuard Captain
		{
			get => (Brain as SimpleGvGGuardBrain)?.Captain;
			set
			{
				if (Brain is SimpleGvGGuardBrain guard)
					guard.Captain = value;
			}
		}

		public SimpleGvGGuard()
		{
			SetOwnBrain(new SimpleGvGGuardBrain());
		}

		public override string GetAggroLevelString(GamePlayer player, bool firstLetterUppercase)
		{
			if (Captain == null)
				return base.GetAggroLevelString(player, firstLetterUppercase);
			var plGuildId = player.Guild != null ? player.GuildID : "NOGUILD";
			if ((player.Guild != null && player.GuildName == GuildName) || Captain.safeGuildIds.Contains(plGuildId))
				return LanguageMgr.GetTranslation(player.Client.Account.Language, "GameNPC.GetAggroLevelString.Friendly1");

			return LanguageMgr.GetTranslation(player.Client.Account.Language, "GameNPC.GetAggroLevelString.Aggressive1");
		}

		public override bool Interact(GamePlayer player)
		{
			if (player.Client.Account.PrivLevel == 1 && !IsWithinRadius(player, InteractDistance))
			{
				player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "GameObject.Interact.TooFarAway", GetName(0, true)), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				Notify(GameObjectEvent.InteractFailed, this, new InteractEventArgs(player));
				return false;
			}
			Notify(GameObjectEvent.Interact, this, new InteractEventArgs(player));
			player.Notify(GameObjectEvent.InteractWith, player, new InteractWithEventArgs(this));

			if (string.IsNullOrWhiteSpace(GuildName) || player.Guild == null)
				return false;
			if (player.Client.Account.PrivLevel == 1 && player.GuildName != GuildName)
				return false;
			if (!player.GuildRank.Claim)
			{
				player.Out.SendMessage($"Bonjour {player.Name}, je ne discute pas avec les bleus, circulez.", eChatType.CT_System, eChatLoc.CL_PopupWindow);
				return true;
			}

			var cloaks = GameServer.Database.SelectAllObjects<DbNpcEquipment>().Where(item => item.TemplateID != null && item.TemplateID.StartsWith("gvg_guard_") && item.Slot == 26);
			player.Out.SendMessage(
				$"Bonjour {player.Name}, vous pouvez modifier l'équippement que je porte, sélectionnez l'ensemble que vous souhaitez :\n" +
				string.Join("\n", cloaks.Select(c => $"[{c.TemplateID.Substring(10)}]")),
				eChatType.CT_System,
				eChatLoc.CL_PopupWindow
			);
			return true;
		}
		public override bool WhisperReceive(GameLiving source, string text)
		{
			if (!base.WhisperReceive(source, text) || string.IsNullOrWhiteSpace(GuildName))
				return false;
			if (source is not GamePlayer player || player.Guild == null)
				return false;
			if (player.Client.Account.PrivLevel == 1 && player.GuildName != GuildName)
				return false;
			if (!player.GuildRank.Claim)
			{
				player.Out.SendMessage($"Bonjour {player.Name}, je ne discute pas avec les bleus, circulez.", eChatType.CT_System, eChatLoc.CL_PopupWindow);
				return true;
			}

			var cloaks = GameServer.Database.SelectAllObjects<DbNpcEquipment>().Where(item => item.TemplateID != null && item.TemplateID.StartsWith("gvg_guard_") && item.Slot == 26);
			text = $"gvg_guard_{text}";
			if (cloaks.Any(c => c.TemplateID == text))
				LoadEquipmentTemplateFromDatabase(text);
			RefreshEmblem();
			return true;
		}

		public override void TakeDamage(GameObject source, eDamageType damageType, int damage, int criticalAmount)
		{
			base.TakeDamage(source, damageType, damage, criticalAmount);
			if (Captain != null)
				Captain.LastGuardCombatTick = DateTime.UtcNow.Ticks;
		}

		public override void Die(GameObject killer)
		{
			base.Die(killer);

			if (Captain != null)
				Captain.LastGuardCombatTick = DateTime.UtcNow.Ticks;

			var plKiller = killer as GamePlayer;
			if (plKiller == null && killer is GameNPC npc)
				plKiller = npc.ControlledBrain?.GetPlayerOwner();
			if (plKiller != null)
			{
				var name = "un inconnu";
				if (!string.IsNullOrEmpty(plKiller.GuildName))
					name = $"un membre de la guilde {plKiller.GuildName}";
				
				var guild = GuildMgr.GetGuildByName(GuildName);
				guild?.SendMessageToGuildMembers($"{Captain?.Name ?? "Capitaine"}: un garde vient d'être tué par {name}.", eChatType.CT_Guild, eChatLoc.CL_ChatWindow);
/*				BreamorFactionMgr.SendMessageToSameFaction(BreamorFaction, $"{Captain?.Name ?? "Capitaine"}: un garde vient d'être tué par {name}.", eChatType.CT_Important, eChatLoc.CL_ChatWindow);
*/			}
		}

		public override bool AddToWorld()
		{
			if (Brain is not SimpleGvGGuardBrain)
				SetOwnBrain(new SimpleGvGGuardBrain());
			if (!base.AddToWorld())
				return false;
			RefreshEmblem();
			return true;
		}

		public void RefreshEmblem()
		{
			if (ObjectState != eObjectState.Active || CurrentRegion == null || Inventory?.VisibleItems == null)
				return;
			if (Captain == null)
			{
				var captain = GvGManager.allCaptains.MinBy(GetDistanceTo);
				if (GetDistanceTo(captain) < GvGManager.AREA_RADIUS * 2)
					Captain = captain;
				return;
			}
			BreamorFaction = Captain.BreamorFaction;
			var emblem = GvGManager.NEUTRAL_EMBLEM;
			var guild = GuildMgr.GetGuildByName(GuildName);
			if (guild != null)
				emblem = guild.Emblem;
			foreach (var item in Inventory.VisibleItems)
				if (item.SlotPosition == (int)eInventorySlot.Cloak || item.Object_Type == (int)eObjectType.Shield)
					item.Emblem = emblem;
			SaveIntoDatabase();
			BroadcastLivingEquipmentUpdate();
		}

		/*
		public override void WalkToSpawn(short speed)
		{
			this.CastSpellOnOwnerAndPets(this, SkillBase.GetSpellByID(GameHastener.SPEEDOFTHEREALMID), SkillBase.GetSpellLine(GlobalSpellsLines.Realm_Spells), false);
			base.WalkToSpawn(MaxSpeed);
		}
		*/

        public void UpdateTemplate(INpcTemplate template)
        {
            LoadTemplate(template);

            if (GuildName == "Le Clan des Brumes")
            {
                Model = 188;
            }

            BroadcastLivingEquipmentUpdate();

            NPCTemplate = template as NpcTemplate;

            SaveIntoDatabase();
        }
    }
}

namespace DOL.AI.Brain
{
	public class SimpleGvGGuardBrain : StandardMobBrain
	{
		private long _lastCaptainUpdate = 0;
		private GuildCaptainGuard _captain;

		private long _lastDeniedBAF = 0;
		private bool _canBaf = true;
		public virtual bool CanBAF
		{
			get
			{
				if (_canBaf)
					return true;
				return GameServer.Instance.TickCount - _lastDeniedBAF > 45000;
			}
			set
			{
				_canBaf = value;
				if (!value)
					_lastDeniedBAF = GameServer.Instance.TickCount;
			}
		}

		public GuildCaptainGuard Captain
		{
			get
			{
				if (_lastCaptainUpdate > DateTime.Now.Ticks)
					return _captain;
				_captain = GvGManager.allCaptains.MinBy(c => Body.GetDistanceTo(c));
				var name = _captain?.GuildName ?? "";
					if (name != Body.GuildName)
						Body.GuildName = name;
				_lastCaptainUpdate = DateTime.Now.Ticks + 60 * 1000 * 10000;
				return _captain;
			}
			set
			{
				_captain = value;
				_lastCaptainUpdate = DateTime.Now.Ticks + 60 * 1000 * 10000;
				var name = _captain?.GuildName ?? "";
				if (name != Body.GuildName)
					Body.GuildName = name;
				if (Body is SimpleGvGGuard guard) guard.BreamorFaction = _captain?.BreamorFaction ?? 0;
				(Body as SimpleGvGGuard)?.RefreshEmblem();
			}
		}

		public SimpleGvGGuardBrain()
		{
			AggroLevel = 100;
		}

		protected override void CheckPlayerAggro()
		{
			if (Body.AttackState)
				return;
			foreach (GamePlayer pl in Body.GetPlayersInRadius((ushort)AggroRange))
			{
				if (!pl.IsAlive || pl.ObjectState != GameObject.eObjectState.Active || !GameServer.ServerRules.IsAllowedToAttack(Body, pl, true))
					continue;

				var aggro = CalculateAggroLevelToTarget(pl);
				if (aggro <= 0)
					continue;
				AddToAggroList(pl, aggro);
			}
		}

		protected virtual void CheckNpcAggro()
		{
			if (Body.AttackState)
				return;
			foreach (GameNPC npc in Body.GetNPCsInRadius((ushort)AggroRange))
			{
				if (npc.Realm != 0 || (npc.Flags & GameNPC.eFlags.PEACE) != 0 ||
					!npc.IsAlive || npc.ObjectState != GameObject.eObjectState.Active ||
					npc is GameTaxi ||
					GetBaseAggroAmount(npc) > 0 ||
					!GameServer.ServerRules.IsAllowedToAttack(Body, npc, true))
					continue;

				var aggro = CalculateAggroLevelToTarget(npc);
				if (aggro <= 0)
					continue;
				AddToAggroList(npc, aggro);
				if (npc.Level > Body.Level)
					BringFriends(npc);
			}
		}

		public virtual int CalculateAggroLevelToTarget(GameLiving target)
		{
			if (target is GamePlayer player)
			{
				if (Captain == null)
					return target.GuildName == Body.GuildName ? 0 : 100;

				var plGuildId = player.Guild != null ? player.GuildID : "NOGUILD";
				if (target.GuildName == Body.GuildName || Captain.safeGuildIds.Contains(plGuildId))
					return 0;
				return 100;
			}
			if (target.Realm == 0)
				return 0;
			return target.Realm != 0 ? 100 : 0;
		}

		protected override void BringFriends(GameLiving attacker)
		{
			var friends = new HashSet<GameNPC>();
			if (!friends.Add(Body))
				return;

			if (!CanBAF || attacker.Level < Body.Level - 5)
				return;
			var puller = attacker as GamePlayer;
			if (puller == null && attacker is GameNPC controlledNpc && controlledNpc.Brain is IControlledBrain brain)
				puller = brain.GetPlayerOwner();
			if (puller == null)
				return;

			CanBAF = false; // Mobs only BAF once per fight

			var bg = puller.TempProperties.GetProperty(BattleGroup.BATTLEGROUP_PROPERTY, null as BattleGroup);
			var attackers = new HashSet<GamePlayer> {puller};
			foreach (GamePlayer player in puller.GetPlayersInRadius(3600))
			{
				if (attackers.Contains(player) || puller.Group.IsInTheGroup(player))
					continue;
				attackers.Add(player);
			}
		}
	}
}
