using System;
using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.DefaultSceneState
{
    using ObjectSelectionSceneState;
    using PlaceObjectSceneState;
    using RoundSceneState;
    using System.Collections.ObjectModel;

    public class DefaultSceneState : SceneStateBehavior<object, object>
    {
        

        public ObjectSelectionSceneState objectSelectionUIScene;
        public PlaceObjectSceneState placeObjectUIScene;
        public RoundSceneState roundUIScene;

        public GameObject test_characterSprite;

        private ObjectSelectionSceneState activeObjectSelectionUIScene;
        private PlaceObjectSceneState activePlaceObjectUIScene;

        private int currentRound;

        private void Awake()
        {
            if (Player.players.Count == 0)
            {
                _InsertTestData();
            }
        }

        protected override void OnActivate(ISceneState unloadedSceneState, object result)
        {
            base.OnActivate(unloadedSceneState, result);

            Type unloadedType = unloadedSceneState?.GetType();
            if (unloadedSceneState == null)
            {
                StartRound();
            }
            else if (activeObjectSelectionUIScene != null)
            {
                OnObjectSelectionUISceneFinished(activeObjectSelectionUIScene);
                activeObjectSelectionUIScene = null;
            }
            else if (activePlaceObjectUIScene != null)
            {
                OnPlaceObjectUISceneFinished(activePlaceObjectUIScene);
                activePlaceObjectUIScene = null;
            }
            else if (unloadedType == typeof(RoundSceneState))
            {
                OnRoundUISceneFinished((ReadOnlyCollection<RoundSceneState.PlayerStatistics>)result);
            }
        }

        private void _InsertTestData()
        {
            Player player = new Player
            {
                characterSprite = test_characterSprite,
                name = "Trim",
                color = Color.blue
            };
            player.MapActionToKey(Player.Action.Left, KeyCode.LeftArrow);
            player.MapActionToKey(Player.Action.Right, KeyCode.RightArrow);
            player.MapActionToKey(Player.Action.Up, KeyCode.UpArrow);
            player.MapActionToKey(Player.Action.Down, KeyCode.DownArrow);
            player.MapActionToKey(Player.Action.Select, KeyCode.Home);
            player.MapActionToKey(Player.Action.Cancel, KeyCode.End);
        }

        private void StartRound()
        {
            ++currentRound;
            activeObjectSelectionUIScene = objectSelectionUIScene;
            SceneStateManager.instance.Push(objectSelectionUIScene, null);
        }

        private void OnObjectSelectionUISceneFinished(ObjectSelectionSceneState objectSelectionUIScene)
        {
            SceneStateManager.instance.Push(placeObjectUIScene, null);
            activePlaceObjectUIScene = placeObjectUIScene;
            activePlaceObjectUIScene.selectedObjects = new Dictionary<Player, GameObject>(objectSelectionUIScene.selectedObjects);
        }

        private void OnPlaceObjectUISceneFinished(PlaceObjectSceneState placeObjectUIScene)
        {
            SceneStateManager.instance.Push(roundUIScene, null);
        }

        private void OnRoundUISceneFinished(ReadOnlyCollection<RoundSceneState.PlayerStatistics> playerStatistics)
        {

        }
    }
}
