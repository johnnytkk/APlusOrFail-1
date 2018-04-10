using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.PlaceObjectSceneState
{
    using KeyCursorController;
    using Objects;
    using ObjectGrid;

    [RequireComponent(typeof(KeyCursorController))]
    public class PlaceObjectSceneState : SceneStateBehavior<Void, Void>
    {
        private class ObjectData
        {
            public readonly GameObject obj;
            public readonly ObjectGridSize objectGridSize;

            public ObjectData(GameObject obj)
            {
                this.obj = obj;
                objectGridSize = obj.GetComponent<ObjectGridSize>();
            }
        }

        public RectTransform uiScene;

        public IDictionary<Player, GameObject> selectedObjects;

        private KeyCursorController keyCursorController;

        private readonly Dictionary<KeyCursorController.KeyCursor, ObjectData> keyCursorObjects = new Dictionary<KeyCursorController.KeyCursor, ObjectData>();
        private KeyValuePair<KeyCursorController.KeyCursor, ObjectData>[] keyCursorObjectsForUpdate;
        private bool keyCursorObjectsModified;

        private void Start()
        {
            keyCursorController = GetComponent<KeyCursorController>();
            HideUI();
        }

        protected override void OnLoad(Void arg)
        {
            base.OnLoad(arg);
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

        private void ShowUI()
        {
            uiScene.gameObject.SetActive(true);

            foreach (KeyValuePair<Player, GameObject> pair in selectedObjects)
            {
                AddKeyCursorWithObject(pair.Key, pair.Value);
            }
        }

        private void HideUI()
        {
            uiScene.gameObject.SetActive(false);
            foreach (KeyValuePair<KeyCursorController.KeyCursor, ObjectData> pair in keyCursorObjects)
            {
                pair.Key.Remove();
                keyCursorObjects.Remove(pair.Key);
                Destroy(pair.Value.obj);
                keyCursorObjectsModified = true;
            }
            if (keyCursorObjectsModified) keyCursorObjects.Clear();
        }

        private void AddKeyCursorWithObject(Player player, GameObject objectPrefab)
        {
            KeyCursorController.KeyCursor keyCursor = keyCursorController.AddKeyCursor(player);
            GameObject obj = Instantiate(objectPrefab);
            keyCursorObjects.Add(keyCursor, new ObjectData(obj));
            keyCursorObjectsModified = true;
        }

        private void RemoveKeyCursorWithObject(KeyCursorController.KeyCursor keyCursor)
        {
            ObjectData objectData;
            if (keyCursorObjects.TryGetValue(keyCursor, out objectData))
            {
                keyCursor.Remove();
                Destroy(objectData.obj);
                keyCursorObjects.Remove(keyCursor);
                keyCursorObjectsModified = true;
            }
        }

        private void Update()
        {
            if (keyCursorObjectsModified)
            {
                keyCursorObjectsForUpdate = keyCursorObjects.Count > 0 ? keyCursorObjects.ToArray() : null;
                keyCursorObjectsModified = false;
            }
            if (phase.IsAtLeast(SceneStatePhase.Activated) && keyCursorObjectsForUpdate != null)
            {
                foreach (KeyValuePair<KeyCursorController.KeyCursor, ObjectData> pair in keyCursorObjectsForUpdate)
                {
                    Vector2Int objectPosition = ObjectGrid.instance.WorldToGridCoordinate(pair.Key.location);
                    objectPosition.x -= pair.Value.objectGridSize.gridSize.x;

                    pair.Value.obj.transform.position = ObjectGrid.instance.GridToWorldPosition(objectPosition);

                    RectInt objectRect = new RectInt(objectPosition, pair.Value.objectGridSize.gridSize);

                    bool placeable = ObjectGrid.instance.IsPlaceable(objectRect);
                    if (placeable)
                    {
                        Debug.LogFormat("Can place!");
                    }
                    else
                    {
                        Debug.LogFormat("Cannot place!");
                    }

                    if (placeable && HasKeyUp(pair.Key.player, Player.Action.Select))
                    {
                        GameObject obj = Instantiate(pair.Value.obj.GetComponent<ObjectPrefabInfo>().prefab);
                        ObjectGrid.instance.Add(objectRect, obj);
                        obj.transform.position = ObjectGrid.instance.GridToWorldPosition(objectRect.min);

                        RemoveKeyCursorWithObject(pair.Key);

                        if (keyCursorObjects.Count == 0)
                        {
                            SceneStateManager.instance.Pop(this);
                        }
                    }
                }
            }
        }

        private bool HasKeyUp(Player player, Player.Action action)
        {
            KeyCode? code = player.GetKeyForAction(action);
            return code != null && Input.GetKeyUp(code.Value);
        }
    }
}
