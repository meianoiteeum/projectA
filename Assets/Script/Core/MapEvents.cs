using UnityEngine;

namespace Script.Core
{
    public enum ArrowState { Player, Node, NoRotation }

    public enum GameState
    {
        MOVIMENTACAO,
        SWITCH,
        ROTACAO,
        MOVIMENTACAO_NODE,
        MORTE,
        ERRO
    }

    public static class MapEvents
    {
        public static System.Action<Transform> OnPlayerSpawned;
        public static System.Action<Transform, bool> OnCharacterSwitched;
        public static System.Action<Transform, ArrowState> OnArrowStateChanged;
        public static System.Action<GameState> OnGameStateChanged;
    }
}
