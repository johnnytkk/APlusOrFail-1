using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.DefaultSceneState
{
    using ObjectSelectionSceneState;
    using PlaceObjectSceneState;
    using RoundSceneState;

    public class DefaultSceneState : SceneState
    {
        

        public ObjectSelectionSceneState objectSelectionUIScene;
        public PlaceObjectSceneState placeObjectUIScene;
        public RoundSceneState roundUIScene;

        public GameObject test_characterSprite;

        private ObjectSelectionSceneState activeObjectSelectionUIScene;
        private PlaceObjectSceneState activePlaceObjectUIScene;
        private RoundSceneState activeRoundUIScene;

        private int currentRound;

        private void Awake()
        {
            if (Player.players.Count == 0)
            {
                _InsertTestData();
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (activeObjectSelectionUIScene != null)
            {
                OnObjectSelectionUISceneFinished(activeObjectSelectionUIScene);
                activeObjectSelectionUIScene = null;
            }
            else if (activePlaceObjectUIScene != null)
            {
                OnPlaceObjectUISceneFinished(activePlaceObjectUIScene);
                activePlaceObjectUIScene = null;
            }
            else if (activeRoundUIScene != null)
            {
                OnRoundUISceneFinished(activeRoundUIScene);
                activeRoundUIScene = null;
            }
            else
            {
                StartRound();
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
            SceneStateManager.instance.PushSceneState(objectSelectionUIScene);
        }

        private void OnObjectSelectionUISceneFinished(ObjectSelectionSceneState objectSelectionUIScene)
        {
            SceneStateManager.instance.PushSceneState(placeObjectUIScene);
            activePlaceObjectUIScene = placeObjectUIScene;
            activePlaceObjectUIScene.selectedObjects = new Dictionary<Player, GameObject>(objectSelectionUIScene.selectedObjects);
        }

        private void OnPlaceObjectUISceneFinished(PlaceObjectSceneState placeObjectUIScene)
        {
            SceneStateManager.instance.PushSceneState(roundUIScene);
            activeRoundUIScene = roundUIScene;
        }

        private void OnRoundUISceneFinished(RoundSceneState roundUIScene)
        {

        }
    }
}
