using System;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.PlaceObjectSceneState
{
    using Objects;
    using ObjectGrid;

    public class ObjectCursor : KeyCursor
    {
        [NonSerialized] public new Camera camera;
        [NonSerialized] public ObjectPrefabInfo objectPrefab;

        public ObjectPrefabInfo attachedObject { get; private set; }
        private RectInt outerBound;
        private ObjectGridPlacer objectPlacer;

        public event EventHandler<ObjectCursor> onCursorDestroyed;

        protected override void Start()
        {
            base.Start();
            if (objectPrefab != null)
            {
                attachedObject = Instantiate(objectPrefab);
                outerBound = attachedObject.GetComponentsInChildren<ObjectGridRect>().GetLocalRects().GetOuterBound();
                objectPlacer = attachedObject.GetComponent<ObjectGridPlacer>();
                objectPlacer.registerInGrid = false;
            }
        }

        protected override void Update()
        {
            base.Update();
            
            RectInt rotatedOuterBound = outerBound.Rotate(objectPlacer.rotation);
            Vector2Int gridOffset = new Vector2Int(-rotatedOuterBound.xMax, -rotatedOuterBound.yMin + 1);
            Vector2Int objGridPosition = ObjectGrid.instance.WorldToGridPosition(camera.ViewportToWorldPoint(viewportLocation)) + gridOffset;

            objectPlacer.gridPosition = objGridPosition;

            bool placable = objectPlacer.IsRegisterable();
            if (placable)
            {
                Debug.LogFormat($"Player {player.id} can place!");
            }
            else
            {
                Debug.LogFormat($"Player {player.id} cannot place!");
            }

            if (placable && HasKeyUp(player, Player.Action.Select))
            {
                objectPlacer.registerInGrid = true;
                Destroy(gameObject);
            }
            else if (HasKeyUp(player, Player.Action.Cancel))
            {
                objectPlacer.rotation = (ObjectGridRects.Rotation)(((int)objectPlacer.rotation + 1) % 4);
            }
        }

        private bool HasKeyUp(Player player, Player.Action action)
        {
            KeyCode? code = player.GetKeyForAction(action);
            return code != null && Input.GetKeyUp(code.Value);
        }

        private void OnDestroy()
        {
            onCursorDestroyed?.Invoke(this);
        }
    }
}
