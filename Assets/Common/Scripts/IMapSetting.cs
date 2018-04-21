using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APlusOrFail
{
    using Objects;
    using Components.AutoResizeCamera;

    public interface IMapSetting
    {
        string name { get; }
        IReadOnlyList<IRoundSetting> roundSettings { get; }
        MapArea mapArea { get; }
        AutoResizeCamera camera { get; }
    }

    public interface IRoundSetting
    {
        string name { get; }
        int roundScore { get; }
        IReadOnlyList<ObjectPrefabInfo> usableObjects { get; }
        ObjectGridPlacer spawnArea { get; }
    }

    public static class MapSettingExtensions
    {
        public static int GetMapScore(this IMapSetting mapStat)
        {
            return mapStat.roundSettings.Sum(s => s.roundScore);
        }
    }
}
