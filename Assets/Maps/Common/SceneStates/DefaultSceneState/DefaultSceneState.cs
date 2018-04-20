using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.DefaultSceneState
{
    using ObjectSelectionSceneState;
    using PlaceObjectSceneState;
    using RoundSceneState;
    using RankSceneState;

    public class DefaultSceneState : SceneStateBehavior<IMapStat, Void>
    {
        public ObjectSelectionSceneState objectSelectionUIScene;
        public PlaceObjectSceneState placeObjectUIScene;
        public RoundSceneState roundUIScene;
        public RankSceneState rankSceneState;
        

        protected override Task OnFocus(ISceneState unloadedSceneState, object result)
        {
            Type unloadedType = unloadedSceneState?.GetType();
            if (unloadedSceneState == null)
            {
                OnMapStart();
            }
            else if (unloadedType == typeof(ObjectSelectionSceneState))
            {
                OnObjectSelectionFinished();
            }
            else if (unloadedType == typeof(PlaceObjectSceneState))
            {
                OnPlaceObjectFinished();
            }
            else if (unloadedType == typeof(RoundSceneState))
            {
                OnRoundUISceneFinished();
            }
            else if (unloadedType == typeof(RankSceneState))
            {
                OnRankFinished();
            }
            return Task.CompletedTask;
        }

        private void OnMapStart()
        {
            OnRankFinished();
        }

        private void OnObjectSelectionFinished()
        {
            SceneStateManager.instance.Push(placeObjectUIScene, arg);
        }

        private void OnPlaceObjectFinished()
        {
            SceneStateManager.instance.Push(roundUIScene, arg);
        }

        private void OnRoundUISceneFinished()
        {
            SceneStateManager.instance.Push(rankSceneState, arg);
        }

        private void OnRankFinished()
        {
            if ((arg.currentRound + 1) < arg.roundCount)
            {
                ++arg.currentRound;
                SceneStateManager.instance.Push(objectSelectionUIScene, arg);
            }
            else
            {
                print("Round Finished!");
            }
        }
    }
}
