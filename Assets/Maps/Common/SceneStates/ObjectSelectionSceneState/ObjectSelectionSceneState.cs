using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.ObjectSelectionSceneState
{
    using Objects;
    using ObjectGrid;
    
    public class ObjectSelectionSceneState : SceneStateBehavior<IMapStat, Void>
    {
        private new Camera camera;
        public RectTransform uiScene;
        public KeyCursor keyCursorPrefab;

        private readonly List<ObjectPrefabInfo> attachedPrefabInfos = new List<ObjectPrefabInfo>();
        private readonly List<KeyCursor> keyCursors = new List<KeyCursor>();
        

        private void Start()
        {
            camera = Camera.main;
            HideUI();
        }

        protected override Task OnFocus(ISceneState unloadedSceneState, object result)
        {
            if (unloadedSceneState == null)
            {
                ShowUI();
            }
            return Task.CompletedTask;
        }

        protected override Task OnBlur()
        {
            HideUI();
            return Task.CompletedTask;
        }

        private void ShowUI()
        {
            uiScene.gameObject.SetActive(true);

            IReadOnlyList<ObjectPrefabInfo> usableObjects = arg.roundSettings[arg.currentRound].usableObjects;

            float angleInterval = 2 * Mathf.PI / usableObjects.Count;
            for (int i = 0; i < usableObjects.Count; ++i)
            {
                // https://answers.unity.com/questions/1007585/reading-and-setting-asn-objects-global-scale-with.html

                ObjectPrefabInfo prefabInfo = Instantiate(usableObjects[i]);

                ObjectGridPlacer gridPlacer = prefabInfo.GetComponent<ObjectGridPlacer>();
                gridPlacer.enabled = false;

                // obj.transform.parent = uiScene; // Fix the scale
                attachedPrefabInfos.Add(prefabInfo);

                prefabInfo.gameObject.SetLayerRecursively(LayerId.SelectableObjects);

                RectInt objLocalGridBound = prefabInfo.GetComponentsInChildren<ObjectGridRect>().GetLocalRects().GetOuterBound();
                Rect objLocalWorldBound = new Rect(ObjectGrid.instance.GridToWorldSize(objLocalGridBound.position), ObjectGrid.instance.GridToWorldSize(objLocalGridBound.size));
                Vector2 center = (objLocalWorldBound.min + objLocalWorldBound.max) / 2;

                float angle = Mathf.PI / 2 - angleInterval * i;
                Vector2 position = new Vector2(2 * Mathf.Cos(angle), 2 * Mathf.Sin(angle));
                position -= center;
                prefabInfo.transform.position = position;
            }

            foreach (Player player in (from ps in arg.playerStats select ps.player))
            {
                AddKeyCursor(player);
            }
        }

        private void HideUI()
        {
            uiScene.gameObject.SetActive(false);

            foreach (ObjectPrefabInfo attachedPrefabInfo in attachedPrefabInfos)
            {
                Destroy(attachedPrefabInfo.gameObject);
            }
            attachedPrefabInfos.Clear();

            for (int i = keyCursors.Count - 1; i >= 0; --i)
            {
                RemoveKeyCursor(keyCursors[i]);
            }
        }

        private void AddKeyCursor(Player player)
        {
            KeyCursor keyCursor = Instantiate(keyCursorPrefab, uiScene);
            keyCursor.player = player;
            keyCursors.Add(keyCursor);
        }

        private void RemoveKeyCursor(KeyCursor keyCursor)
        {
            keyCursors.Remove(keyCursor);
            Destroy(keyCursor.gameObject);
        }

        private void Update()
        {
            if (phase.IsAtLeast(SceneStatePhase.Focused))
            {
                for (int i = keyCursors.Count - 1; i >= 0; --i)
                {
                    KeyCursor keyCursor = keyCursors[i];
                    if (HasKeyUp(keyCursor.player, Player.Action.Action1))
                    {
                        ObjectPrefabInfo prefabInfo = Physics2D.OverlapPoint(camera.ViewportToWorldPoint(keyCursor.viewportLocation), 1 << LayerId.SelectableObjects)?.gameObject.GetComponentInParent<ObjectPrefabInfo>();
                        if (prefabInfo != null)
                        {
                            arg.GetRoundPlayerStat(arg.currentRound, arg.playerStats.FindIndex(ps => ps.player ==  keyCursor.player))
                                .selectedObjectPrefab = prefabInfo.prefab;

                            RemoveKeyCursor(keyCursor);

                            Destroy(prefabInfo.gameObject);
                            attachedPrefabInfos.Remove(prefabInfo);

                            if (keyCursors.Count == 0 || attachedPrefabInfos.Count == 0)
                            {
                                SceneStateManager.instance.Pop(this, null);
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
