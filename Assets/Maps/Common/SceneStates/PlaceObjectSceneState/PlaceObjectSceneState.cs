using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.PlaceObjectSceneState
{
    using KeyCursorController;
    using Objects;
    using ObjectGrid;

    [RequireComponent(typeof(KeyCursorController))]
    public class PlaceObjectSceneState : SceneStateBehavior<IDictionary<Player, GameObject>, Void>
    {
        private class ObjectCursor
        {
            private readonly PlaceObjectSceneState outer;

            public readonly Player player;
            public readonly KeyCursorController.KeyCursor keyCursor;
            public readonly GameObject obj;
            public readonly ObjectGridPlacer objectGridPlacer;
            public readonly RectInt outerBound;

            public ObjectCursor(PlaceObjectSceneState outer, Player player, GameObject prefab)
            {
                this.outer = outer;

                this.player = player;
                keyCursor = outer.keyCursorController.AddKeyCursor(player);
                obj = Instantiate(prefab);
                objectGridPlacer = obj.GetComponent<ObjectGridPlacer>();
                outerBound = obj.GetComponentsInChildren<ObjectGridRect>().GetLocalRects().GetOuterBound();

                objectGridPlacer.registerInGrid = false;

                outer.objectCursors.Add(this);
            }

            public void Update()
            {
                RectInt rotatedOuterBound = outerBound.Rotate(objectGridPlacer.rotation);
                Vector2Int gridOffset = new Vector2Int(-rotatedOuterBound.xMax, -rotatedOuterBound.yMin + 1);
                Vector2Int objGridPosition = ObjectGrid.instance.WorldToGridPosition(keyCursor.location) + gridOffset;

                objectGridPlacer.gridPosition = objGridPosition;

                bool placable = objectGridPlacer.IsRegisterable();
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
                    objectGridPlacer.registerInGrid = true;
                    Remove();
                }
                else if (HasKeyUp(player, Player.Action.Cancel))
                {
                    objectGridPlacer.rotation = (ObjectGridRects.Rotation)(((int)objectGridPlacer.rotation + 1) % 4);
                }
            }

            private bool HasKeyUp(Player player, Player.Action action)
            {
                KeyCode? code = player.GetKeyForAction(action);
                return code != null && Input.GetKeyUp(code.Value);
            }

            public void Remove()
            {
                keyCursor.Remove();
                outer.objectCursors.Remove(this);

                if (outer.objectCursors.Count == 0)
                {
                    SceneStateManager.instance.Pop(outer);
                }
            }
        }

        public RectTransform uiScene;

        private KeyCursorController keyCursorController;

        private readonly List<ObjectCursor> objectCursors = new List<ObjectCursor>();

        private void Start()
        {
            keyCursorController = GetComponent<KeyCursorController>();
            HideUI();
        }

        protected override void OnActivate(ISceneState unloadedSceneState, object result)
        {
            base.OnActivate(unloadedSceneState, result);
            ShowUI();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            HideUI();
        }

        private void Update()
        {
            if (phase.IsAtLeast(SceneStatePhase.Activated))
            {
                for (int i = objectCursors.Count - 1; i >= 0; --i)
                {
                    objectCursors[i].Update();
                }
            }
        }

        private void ShowUI()
        {
            uiScene.gameObject.SetActive(true);

            foreach (KeyValuePair<Player, GameObject> pair in arg)
            {
                new ObjectCursor(this, pair.Key, pair.Value);
            }
        }

        private void HideUI()
        {
            uiScene.gameObject.SetActive(false);
            for (int i = objectCursors.Count - 1; i >= 0; --i)
            {
                objectCursors[i].Remove();
            }
        }
    }
}
