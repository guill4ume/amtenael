using System;
using System.Reflection;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Scheduler;
using log4net;

namespace OpenDAoC_SPB.Custom
{
	public static class PvPBonusManager
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private const int START_HOUR = 21;
		private const int END_HOUR = 23;
		public const double BONUS_MULTIPLIER = 1.25;

		private static ScheduledTask _timerTask;
		private static bool _isBonusActive = false;

		public static bool IsBonusActive => _isBonusActive;

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			if (GameServer.Instance.Scheduler != null)
			{
				// Tick every 1 minute
				_timerTask = GameServer.Instance.Scheduler.Start(CheckBonusTick, 60 * 1000);
				log.Info("PvPBonusManager timer started.");
				
				// Initial check without announcement to avoid spam on reload
				CheckBonus(false);
			}
			else
			{
				log.Error("PvPBonusManager: GameServer.Instance.Scheduler is null!");
			}
		}

		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
		{
			if (_timerTask != null)
			{
				_timerTask.Stop();
				_timerTask = null;
			}
		}

		private static int CheckBonusTick()
		{
			CheckBonus(true);
			return 60 * 1000;
		}

		private static void CheckBonus(bool announce)
		{
			int hour = DateTime.Now.Hour;
			bool shouldBeActive = (hour >= START_HOUR && hour < END_HOUR);

			if (shouldBeActive && !_isBonusActive)
			{
				_isBonusActive = true;
				if (announce)
				{
					BroadcastMessage("Message automatique : début du bonus realm points");
				}
				log.Info("PvP Bonus Started (21h-23h).");
			}
			else if (!shouldBeActive && _isBonusActive)
			{
				_isBonusActive = false;
				if (announce)
				{
					BroadcastMessage("Message automatique : fin du bonus realm points");
				}
				log.Info("PvP Bonus Ended.");
			}
		}

		private static void BroadcastMessage(string message)
		{
			foreach (GameClient client in ClientService.GetClients())
			{
				if (client != null && client.Player != null && client.Player.ObjectState == GameObject.eObjectState.Active)
				{
					client.Player.Out.SendMessage(message, eChatType.CT_Important, eChatLoc.CL_SystemWindow);
				}
			}
		}
	}
}
