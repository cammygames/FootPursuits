using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using static System.Drawing.Color;
using static FootPursuits.Util.Common;
using static FootPursuits.Util.Enums;

namespace FootPursuits.Callouts.Base
{
    public class CalloutBase : Callout
    {
        public new CalloutState State { get; set; }
        public ResponseType ResponseType { get; set; }
        public Blip CalloutBlip { get; set; }
        public Vector3 CalloutLocation { get; set; }

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

        protected Vector3 GetRandomLocationNearPlayer(float arround)
        {
            return GetNextPositionOnSidewalk(World.GetNextPositionOnStreet(PlayerPed.Position.Around(arround)), true);
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            State = CalloutState.Responding;

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
        }

        public override void End()
        {
            base.End();
        }
    }
}
