using UnityEngine;

namespace APlusOrFail.Objects
{
    using ObjectGrid;

    [ExecuteInEditMode]
    [RequireComponent(typeof(ObjectGridSize))]
    public class ObjectGridPlacer : MonoBehaviour
    {
#if UNITY_EDITOR
        public int gridX;
        public int gridY;

        private void OnValidate()
        {
            if (ObjectGrid.instance != null)
            {
                ObjectGridSize objectGridSize = GetComponent<ObjectGridSize>();
                if (objectGridSize != null)
                {
                    transform.position = ObjectGrid.instance.GridRectToRect(
                        new RectInt(gridX, gridY, objectGridSize.width, objectGridSize.height)).center;
                }
            }
        }
    }
#endif
}
