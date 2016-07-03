using System;
using Rage;
using Rage.Native;

namespace FootPursuits.Util
{
    public static class Common
    {
        public static Random random = new Random();
        public static Ped PlayerPed = Game.LocalPlayer.Character;

        public static string getCurrentVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        
        public static Vector3 GetNextPositionOnSidewalk(Vector3 position, bool onGround)
        {
            return NativeFunction.Natives.GET_SAFE_COORD_FOR_PED(position.X, position.Y, position.Z, onGround, out position, 0);
        }
    }
}
