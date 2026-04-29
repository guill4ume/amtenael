using System;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.Database;

namespace DOL.GS.Scripts
{
    public class HerautDesDieux : GameNPC
    {
        public override bool AddToWorld()
        {
            Name = "Héraut des dieux";
            GuildName = "Services Alpha";
            Model = 1198; // Modèle de Héraut standard
            Size = 70;
            Level = 50;
            Flags |= eFlags.PEACE;
            return base.AddToWorld();
        }

        public override bool Interact(GamePlayer player)
        {
            if (!base.Interact(player)) return false;

            player.Out.SendMessage("Salutations " + player.Name + ". Je suis ici pour vous aider durant cette phase de test.\n\n" +
                "Dites-moi si vous souhaitez :\n" +
                "- Monter au [Niveau 50]\n" +
                "- Obtenir la [Richesse] (100 platines)\n" +
                "- Maîtriser l' [artisanat] (1100 partout)\n" +
                "- Passer au [Rang de Royaume 13L0]", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
            return true;
        }

        public override bool WhisperReceive(GameLiving source, string str)
        {
            if (!base.WhisperReceive(source, str)) return false;
            GamePlayer player = source as GamePlayer;
            if (player == null) return false;

            switch (str.ToLower())
            {
                case "niveau 50":
                    player.Level = 50;
                    player.Out.SendMessage("Vous êtes maintenant niveau 50 !", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                    break;

                case "richesse":
                    player.AddMoney(100000000); // 100 Platines (1 plat = 1,000,000)
                    player.Out.SendMessage("Vous avez reçu 100 pièces de platine !", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                    break;

                case "artisanat":
                    SetSkill(player, eCraftingSkill.ArmorCrafting, 1100);
                    SetSkill(player, eCraftingSkill.WeaponCrafting, 1100);
                    SetSkill(player, eCraftingSkill.Fletching, 1100);
                    SetSkill(player, eCraftingSkill.Alchemy, 1100);
                    SetSkill(player, eCraftingSkill.SpellCrafting, 1100);
                    SetSkill(player, eCraftingSkill.Tailoring, 1100);
                    player.Out.SendMessage("Vos compétences d'artisanat sont maintenant à 1100 !", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                    break;

                case "rang de royaume 13l0":
                    player.GainRealmPoints(24044484 - player.RealmPoints);
                    player.Out.SendMessage("Vous êtes maintenant Rang de Royaume 13L0 !", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
                    break;

                default:
                    return false;
            }
            return true;
        }

        private void SetSkill(GamePlayer player, eCraftingSkill skill, int value)
        {
            int current = player.GetCraftingSkillValue(skill);
            if (current < value)
            {
                player.GainCraftingSkill(skill, value - current);
            }
        }
    }
}
