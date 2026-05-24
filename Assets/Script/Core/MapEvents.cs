using UnityEngine;

namespace Script.Core
{
    public enum ArrowState { Player, Node, NoRotation }

    public static class MapEvents
    {
        public static System.Action<Transform> OnPlayerSpawned;
        public static System.Action<Transform, bool> OnCharacterSwitched;
        public static System.Action<Transform, ArrowState> OnArrowStateChanged;
    }
}
