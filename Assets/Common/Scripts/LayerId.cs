using UnityEngine;

namespace APlusOrFail
{
    public static class LayerId
    {
        public static readonly int Characters = LayerMask.NameToLayer("Characters");
        public static readonly int SelectableObjects = LayerMask.NameToLayer("Selectable Objects");
    }
}
