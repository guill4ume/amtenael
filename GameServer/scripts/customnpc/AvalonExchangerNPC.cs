using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DOL.GS;
using DOL.Database;
using DOL.GS.PacketHandler;
using log4net;
using System.Reflection;

namespace DOL.GS.Scripts
{
    public class AvalonExchangerNPC : GameNPC
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class ExchangeRecipe
        {
            public string ItemID;
            public string ItemName;
            public int Quantity;
            public long RewardXP;
            public long RewardMoney;
        }

        private static Dictionary<string, List<ExchangeRecipe>> _recipesByName = new Dictionary<string, List<ExchangeRecipe>>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, List<ExchangeRecipe>> _recipesByGuid = new Dictionary<string, List<ExchangeRecipe>>(StringComparer.OrdinalIgnoreCase);
        private static bool _loaded = false;

        public AvalonExchangerNPC() : base()
        {
            LoadRecipes();
        }

        public static void LoadRecipes()
        {
            if (_loaded) return;
            _loaded = true;

            string filePath = @"C:\OpenDAOC_server\ProjetsAnnexes\DossierPortage\Archives\LootsAvalon\echangeur_final.txt";
            if (!File.Exists(filePath))
            {
                log.Error("AvalonExchangerNPC: Recipes file not found at " + filePath);
                return;
            }

            try
            {
                // File is UTF-16LE
                string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.Unicode);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split('|');
                    if (parts.Length < 7) continue;

                    string npcName = parts[0];
                    string npcGuid = parts[1];
                    string itemName = parts[2];
                    string itemId = parts[3];
                    int qty = int.Parse(parts[4]);
                    long xp = long.Parse(parts[5]);
                    long money = long.Parse(parts[6]);

                    ExchangeRecipe recipe = new ExchangeRecipe
                    {
                        ItemID = itemId,
                        ItemName = itemName,
                        Quantity = qty,
                        RewardXP = xp,
                        RewardMoney = money
                    };

                    if (!_recipesByName.ContainsKey(npcName)) _recipesByName[npcName] = new List<ExchangeRecipe>();
                    _recipesByName[npcName].Add(recipe);

                    if (!_recipesByGuid.ContainsKey(npcGuid)) _recipesByGuid[npcGuid] = new List<ExchangeRecipe>();
                    _recipesByGuid[npcGuid].Add(recipe);
                }
                log.Info("AvalonExchangerNPC: Loaded " + lines.Length + " exchange recipes.");
            }
            catch (Exception ex)
            {
                log.Error("AvalonExchangerNPC: Error loading recipes", ex);
            }
        }

        public override bool Interact(GamePlayer player)
        {
            if (!base.Interact(player)) return false;

            List<ExchangeRecipe> recipes = GetMyRecipes();
            if (recipes == null || recipes.Count == 0)
            {
                SayTo(player, "Je n'ai rien à échanger pour le moment.");
                return true;
            }

            SayTo(player, "Salutations. Je recherche les objets suivants :");
            foreach (var recipe in recipes.Take(10)) // Show first 10
            {
                player.Out.SendMessage("- " + recipe.Quantity + "x [" + recipe.ItemName + "]", eChatType.CT_System, eChatLoc.CL_ChatWindow);
            }
            if (recipes.Count > 10) player.Out.SendMessage("... et d'autres encore.", eChatType.CT_System, eChatLoc.CL_ChatWindow);

            SayTo(player, "Donnez-moi simplement ces objets pour recevoir votre récompense.");
            return true;
        }

        public override bool ReceiveItem(GameLiving source, DbInventoryItem item)
        {
            GamePlayer player = source as GamePlayer;
            if (player == null || item == null) return false;

            List<ExchangeRecipe> recipes = GetMyRecipes();
            if (recipes == null) return false;

            ExchangeRecipe match = recipes.FirstOrDefault(r => r.ItemID.Equals(item.Id_nb, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                // Check quantity
                // In DAoC, ReceiveItem is called for one item (or a stack if dragged as stack)
                // If it's a stack, item.Count is > 1.
                
                int count = item.Count;
                if (count < match.Quantity)
                {
                    SayTo(player, "Il m'en faut au moins " + match.Quantity + " pour procéder à l'échange.");
                    return false;
                }

                // Proceed with exchange
                int exchanges = count / match.Quantity;
                long totalXP = match.RewardXP * exchanges;
                long totalMoney = match.RewardMoney * exchanges;

                if (player.Inventory.RemoveItem(item))
                {
                    if (totalXP > 0) player.GainExperience(new global::DOL.Events.GainedExperienceEventArgs(totalXP, 0, 0, 0, 0, 0, true, false, eXPSource.Other));
                    if (totalMoney > 0) player.AddMoney(totalMoney);

                    SayTo(player, "Merci ! Voici votre récompense.");
                    return true;
                }
            }

            SayTo(player, "Cet objet ne m'intéresse pas.");
            return false;
        }

        private List<ExchangeRecipe> GetMyRecipes()
        {
            if (_recipesByGuid.TryGetValue(this.InternalID, out List<ExchangeRecipe> recipesByGuid))
                return recipesByGuid;

            if (_recipesByName.TryGetValue(this.Name, out List<ExchangeRecipe> recipesByName))
                return recipesByName;

            return null;
        }
    }
}
