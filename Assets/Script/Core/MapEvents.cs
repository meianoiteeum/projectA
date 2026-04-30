using UnityEngine;

namespace Script.Core
{
    public static class MapEvents
    {
        public static System.Action<Transform> OnPlayerSpawned;
        public static System.Action<Transform, bool> OnCharacterSwitched;
    }
}
