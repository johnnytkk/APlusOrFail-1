using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Objects
{
    using ObjectGrid;

    public class ObjectGridPlacer : MonoBehaviour
    {
        [SerializeField] private Vector2Int _gridPosition;
        public Vector2Int gridPosition { get { return _gridPosition; } set { SetProperty(ref _gridPosition, value); } }

        [SerializeField] private ObjectGridRects.Rotation _rotation;
        public ObjectGridRects.Rotation rotation { get { return _rotation; } set { SetProperty(ref _rotation, value); } }

        [SerializeField] private bool _registerInGrid;
        public bool registerInGrid { get { return _registerInGrid; } set { SetProperty(ref _registerInGrid, value); } }


        private bool started;
        private bool registeredInGrid;
        private readonly List<ObjectGridRect> objectGridRects = new List<ObjectGridRect>();
        private IEnumerable<RectInt> worldGridRects => objectGridRects.GetLocalRects().Rotate(rotation).Move(gridPosition);


        private void Start()
        {
            started = true;
            UpdateProperties();
        }

        private void OnEnable()
        {
            UpdateProperties();
        }

        private void OnValidate()
        {
            UpdateProperties();
        }

        private void OnDestroy()
        {
            if (registeredInGrid)
            {
                ObjectGrid.instance?.Remove(gameObject);
                registeredInGrid = false;
            }
        }

        private void SetProperty<T>(ref T property, T value)
        {
            if (!property.Equals(value))
            {
                property = value;
                UpdateProperties();
            }
        }

        public void UpdateProperties()
        {
            if (Application.isPlaying)
            {
                if (started && enabled)
                {
                    if (registeredInGrid)
                    {
                        ObjectGrid.instance.Remove(gameObject);
                        registeredInGrid = false;
                    }

                    GetComponentsInChildren(true, objectGridRects);
                    if (registerInGrid)
                    {
                        ObjectGrid.instance.Add(worldGridRects, gameObject);
                        registeredInGrid = true;
                    }
                    transform.position = ObjectGrid.instance.GridToWorldPosition(gridPosition);
                    transform.rotation = Quaternion.Euler(0, 0, 90 * (byte)rotation);
                }
            }
            else
            {
                ObjectGrid objectGrid = FindObjectOfType<ObjectGrid>();
                if (objectGrid != null)
                {
                    GetComponentsInChildren(true, objectGridRects);
                    transform.position = objectGrid.GridToWorldPosition(gridPosition);
                    transform.rotation = Quaternion.Euler(0, 0, 90 * (byte)rotation);
                }
            }
        }

        public bool IsRegisterable() => started && enabled && ObjectGrid.instance?.IsPlaceable(worldGridRects) == true;
    }
}
