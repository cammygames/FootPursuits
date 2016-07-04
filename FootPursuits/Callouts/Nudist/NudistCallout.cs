using Rage;
using FootPursuits.Util;
using FootPursuits.Callouts.Base;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System;

namespace FootPursuits.Callouts.Nudist
{
    [CalloutInfo("NudistCallout", CalloutProbability.Always)]
    class NudistCallout : CalloutBase
    {
        const int ChanceOfWeapon = 100;
        const int ChanceOfFlee = 100;

        public override string CalloutName { get { return "NudistCallout"; } }
        public Ped nudist { get; set; }

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
            if (NudistFlees())
            {
                NudistPursuit Pursuit = new NudistPursuit(this);
                Pursuit.run();
            }
        }

        protected override void CleanupCallout()
        {
            if (nudist.Exists()) nudist.Dismiss();
        }

        protected void CreateNudist()
        {
            nudist = new Ped (CalloutLocation);

            if (!nudist.Exists())
            {
                Game.LogTrivial(CalloutName + ": Ped did not spawn");
                End();
            }

            nudist.IsPersistent = true;
            nudist.Alertness = 1;
            nudist.BlockPermanentEvents = true;
            nudist.Tasks.Wander();
            Game.LogTrivialDebug(CalloutName + ": Created new Nudist");
        }

        protected bool NudistFlees()
        {
            return Common.GetRandomNumber() < ChanceOfFlee;
        }

        protected bool NudistHasWeapon()
        {
            return Common.GetRandomNumber() < ChanceOfWeapon;
        }

        protected void GiveNudistWeapon()
        {
            WeaponAsset weapon = Common.GetRandomWeapon();
            nudist.Inventory.GiveNewWeapon(weapon, (short) Common.GetRandomNumber(), true);
            Game.LogTrivialDebug(CalloutName + ": Gave player weapon");
        }

        protected override void OfficerDown()
        {
            throw new NotImplementedException();
        }
    }
}
