using System;
using DOL.GS;
using DOL.GS.Commands;
using DOL.GS.PacketHandler;
using DOL.Language;
using DOL.GS.Custom.Echoppe;

namespace DOL.GS.Commands
{
    [CmdAttribute(
        "&echoppe",
        ePrivLevel.Player,
        "Permet de déployer ou ranger son échoppe (marché itinérant)",
        "/echoppe")]
    public class EchoppeCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (IsSpammingCommand(client.Player, "echoppe"))
                return;

            GamePlayer player = client.Player;

            // Vérifications basiques d'état
            if (player.InCombat || player.IsRiding || player.IsOnHorse || player.IsDead || player.IsMezzed || player.IsStunned)
            {
                player.Out.SendMessage("Vous ne pouvez pas ouvrir votre échoppe dans cet état.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // Vérifications de région
            if (player.CurrentRegion.IsDungeon || player.IsInPvP || player.IsInRvR)
            {
                player.Out.SendMessage("Vous ne pouvez pas installer d'échoppe dans cette zone.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // Vérification zone de sécurité
            bool isInSafeArea = false;
            if (player.CurrentAreas != null)
            {
                foreach (var area in player.CurrentAreas)
                {
                    if (area.IsSafeArea)
                    {
                        isInSafeArea = true;
                        break;
                    }
                }
            }

            if (!isInSafeArea)
            {
                player.Out.SendMessage("Vous devez être dans une zone de sécurité (Safe Area) pour ouvrir votre échoppe.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // Toggle On/Off
            var activeEchoppe = player.TempProperties.GetProperty<EchoppeMerchant>("ActiveEchoppe", null);
            if (activeEchoppe != null)
            {
                // Fermeture
                activeEchoppe.Delete();
                player.TempProperties.RemoveProperty("ActiveEchoppe");
                player.Out.SendMessage("Votre échoppe a été refermée. Les objets restent stockés en sécurité.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
            }
            else
            {
                // Ouverture
                EchoppeMerchant echoppe = new EchoppeMerchant();
                echoppe.OwnerID = player.ObjectId;
                echoppe.PlayerOwner = player;
                echoppe.Name = player.Name + "'s Market";
                echoppe.Realm = player.Realm;
                echoppe.Level = 70;
                echoppe.Position = player.Position;
                
                if (echoppe.AddToWorld())
                {
                    player.TempProperties.SetProperty("ActiveEchoppe", echoppe);
                    player.Out.SendMessage("Votre échoppe est désormais ouverte !", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                }
                else
                {
                    player.Out.SendMessage("Une erreur est survenue lors du déploiement de l'échoppe.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                }
            }
        }
    }
}
