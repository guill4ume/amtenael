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
		private static readonly int RP_PER_WORD = 5; 
		
		private static readonly HashSet<string> RP_WORDS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			// ==========================================
			// 1. UNIVERS DARK AGE OF CAMELOT (DAoC)
			// ==========================================
			
			// Classes & Archétypes
			"paladin", "maître d'armes", "théurge", "cabaliste", "sorcier", "clerc", "moine", "mercenaire", "infiltrateur", "fléau d'arawn", "hérétique",
			"guerrier", "berserker", "chasseur", "skald", "thane", "runiste", "spiritiste", "chaman", "guérisseur", "ombre", "bogdar", "sauvage", "valkyrie",
			"héros", "finelame", "veilleurs", "barde", "eldritch", "enchanteur", "mentaliste", "druide", "protecteur", "empathe", "nightshade", "faucheur",
			"animiste", "valewalker", "vampiir", "sentinelle", "champion", "faucheur", "faucheuse", "relique", "fort", "avant-poste",
			
			// Races
			"breton", "avalonien", "sarrazin", "highlander", "inconnu", "demi-ogre", "nordique", "nain", "troll", "kobold", "frostalf", "valkyn",
			"celte", "elfe", "firbolg", "lurikeen", "sylvestre", "shar", "korazh", "graoch", "luridien",
			
			// Lieux & Zones emblématiques
			"camelot", "jordheim", "tir na nog", "salisbury", "snowdonia", "dartmoor", "myrkwood", "raumarike", "malmohus", "lough derg",
			"lyonesse", "sauvage", "glass", "svealand", "mularn", "moher", "sheeroe", "connacht", "shannon", "pennine", "anderson",
			"emain macha", "breifine", "odin", "thor", "freya", "hibernia", "albion", "midgard", "frontier", "passage",
			
			// Bestiaire DAoC
			"golestandt", "cuuldurach", "legion", "siabra", "bwca", "puca", "kelpie", "morrigane", "glimmer", "granit", "sylvain", "draco", "wyrm",
			
			// ==========================================
			// 2. VIE MÉDIÉVALE, ARTISANAT & SOCIAL
			// ==========================================
			
			// Rôles & Titres
			"messire", "damoiseau", "sire", "dame", "seigneur", "roi", "reine", "prince", "princesse", "monarque", "duc", "baron", "comte",
			"châtelain", "noble", "chevalier", "preux", "héros", "champion", "écuyer", "sénéchal", "connétable", "intendant", "héraut", "page",
			"bailli", "prévôt", "chancelier", "grand-maître", "templier", "exorciste", "druidesse", "palfrenier", "éclaireur", "vigie", "messager",
			"confrère", "soeur", "frère", "père", "mère", "suzerain", "vassal", "chambellan", "maréchal", "vidame", "trouvère", "troubadour",
			"saltimbanque", "apothicaire", "tanneur", "tisserand", "forgeron", "herboriste", "alchimiste", "archimage", "prêtre", "clerc", "moine",
			"brigand", "voleur", "assassin", "garde", "sentinelle", "capitaine", "nécromancien", "druide", "sorcier", "chaman", "chasseur",
			"archer", "guerrier", "gueux", "maraud", "manant", "félon", "misérable", "vil", "acapte", "adoubeur", "aubain", "baile", "bégard",
			"capitulaire", "captal", "chirographe", "clergesse", "coutilier", "écolâtre", "huchier", "imagier", "jurat", "mire", "drapier", "colporteur",
			
			// Vie quotidienne & Cuisine
			"taverne", "auberge", "festin", "hydromel", "cervoise", "hypocras", "venaison", "brouet", "gruau", "miche", "saugrenée", "potée",
			"mansion", "manteline", "cliquette", "hanap", "aquamanile", "isopet", "ysopet", "fable", "fabliau", "fatrasie", "lendit",
			
			// ==========================================
			// 3. ARCHITECTURE & FORTIFICATIONS
			// ==========================================
			
			"royaume", "empire", "fief", "bourg", "hameau", "village", "cité", "forteresse", "château", "tour", "donjon", "rempart",
			"herse", "douve", "meurtrière", "lice", "poterne", "chemin de ronde", "castrum", "abbaye", "ermitage", "cairn", "dolmen",
			"menhir", "bosquet", "marais", "estuaire", "archipel", "vallon", "causse", "plateau", "ravin", "gouffre", "crypte", "abysse",
			"sanctuaire", "autel", "frontière", "contrée", "territoire", "montagne", "val", "vallée", "rivière", "fleuve", "océan", "mer",
			"caverne", "tombeau", "assommoir", "barbacane", "bastille", "bretèche", "caponnière", "châtelet", "courtine", "cunette", "créneau",
			"échauguette", "embrasure", "mâchicoulis", "merlon", "motte castrale", "pont-dormant", "oubliette", "hourd", "bassefosse", "ferté",
			
			// ==========================================
			// 4. ÉQUIPEMENT, ARMES & ARMURES
			// ==========================================
			
			"épée", "bouclier", "armure", "heaume", "hache", "lance", "arc", "grimoire", "rune", "talisman", "relique", "parchemin", "potion",
			"cotte", "mailles", "gantelet", "dague", "poignard", "masse", "fléau", "hallebarde", "javelot", "flèche", "carquois", "baguette",
			"sceptre", "couronne", "cape", "besace", "bourse", "amulette", "anneau", "joyau", "gemme", "cristal", "pierre", "acier", "fer",
			"mithril", "or", "argent", "gladius", "spatha", "claymore", "francisque", "framée", "scramasaxe", "broigne", "gambison", "haubert",
			"bassinet", "salade", "gorgerin", "éperon", "destrier", "palefroi", "roncin", "cuirasse", "boucle", "fourreau", "miséricorde",
			"espadon", "vouge", "pertuisane", "fauchart", "arbalète", "brigandine", "camail", "plastron", "brassard", "grève", "soleret",
			"cuissard", "spallière", "armet", "pavois", "targe", "dondaine", "espringale", "guisarme", "haubergeon", "heaumier", "mézail", "bouterolle",
			
			// ==========================================
			// 5. MAGIE, RELIGION & ALCHIMIE
			// ==========================================
			
			"magie", "sortilège", "malédiction", "rituel", "incantation", "prophétie", "oracle", "ombre", "flamme", "mana", "bénédiction",
			"pacte", "démoniaque", "divin", "sacré", "profane", "esprit", "âme", "dieu", "dieux", "déesse", "foi", "prière", "miracle",
			"ténèbres", "lumière", "enchantement", "illusion", "invocation", "dogme", "hérésie", "pénitence", "absolution", "ferveur",
			"dévotion", "ascèse", "mystique", "augure", "athanor", "cornue", "aludel", "creuset", "alambic", "transmutation", "hermétique",
			"ésotérisme", "occultisme", "cosmogonie", "cosmologie", "eschatologie", "sotériologie", "hiérogamie", "analogie", "décoction",
			"pèlerin", "excommunication", "phylactère", "abjurateur", "ectoplasme", "khumeia", "alchimia",
			
			// ==========================================
			// 6. THÈMES, ACTIONS & ÉMOTIONS
			// ==========================================
			
			"honneur", "gloire", "sang", "bataille", "guerre", "quête", "destin", "serment", "alliance", "vengeance", "courage", "héroïsme",
			"siège", "victoire", "défaite", "trahison", "bravoure", "lâcheté", "mort", "vie", "croisade", "périple", "voyage", "combat",
			"lutte", "triomphe", "sacrifice", "péril", "danger", "embuscade", "escarmouche", "trêve", "paix", "justice", "châtiment",
			"pardon", "renommée", "mandat", "quérir", "oyez", "festoyer", "guerroyer", "jouter", "pourfendre", "occire", "truander",
			"haranguer", "vaillant", "belliqueux", "infâme", "loyal", "fourbe", "austère", "magnanime", "intrépide", "outrecuidant",
			"bouter", "férir", "choir", "ouïr", "mandier", "mander", "bailler", "vergogner", "conchier", "mugueter", "pandiculer",
			"adouber", "ensaisiner", "ordalie", "plaid", "wergeld", "niquedouille", "baliverne", "bobance", "braconnier", "bramer",
			"courroux", "allégresse", "mélancolie", "tourment", "ravissement", "épouvante", "dédain", "fierté", 
			"orgueil", "humilité", "compassion", "rancoeur", "fureur", "tourmente", "destinée", "karma", 
			"équilibre", "chaos", "ordre", "dualité", "infini", "éphémère", "éternité", "auguste", "solennel",
			"vérité", "mensonge", "parjure", "loyauté", "vertu", "vice", "légende", "chroniques", "épopée",
			
			// ==========================================
			// 7. CRÉATURES & BESTIAIRE
			// ==========================================
			
			"dragon", "troll", "elfe", "nain", "orc", "gobelin", "géant", "vampire", "spectre", "griffon", "licorne", "hydre", "wyrm",
			"fée", "celte", "loup", "démon", "fantôme", "goule", "squelette", "mort-vivant", "monstre", "bête", "créature", "minotaure",
			"centaure", "gargouille", "harpie", "golem", "harpie", "satyre", "titan", "hippocampe", "basilic", "chimère", "banshee",
			"farfadet", "gnome", "dryade", "nymphe", "sylphide", "ondine", "kraken", "léviathan", "manticore", "wyverne", "pégase",
			"aspi", "cocatrix", "échéneis", "lantichare", "leucota", "monocéros", "vouivre", "amphisbène", "loup-garou",
			
			// ==========================================
			// 8. HÉRALDIQUE (BLASONS & SYMBOLES)
			// ==========================================
			
			"azur", "gueules", "sinople", "sable", "hermine", "vair", "blason", "armoiries", "écu", "chevron", "fasce", "pal", "sautoir",
			"lambel", "billette", "besant", "bezant", "alérion", "merlette", "fleur de lys", "quintefeuille", "rampant", "passant", "volant",
			"issant", "essorant", "contourné", "écartelé", "échiqueté", "émail", "métaux", "brisure", "lambrequin", "cimier", "bourdon",
			"pairle", "vivré", "saltire", "orle", "macle", "fusée", "frette", "écu", "écusson", "cabochon", "chaperon", "senestre", "dextre",
			
			// ==========================================
			// 9. FLORE & HERBORISTERIE
			// ==========================================
			
			"belladone", "mandragore", "jusquiame", "datura", "ciguë", "digitale", "vératre", "livèche", "aurone", "hysope", "verveine",
			"armoise", "millepertuis", "sauge", "romarin", "thym", "sarriette", "marjolaine", "angélique", "bénédicte", "souci", "mauve",
			"guimauve", "bouillon blanc", "coquelicot", "bleuet", "bourrache", "consoude", "ortie", "pissenlit", "plantain", "achillée",
			"tanaisie", "absinthe", "rue", "sétaire", "camomille", "valériane", "mélisse", "fenouil", "aneth", "coriandre", "cumin", "carvi",
			
			// ==========================================
			// 10. MÉTIERS & RÔLES SOCIAUX (EXTENTION)
			// ==========================================
			
			"tanneur", "teinturier", "cordonnier", "charpentier", "maçon", "tailleur", "tisserand", "meunier", "boulanger", "boucher",
			"poissonnerie", "tavernier", "aubergiste", "palfrenier", "maréchal-ferrant", "armurier", "heaumier", "arbalétrier", "archetier",
			"forgeron", "orfèvre", "changeur", "mercier", "drapier", "pelletier", "parcheminier", "enlumineur", "copiste", "relieur",
			"apothicaire", "barbier", "chirurgien", "mire", "alchimiste", "astrologue", "ménestrel", "trouvère", "troubadour", "jongleur",
			"acrobate", "saltimbanque", "bouffon", "héraut", "crieur", "sergent", "garde", "guetteur", "veilleur", "prévôt", "bailli",
			"sénéchal", "connétable", "chambellan", "échanson", "panetier", "sommelier", "cuisinier", "marmiton", "lavandière", "lingère",
			"fileuse", "tisserande", "brodeuse", "dentellière", "nourrice", "gouvernante", "sage-femme", "prêtre", "moine", "nonne",
			"abbé", "abbesse", "évêque", "cardinal", "pape", "ermite", "pèlerin", "croisé", "chevalier", "écuyer", "page", "suzerain",
			"vassal", "serf", "vilain", "manant", "paysan", "laboureur", "vigneron", "berger", "porcher", "bûcheron", "charbonnier",
			"chasseur", "braconnier", "pêcheur", "batelier", "marin", "corsaire", "pirate", "brigand", "voleur", "assassin", "mendiant",
			"gueux", "vagabond", "lépreux", "bourreau", "geôlier",
			
			// ==========================================
			// 11. OBJETS, MOBILIER & QUOTIDIEN
			// ==========================================
			
			"coffre", "huche", "dressoir", "bahut", "escabeau", "banc", "tabouret", "chaise", "fauteuil", "lit", "paillasse", "traversin",
			"oreiller", "courtepointe", "couverture", "drap", "rideau", "tapisserie", "tapis", "natte", "chandelier", "bougeoir", "torche",
			"lanterne", "lampe à huile", "brasero", "cheminée", "chenet", "soufflet", "tisonnier", "crémaillère", "chaudron", "marmite",
			"poêle", "faitout", "louche", "cuillère", "couteau", "hanap", "coupe", "verre", "pichet", "cruche", "carafe", "bouteille",
			"flacon", "baril", "tonneau", "cuve", "baquet", "seau", "bassine", "jarre", "amphore", "sac", "besace", "gibecière",
			"escarcelle", "bourse", "panier", "corbeille", "malle", "coffret", "écrin", "miroir", "peigne", "brosse", "rasoir", "éponge",
			"savon", "serviette", "nappe", "assiette", "plat", "écuelle", "tranchoir", "mortier", "pilon", "balance", "poids", "mesure",
			"clef", "serrure", "verrou", "gond", "charnière", "clou", "vis", "marteau", "tenailles", "scie", "hache", "rabot", "ciseau",
			"tarière", "enclume", "soufflet de forge", "creuset", "alambic", "cornue", "éprouvette", "grimoire", "parchemin", "plume",
			"encre", "encrier", "sceau", "cire", "cachet", "boussole", "sextant", "astrolabe", "sablier", "cadran solaire", "horloge",
			
			// ==========================================
			// 12. VÊTEMENTS & PARURES
			// ==========================================
			
			"tunique", "chemise", "braies", "chausses", "pourpoint", "cotte", "surcot", "cape", "manteau", "pèlerine", "chaperon", "bonnet",
			"calotte", "toque", "chapeau", "feutre", "voile", "guimpe", "coiffe", "couronne", "diadème", "tiare", "mitre", "chasuble",
			"aube", "étole", "manipule", "dalmatique", "surplis", "soutane", "froc", "coule", "bure", "habit", "livrée", "uniforme",
			"tabard", "jaque", "brigandine", "haubert", "camail", "gambison", "hoqueton", "cuirasse", "plastron", "dossière", "épaulière",
			"brassard", "gantelet", "cuissard", "grève", "soleret", "éperon", "botte", "bottine", "soulier", "sandale", "socque", "patin",
			"ceinture", "ceinturon", "baudrier", "boucle", "agrafe", "bouton", "lacet", "ruban", "galon", "broderie", "dentelle",
			"fourrure", "hermine", "vair", "zibeline", "martre", "renard", "mouton", "agneau", "laine", "soie", "velours", "satin",
			"damas", "brocart", "taffetas", "lin", "chanvre", "coton", "peau", "velin", "anneau", "bague", "alliance", "chevalière",
			"bracelet", "collier", "torque", "pendentif", "médaille", "broche", "fibule", "boucle d'oreille", "sceptre", "globe",
			
			// ==========================================
			// 13. EXCLAMATIONS & EXPRESSIONS DAoC
			// ==========================================
			
			"parbleu", "fichtre", "diantre", "pardi", "guilde", "certes", "nonobstant", "hélas", "mortecouille", "morbleu", "ventrebleu",
			"sacrebleu", "nonpoint", "icelle", "icelui", "que trépas me prenne", "par les dieux", "par ma barbe", "malepeste", "faquin",
			"pendart", "jarnidieu", "ventre saint-gris", "bigre", "hurluberlu", "iconoclasse", "lacrimable", "accointe", "baliverne",
			"oyez", "bienvenue", "adieu", "salutations", "printemps", "été", "automne", "hiver", "givre", "rosée", "brise", "orage", "tempête", 
			"météore", "comète", "éclipse", "foudre", "tonnerre", "zénith", "aurore", "crépuscule", "autan", "borée", 
			"zéphyr", "frimas", "nimbes", "éther", "firmament", "constellation", "astral"
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
