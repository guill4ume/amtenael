using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&giveflyingmount",
		ePrivLevel.Admin,
		"Give a test flying mount item (Ancient Dragon)",
		"/giveflyingmount")]
	public class GiveFlyingMountCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			GamePlayer player = client.Player;
			if (player == null) return;

			// Create a template for the flying mount
			DbItemTemplate template = new DbItemTemplate();
			template.Name = "Ancient Dragon Mount";
			template.Id_nb = "dragon_mount_test";
			template.Level = 1;
			template.Item_Type = (int)eInventorySlot.Horse;
			template.Object_Type = (int)eObjectType.Magical;
			template.Model = 612; // Ancient Dragon Model
			template.ClassType = "DOL.GS.FlyingMountItem";
			template.IsDropable = false;
			template.IsPickable = true;
			template.IsStackable = false;
            template.Quality = 100;
            template.Condition = 10000;
            template.MaxCondition = 10000;
            template.Durability = 10000;
            template.MaxDurability = 10000;

			// Instantiate using our new class
			FlyingMountItem item = new FlyingMountItem(template);
			
			if (player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, item))
			{
				client.Out.SendMessage("You received an Ancient Dragon Mount! Equip it in the Horse slot and click it to fly.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			}
			else
			{
				client.Out.SendMessage("Your inventory is full!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
			}
		}
	}
}
