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

        private readonly Stack<ISceneState> sceneStateStack = new Stack<ISceneState>();
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
            while (sceneStateStack.Count > 0)
            {
                ISceneState sceneState = sceneStateStack.Pop();
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
                    new Action<ISceneState<object, object>, object>(Push).Method
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
                    if (sceneStateStack.Count > 0)
                    {
                        ISceneState scene = sceneStateStack.Peek();
                        scene.Deactivate();
                    }
                    sceneStateStack.Push(pendingSceneState);
                    pendingSceneState.Load(pendingArg);
                    pendingSceneState.Activate(null, null);
                    break;

                case Action.Replace:
                    if (sceneStateStack.Count > 0)
                    {
                        ISceneState unloadedSceneState = sceneStateStack.Peek();
                        unloadedSceneState.Deactivate();
                        unloadedSceneState.Unload();
                        sceneStateStack.Pop();
                    }
                    sceneStateStack.Push(pendingSceneState);
                    pendingSceneState.Load(pendingArg);
                    pendingSceneState.Activate(null, null);
                    break;

                case Action.Pop:
                    if (sceneStateStack.Count > 0)
                    {
                        ISceneState detachedScene = sceneStateStack.Peek();
                        detachedScene.Deactivate();
                        object result = detachedScene.Unload();
                        sceneStateStack.Pop();
                        if (sceneStateStack.Count > 0)
                        {
                            ISceneState topScene = sceneStateStack.Peek();
                            topScene.Activate(detachedScene, result);
                        }
                    }
                    break;
            }
        }

        public void Push<TA, TR>(ISceneState<TA, TR> sceneState, TA arg)
        {
            pendingAction = Action.Push;
            pendingSceneState = sceneState;
            pendingArg = arg;
        }

        public void Replace<TA, TR>(ISceneState<TA, TR> sceneState, TA arg)
        {
            pendingAction = Action.Replace;
            pendingSceneState = sceneState;
            pendingArg = arg;
        }

        public void Pop(ISceneState sceneState)
        {
            if (sceneStateStack.Peek() != sceneState)
            {
                throw new InvalidOperationException("The scene to pop is not active!");
            }
            pendingAction = Action.Pop;
            pendingSceneState = null;
            pendingArg = null;
        }

        public void Undo()
        {
            pendingAction = Action.None;
            pendingSceneState = null;
        }
    }
}
