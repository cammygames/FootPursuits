using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using static System.Drawing.Color;
using static FootPursuits.Util.Common;
using static FootPursuits.Util.Enums;
using System.Collections.Generic;
using Rage.Native;

namespace FootPursuits.Callouts.Base
{
    public abstract class CalloutBase : Callout
    {
        public abstract string CalloutName { get; }

        protected new CalloutState State { get; set; }
        protected ResponseType ResponseType { get; set; }
        protected Blip CalloutBlip { get; set; }
        protected Vector3 CalloutLocation { get; set; }

        protected abstract void DisplayCallout();
        protected abstract void AcceptedCallout();
        protected abstract void ProcessCallout();
        protected abstract void CleanupCallout();

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
            if (CalloutBlip.Exists())
            {
                CalloutBlip.DisableRoute();
                CalloutBlip.Delete();
            }
        }

        protected Vector3 GetRandomLocationNearPlayer(float rangeMin, float rangeMax, bool sideWalk = true)
        {
            if (sideWalk)
            {
                return GetNextPositionOnSidewalk(World.GetNextPositionOnStreet(PlayerPed.Position.Around(rangeMin, rangeMax)), true);
            }
            else
            {
                return World.GetNextPositionOnStreet(PlayerPed.Position.Around(rangeMin, rangeMax));
            }
        }

        protected Ped RecruitNearPed(Vector3 location, float arround, int type = -1)
        {
            Ped ped;
            bool function = NativeFunction.Natives.GET_CLOSEST_PED(location.X, location.Y, location.Z, arround, false, false, out ped, false, false, type);
            return ped;
        }

        public override bool OnBeforeCalloutDisplayed()
        {
            Game.LogTrivialDebug(CalloutName + ": Displaying Callout");
            CalloutLocation = GetRandomLocationNearPlayer(10f, 30f);
            CalloutPosition = CalloutLocation;

            ShowCalloutAreaBlipBeforeAccepting(CalloutLocation, 15f);
            AddMinimumDistanceCheck(5f, CalloutLocation);

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
                Game.LogTrivialDebug(CalloutName + ": Officer Died in callout");
                OfficerDown();
                End();
            }

            if (State == CalloutState.Responding && PlayerPed.DistanceTo(CalloutLocation) < 30f)
            {
                Game.LogTrivialDebug(CalloutName + ": Arrived at callout");
                State = CalloutState.OnScene;
                OnArrival();
            }

            ProcessCallout();
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
