using UnityEngine.InputSystem;

namespace DeadAir
{
    internal static class ReadingNavigation
    {
        public static bool KeysReleased
        {
            get
            {
                var keys = Keyboard.current;
                return keys == null || !(keys.aKey.isPressed || keys.dKey.isPressed);
            }
        }
    }
}
