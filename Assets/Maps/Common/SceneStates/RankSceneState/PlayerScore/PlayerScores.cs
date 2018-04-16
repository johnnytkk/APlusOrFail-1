using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APlusOrFail.Maps.SceneStates.RankSceneState
{
    public class PlayerScores : MonoBehaviour
    {
        private struct NormalizedPlayerStat
        {
            public Player player;
            public int score;
        }


        public PlayerScore playerScorePrefab;

        private List<PlayerScore> playerScoreChildren = new List<PlayerScore>();


        private bool started;
        private IMapStat mapStat;

        private void Start()
        {
            started = true;
            UpdatePlayerScoreList(mapStat);
            mapStat = null;
        }

        public void UpdatePlayerScoreList(IMapStat mapStat)
        {
            if (started)
            {
                int i = 0;
                if (mapStat != null)
                {
                    for (; i < mapStat.playerCount; ++i)
                    {
                        PlayerScore playerScore;
                        if (i < playerScoreChildren.Count)
                        {
                            playerScore = playerScoreChildren[i];
                        }
                        else
                        {
                            playerScore = Instantiate(playerScorePrefab, transform);
                            playerScoreChildren.Add(playerScore);
                        }
                        
                        playerScore.UpdatePlayerScore(mapStat, i);
                    }
                }
                for (int j = i; j < playerScoreChildren.Count; ++j)
                {
                    playerScoreChildren[j].UpdatePlayerScore(null, -1);
                }
            }
            else
            {
                this.mapStat = mapStat;
            }
        }
    }
}
