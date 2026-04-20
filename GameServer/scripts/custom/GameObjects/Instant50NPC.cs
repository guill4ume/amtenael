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

            if (player.Level >= 50)
            {
                SayTo(player, "Vous êtes déjà un héros accompli !");
                return true;
            }

            player.Out.SendMessage("Bonjour " + player.Name + ", je peux vous aider à atteindre votre plein potentiel.", eChatType.CT_System, eChatLoc.CL_ChatWindow);
            player.Out.SendMessage("Souhaitez-vous monter au [Niveau 50] maintenant ?", eChatType.CT_System, eChatLoc.CL_ChatWindow);
            
            return true;
        }

        public override bool WhisperReceive(GameLiving source, string text)
        {
            GamePlayer player = source as GamePlayer;
            if (player == null) return false;
            if (!base.Whisper(player, text))
                return false;

            if (text.ToLower() == "niveau 50")
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

            return true;
        }
    }
}
