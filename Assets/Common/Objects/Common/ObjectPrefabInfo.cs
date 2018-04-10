using UnityEngine;

namespace APlusOrFail.Objects
{
    public class ObjectPrefabInfo : MonoBehaviour
    {
        public ObjectPrefabLink prefabLink;

        public GameObject prefab => prefabLink?.prefab;
    }
}
