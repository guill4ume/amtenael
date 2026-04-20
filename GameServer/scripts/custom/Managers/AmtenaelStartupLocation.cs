using System;
using DOL.Events;
using DOL.Database;
using DOL.GS;

namespace OpenDAoC_SPB.Custom
{
	/// <summary>
	/// Overrides the startup location of every new character to point to the historical Avalon Isle (Region 51)
	/// </summary>
	public static class AmtenaelStartupLocation
	{
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			GameEventMgr.AddHandler(DatabaseEvent.CharacterCreated, CharacterCreation);
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			GameEventMgr.RemoveHandler(DatabaseEvent.CharacterCreated, CharacterCreation);
		}

		private static void CharacterCreation(DOLEvent e, object sender, EventArgs arguments)
		{
			if (arguments is not CharacterEventArgs charArgs)
				return;
			
			DbCoreCharacter ch = charArgs.Character;
			
			if (ch == null)
				return;

			// Amtenaël: Force spawn on Avalon Isle (Map 51)
			ch.Region = 51;
			ch.Xpos = 525472;
			ch.Ypos = 544419;
			ch.Zpos = 3120;
			ch.Direction = 2067;

			// Force Bind at the same location
			ch.BindRegion = 51;
			ch.BindXpos = 525472;
			ch.BindYpos = 544419;
			ch.BindZpos = 3120;
			ch.BindHeading = 2067;
			
			// Amtenaël: No default guild assigned (player must join manually)
			ch.GuildID = string.Empty;

			Console.WriteLine("AmtenaelStartupLocation: Forced spawn for {0} on Map 51 (Avalon)", ch.Name);
		}
	}
}
