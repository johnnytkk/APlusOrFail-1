using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.PlaceObjectSceneState
{
    using Objects;
    
    public class PlaceObjectSceneState : SceneStateBehavior<IMapStat, Void>
    {
        private new Camera camera;
        public RectTransform uiScene;
        public ObjectCursor cursorPrefab;

        private readonly List<ObjectCursor> objectCursors = new List<ObjectCursor>();


        private void Start()
        {
            camera = Camera.main;
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

            foreach (IRoundPlayerStat roundPlayerStat in arg.GetRoundPlayerStatOfRound(arg.currentRound))
            {
                if (roundPlayerStat.selectedObjectPrefab != null)
                {
                    ObjectCursor cursor = Instantiate(cursorPrefab, uiScene);
                    cursor.player = roundPlayerStat.playerStat.player;
                    cursor.objectPrefab = roundPlayerStat.selectedObjectPrefab;
                    cursor.camera = camera;
                    cursor.onCursorDestroyed += OnObjectCursorDestroyed;
                    objectCursors.Add(cursor);
                }
            }
            if (objectCursors.Count == 0)
            {
                SceneStateManager.instance.Pop(this);
            }
        }

        private void HideUI()
        {
            uiScene.gameObject.SetActive(false);
            for (int i = objectCursors.Count - 1; i >= 0; --i)
            {
                ObjectCursor cursor = objectCursors[i];
                RemoveObjectCursor(cursor);
                Destroy(cursor);
            }
        }

        private void RemoveObjectCursor(ObjectCursor cursor)
        {
            cursor.onCursorDestroyed -= OnObjectCursorDestroyed;
            objectCursors.Remove(cursor);
        }

        private void OnObjectCursorDestroyed(ObjectCursor cursor)
        {
            RemoveObjectCursor(cursor);
            if (objectCursors.Count == 0)
            {
                SceneStateManager.instance.Pop(this);
            }
        }
    }
}
