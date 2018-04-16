using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.RoundSceneState
{
    using Character;

    public class RoundSceneState : SceneStateBehavior<MapStat, Void>
    {
        public CharacterControl characterPrefab;
        public Transform spawnPoint;
        
        private readonly HashSet<CharacterControl> notEndedCharControls = new HashSet<CharacterControl>();
        private readonly HashSet<CharacterControl> endedCharControls = new HashSet<CharacterControl>();


        protected override void OnActivate(ISceneState unloadedSceneState, object result)
        {
            base.OnActivate(unloadedSceneState, result);

            if (unloadedSceneState == null)
            {
                foreach (Player player in (from ps in arg.playerStatList select ps.player))
                {
                    CharacterControl charControl = Instantiate(characterPrefab, spawnPoint.position, characterPrefab.transform.rotation);
                    CharacterPlayer charPlayer = charControl.GetComponent<CharacterPlayer>();

                    charControl.onEndedChanged += OnCharEnded;
                    charPlayer.player = player;

                    if (charControl.ended)
                    {
                        endedCharControls.Add(charControl);
                    }
                    else
                    {
                        notEndedCharControls.Add(charControl);
                    }
                }
                if (notEndedCharControls.Count == 0)
                {
                    OnAllCharacterEnded();
                }
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            
            foreach (CharacterControl charControl in notEndedCharControls.Concat(endedCharControls))
            {
                Player player = charControl.GetComponent<CharacterPlayer>().player;
                RoundPlayerStat roundPlayerStat = arg.GetRoundPlayerStat(arg.currentRound, arg.playerStatList.FindIndex(ps => ps.player == player));

                if (charControl.won)
                {
                    charControl.ChangeScore(new CharacterControl.ScoreChange(30));
                }

                roundPlayerStat.healthChangeList.AddRange(charControl.healthChanges);
                roundPlayerStat.scoreChangeList.AddRange(charControl.scoreChanges);

                charControl.onEndedChanged -= OnCharEnded;
                Destroy(charControl.gameObject);
            }
            notEndedCharControls.Clear();
            endedCharControls.Clear();
        }

        private void OnCharEnded(CharacterControl charControl, bool ended)
        {
            if (ended)
            {
                endedCharControls.Add(charControl);
                notEndedCharControls.Remove(charControl);
            }
            else
            {
                endedCharControls.Remove(charControl);
                notEndedCharControls.Add(charControl);
            }

            if (notEndedCharControls.Count == 0)
            {
                OnAllCharacterEnded();
            }
        }

        private void OnAllCharacterEnded()
        {
            SceneStateManager.instance.Pop(this);
        }
    }
}
