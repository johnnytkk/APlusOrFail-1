using System;
using System.Collections.Generic;
using System.Linq;
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

        public UnityEngine.Object initialSceneState;

        private Stack<ISceneState> SceneStateStack = new Stack<ISceneState>();
        private Action pendingAction = Action.None;
        private ISceneState pendingSceneState;
        private object pendingArg;

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
                ISceneState sceneState = SceneStateStack.Pop();
                if (sceneState.phase.IsAtLeast(SceneStatePhase.Activated))
                {
                    sceneState.Deactivate();
                }
                if (sceneState.phase.IsAtLeast(SceneStatePhase.Loaded))
                {
                    sceneState.Unload();
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
                if (initialSceneState.GetType().GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Any(i => i.GetGenericTypeDefinition() == typeof(ISceneState<,>)))
                {
                    ISceneState state = (ISceneState)initialSceneState;
                    Type argType = state.argType;
                    Type resultType = state.resultType;
                    new Action<ISceneState<object, object>, object>(PushSceneState).Method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(argType, resultType)
                        .Invoke(this, new object[] { state, argType.IsValueType ? Activator.CreateInstance(argType) : null });
                }
                else
                {
                    Debug.LogErrorFormat($"initial scene state has type {initialSceneState.GetType()} which does not implement {typeof(ISceneState<,>)}");
                }
            }
        }

        private void Update()
        {
            Action pendingAction = this.pendingAction;
            ISceneState pendingSceneState = this.pendingSceneState;
            object pendingArg = this.pendingArg;

            this.pendingAction = Action.None;
            this.pendingSceneState = null;
            this.pendingArg = null;

            switch (pendingAction)
            {
                case Action.Push:
                    if (SceneStateStack.Count > 0)
                    {
                        ISceneState scene = SceneStateStack.Peek();
                        scene.Deactivate();
                    }
                    SceneStateStack.Push(pendingSceneState);
                    pendingSceneState.Load(pendingArg);
                    pendingSceneState.Activate(null, null);
                    break;

                case Action.Replace:
                    if (SceneStateStack.Count > 0)
                    {
                        ISceneState unloadedSceneState = SceneStateStack.Peek();
                        unloadedSceneState.Deactivate();
                        unloadedSceneState.Unload();
                        SceneStateStack.Pop();
                    }
                    SceneStateStack.Push(pendingSceneState);
                    pendingSceneState.Load(pendingArg);
                    pendingSceneState.Activate(null, null);
                    break;

                case Action.Pop:
                    if (SceneStateStack.Count > 0)
                    {
                        ISceneState detachedScene = SceneStateStack.Peek();
                        detachedScene.Deactivate();
                        object result = detachedScene.Unload();
                        SceneStateStack.Pop();
                        if (SceneStateStack.Count > 0)
                        {
                            ISceneState topScene = SceneStateStack.Peek();
                            topScene.Activate(detachedScene, result);
                        }
                    }
                    break;
            }
        }

        public void PushSceneState<TA, TR>(ISceneState<TA, TR> sceneState, TR arg)
        {
            pendingAction = Action.Push;
            pendingSceneState = sceneState;
            pendingArg = arg;
        }

        public void ReplaceSceneState<TA, TR>(ISceneState<TA, TR> sceneState, TR arg)
        {
            pendingAction = Action.Replace;
            pendingSceneState = sceneState;
            pendingArg = arg;
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
