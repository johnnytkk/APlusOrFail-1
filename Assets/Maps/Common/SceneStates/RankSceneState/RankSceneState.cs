using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.RankSceneState
{
    public class RankSceneState : SceneStateBehavior<IMapStat, Void>
    {
        public Canvas canvas;
        public Regions regions;
        public PlayerScores playerScores;

        private readonly List<Player> waitingPlayers = new List<Player>();
        
        private void Start()
        {
            HideUI();
        }

        protected override Task OnFocus(ISceneState unloadedSceneState, object result)
        {
            if (unloadedSceneState == null)
            {
                waitingPlayers.AddRange(arg.playerStats.Select(ps => ps.player));
                ShowUI();
            }
            return Task.CompletedTask;
        }

        protected override Task OnBlur()
        {
            HideUI();
            return Task.CompletedTask;
        }

        private void Update()
        {
            if (phase.IsAtLeast(SceneStatePhase.Focused))
            {
                for (int i = waitingPlayers.Count - 1; i >= 0; --i)
                {
                    bool ok = HasKeyUp(waitingPlayers[i], Player.Action.Action1);
                    if (ok)
                    {
                        waitingPlayers.RemoveAt(i);
                    }
                }

                if (waitingPlayers.Count == 0)
                { 
                    SceneStateManager.instance.Pop(this, null);
                }
            }
        }

        private void ShowUI()
        {
            canvas.gameObject.SetActive(true);
            regions.UpdateRegions(arg);
            playerScores.UpdatePlayerScoreList(arg);
        }

        private void HideUI()
        {
            canvas.gameObject.SetActive(false);
        }

        private bool HasKeyUp(Player player, Player.Action action)
        {
            KeyCode? code = player.GetKeyForAction(action);
            return code != null && Input.GetKeyUp(code.Value);
        }
    }
}
