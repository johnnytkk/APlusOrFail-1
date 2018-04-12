using System;
using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.RankSceneState
{
    using RoundSceneState;

    public class RankSceneState : SceneStateBehavior<IEnumerable<RoundSceneState.PlayerStatistics>, Void>
    {
        public Canvas canvas;

        
        private void Start()
        {
            HideUI();
        }
        
        private void Update()
        {

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

        private void HideUI()
        {
            canvas.gameObject.SetActive(false);
        }

        private void ShowUI()
        {
            canvas.gameObject.SetActive(true);
        }
    }
}
