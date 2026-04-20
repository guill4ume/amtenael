using System;
using System.Collections.Generic;
using System.Linq;
using DOL.Events;
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts;

public static class GvGManager
{
	public const long CLAIM_COST = 50 * 100 * 100; // 50g
    public static readonly TimeSpan CLAIM_SAFE_DURATION = TimeSpan.FromMinutes(10);
	public const ushort AREA_RADIUS = 5500;
	public const int NEUTRAL_EMBLEM = 256;
    private static readonly TimeSpan _startTime = new TimeSpan(20, 0, 0); //UTC TIME ! 
    private static readonly TimeSpan _endTime = new TimeSpan(21, 59, 59);
    private static readonly HashSet<DayOfWeek> _activeDays = new HashSet<DayOfWeek>
        {
			DayOfWeek.Monday,
			DayOfWeek.Tuesday,
			DayOfWeek.Wednesday,
			DayOfWeek.Thursday,
			DayOfWeek.Friday,
			DayOfWeek.Saturday,
			DayOfWeek.Sunday
        };

    /// <summary>
    /// "Fausses" guildes : Albion, Hibernia, Midgard, Les Maitres du Temps, Citoyens d'Amtenael
    /// </summary>
    public static readonly string[] systemGuildIds =
	{
		"063bbcc7-0005-4667-a9ba-402746c5ae15",
		"bdbc6f4a-b9f8-4316-b88b-9698e06cdd7b",
		"50d7af62-7142-4955-9f31-0c58ac1ac33f",
		"ce6f0b34-78bc-45a9-9f65-6e849d498f6c",
		"386c822f-996b-4db6-8bd8-121c07fc11cd",
	};

	public static readonly List<GuildCaptainGuard> allCaptains = new();

	public static bool ForceOpen = false;

    public static bool IsOpen(GamePlayer player)
    {
        if (ForceOpen)
            return true;

        DayOfWeek today = DateTime.UtcNow.DayOfWeek;
        int currentHour = DateTime.UtcNow.Hour;

        bool isWrongTime = currentHour < _startTime.Hours || currentHour > _endTime.Hours;
        bool isWrongDay = !_activeDays.Contains(today);

        if (player.Client?.Account?.PrivLevel < (uint)ePrivLevel.GM && (isWrongTime || isWrongDay))
        {
            player.Out.SendMessage(
                "Il n'est pas possible de capturer des territoires pour le moment.\n" +
                "Les captures ne sont possibles que les Mardi, Jeudi, et Samedi, entre 21h et minuit.",
                eChatType.CT_System,
                eChatLoc.CL_PopupWindow
            );
            return false;
        }

        return true;
    }

    [GameServerStartedEvent]
	public static void ServerInit(DOLEvent _e, object _sender, EventArgs _args)
	{
		try
		{
			foreach (var captain in allCaptains)
			{
				foreach (var guard in captain.GetNPCsInRadius(AREA_RADIUS).OfType<SimpleGvGGuard>())
					guard.Captain = captain;
			}
		}
		catch (Exception e)
		{
			Console.Error.WriteLine(e);
			throw;
		}
	}
}