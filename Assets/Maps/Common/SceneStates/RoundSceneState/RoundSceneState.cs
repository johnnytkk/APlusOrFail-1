using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.RoundSceneState
{
    using Character;
    using Objects;
    using ObjectGrid;

    public class RoundSceneState : SceneStateBehavior<IMapStat, Void>
    {
        public CharacterControl characterPrefab;
        
        private readonly HashSet<CharacterControl> notEndedCharControls = new HashSet<CharacterControl>();
        private readonly HashSet<CharacterControl> endedCharControls = new HashSet<CharacterControl>();


        protected override Task OnFocus(ISceneState unloadedSceneState, object result)
        {
            if (unloadedSceneState == null)
            {
                ObjectGridPlacer spawnArea = arg.roundSettings[arg.currentRound].spawnArea;
                Rect bound = ObjectGrid.instance.GridToWorldRect(spawnArea.GetComponentsInChildren<ObjectGridRect>()
                    .GetLocalRects()
                    .Rotate(spawnArea.rotation)
                    .Move(spawnArea.gridPosition)
                    .GetInnerBound());

                foreach (Player player in (from ps in arg.playerStats select ps.player))
                {
                    CharacterControl charControl = Instantiate(characterPrefab, bound.center, characterPrefab.transform.rotation);
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
            return Task.CompletedTask;
        }

        protected override Task OnBlur()
        {
            foreach (CharacterControl charControl in notEndedCharControls.Concat(endedCharControls))
            {
                Player player = charControl.GetComponent<CharacterPlayer>().player;
                IRoundPlayerStat roundPlayerStat = arg.GetRoundPlayerStat(arg.currentRound, arg.playerStats.FindIndex(ps => ps.player == player));

                if (charControl.won)
                {
                    charControl.ChangeScore(new CharacterControl.ScoreChange(30));
                }

                roundPlayerStat.healthChanges.AddRange(charControl.healthChanges);
                roundPlayerStat.scoreChanges.AddRange(charControl.scoreChanges);

                charControl.onEndedChanged -= OnCharEnded;
                Destroy(charControl.gameObject);
            }
            notEndedCharControls.Clear();
            endedCharControls.Clear();

            return Task.CompletedTask;
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
            SceneStateManager.instance.Pop(this, null);
        }
    }
}
