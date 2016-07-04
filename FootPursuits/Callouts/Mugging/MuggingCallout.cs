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
        private Blip AttackerBlip;
        private LHandle Pursuit;

        protected override void DisplayCallout()
        {
            Attacker = new Ped(CalloutLocation);
            if (!Attacker.Exists()) End();

            Victim = new Ped(CalloutLocation.Around(3f));
            if (!Victim.Exists()) End();

            CalloutMessage = "Mugging in progress";
            minimumDistance = 15f;

            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 CRIME_RESIST_ARREST_01 IN_OR_ON_POSITION", CalloutLocation);
        }

        protected override void AcceptedCallout()
        {
            Functions.PlayScannerAudio("UNIT_RESPONDING_DISPATCH_01");

            //give the attacker a weapon and give him a blip.
            Attacker.Inventory.GiveNewWeapon("WEAPON_PISTOL", 50, true);
            AttackerBlip = Attacker.AttachBlip();

            if (!AttackerBlip.Exists()) End();
            AttackerBlip.IsFriendly = false;
            AttackerBlip.Scale = 0.5f;

            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", Attacker, Victim, -1, true);
            Victim.Tasks.PutHandsUp(-1, Attacker);
            Victim.BlockPermanentEvents = true;

            onSceneDistance = 3f;
        }

        public override void OnArrival()
        {
            base.OnArrival();

            if (!Attacker.Exists()) End();
            if (!Victim.Exists()) End();

            Attacker.PlayAmbientSpeech("GENERIC_INSULT_MED");
            Mugging();
        }

        protected override void ProcessCallout()
        {
            if (!Attacker.Exists()) End();
            if (!Victim.Exists()) End();
        }

        private void Mugging()
        {
            GameFiber.StartNew(delegate
            {
                Pursuit = Functions.CreatePursuit();
                Functions.SetPursuitDisableAI(Pursuit, true);

                int chance = random.Next(1, 5);

                if (chance == 3 && Attacker.Exists() && Victim.Exists())
                {
                    Victim.Tasks.ReactAndFlee(Attacker); // run away from attacker
                    Attacker.Tasks.FireWeaponAt(Victim, 2000, FiringPattern.SingleShot);

                    GameFiber.Sleep(2000);

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
                        End();
                    }

                }

                if (Victim.Exists()) Victim.Dismiss();

                if (Attacker.Exists())
                {
                    Functions.AddPedToPursuit(Pursuit, Attacker);
                    Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                }
                else
                {
                    End();
                }


                while (Functions.IsPursuitStillRunning(Pursuit))
                {
                    GameFiber.Yield();
                }

                End();
                GameFiber.Hibernate();
            }, CalloutName);
        }

        protected override void CleanupCallout()
        {
            if (Pursuit != null && Functions.IsPursuitStillRunning(Pursuit)) Functions.ForceEndPursuit(Pursuit);
            if (Attacker.Exists()) Attacker.Dismiss();
            if (AttackerBlip.Exists()) AttackerBlip.Delete();
            if (Victim.Exists()) Victim.Dismiss();
        }
    }
}
