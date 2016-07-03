using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using FootPursuits.Util;

namespace FootPursuits
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnDutyStateChangedEventHandler;
            Game.LogTrivial("Plguin FootPursuits " + Common.getCurrentVersion() + " by Maurice Moss & Sam Collins has been initialised. Checking for updates.");

            GameFiber.StartNew(delegate 
            {
                string latestVersion = UpdateAPI.GetLatestVersion(11111, false);
                if (latestVersion != Common.getCurrentVersion()) Game.DisplayNotification("A new version of FootPursuits is available ~g~V" + latestVersion);

                GameFiber.Hibernate();
            }, "Update Check");
        }
        public override void Finally()
        {
            Game.LogTrivial("Plguin FootPursuits " + Common.getCurrentVersion() + " plugin cleaned up.");
        }

        private static void OnDutyStateChangedEventHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
            }
        }

        private static void RegisterCallouts()
        {
            //Functions.RegisterCallout(typeof(Callouts.RTC));
            Game.DisplayNotification("~o~Foot Pursuits ~g~V" + Common.getCurrentVersion() + " ~w~by ~b~Maurice Moss ~w~& ~p~Sam Collins ~w~has been initialised.");
        }
    }
}
