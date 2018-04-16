using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace APlusOrFail.Maps.SceneStates.RankSceneState
{
    using UI;

    [RequireComponent(typeof(Image), typeof(FractionLayoutElement))]
    public class SubScore : MonoBehaviour
    {
        private Image image;
        private FractionLayoutElement layoutElement;

        private bool started;
        private IMapStat mapStat;
        private int playerOrder = -1;
        private int scoreOrder = -1;


        private void Start()
        {
            started = true;

            image = GetComponent<Image>();
            layoutElement = GetComponent<FractionLayoutElement>();

            UpdateSubScore(mapStat, playerOrder, scoreOrder);
            mapStat = null; playerOrder = -1; scoreOrder = -1;
        }

        public void UpdateSubScore(IMapStat mapStat, int playerOrder, int scoreOrder)
        {
            if (started)
            {
                gameObject.SetActive(mapStat != null);
                if (mapStat != null)
                {
                    IPlayerScoreChange stat = mapStat.GetRoundPlayerStatOfPlayer(playerOrder).SelectMany(rps => rps.scoreChanges).Skip(scoreOrder).First();
                    layoutElement.numeratorWidth = Mathf.Max(stat.scoreDelta, 0);
                    image.color = Random.ColorHSV(0, 1, 1, 1, 1, 1);
                }
            }
            else
            {
                this.mapStat = mapStat;
                this.playerOrder = playerOrder;
                this.scoreOrder = scoreOrder;
            }
        }
    }
}
