using UnityEngine;

public static  partial class GameEvents
{
    public  static class SewPointEvents
    {
        public static readonly GameEvent<bool> onSelected = new();
        public static readonly GameQuery<bool> onPointSelectedStatus = new();
    }
}

