using System;
using System.Threading.Tasks;
using UnityEngine;

namespace APlusOrFail
{
    public enum SceneStatePhase
    {
        Initialized,
        Loaded,
        Visible,
        Focused
    }

    public static class SceneStateExtensions
    {
        public static bool IsAtLeast(this SceneStatePhase state, SceneStatePhase min)
        {
            return state >= min;
        }
    }

    public interface ISceneState
    {
        Type argType { get; }
        Type resultType { get; }

        SceneStatePhase phase { get; }

        Task Load(object arg, ISceneState unloadedSceneState, object result);

        Task MakeVisible(ISceneState unloadedSceneState, object result);

        Task Focus(ISceneState unloadedSceneState, object result);

        Task Blur();

        Task MakeInvisible();

        Task Unload();
    }

    public interface ISceneState<TArg, TResult> : ISceneState
    {
        Task Load(TArg arg, ISceneState unloadedSceneState, object result);
    }

    public class SceneStateBehavior<TArg, TResult> : MonoBehaviour, ISceneState<TArg, TResult>
    {
        public Type argType => typeof(TArg);
        public Type resultType => typeof(TResult);
        
        public SceneStatePhase phase { get; private set; } = SceneStatePhase.Initialized;
        protected TArg arg { get; private set; }

        Task ISceneState.Load(object arg, ISceneState unloadedSceneState, object result) => Load((TArg)arg, unloadedSceneState, result);
        public Task Load(TArg arg, ISceneState unloadedSceneState, object result)
        {
            phase = SceneStatePhase.Loaded;
            this.arg = arg;
            return OnLoad(unloadedSceneState, result);
        }

        public Task MakeVisible(ISceneState unloadedSceneState, object result)
        {
            phase = SceneStatePhase.Visible;
            return OnMakeVisible(unloadedSceneState, result);
        }

        public Task Focus(ISceneState unloadedSceneState, object result)
        {
            phase = SceneStatePhase.Focused;
            return OnFocus(unloadedSceneState, result);
        }

        public async Task Blur()
        {
            await OnBlur();
            phase = SceneStatePhase.Visible;
        }

        public async Task MakeInvisible()
        {
            await OnMakeInvisible();
            phase = SceneStatePhase.Loaded;
        }
        
        public async Task Unload()
        {
            await OnUnload();
            arg = default(TArg);
            phase = SceneStatePhase.Initialized;
        }


        protected virtual Task OnLoad(ISceneState unloadedSceneState, object result) => Task.CompletedTask;

        protected virtual Task OnMakeVisible(ISceneState unloadedSceneState, object result) => Task.CompletedTask;

        protected virtual Task OnFocus(ISceneState unloadedSceneState, object result) => Task.CompletedTask;

        protected virtual Task OnBlur() => Task.CompletedTask;

        protected virtual Task OnMakeInvisible() => Task.CompletedTask;

        protected virtual Task OnUnload() => Task.CompletedTask;
    }
}
