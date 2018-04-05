using UnityEngine;

namespace APlusOrFail.Objects
{
    public class ObjectPrefabLink : MonoBehaviour
    {
        public GameObject prefab;

        private void Start()
        {
            Debug.LogErrorFormat("Prefab link is not designed for initantiate!");
        }
    }
}
