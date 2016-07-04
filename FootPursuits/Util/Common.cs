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
        
        public static Vector3 GetNextPositionOnSidewalk(Vector3 position, bool onGround = true)
        {
            Vector3 location;
            NativeFunction.Natives.GET_SAFE_COORD_FOR_PED(position.X, position.Y, position.Z, onGround, out location, 16);
            return location;
        }

        public static Vector3 GetRandomLocationNearPlayer(float rangeMin, float rangeMax, bool sideWalk = true)
        {
            if (sideWalk)
            {
                Vector3 location = GetNextPositionOnSidewalk(World.GetNextPositionOnStreet(PlayerPed.Position.Around(rangeMin, rangeMax)));
                if (location == Vector3.Zero) return World.GetNextPositionOnStreet(PlayerPed.Position.Around(rangeMin, rangeMax));
                return location;
            }

            return World.GetNextPositionOnStreet(PlayerPed.Position.Around(rangeMin, rangeMax));
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
