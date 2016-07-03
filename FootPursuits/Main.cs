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
            Game.LogTrivial("Plguin FootPursuits " + Common.getCurrentVersion() + " by Maurice Moss & Sam Collins has been initialised.");
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
            Game.DisplayNotification("~o~Foot Pursuits ~g~V" + Common.getCurrentVersion() + " ~w~by ~b~Maurice Moss & Sam Collins ~w~has been initialised.");
        }
    }
}
