using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.ObjectSelectionSceneState
{
    using KeyCursorController;
    using Objects;
    
    [RequireComponent(typeof(KeyCursorController))]
    public class ObjectSelectionSceneState : SceneStateBehavior<object, object>
    {
        private static int objectLayerIndex = -1;

        public RectTransform uiScene;
        public List<GameObject> objectPrefabs;

        public readonly ReadOnlyDictionary<Player, GameObject> selectedObjects;

        private KeyCursorController keyCursorController;

        private readonly List<GameObject> attachedObjects = new List<GameObject>();
        private readonly List<KeyCursorController.KeyCursor> keyCursors = new List<KeyCursorController.KeyCursor>();
        private KeyCursorController.KeyCursor[] keyCursorsForUpdate;
        private bool keyCursorsModified;
        private readonly Dictionary<Player, GameObject> selectedObjectsInternal = new Dictionary<Player, GameObject>();

        public ObjectSelectionSceneState()
        {
            selectedObjects = new ReadOnlyDictionary<Player, GameObject>(selectedObjectsInternal);
        }

        private void Awake()
        {
            if (objectLayerIndex < 0)
            {
                objectLayerIndex = LayerMask.NameToLayer("Selectable Objects");
                if (objectLayerIndex < 0)
                {
                    Debug.LogErrorFormat("Cannot find layer \"Selectable Objects\"");
                }
            }
        }

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

        private void ShowUI()
        {
            uiScene.gameObject.SetActive(true);

            float angleInterval = 2 * Mathf.PI / objectPrefabs.Count;
            for (int i = 0; i < objectPrefabs.Count; ++i)
            {
                GameObject obj = Instantiate(objectPrefabs[i], transform);
                attachedObjects.Add(obj);

                obj.layer = objectLayerIndex;

                // https://answers.unity.com/questions/1007585/reading-and-setting-asn-objects-global-scale-with.html

                obj.transform.localScale = Vector3.one;
                Vector3 scale = new Vector3(1 / obj.transform.lossyScale.x, 1 / obj.transform.lossyScale.y, 1 / obj.transform.lossyScale.z);

                obj.transform.localScale = scale;

                float angle = Mathf.PI / 2 - angleInterval * i;
                Vector2 locationPosition = new Vector2(2 * Mathf.Cos(angle) * scale.x, 2 * Mathf.Sin(angle) * scale.y);
                obj.transform.localPosition = locationPosition;
            }

            foreach (Player player in Player.players)
            {
                AddKeyCursor(player);
            }
        }

        private void HideUI()
        {
            uiScene.gameObject.SetActive(false);

            foreach (GameObject attachedObject in attachedObjects)
            {
                Destroy(attachedObject);
            }
            attachedObjects.Clear();

            foreach (KeyCursorController.KeyCursor keyCursor in keyCursors)
            {
                keyCursor.Remove();
                keyCursorsModified = true;
            }
            if (keyCursorsModified) keyCursors.Clear();
        }

        private void AddKeyCursor(Player player)
        {
            KeyCursorController.KeyCursor keyCursor = keyCursorController.AddKeyCursor(player);
            keyCursors.Add(keyCursor);
            keyCursorsModified = true;
        }

        private void RemoveKeyCursor(KeyCursorController.KeyCursor keyCursor)
        {
            if (keyCursors.Remove(keyCursor))
            {
                keyCursor.Remove();
                keyCursorsModified = true;
            }
        }

        private void Update()
        {
            if (keyCursorsModified)
            {
                keyCursorsForUpdate = keyCursors.Count > 0 ? keyCursors.ToArray() : null;
                keyCursorsModified = false;
            }
            if (phase.IsAtLeast(SceneStatePhase.Activated) && keyCursorsForUpdate != null)
            {
                foreach (KeyCursorController.KeyCursor keyCursor in keyCursorsForUpdate)
                {
                    if (HasKeyUp(keyCursor.player, Player.Action.Select))
                    {
                        GameObject selectedObject = Physics2D.OverlapPoint(keyCursor.location, 1 << objectLayerIndex)?.gameObject;
                        if (selectedObject != null)
                        {
                            selectedObjectsInternal.Add(keyCursor.player, selectedObject.GetComponent<ObjectPrefabInfo>().prefabLink.prefab);

                            RemoveKeyCursor(keyCursor);

                            Destroy(selectedObject);
                            attachedObjects.Remove(selectedObject);

                            if (keyCursors.Count == 0 || attachedObjects.Count == 0)
                            {
                                SceneStateManager.instance.PopSceneState();
                            }
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
