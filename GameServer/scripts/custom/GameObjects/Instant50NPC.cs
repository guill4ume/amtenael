using System;
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts.Custom.GameObjects
{
    public class Instant50NPC : GameNPC
    {
        public override bool Interact(GamePlayer player)
        {
            if (!base.Interact(player))
                return false;

            player.Out.SendMessage("Bonjour " + player.Name + ", je suis là pour accélérer votre test sur le serveur.", eChatType.CT_System, eChatLoc.CL_ChatWindow);
            
            if (player.Level < 50)
                player.Out.SendMessage("Souhaitez-vous monter au [Niveau 50] maintenant ?", eChatType.CT_System, eChatLoc.CL_ChatWindow);
            else
                player.Out.SendMessage("Vous avez déjà atteint le summum du niveau !", eChatType.CT_System, eChatLoc.CL_ChatWindow);
                
            player.Out.SendMessage("Voulez-vous devenir un [Maître Artisan] légendaire ?", eChatType.CT_System, eChatLoc.CL_ChatWindow);
            player.Out.SendMessage("Avez-vous besoin d'une [Richesse] initiale pour bien démarrer ?", eChatType.CT_System, eChatLoc.CL_ChatWindow);

            return true;
        }

        public override bool WhisperReceive(GameLiving source, string text)
        {
            GamePlayer player = source as GamePlayer;
            if (player == null) return false;
            if (!base.WhisperReceive(source, text))
                return false;

            string lowerText = text.ToLower();

            if (lowerText == "niveau 50")
            {
                if (player.Level >= 50)
                {
                    SayTo(player, "Vous avez déjà atteint le sommet !");
                    return true;
                }

                long xpNeeded = player.GetExperienceNeededForLevel(49) - player.Experience;
                
                if (xpNeeded > 0)
                {
                    player.GainExperience(eXPSource.Other, xpNeeded);
                    player.UsedLevelCommand = true;
                    player.Out.SendMessage("Félicitations ! Vous avez atteint le niveau 50.", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
                    player.Out.SendMessage("Pensez à rendre visite à votre entraîneur pour apprendre vos nouvelles compétences.", eChatType.CT_System, eChatLoc.CL_ChatWindow);
                    player.SaveIntoDatabase();
                }
                else
                {
                    SayTo(player, "Votre expérience semble déjà suffisante.");
                }
            }
            else if (lowerText == "maître artisan" || lowerText == "maitre artisan")
            {
                // Donne le rang LGM (1100) dans tous les metiers
                for (int i = 1; i <= 15; i++)
                {
                    eCraftingSkill skill = (eCraftingSkill)i;
                    if (skill == eCraftingSkill.NoCrafting) continue;
                    
                    int currentSkill = player.GetCraftingSkillValue(skill);
                    int skillToGain = 1100 - (currentSkill < 0 ? 0 : currentSkill);
                    
                    if (skillToGain > 0)
                    {
                        if (currentSkill == -1)
                        {
                            // Init key in dictionary explicitly if not present to avoid KeyNotFound in GainCraftingSkill
                            player.CraftingSkills[skill] = 0;
                        }
                        player.GainCraftingSkill(skill, skillToGain);
                    }
                }
                player.Out.SendMessage("Vous êtes désormais un Maître Artisan légendaire dans toutes les disciplines !", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
                player.SaveIntoDatabase();
            }
            else if (lowerText == "richesse")
            {
                // 100 Platinum initial
                long targetMoney = 1000000000L; 
                if (player.GetCurrentMoney() < targetMoney)
                {
                    long toGive = targetMoney - player.GetCurrentMoney();
                    player.AddMoney(toGive, "Vous avez reçu {0} des dieux !");
                    player.SaveIntoDatabase();
                }
                else
                {
                    SayTo(player, "Vous êtes déjà immensément riche !");
                }
            }

            return true;
        }
    }
}
