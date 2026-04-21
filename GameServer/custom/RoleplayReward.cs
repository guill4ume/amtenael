using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Scheduler;
using log4net;
using System.Reflection;

namespace OpenDAoC_SPB.Custom
{
	public static class RoleplayReward
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly int COOLDOWN_MINUTES_PER_WORD = 10;
		private static readonly int REWARD_TIMER_MINUTES = 5;
		private static readonly int RP_PER_WORD = 2; 
		
		private static readonly HashSet<string> RP_WORDS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			// Titres, salutations et rôles
			"messire", "damoiseau", "sire", "dame", "seigneur", "roi", "reine", "prince", "princesse", "monarque",
			"duc", "baron", "comte", "châtelain", "noble", "paladin", "chevalier", "preux", "héros", "champion",
			"mercenaire", "paysan", "tavernier", "barde", "ménestrel", "forgeron", "herboriste", "alchimiste",
			"archimage", "prêtre", "clerc", "moine", "brigand", "voleur", "assassin", "garde", "sentinelle",
			"capitaine", "nécromancien", "druide", "sorcier", "chaman", "chasseur", "archer", "guerrier",
			"gueux", "maraud", "manant", "félon", "misérable", "vil", "salutations", "adieu", "bienvenue",
			"écuyer", "sénéchal", "connétable", "intendant", "héraut", "page", "bailli", "prévôt", "chancelier",
			"grand-maître", "templier", "exorciste", "druidesse", "palfrenier", "éclaireur", "vigie", "messager",
			"confrère", "soeur", "frère", "père", "mère", "daim", "cerf", "lignée", "ancêtre", "suzerain", "vassal",
			
			// Lieux et géographie
			"royaume", "empire", "fief", "comté", "duché", "village", "hameau", "bourg", "auberge", "taverne",
			"crypte", "donjon", "abysse", "sanctuaire", "autel", "forteresse", "tour", "cité", "forêt", "temple",
			"grotte", "ruine", "nécropole", "château", "rempart", "frontière", "contrée", "territoire", "montagne",
			"val", "vallée", "rivière", "fleuve", "océan", "mer", "caverne", "tombeau", "herse", "douve", "meurtrière",
			"lice", "poterne", "chemin de ronde", "castrum", "abbaye", "ermitage", "cairn", "dolmen", "menhir",
			"bosquet", "marais", "estuaire", "archipel", "vallon", "causse", "plateau", "ravin", "gouffre",
			
			// Équipement, armes et objets
			"épée", "bouclier", "armure", "heaume", "hache", "lance", "arc", "grimoire", "rune", "talisman", "relique",
			"parchemin", "potion", "hydromel", "cotte", "mailles", "gantelet", "dague", "poignard", "masse", "fléau",
			"hallebarde", "javelot", "flèche", "carquois", "baguette", "sceptre", "couronne", "cape", "besace", "bourse",
			"amulette", "anneau", "joyau", "gemme", "cristal", "pierre", "acier", "fer", "mithril", "or", "argent",
			"gladius", "spatha", "claymore", "francisque", "framée", "scramasaxe", "broigne", "gambison", "haubert",
			"bassinet", "salade", "gorgerin", "éperon", "destrier", "palefroi", "roncin", "cuirasse", "boucle", "fourreau",
			
			// Magie, religion et concepts
			"magie", "sortilège", "malédiction", "rituel", "incantation", "prophétie", "oracle", "ombre", "flamme",
			"mana", "bénédiction", "pacte", "démoniaque", "divin", "sacré", "profane", "esprit", "âme", "dieu", "dieux",
			"déesse", "foi", "prière", "miracle", "ténèbres", "lumière", "enchantement", "illusion", "invocation",
			"dogme", "hérésie", "pénitence", "absolution", "ferveur", "dévotion", "ascèse", "mystique", "augure",
			
			// Thèmes, actions et émotions
			"honneur", "gloire", "sang", "bataille", "guerre", "quête", "destin", "serment", "alliance", "vengeance",
			"courage", "héroïsme", "siège", "victoire", "défaite", "trahison", "bravoure", "lâcheté", "mort", "vie",
			"croisade", "périple", "voyage", "festin", "combat", "lutte", "triomphe", "sacrifice", "péril", "danger",
			"embuscade", "escarmouche", "trêve", "paix", "justice", "châtiment", "pardon", "gloire", "renommée",
			"mandat", "quérir", "oyez", "festoyer", "guerroyer", "jouter", "pourfendre", "occire", "truander", "haranguer",
			"vaillant", "belliqueux", "infâme", "loyal", "fourbe", "austère", "magnanime", "intrépide", "outrecuidant",
			
			// Créatures et bestiaire (incluant Daoc)
			"dragon", "troll", "elfe", "nain", "orc", "gobelin", "géant", "vampire", "spectre", "griffon", "licorne",
			"hydre", "wyrm", "fée", "celte", "breton", "nordique", "loup", "démon", "fantôme", "goule", "squelette",
			"mort-vivant", "monstre", "bête", "créature", "minotaure", "centaure", "gargouille", "harpie", "golem",
			"lurikeen", "firbolg", "sylvestre", "sarrazin", "highlander", "kobold", "valkyrie", "berserker", "skald",
			"thane", "runiste", "druide", "barde", "eldritch", "enchanteur", "mentaliste", "animiste", "faucheur",
			
			// Exclamations et mots typiques DAoC / Médiéval
			"parbleu", "fichtre", "diantre", "pardi", "guilde", "royaume", "relique", "fort", "albion", "midgard", 
			"hibernia", "certes", "nonobstant", "hélas", "mortecouille", "morbleu", "ventrebleu", "sacrebleu", 
			"oyez", "nonpoint", "icelle", "icelui", "que trépas me prenne", "par les dieux", "par ma barbe",
			
			// Nature and Seasons
			"printemps", "été", "automne", "hiver", "givre", "rosée", "brise", "orage", "tempête", "météore", 
			"comète", "éclipse", "foudre", "tonnerre", "zénith", "aurore", "crépuscule", "autan", "borée", 
			"zéphyr", "frimas", "nimbes", "éther", "firmament", "constellation", "astral",
			
			// Food and Feast
			"festin", "banquet", "marcassin", "faisan", "rôtie", "tonnelet", "piquette", "cervoise", "vin", 
			"pain", "blé", "miche", "ripaille", "gaubiche", "écrouelles", "affamer", "rassasier", "abreuvé", 
			"écuelle", "broche", "victuailles", "fripouille", "godailleur", "souillon", "garde-manger",
			
			// Emotions and Concepts
			"courroux", "allégresse", "mélancolie", "tourment", "ravissement", "épouvante", "dédain", "fierté", 
			"orgueil", "humilité", "compassion", "rancoeur", "fureur", "tourmente", "destinée", "karma", 
			"équilibre", "chaos", "ordre", "dualité", "infini", "éphémère", "éternité", "auguste", "solennel",
			"vérité", "mensonge", "parjure", "loyauté", "vertu", "vice"
		};

		private const string COOLDOWN_DICT_KEY = "RP_Word_Cooldowns";
		private const string COUNTER_KEY = "RP_Word_Counter";

		private static ScheduledTask _rewardTask;

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			log.Info("RoleplayReward script loading...");
			Console.WriteLine("RoleplayReward script loading... (Console.WriteLine)");
			GameEventMgr.AddHandler(GameLivingEvent.Say, OnPlayerSay);
			GameEventMgr.AddHandler(GameLivingEvent.Yell, OnPlayerSay);
			GameEventMgr.AddHandler(GameLivingEvent.Whisper, OnPlayerSay);
			
			if (GameServer.Instance.Scheduler != null)
			{
				_rewardTask = GameServer.Instance.Scheduler.Start(RewardTick, REWARD_TIMER_MINUTES * 60 * 1000);
				log.Info("RoleplayReward timer started.");
				Console.WriteLine("RoleplayReward timer started. (Console.WriteLine)");
			}
			else
			{
				log.Error("RoleplayReward: GameServer.Instance.Scheduler is null!");
			}
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			GameEventMgr.RemoveHandler(GameLivingEvent.Say, OnPlayerSay);
			GameEventMgr.RemoveHandler(GameLivingEvent.Yell, OnPlayerSay);
			GameEventMgr.RemoveHandler(GameLivingEvent.Whisper, OnPlayerSay);
			
			if (_rewardTask != null)
			{
				_rewardTask.Stop();
				_rewardTask = null;
			}
		}

		private static void OnPlayerSay(DOLEvent e, object sender, EventArgs args)
		{
			if (!(sender is GamePlayer player)) return;
			if (!(args is SayEventArgs sayArgs)) return;

			string text = sayArgs.Text;
			if (string.IsNullOrEmpty(text) || text.StartsWith("/")) return;

			log.Debug($"RoleplayReward: {player.Name} said: {text}");

			MatchCollection words = Regex.Matches(text, @"\b[\p{L}]+\b");

			Dictionary<string, DateTime> cooldowns = player.TempProperties.GetProperty<Dictionary<string, DateTime>>(COOLDOWN_DICT_KEY);
			if (cooldowns == null)
			{
				cooldowns = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
				player.TempProperties.SetProperty(COOLDOWN_DICT_KEY, cooldowns);
			}

			int currentCounter = player.TempProperties.GetProperty<int>(COUNTER_KEY, 0);
			int newValidWords = 0;

			foreach (Match match in words)
			{
				string word = match.Value;

				if (RP_WORDS.Contains(word))
				{
					if (cooldowns.ContainsKey(word))
					{
						if ((DateTime.Now - cooldowns[word]).TotalMinutes < COOLDOWN_MINUTES_PER_WORD)
							continue; 
					}

					cooldowns[word] = DateTime.Now;
					newValidWords++;
					log.Debug($"RoleplayReward: {player.Name} used valid word: {word}");
				}
			}

			if (newValidWords > 0)
			{
				currentCounter += newValidWords;
				player.TempProperties.SetProperty(COUNTER_KEY, currentCounter);
			}
		}

		private static int RewardTick()
		{
			log.Info("RoleplayReward: RewardTick starting...");
			Console.WriteLine("RoleplayReward: RewardTick starting... (Console.WriteLine)");
			foreach (GameClient client in ClientService.GetClients())
			{
				if (client.Player == null || client.Player.ObjectState != GameObject.eObjectState.Active)
					continue;

				GamePlayer player = client.Player;
				int counter = player.TempProperties.GetProperty<int>(COUNTER_KEY, 0);

				if (counter > 0)
				{
					int rpReward = counter * RP_PER_WORD;
					player.GainRealmPoints(rpReward);
					
					string[] messages = new string[] 
					{ 
						$"Les dieux ont favorisé vos paroles habiles. (+{rpReward} RP)",
						$"Votre prestance marque les esprits. (+{rpReward} RP)",
						$"Le peuple loue votre éloquence. (+{rpReward} RP)",
						$"Votre verbe touche l'âme de ceux qui vous écoutent. (+{rpReward} RP)",
						$"La puissance de vos mots résonne dans tout le royaume. (+{rpReward} RP)",
						$"Vos paroles sont empreintes de sagesse et d'honneur. (+{rpReward} RP)",
						$"Un barde pourrait chanter vos récits. (+{rpReward} RP)",
						$"L'aura de votre noblesse grandit à chaque mot. (+{rpReward} RP)"
					};
					int rnd = Util.Random(0, messages.Length - 1);
					
					player.Out.SendMessage(messages[rnd], eChatType.CT_System, eChatLoc.CL_SystemWindow);
					player.TempProperties.SetProperty(COUNTER_KEY, 0);
				}
			}


			int nextTickMinutes = REWARD_TIMER_MINUTES + Util.Random(-1, 1);
			if (nextTickMinutes < 1) nextTickMinutes = 1;
			return nextTickMinutes * 60 * 1000;
		}
	}
}
