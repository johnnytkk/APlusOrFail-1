using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.RoundSceneState
{
    using Character;
    using Objects;
    using Components.NameTag;

    public class RoundSceneState : SceneStateBehavior<IMapStat, Void>
    {
        public CharacterControl characterPrefab;
        public RectTransform canvasRectTransform;
        public NameTag nameTagPrefab;
        
        private readonly HashSet<CharacterControl> notEndedCharControls = new HashSet<CharacterControl>();
        private readonly HashSet<CharacterControl> endedCharControls = new HashSet<CharacterControl>();
        private readonly List<NameTag> nameTags = new List<NameTag>();


        protected override Task OnFocus(ISceneState unloadedSceneState, object result)
        {
            if (unloadedSceneState == null)
            {
                canvasRectTransform.gameObject.SetActive(true);

                ObjectGridPlacer spawnArea = arg.roundSettings[arg.currentRound].spawnArea;
                RectInt bound = spawnArea.GetComponentsInChildren<ObjectGridRect>()
                    .GetLocalRects()
                    .Rotate(spawnArea.rotation)
                    .Move(spawnArea.gridPosition)
                    .GetInnerBound();
                Vector2 spawnPoint = MapManager.mapStat.mapArea.LocalToWorldPosition(bound.center);

                int i = 0;
                foreach (Player player in (from ps in arg.playerStats select ps.player))
                {
                    CharacterControl charControl = Instantiate(characterPrefab, spawnPoint, characterPrefab.transform.rotation);
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


                    NameTag nameTag;
                    if (i >= nameTags.Count)
                    {
                        nameTag = Instantiate(nameTagPrefab, canvasRectTransform);
                        nameTag.camera = arg.camera.GetComponent<Camera>();
                        nameTag.canvasRectTransform = canvasRectTransform;
                        nameTags.Add(nameTag);
                    }
                    else
                    {
                        nameTag = nameTags[i];
                    }
                    nameTag.charPlayer = charPlayer;


                    arg.camera.AddTracingSprite(charControl.gameObject);


                    ++i;
                }
                for (int j = i; j < nameTags.Count; ++j)
                {
                    nameTags[j].charPlayer = null;
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
            canvasRectTransform.gameObject.SetActive(false);

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

            foreach (NameTag nameTag in nameTags)
            {
                nameTag.charPlayer = null;
            }

            arg.camera.UntraceAll();

            return Task.CompletedTask;
        }

        private void OnCharEnded(CharacterControl charControl, bool ended)
        {
            if (ended)
            {
                endedCharControls.Add(charControl);
                notEndedCharControls.Remove(charControl);
                arg.camera.RemoveTracingSprite(charControl.gameObject);
            }
            else
            {
                endedCharControls.Remove(charControl);
                notEndedCharControls.Add(charControl);
                arg.camera.AddTracingSprite(charControl.gameObject);
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
