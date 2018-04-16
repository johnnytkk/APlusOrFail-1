using System;
using UnityEngine;
using System.Collections;

namespace APlusOrFail.Maps.Map0
{
    public class Map0Setting : MapSetting
    {
        private void Awake()
        {
            mapName = "CBA";
            roundSettings = Array.AsReadOnly(new IRoundSetting[]
            {
                new RoundSetting("Ask the professor", 50),
                new RoundSetting("Capture the professor", 50)
            });
        }
    }
}
