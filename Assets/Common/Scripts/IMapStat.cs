using System.Linq;
using System.Collections.Generic;

namespace APlusOrFail
{
    using Objects;

    public interface IReadOnlyMapStat : IMapSetting
    {
        IReadOnlyList<IReadonlyRoundStat> roundStats { get; }
        int roundCount { get; }

        IReadOnlyList<IReadOnlyPlayerStat> playerStats { get; }
        int playerCount { get; }

        IReadOnlyRoundPlayerStat GetRoundPlayerStat(int roundOrder, int playerOrder);

        int currentRound { get; }
    }

    public interface IMapStat : IReadOnlyMapStat
    {
        new IReadOnlyList<IRoundStat> roundStats { get; }

        new IReadOnlyList<IPlayerStat> playerStats { get; }

        new IRoundPlayerStat GetRoundPlayerStat(int roundOrder, int playerOrder);

        new int currentRound { get; set; }
    }


    public interface IReadonlyRoundStat : IRoundSetting
    {
        IReadOnlyMapStat mapStat { get; }

        int order { get; }
    }

    public interface IRoundStat : IReadonlyRoundStat
    {
        new IMapStat mapStat { get; }
    }


    public interface IReadOnlyPlayerStat
    {
        IReadOnlyMapStat mapStat { get; }

        int order { get; }
        Player player { get; }
    }

    public interface IPlayerStat : IReadOnlyPlayerStat
    {
        new IMapStat mapStat { get; }
    }


    public interface IReadOnlyRoundPlayerStat
    {
        IReadOnlyMapStat mapStat { get; }
        IReadonlyRoundStat roundStat { get; }
        IReadOnlyPlayerStat playerStat { get; }

        ObjectPrefabInfo selectedObjectPrefab { get; }
        bool won { get; }
        IReadOnlyList<IPlayerHealthChange> healthChanges { get; }
        IReadOnlyList<IPlayerScoreChange> scoreChanges { get; }
    }

    public interface IRoundPlayerStat : IReadOnlyRoundPlayerStat
    {
        new IMapStat mapStat { get; }
        new IRoundStat roundStat { get; }
        new IPlayerStat playerStat { get; }

        new ObjectPrefabInfo selectedObjectPrefab { get; set; }
        new bool won { get; set; }
        new IList<IPlayerHealthChange> healthChanges { get; }
        new IList<IPlayerScoreChange> scoreChanges { get; }
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
