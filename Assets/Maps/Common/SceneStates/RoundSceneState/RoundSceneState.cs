using System.Collections.Generic;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.RoundSceneState
{
    using Character;

    public class RoundSceneState : SceneState
    {

        public class CharacterInfo
        {

            private readonly RoundSceneState enclosing;
            private GameObject character;

            public CharacterInfo(RoundSceneState enclosing, Player player)
            {
                this.enclosing = enclosing;
                enclosing.characterInfos.Add(this);

                character = Instantiate(enclosing.characterPrefab, enclosing.characterPrefab.transform.position, Quaternion.identity);
                character.GetComponent<CharacterPlayer>().player = player;
                character.GetComponent<CharacterHealth>().onHealthChanged += OnCharacterHealthChanged;
            }

            public void Update()
            {

            }

            private void OnCharacterHealthChanged(CharacterHealth charHealth, int health)
            {
                if (enclosing.state.IsAtLeast(State.Activated))
                {
                    if (health <= 0)
                    {
                        Remove();
                    }
                }
            }

            public void Remove()
            {
                character.GetComponent<CharacterPlayer>().player = null;
                character.GetComponent<CharacterHealth>().onHealthChanged -= OnCharacterHealthChanged;
                enclosing.characterInfos.Remove(this);
            }
        }

        public GameObject characterPrefab;
        public Transform spawnPoint;
        
        public readonly List<CharacterInfo> characterInfos = new List<CharacterInfo>();

        protected override void OnActivate()
        {
            base.OnActivate();
            foreach (Player player in Player.players)
            {
                new CharacterInfo(this, player);
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            foreach (CharacterInfo info in characterInfos)
            {
                info.Remove();
            }
        }
    }
}
