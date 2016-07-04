using Rage;
using Rage.Native;
using FootPursuits.Util;
using FootPursuits.Callouts.Base;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace FootPursuits.Callouts.Nudist
{
    class NudistPursuit
    {
        protected LHandle Pursuit = Functions.CreatePursuit();
        protected Ped Nudist;
        protected NudistCallout Callout;

        public NudistPursuit(NudistCallout callout)
        {
            Nudist = callout.nudist;
            Callout = callout;
        }

        public void attack()
        {
            
        }

        public void run()
        {
            Game.LogTrivialDebug("Started New Thread");
            GameFiber.StartNew(delegate
            {
                Game.LogTrivialDebug("Ped is Running");
                Nudist.Tasks.ReactAndFlee(Common.PlayerPed);

                Nudist.Dismiss();
                Functions.AddPedToPursuit(Pursuit, Nudist);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);

                while (Functions.IsPursuitStillRunning(Pursuit))
                {
                    GameFiber.Yield();
                }

                Callout.End();

                GameFiber.Hibernate();
            }, Callout.CalloutName + " Pursuit");
        }
    }
}
