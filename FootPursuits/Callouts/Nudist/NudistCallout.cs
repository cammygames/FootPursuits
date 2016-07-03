using System;
using FootPursuits.Callouts.Base;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using Rage;
using FootPursuits.Util;

namespace FootPursuits.Callouts.Nudist
{
    [CalloutInfo("NudistCallout", CalloutProbability.Always)]
    class NudistCallout : CalloutBase
    {
        const int ChanceOfWeapon = 100;

        public override string CalloutName { get { return "NudistCallout"; } }
        protected Ped nudist;

        protected override void DisplayCallout()
        {
            CalloutMessage = "Public Indecency";

            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_02 CIVILIAN_ASSISTANCE IN_OR_ON_POSITION", CalloutLocation);
        }

        protected override void AcceptedCallout()
        {
            CreateNudist();

            if (NudistHasWeapon())
            {
                GiveNudistWeapon();
            }
        }

        protected override void ProcessCallout()
        {
            throw new NotImplementedException();
        }

        protected override void CleanupCallout()
        {
            if (nudist.Exists()) nudist.Dismiss();
        }

        protected void CreateNudist()
        {
            nudist = new Ped(CalloutLocation);
            nudist.IsPersistent = true;
            nudist.Alertness = 1;
            nudist.BlockPermanentEvents = true;
            nudist.Tasks.Wander();
            Game.LogTrivialDebug(CalloutName + ": Created new Nudist");
        }

        protected bool NudistHasWeapon()
        {
            return Common.GetRandomNumber() < ChanceOfWeapon;
        }

        protected void GiveNudistWeapon()
        {
            WeaponAsset weapon = Common.GetRandomWeapon();
            Game.LogTrivialDebug(CalloutName + ": Gave player weapon: " + weapon);
            nudist.Inventory.GiveNewWeapon(weapon, (short) Common.GetRandomNumber(), true);
        }
    }
}
