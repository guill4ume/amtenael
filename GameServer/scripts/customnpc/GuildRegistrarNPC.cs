using System;
using System.Linq;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.Database;

namespace DOL.GS.Scripts
{
    [NPCGuildScript("Guild Registrar")]
    public class GuildRegistrarNPC : GameNPC
    {
        private const string FORM_A_GUILD = "créer une guilde";

        public override bool Interact(GamePlayer player)
        {
            if (!base.Interact(player))
                return false;

            if (player.Guild != null)
            {
                SayTo(player, "Vous faites déjà partie d'une guilde !");
                return true;
            }

            SayTo(player, "Salutations, " + player.Name + ". Êtes-vous venu pour [" + FORM_A_GUILD + "] ?");
            return true;
        }

        public override bool WhisperReceive(GameLiving source, string text)
        {
            if (!base.WhisperReceive(source, text))
                return false;

            GamePlayer player = source as GamePlayer;
            if (player == null) return true;

            if (text.ToLower() == FORM_A_GUILD)
            {
                SayTo(player, "Très bien. Quel nom souhaitez-vous donner à votre guilde ? (Chuchotez-moi simplement le nom)");
                return true;
            }

            // If player is not in a guild and whispered something else, assume it's a guild name
            if (player.Guild == null)
            {
                string guildName = text.Trim();
                if (guildName.Length < 3 || guildName.Length > 24)
                {
                    SayTo(player, "Le nom de guilde doit comporter entre 3 et 24 caractères.");
                    return true;
                }

                if (GuildMgr.DoesGuildExist(guildName))
                {
                    SayTo(player, "Ce nom de guilde est déjà utilisé !");
                    return true;
                }

                // Create the guild
                Guild newGuild = GuildMgr.CreateGuild(player.Realm, guildName, player);
                if (newGuild != null)
                {
                    newGuild.AddPlayer(player);
                    SayTo(player, "Félicitations ! La guilde '" + guildName + "' a été créée.");
                    
                    // Attempt to unlock all emblems by temporarily neutralizing realm
                    eRealm originalRealm = player.Realm;
                    try 
                    {
                        // Some clients show all emblems if realm is None
                        player.Realm = eRealm.None; 
                        player.Out.SendEmblemDialogue();
                    }
                    finally
                    {
                        player.Realm = originalRealm;
                    }
                }
                else
                {
                    SayTo(player, "Une erreur est survenue lors de la création de la guilde.");
                }
            }

            return true;
        }
    }
}
