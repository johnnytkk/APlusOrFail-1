using UnityEngine;

namespace APlusOrFail
{
    public static class SceneStateExtensions
    {
        public static bool IsAtLeast(this SceneState.State state, SceneState.State min)
        {
            return state >= min;
        }
    }

    public class SceneState : MonoBehaviour
    {
        public enum State
        {
            Initialized,
            Loaded,
            Activated
        }
        
        public State state { get; private set; } = State.Initialized;

        public void Load()
        {
            state = State.Loaded;
            OnLoad();
        }

        public void Activate()
        {
            state = State.Activated;
            OnActivate();
        }

        public void Deactivate()
        {
            OnDeactivate();
            state = State.Loaded;
        }

        public void UnLoad()
        {
            OnUnLoad();
            state = State.Initialized;
        }


        protected virtual void OnLoad() { }

        protected virtual void OnActivate() { }

        protected virtual void OnDeactivate() { }

        protected virtual void OnUnLoad() { }
    }
}
