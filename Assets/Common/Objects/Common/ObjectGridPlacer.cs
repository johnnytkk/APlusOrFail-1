using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace APlusOrFail.Objects
{
    using ObjectGrid;

    public class ObjectGridPlacer : PropertyFieldBehavior
    {
        [SerializeField, HideInInspector]
        private Vector2Int _gridPosition;
        [EditorPropertyField]
        public Vector2Int gridPosition
        {
            get
            {
                return _gridPosition;
            }
            set
            {
                if (_gridPosition != value)
                {
                    _gridPosition = value;
                    if (Application.isPlaying)
                    {
                        if (started && enabled) ApplyProperties();
                    }
                    else ApplyPropertiesEditorMode();
                    
                }
            }
        }

        [SerializeField, HideInInspector]
        private ObjectGridRects.Rotation _rotation;
        [EditorPropertyField]
        public ObjectGridRects.Rotation rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    if (Application.isPlaying)
                    {
                        if (started && enabled) ApplyProperties();
                    }  
                    else ApplyPropertiesEditorMode();
                }
            }
        }

        [SerializeField, HideInInspector]
        private bool _registerInGrid;
        [EditorPropertyField]
        public bool registerInGrid
        {
            get
            {
                return _registerInGrid;
            }
            set
            {
                if (_registerInGrid != value)
                {
                    _registerInGrid = value;
                    if (started && enabled) ApplyProperties();
                }
            }
        }


        public ReadOnlyCollection<ObjectGridRect> objectGridRects { get; private set; }

        public IEnumerable<RectInt> worldGridRects => objectGridRects.GetLocalRects().Rotate(rotation).Move(gridPosition);

        private bool started;
        private bool registeredInGrid;

        private void Start()
        {
            started = true;
            objectGridRects = new ReadOnlyCollection<ObjectGridRect>(GetComponentsInChildren<ObjectGridRect>().ToList());
            ApplyProperties();
        }

        private void OnEnable()
        {
            if (started) ApplyProperties();
        }

        public void ApplyProperties()
        {
            if (started && enabled)
            {
                if (registeredInGrid)
                {
                    ObjectGrid.instance.Remove(gameObject);
                    registeredInGrid = false;
                }
                if (registerInGrid)
                {
                    ObjectGrid.instance.Add(worldGridRects, gameObject);
                    registeredInGrid = true;
                }
                transform.position = ObjectGrid.instance.GridToWorldPosition(gridPosition);
                transform.rotation = Quaternion.Euler(0, 0, 90 * (byte)rotation);
            }
        }
        
        private void ApplyPropertiesEditorMode()
        {
            ObjectGrid objectGrid = FindObjectOfType<ObjectGrid>();
            transform.position = objectGrid.GridToWorldPosition(gridPosition);
            transform.rotation = Quaternion.Euler(0, 0, 90 * (byte)rotation);
        }

        private void OnDestroy()
        {
            if (registeredInGrid)
            {
                ObjectGrid.instance?.Remove(gameObject);
                registeredInGrid = false;
            }
        }

        public bool IsRegisterable()
        {
            return started && ObjectGrid.instance.IsPlaceable(worldGridRects);
        }
    }
}
