using UnityEngine;
using UnityEngine.UI;

namespace APlusOrFail.Maps.SceneStates.RankSceneState
{
    using UI;

    public class Region : MonoBehaviour
    {
        public Text nameText;
        private FractionLayoutElement layoutElement;

        private bool started;
        private IRoundStat roundStat;

        private void Start()
        {
            started = true;
            layoutElement = GetComponent<FractionLayoutElement>();
            UpdateRegion(roundStat);
            roundStat = null;
        }

        public void UpdateRegion(IRoundStat roundStat)
        {
            if (started)
            {
                gameObject.SetActive(roundStat != null);
                nameText.text = roundStat?.name ?? "";
                layoutElement.numeratorWidth = roundStat?.roundScore ?? 0;
            }
            else
            {
                this.roundStat = roundStat;
            }
        }
    }
}
