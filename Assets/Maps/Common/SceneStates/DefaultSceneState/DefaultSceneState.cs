using System;
using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.DefaultSceneState
{
    using Objects;
    using ObjectSelectionSceneState;
    using PlaceObjectSceneState;
    using RoundSceneState;
    using RankSceneState;

    public class DefaultSceneState : SceneStateBehavior<Void, Void>
    {

        public ObjectSelectionSceneState objectSelectionUIScene;
        public PlaceObjectSceneState placeObjectUIScene;
        public RoundSceneState roundUIScene;
        public RankSceneState rankSceneState;
        public GameObject test_characterSprite;
        
        private MapStat mapStat;


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

            Player player2 = new Player
            {
                characterSprite = test_characterSprite,
                name = "Leung",
                color = Color.red
            };
            player2.MapActionToKey(Player.Action.Left, KeyCode.A);
            player2.MapActionToKey(Player.Action.Right, KeyCode.D);
            player2.MapActionToKey(Player.Action.Up, KeyCode.W);
            player2.MapActionToKey(Player.Action.Down, KeyCode.S);
            player2.MapActionToKey(Player.Action.Select, KeyCode.Q);
            player2.MapActionToKey(Player.Action.Cancel, KeyCode.E);
        }


        private void OnMapStart()
        {
            IMapSetting mapSetting = GameObject.Find("MapSetting")?.GetComponent<IMapSetting>();
            if (mapSetting != null)
            {
                mapStat = new MapStat(mapSetting);
                OnRankFinished();
            }
            else
            {
                Debug.LogErrorFormat("Cannot find map setting!");
            }
        }

        private void OnObjectSelectionFinished()
        {
            SceneStateManager.instance.Push(placeObjectUIScene, mapStat);
        }

        private void OnPlaceObjectFinished()
        {
            SceneStateManager.instance.Push(roundUIScene, mapStat);
        }

        private void OnRoundUISceneFinished()
        {
            SceneStateManager.instance.Push(rankSceneState, mapStat);
        }

        private void OnRankFinished()
        {
            if ((mapStat.currentRound + 1) < mapStat.roundCount)
            {
                ++mapStat.currentRound;
                SceneStateManager.instance.Push(objectSelectionUIScene, mapStat);
            }
            else
            {
                print("Round Finished!");
            }
        }
    }
}
