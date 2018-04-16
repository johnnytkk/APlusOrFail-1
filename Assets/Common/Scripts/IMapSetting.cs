using System.Collections.Generic;
using System.Linq;

namespace APlusOrFail
{
    public interface IMapSetting
    {
        string name { get; }
        IReadOnlyList<IRoundSetting> roundSettings { get; }
    }

    public interface IRoundSetting
    {
        string name { get; }
        int roundScore { get; }
    }

    public static class MapSettingExtensions
    {
        public static int GetMapScore(this IMapSetting mapStat)
        {
            return mapStat.roundSettings.Sum(s => s.roundScore);
        }
    }
}
