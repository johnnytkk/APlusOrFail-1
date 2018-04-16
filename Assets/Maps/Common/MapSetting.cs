using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace APlusOrFail.Maps
{
    public abstract class MapSetting : MonoBehaviour, IMapSetting
    {
        protected class RoundSetting : IRoundSetting
        {
            public string name { get; private set; }
            public int roundScore { get; private set; }

            public RoundSetting(string name, int roundScore)
            {
                this.name = name;
                this.roundScore = roundScore;
            }
        }

        protected string mapName;
        string IMapSetting.name => mapName;
        public IReadOnlyList<IRoundSetting> roundSettings { get; protected set; }
    }
}
