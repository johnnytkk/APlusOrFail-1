using System.Linq;
using System.Collections.Generic;

namespace APlusOrFail
{
    using Objects;

    public interface IMapStat : IMapSetting
    {
        IReadOnlyList<IRoundStat> roundStats { get; }
        int roundCount { get; }

        IReadOnlyList<IPlayerStat> playerStats { get; }
        int playerCount { get; }

        IRoundPlayerStat GetRoundPlayerStat(int roundOrder, int playerOrder);

        int currentRound { get; }
    }

    public interface IRoundStat : IRoundSetting
    {
        IMapStat mapStat { get; }

        int order { get; }
    }

    public interface IPlayerStat
    {
        IMapStat mapStat { get; }

        int order { get; }
        Player player { get; }
    }

    public interface IRoundPlayerStat
    {
        IMapStat mapStat { get; }
        IRoundStat roundStat { get; }
        IPlayerStat playerStat { get; }

        ObjectPrefabInfo selectedObjectPrefab { get; }
        bool won { get; }
        IReadOnlyList<IPlayerHealthChange> healthChanges { get; }
        IReadOnlyList<IPlayerScoreChange> scoreChanges { get; }
    }

    public interface IPlayerHealthChange
    {
        int healthDelta { get; }
    }

    public interface IPlayerScoreChange
    {
        int scoreDelta { get; }
    }

    public static class MapStatExtensions
    {
        public static IEnumerable<IRoundPlayerStat> GetRoundPlayerStatOfRound(this IMapStat mapStat, int roundOrder)
        {
            return Enumerable.Range(0, mapStat.playerCount).Select(i => mapStat.GetRoundPlayerStat(roundOrder, i));
        }

        public static IEnumerable<IRoundPlayerStat> GetRoundPlayerStatOfPlayer(this IMapStat mapStat, int playerOrder)
        {
            return Enumerable.Range(0, mapStat.roundCount).Select(i => mapStat.GetRoundPlayerStat(i, playerOrder));
        }
    }
}
