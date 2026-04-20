using System;
using DOL.GS;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
    [CmdAttribute(
        "&vol",
        ePrivLevel.Player,
        "Tentative de vol à la tire sur votre cible actuelle (nécessite le mode Furtif).",
        "/vol")]
    public class StealCommand : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (client == null || client.Player == null)
                return;

            GamePlayer thief = client.Player;

            // Vérifier si le joueur a une cible
            GameObject target = thief.TargetObject;
            if (target == null)
            {
                thief.Out.SendMessage("Vous devez sélectionner une cible pour pouvoir la voler.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // Exécution de la requête de vol via le StealingService
            StealingService.TrySteal(thief, target);
        }
    }
}
