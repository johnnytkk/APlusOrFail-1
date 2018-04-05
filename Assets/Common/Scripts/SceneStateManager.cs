using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail
{
    public class SceneStateManager : MonoBehaviour
    {
        private enum Action
        {
            None,
            Push,
            Replace,
            Pop
        }

        public static SceneStateManager instance { get; private set; }

        public SceneState initialSceneState;

        private Stack<SceneState> SceneStateStack = new Stack<SceneState>();
        private Action pendingAction = Action.None;
        private SceneState pendingSceneState;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
                Debug.LogErrorFormat("Found another scene state manager!");
            }
        }

        private void OnDestroy()
        {
            while (SceneStateStack.Count > 0)
            {
                SceneState sceneState = SceneStateStack.Pop();
                if (sceneState.state.IsAtLeast(SceneState.State.Activated))
                {
                    sceneState.Deactivate();
                }
                if (sceneState.state.IsAtLeast(SceneState.State.Loaded))
                {
                    sceneState.UnLoad();
                }
            }
            if (instance == this)
            {
                instance = null;
            }
        }

        private void Start()
        {
            if (initialSceneState != null)
            {
                PushSceneState(initialSceneState);
            }
        }

        private void Update()
        {
            Action pendingAction = this.pendingAction;
            SceneState pendingSceneState = this.pendingSceneState;

            this.pendingAction = Action.None;
            this.pendingSceneState = null;

            switch (pendingAction)
            {
                case Action.Push:
                    if (SceneStateStack.Count > 0)
                    {
                        SceneState scene = SceneStateStack.Peek();
                        scene.Deactivate();
                    }
                    SceneStateStack.Push(pendingSceneState);
                    pendingSceneState.Load();
                    pendingSceneState.Activate();
                    break;

                case Action.Replace:
                    if (SceneStateStack.Count > 0)
                    {
                        SceneState scene = SceneStateStack.Peek();
                        scene.Deactivate();
                        scene.UnLoad();
                        SceneStateStack.Pop();
                    }
                    SceneStateStack.Push(pendingSceneState);
                    pendingSceneState.Load();
                    pendingSceneState.Activate();
                    break;

                case Action.Pop:
                    if (SceneStateStack.Count > 0)
                    {
                        SceneState detachedScene = SceneStateStack.Peek();
                        detachedScene.Deactivate();
                        detachedScene.UnLoad();
                        SceneStateStack.Pop();
                        if (SceneStateStack.Count > 0)
                        {
                            SceneState topScene = SceneStateStack.Peek();
                            topScene.Activate();
                        }
                    }
                    break;
            }
        }

        public void PushSceneState(SceneState sceneState)
        {
            pendingAction = Action.Push;
            pendingSceneState = sceneState;
        }

        public void ReplaceSceneState(SceneState sceneState)
        {
            pendingAction = Action.Replace;
            pendingSceneState = sceneState;
        }

        public void PopSceneState()
        {
            pendingAction = Action.Pop;
            pendingSceneState = null;
        }

        public void RemovePendingAction()
        {
            pendingAction = Action.None;
            pendingSceneState = null;
        }
    }
}
