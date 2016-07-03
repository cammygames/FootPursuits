using System;
using FootPursuits.Callouts.Base;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using Rage;

namespace FootPursuits.Callouts.Mugging
{
    [CalloutInfo("Mugging", CalloutProbability.Always)]
    class MuggingCallout : CalloutBase
    {
        private Ped attacker, victim;

        protected override void DisplayCallout()
        {
            CalloutMessage = "Mugging in progress";

            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 CRIME_POSSIBLE_MUGGING IN_OR_ON_POSITION", CalloutLocation);
        }

        protected override void AcceptedCallout()
        {
            attacker = new Ped(CalloutLocation);
            victim = new Ped(CalloutLocation.Around(1f));

            //give the attacker a weapon.
            attacker.Inventory.GiveNewWeapon("WEAPON_PISTOL", 50, true);
        }

        public override void OnArrival()
        {
            base.OnArrival();

        }

        protected override void ProcessCallout()
        {
            throw new NotImplementedException();
        }

    }
}
