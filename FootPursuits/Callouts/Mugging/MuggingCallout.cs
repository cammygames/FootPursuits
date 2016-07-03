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
            CalloutMessage = "Mugging in progress";

            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 CRIME_POSSIBLE_MUGGING IN_OR_ON_POSITION", CalloutLocation);
        }

        protected override void AcceptedCallout()
        {
            Functions.PlayScannerAudio("UNIT_RESPONDING_DISPATCH_01");

            Attacker = new Ped(CalloutLocation);
            Victim = new Ped(CalloutLocation.Around(1f));

            //give the attacker a weapon and give him a blip.
            Attacker.Inventory.GiveNewWeapon("WEAPON_PISTOL", 50, true);
            AttackerBlip = Attacker.AttachBlip();
            AttackerBlip.IsFriendly = false;

            NativeFunction.CallByName<uint>("TASK_AIM_GUN_AT_ENTITY", Attacker, Victim, -1, true);
            Victim.Tasks.PutHandsUp(-1, Attacker);
            Victim.BlockPermanentEvents = true;
        }

        public override void OnArrival()
        {
            base.OnArrival();

            Attacker.PlayAmbientSpeech("GENERIC_INSULT_MED");
        }

        protected override void ProcessCallout()
        {
            if (!Attacker.Exists()) End();
            if (!Victim.Exists()) End();

            Mugging();
        }

        private void Mugging()
        {
            GameFiber.StartNew(delegate
            {
                Pursuit = Functions.CreatePursuit();
                Functions.SetPursuitDisableAI(Pursuit, true);

                int chance = random.Next(1, 5);

                if (chance == 3)
                {
                    Attacker.Tasks.FireWeaponAt(Victim, 2000, FiringPattern.SingleShot);
                    Victim.Tasks.ReactAndFlee(Attacker);

                    GameFiber.Sleep(2000);

                    Attacker.Tasks.ReactAndFlee(PlayerPed);
                }
                else
                {
                    Victim.Tasks.ReactAndFlee(Attacker);
                    Attacker.Tasks.ReactAndFlee(PlayerPed);
                }

                Victim.Dismiss();

                Functions.AddPedToPursuit(Pursuit, Attacker);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);

                while (Functions.IsPursuitStillRunning(Pursuit))
                {
                    GameFiber.Yield();
                }

                End();
                GameFiber.Hibernate();
            }, CalloutName);
        }

    }
}
