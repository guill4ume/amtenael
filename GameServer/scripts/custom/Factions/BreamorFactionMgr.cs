using System;
using System.Collections.Generic;
using System.Linq;
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts.Custom.Factions
{
	public class BreamorFactionMgr
	{
		public const int Zone_Min = -10_000_000;
		public const int Zone_Destructeurs = -10_001;
		public const int Zone_Neutral_Min = -10_000;
		public const int Zone_Neutral_Max = 10_000;
		public const int Zone_Constructeurs = 10_001;
		public const int Zone_Max = 10_000_000;

		public static readonly Dictionary<int, (string, string)> Zone_Ranks = new()
		{
			// Destructeurs
			{ -850_000, ("Diwaller", "Gardien de la destruction") },
			{ -600_000, ("Priñs", "Prince démon") },
			{ -400_000, ("Aotrou", "Seigneur de sang") },
			{ -250_000, ("Llygad", "Yeux des abysses") },
			{ -150_000, ("Evour", "Buveur de sang") },
			{ -100_000, ("Kastizer", "Bourreau de la destruction") },
			{ -0_65_000, ("Mestr", "Maitre du sang") },
			{ -0_40_000, ("Mestr", "Maitre des crânes") },
			{ -0_20_000, ("Kigerien", "Equarisseurs") },
			{ -0_10_001, ("Feal", "Fidèle de la destruction") },

			{ +0_10_000, ("Neutre", "Neutre") },

			// Constructeurs
			{ +0_10_001, ("Krouer", "Bâtisseur") },
			{ +0_20_000, ("Artizan", "Artisan") },
			{ +0_40_000, ("Tisour", "Architecte") },
			{ +0_65_000, ("Rener", "Planificateur") },
			{ +100_000, ("Diazezer", "Fondateur") },
			{ +150_000, ("Krouer", "Créateur") },
			{ +250_000, ("Ardivizer", "Artificier") },
			{ +400_000, ("Geniver", "Géniteur") },
			{ +600_000, ("Uhel", "Ordre Suprême") },
			{ +850_000, ("Lugan", "Lumière") },
		};

        public static bool IsNeutral(GameLiving living) => living is GamePlayer player && IsNeutral(player.BreamorFaction);
		public static bool IsNeutral(int faction) => Zone_Neutral_Min <= faction && faction <= Zone_Neutral_Max;

		public static bool IsDestructeur(GameLiving living) => living is GamePlayer player && IsDestructeur(player.BreamorFaction);
		public static bool IsDestructeur(int faction) => faction < Zone_Neutral_Min;

		public static bool IsConstructeur(GameLiving living) => living is GamePlayer player && IsConstructeur(player.BreamorFaction);
		public static bool IsConstructeur(int faction) => faction > Zone_Neutral_Max;

		public static bool IsSameFaction(GameLiving a, GameLiving b)
		{
            if (a is GamePlayer pa && b is GamePlayer pb)
                return IsSameFaction(pa.BreamorFaction, pb.BreamorFaction);
            return false;
		}

		public static bool IsSameFaction(int a, int b)
		{
			if (IsConstructeur(a) && IsConstructeur(b))
				return true;
			if (IsDestructeur(a) && IsDestructeur(b))
				return true;
			return false;
		}

		public static (string, string) GetRank(GameLiving living)
		{
            int faction = living is GamePlayer player ? player.BreamorFaction : 0;
			foreach (var (key, rank) in Zone_Ranks)
				if (faction <= key)
					return rank;
			return Zone_Ranks.Last().Value;
		}

		public static void UpdateFromKill(GamePlayer killer, GameLiving killed, float damageRatio)
		{
			if (killer == null || killed == null)
				return;

			if (killer.GetConLevel(killed) < -2) // no change if gray
				return;

            int killedFaction = killed is GamePlayer killedPlayer ? killedPlayer.BreamorFaction : 0;

			var killedValue = Math.Abs(killedFaction) * damageRatio / 500;
			if (killedFaction > 0)
				killedValue = -killedValue;

			if ((killedFaction < Zone_Neutral_Min && killer.BreamorFaction < Zone_Neutral_Min) || (killedFaction > Zone_Neutral_Max && killer.BreamorFaction > Zone_Neutral_Max))
				killedValue *= 10;

			Gain(killer, (int)killedValue);
		}

		public static void Gain(GamePlayer player, int amount)
		{
			player.BreamorFaction = Math.Clamp(player.BreamorFaction + amount, Zone_Min, Zone_Max);
			player.Out.SendMessage($"Faction changed: {amount} points (new faction: {player.BreamorFaction})", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
		}
	}
}
