using System;
using FootPursuits.Callouts.Base;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;

namespace FootPursuits.Callouts.Mugging
{
    [CalloutInfo("Mugging", CalloutProbability.Always)]
    class MuggingCallout : CalloutBase
    {
        protected override void AcceptedCallout()
        {
            throw new NotImplementedException();
        }

        protected override void DisplayCallout()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessCallout()
        {
            throw new NotImplementedException();
        }

        protected override void StartCallout()
        {
            throw new NotImplementedException();
        }

        public override void OnArrival()
        {
            base.OnArrival();

        }
    }
}
