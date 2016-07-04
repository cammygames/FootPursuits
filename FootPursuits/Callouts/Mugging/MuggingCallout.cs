using System;
using FootPursuits.Callouts.Base;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using static FootPursuits.Util.Common;

namespace FootPursuits.Callouts.Mugging
{
    [CalloutInfo("Mugging", CalloutProbability.Always)]
    class MuggingCallout : CalloutBase
    {
        public override string CalloutName { get { return "Mugging"; } }

        private Ped Attacker, Victim;
        private LHandle Pursuit;

        protected override void DisplayCallout()
        {
            GameFiber.StartNew(delegate
            {
                CreateAttacker(CalloutLocation);
                CreateVictim(CalloutLocation.Around(1f));
                GameFiber.Hibernate();
            }, CalloutName + " Creation Thread");

            CalloutMessage = "Mugging in progress";

            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 CRIME_RESIST_ARREST_01 IN_OR_ON_POSITION", CalloutLocation);
        }

        protected override void AcceptedCallout()
        {
            Functions.PlayScannerAudio("UNIT_RESPONDING_DISPATCH_01");

            if (!Attacker.Exists()) EndCallout("Attacker does not exist!");
            if (!Victim.Exists()) EndCallout("Victim does not exist!");

            Attacker.Tasks.AimWeaponAt(Victim, -1);
            Victim.Tasks.PutHandsUp(-1, Attacker);

            OnSceneDistance = 20f;
        }

        public override void OnArrival()
        {
            base.OnArrival();

            if (!Attacker.Exists()) EndCallout("Attacker does not exist!");
            if (!Victim.Exists()) EndCallout("Victim does not exist!");

            Mugging();
        }

        protected override void ProcessCallout()
        {
            if (!Attacker.Exists()) EndCallout("Attacker does not exist!");
            if (!Victim.Exists()) EndCallout("Victim does not exist!");
        }

        private void CreateAttacker(Vector3 location)
        {
            Attacker = new Ped(location);
            if (!Attacker.Exists()) EndCallout("Attacker did not spawn.");

            Attacker.IsPersistent = true;
            Attacker.Alertness = 1;
            Attacker.BlockPermanentEvents = true;
            Attacker.Inventory.GiveNewWeapon("WEAPON_PISTOL", 50, true);

            Game.LogTrivial(CalloutName + "Attacker Created.");
        }

        private void CreateVictim(Vector3 location)
        {
            Victim = new Ped(location);
            if (!Victim.Exists()) EndCallout("Victim did not spawn.");

            Attacker.IsPersistent = true;
            Attacker.BlockPermanentEvents = true;
            Game.LogTrivial(CalloutName + "Victim Created.");
        }

        private void Mugging()
        {
            GameFiber.StartNew(delegate
            {
                Attacker.PlayAmbientSpeech("Shout_Threaten_Ped");
                GameFiber.Sleep(500);
                Victim.PlayAmbientSpeech("Generic_Frightened_Med");

                Pursuit = Functions.CreatePursuit();
                Functions.SetPursuitDisableAI(Pursuit, true);

                int chance = random.Next(1, 5);

                if (chance == 3 && Attacker.Exists() && Victim.Exists())
                {
                    Victim.Tasks.ReactAndFlee(Attacker); // run away from attacker
                    GameFiber.Sleep(500);

                    Attacker.Tasks.FireWeaponAt(Victim, 2000, FiringPattern.BurstFirePistol);

                    GameFiber.Sleep(500);

                    Attacker.Tasks.ReactAndFlee(PlayerPed); // run away from player.
                }
                else
                {
                    if (Attacker.Exists() && Victim.Exists())
                    {
                        Victim.Tasks.ReactAndFlee(Attacker);
                        Attacker.Tasks.ReactAndFlee(PlayerPed);
                    }
                    else
                    {
                        EndCallout("Either Victim or Attacker didnt exist!");
                    }

                }

                if (Victim.Exists()) Victim.Tasks.ReactAndFlee(Attacker);

                if (Attacker.Exists())
                {
                    Functions.AddPedToPursuit(Pursuit, Attacker);
                    Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                }
                else
                {
                    EndCallout("Attacker didnt exist!");
                }


                while (Functions.IsPursuitStillRunning(Pursuit))
                {
                    GameFiber.Yield();
                }

                EndCallout("Callout finished, shutting down thread");
                GameFiber.Hibernate();
            }, CalloutName);
        }

        protected override void CleanupCallout()
        {
            if (Pursuit != null && Functions.IsPursuitStillRunning(Pursuit)) Functions.ForceEndPursuit(Pursuit);
            if (Attacker.Exists()) Attacker.Dismiss();
            if (Victim.Exists()) Victim.Dismiss();
        }

        protected override void OfficerDown() {}
    }
}
