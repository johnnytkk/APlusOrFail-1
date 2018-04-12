using System;
using UnityEngine;

namespace APlusOrFail
{
    public static class SceneStateExtensions
    {
        public static bool IsAtLeast(this SceneStatePhase state, SceneStatePhase min)
        {
            return state >= min;
        }
    }

    public enum SceneStatePhase
    {
        Initialized,
        Loaded,
        Activated
    }

    public interface ISceneState
    {
        Type argType { get; }
        Type resultType { get; }

        SceneStatePhase phase { get; }

        void Load(object arg);

        void Activate(ISceneState unloadedSceneState, object result);

        void Deactivate();

        object Unload();
    }

    public interface ISceneState<TArg, TResult> : ISceneState
    {
        void Load(TArg arg);

        new TResult Unload();
    }

    public class SceneStateBehavior<TArg, TResult> : MonoBehaviour, ISceneState<TArg, TResult>
    {
        public Type argType => typeof(TArg);
        public Type resultType => typeof(TResult);
        
        public SceneStatePhase phase { get; private set; } = SceneStatePhase.Initialized;

        void ISceneState.Load(object arg) => Load((TArg)arg);

        public void Load(TArg arg)
        {
            phase = SceneStatePhase.Loaded;
            OnLoad(arg);
        }

        public void Activate(ISceneState unloadedSceneState, object result)
        {
            phase = SceneStatePhase.Activated;
            OnActivate(unloadedSceneState, result);
        }

        public void Deactivate()
        {
            OnDeactivate();
            phase = SceneStatePhase.Loaded;
        }

        object ISceneState.Unload() => Unload();

        public TResult Unload()
        {
            TResult result = OnUnload();
            phase = SceneStatePhase.Initialized;
            return result;
        }

        protected TArg arg { get; private set; }

        protected virtual void OnLoad(TArg arg) => this.arg = arg;

        protected virtual void OnActivate(ISceneState unloadedSceneState, object result) { }

        protected virtual void OnDeactivate() { }

        protected virtual TResult OnUnload()
        {
            arg = default(TArg);
            return default(TResult);
        }
    }
}
