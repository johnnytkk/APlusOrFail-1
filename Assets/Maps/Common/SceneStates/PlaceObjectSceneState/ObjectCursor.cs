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

        private ObjectPrefabInfo attachedObject;
        private ICustomizableObject customizableObject;
        private ObjectGridPlacer objectPlacer;
        private RectInt outerBound;

        public event EventHandler<ObjectCursor> onCursorDestroyed;

        protected override void Start()
        {
            base.Start();
            if (objectPrefab != null)
            {
                attachedObject = Instantiate(objectPrefab);
                customizableObject = attachedObject.GetComponent<ICustomizableObject>();
                objectPlacer = attachedObject.GetComponent<ObjectGridPlacer>();
                objectPlacer.registerInGrid = false;
                outerBound = attachedObject.GetComponentsInChildren<ObjectGridRect>().GetLocalRects().GetOuterBound();
            }
        }

        protected override void Update()
        {
            base.Update();

            int customizeAction = GetCustomizeAction(player);
            if (customizeAction >= 0 && customizableObject.NextSetting(customizeAction))
            {
                outerBound = attachedObject.GetComponentsInChildren<ObjectGridRect>().GetLocalRects().GetOuterBound();
            }
            bool place = customizeAction < 0 && HasKeyUp(player, Player.Action.Action1);
            bool rotate = customizeAction < 0 && !place && (HasKeyUp(player, Player.Action.Action2));

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

            if (place && placable)
            {
                objectPlacer.registerInGrid = true;
                Destroy(gameObject);
            }
            else if (rotate)
            {
                objectPlacer.rotation = (ObjectGridRects.Rotation)(((int)objectPlacer.rotation + 1) % 4);
            }
        }

        private bool HasKeyUp(Player player, Player.Action action)
        {
            KeyCode? code = player.GetKeyForAction(action);
            return code != null && Input.GetKeyUp(code.Value);
        }

        private bool HasKeyHold(Player player, Player.Action action)
        {
            KeyCode? code = player.GetKeyForAction(action);
            return code != null && Input.GetKey(code.Value);
        }

        private int GetCustomizeAction(Player player)
        {
            if (HasKeyPressed(player, Player.Action.Action2)) {
                if (HasKeyUp(player, Player.Action.Up)) return 0;
                if (HasKeyUp(player, Player.Action.Left)) return 1;
                if (HasKeyUp(player, Player.Action.Down)) return 2;
                if (HasKeyUp(player, Player.Action.Right)) return 3;
            }
            return -1;
        }

        private void OnDestroy()
        {
            onCursorDestroyed?.Invoke(this);
        }
    }
}
