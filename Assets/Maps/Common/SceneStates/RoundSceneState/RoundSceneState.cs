using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.RoundSceneState
{
    using Character;

    public class RoundSceneState : SceneStateBehavior<Void, ReadOnlyCollection<RoundSceneState.PlayerStatistics>>
    {
        private class CharacterInfo
        {
            private readonly RoundSceneState outer;
            private GameObject character;
            private CharacterPlayer characterPlayer;
            private CharacterControl characterControl;

            public CharacterInfo(RoundSceneState outer, Player player)
            {
                this.outer = outer;

                outer.characterInfos.Add(this);
                character = Instantiate(outer.characterPrefab, outer.spawnPoint.transform.position, Quaternion.identity);
                characterPlayer = character.GetComponent<CharacterPlayer>();
                characterPlayer.player = player;
                characterControl = character.GetComponent<CharacterControl>();
                characterControl.onEndedChanged += OnCharacterEndedChanged;

                UpdateOngoingInfos(characterControl.ended);
            }

            private void OnCharacterEndedChanged(CharacterControl charControl, bool ended)
            {
                UpdateOngoingInfos(ended);
            }

            private void UpdateOngoingInfos(bool ended)
            {
                if (ended)
                {
                    outer.ongoingCharacterInfos.Remove(this);
                    if (outer.ongoingCharacterInfos.Count == 0)
                    {
                        outer.OnAllCharacterEnded();
                    }
                }
                else
                {
                    outer.ongoingCharacterInfos.Add(this);
                }
            }

            public void Remove()
            {
                outer.result.Add(new PlayerStatistics(characterPlayer.player, characterControl.healthChanges, characterControl.won));

                characterControl.onEndedChanged -= OnCharacterEndedChanged;
                characterPlayer.player = null;
                Destroy(character);

                outer.characterInfos.Remove(this);
                outer.ongoingCharacterInfos.Remove(this);
            }
        }


        public class PlayerStatistics
        {
            public readonly Player player;
            public readonly ReadOnlyCollection<CharacterControl.HealthChange> healthChangeData;
            public readonly bool won;

            public PlayerStatistics(Player player, IEnumerable<CharacterControl.HealthChange> healthChangeData, bool won)
            {
                this.player = player;
                this.healthChangeData = new ReadOnlyCollection<CharacterControl.HealthChange>(healthChangeData.ToList());
                this.won = won;
            }
        }


        public GameObject characterPrefab;
        public Transform spawnPoint;

        
        private readonly List<CharacterInfo> characterInfos = new List<CharacterInfo>();
        private readonly HashSet<CharacterInfo> ongoingCharacterInfos = new HashSet<CharacterInfo>();
        private readonly List<PlayerStatistics> result = new List<PlayerStatistics>();


        protected override void OnActivate(ISceneState unloadedSceneState, object result)
        {
            base.OnActivate(unloadedSceneState, result);
            if (unloadedSceneState == null)
            {
                foreach (Player player in Player.players)
                {
                    new CharacterInfo(this, player);
                }
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            for (int i = characterInfos.Count - 1; i >= 0; --i)
            {
                characterInfos[i].Remove();
            }
        }

        protected override ReadOnlyCollection<PlayerStatistics> OnUnload()
        {
            base.OnUnload();
            var result = new ReadOnlyCollection<PlayerStatistics>(this.result.ToList());
            this.result.Clear();
            return result;
        }

        private void OnAllCharacterEnded()
        {
            SceneStateManager.instance.Pop(this);
        }
    }
}
