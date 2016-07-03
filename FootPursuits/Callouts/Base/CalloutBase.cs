using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using static System.Drawing.Color;
using static FootPursuits.Util.Common;
using static FootPursuits.Util.Enums;

namespace FootPursuits.Callouts.Base
{
    public abstract class CalloutBase : Callout
    {
        protected new CalloutState State { get; set; }
        protected ResponseType ResponseType { get; set; }
        protected Blip CalloutBlip { get; set; }
        protected Vector3 CalloutLocation { get; set; }

        protected abstract void StartCallout();
        protected abstract void DisplayCallout();
        protected abstract void AcceptedCallout();
        protected abstract void ProcessCallout();

        public CalloutBase()
        {
            State = CalloutState.Created;
            ResponseType = ResponseType.Code2;
        }     

        public virtual void OfficerDown() { }
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

        protected Vector3 GetRandomLocationNearPlayer(float arround, bool sideWalk = true)
        {
            if (sideWalk)
            {
                return GetNextPositionOnSidewalk(World.GetNextPositionOnStreet(PlayerPed.Position.Around(arround)), true);
            }
            else
            {
                return World.GetNextPositionOnStreet(PlayerPed.Position.Around(arround));
            }
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            DisplayCallout();

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            State = CalloutState.Responding;
            AcceptedCallout();

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            State = CalloutState.Cancelled;

            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (PlayerPed.IsDead)
            {
                OfficerDown();
                End();
            }

            if (State == CalloutState.Responding && PlayerPed.DistanceTo(CalloutLocation) < 30f)
            {
                State = CalloutState.OnScene;
                OnArrival();
            }

            ProcessCallout();
        }

        public override void End()
        {
            base.End();

            DeleteBlip();
            State = CalloutState.Complete;
        }
    }
}
