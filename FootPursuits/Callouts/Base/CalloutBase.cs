using Rage;
using LSPD_First_Response.Mod.Callouts;
using static System.Drawing.Color;
using static FootPursuits.Util.Common;
using static FootPursuits.Util.Enums;

namespace FootPursuits.Callouts.Base
{
    public abstract class CalloutBase : Callout
    {
        public abstract string CalloutName { get; }

        protected new CalloutState State { get; set; }
        protected ResponseType ResponseType { get; set; }
        protected Blip CalloutBlip { get; set; }
        protected Vector3 CalloutLocation { get; set; }
        protected float onSceneDistance { get; set; }

        protected abstract void DisplayCallout();
        protected abstract void AcceptedCallout();
        protected abstract void ProcessCallout();
        protected abstract void OfficerDown();
        protected abstract void CleanupCallout();

        public CalloutBase()
        {
            State = CalloutState.Created;
            ResponseType = ResponseType.Code2;
        }     

        public virtual void OnArrival()
        {
            DeleteBlip();
        }

        public void CreateBlip()
        {
            CalloutBlip = new Blip(CalloutPosition);
            CalloutBlip.Color = Yellow;
            CalloutBlip.EnableRoute(Yellow);
        }

        public void DeleteBlip()
        {
            if (CalloutBlip != null && CalloutBlip.Exists())
            {
                CalloutBlip.DisableRoute();
                CalloutBlip.Delete();
            }
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            Game.LogTrivialDebug(CalloutName + ": Displaying Callout");
            CalloutLocation = GetRandomLocationNearPlayer(15f, 50f);
            CalloutPosition = CalloutLocation;

            AddMinimumDistanceCheck(15f, CalloutLocation);
            ShowCalloutAreaBlipBeforeAccepting(CalloutLocation, 15f);

            DisplayCallout();

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivialDebug(CalloutName + ": Callout Accepted");
            State = CalloutState.Responding;

            CreateBlip();
            AcceptedCallout();

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            Game.LogTrivialDebug(CalloutName + ": Callout Rejected");
            State = CalloutState.Cancelled;

            CleanupCallout();
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (PlayerPed.IsDead)
            {
                OfficerDown();
                EndCallout("Officer Died in callout");
            }

            if (State == CalloutState.Responding && PlayerPed.DistanceTo(CalloutLocation) < onSceneDistance)
            {
                Game.LogTrivialDebug(CalloutName + ": Arrived at callout");
                State = CalloutState.OnScene;
                OnArrival();
            }

            ProcessCallout();
        }

        protected void EndCallout(string reason)
        {
            Game.LogTrivial(CalloutName + ": Has ended becuase " + reason);
            End();
        }

        public override void End()
        {
            Game.LogTrivialDebug(CalloutName + ": Callout ended");
            CleanupCallout();
            DeleteBlip();
            base.End();

            State = CalloutState.Complete;
        }
    }
}
