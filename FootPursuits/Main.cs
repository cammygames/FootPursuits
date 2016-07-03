using System;
using Rage;
using LSPD_First_Response.Mod.API;
using FootPursuits.Util;
using FootPursuits.Util.Intergration;
using System.Reflection;

namespace FootPursuits
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnDutyStateChangedEventHandler;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFRResolveEventHandler);

            Game.LogTrivial("Plugin Foot Pursuits " + Common.getCurrentVersion() + " by Maurice Moss & Sam Collins has been initialised. Checking for updates & checking intergration.");

            GameFiber.StartNew(delegate 
            {
                //string latestVersion = UpdateAPI.GetLatestVersion(11111, false);
                //if (latestVersion != Common.getCurrentVersion()) Game.DisplayNotification("A new version of Foot Pursuits is available ~g~V" + latestVersion);

                GameFiber.Sleep(100); //lets give LSPDFR a chance to load the other plugins

                if (Intergration.IsLSPDFRPluginRunning("LSPDFR+"))
                {
                    Game.DisplaySubtitle("You have LSPDFR+");
                    Game.LogTrivial("Intergration with LSPDFR started");
                    Has.LSPDFRPlus = true;
                }

                GameFiber.Hibernate();
            }, "Update Check");
        }
        public override void Finally()
        {
            Game.LogTrivial("Plugin Foot Pursuits " + Common.getCurrentVersion() + " plugin cleaned up.");
        }

        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                if (args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()))
                {
                    return assembly;
                }
            }
            return null;
        }

        private static void OnDutyStateChangedEventHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
                Game.DisplayNotification("~o~Foot Pursuits ~g~V" + Common.getCurrentVersion() + " ~w~by ~b~Maurice Moss ~w~& ~p~Sam Collins ~w~has been initialised.");
            }
        }

        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(Callouts.Nudist.NudistCallout));
            Functions.RegisterCallout(typeof(Callouts.Mugging.MuggingCallout));
        }
    }
}
