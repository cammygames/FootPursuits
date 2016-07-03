using System;
using Rage;
using Rage.Native;
using System.Reflection;

namespace FootPursuits.Util
{
    public static class Common
    {
        static readonly String[] Weapons = { "WEAPON_BAT", "WEAPON_HAMMER", "WEAPON_KNIFE", "WEAPON_CROWBAR", "WEAPON_PISTOL" };

        public static Random random = new Random();
        public static Ped PlayerPed = Game.LocalPlayer.Character;

        public static string getCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        
        public static Vector3 GetNextPositionOnSidewalk(Vector3 position, bool onGround)
        {
            return NativeFunction.Natives.GET_SAFE_COORD_FOR_PED(position.X, position.Y, position.Z, onGround, out position, 0);
        }

        public static int GetRandomNumber(int Start = 1, int End = 100)
        {
            return random.Next(Start, End);
        }

        public static WeaponAsset GetRandomWeapon()
        {
            int key = GetRandomNumber(0, Weapons.Length);
            return new WeaponAsset(Weapons[key]);
        }
    }
}
