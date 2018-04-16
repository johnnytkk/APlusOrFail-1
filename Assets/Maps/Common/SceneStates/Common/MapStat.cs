using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace APlusOrFail.Maps.SceneStates
{
    using Objects;

    public class MapStat : IMapStat
    {
        public string name { get; }
        IReadOnlyList<IRoundSetting> IMapSetting.roundSettings => ((IMapStat)this).roundStats;

        public readonly List<RoundStat> roundStatList = new List<RoundStat>();
        private readonly IReadOnlyList<IRoundStat> _roundStats;
        IReadOnlyList<IRoundStat> IMapStat.roundStats => _roundStats;
        public int roundCount => roundStatList.Count;

        public readonly List<PlayerStat> playerStatList = new List<PlayerStat>();
        private readonly IReadOnlyList<IPlayerStat> _playerStats;
        IReadOnlyList<IPlayerStat> IMapStat.playerStats => _playerStats;
        public int playerCount => playerStatList.Count;

        public RoundPlayerStat[,] roundPlayerStats;
        IRoundPlayerStat IMapStat.GetRoundPlayerStat(int roundOrder, int playerOrder) => GetRoundPlayerStat(roundOrder, playerOrder);
        public RoundPlayerStat GetRoundPlayerStat(int roundOrder, int playerOrder) => roundPlayerStats[roundOrder, playerOrder];

        public int currentRound { get; set; } = -1;

        public MapStat(IMapSetting mapSetting)
        {
            name = mapSetting.name;
            _roundStats = new ReadOnlyCollection<RoundStat>(roundStatList);
            _playerStats = new ReadOnlyCollection<PlayerStat>(playerStatList);

            roundPlayerStats = new RoundPlayerStat[mapSetting.roundSettings.Count, Player.players.Count];

            for (int i = 0; i < mapSetting.roundSettings.Count; ++i)
            {
                roundStatList.Add(new RoundStat(this, i, mapSetting.roundSettings[i]));
            }

            for (int j = 0; j < Player.players.Count; ++j)
            {
                playerStatList.Add(new PlayerStat(this, j, Player.players[j]));
            }

            for (int i = 0; i < mapSetting.roundSettings.Count; ++i)
            {
                for (int j = 0; j < Player.players.Count; ++j)
                {
                    roundPlayerStats[i, j] = new RoundPlayerStat(this, roundStatList[i], playerStatList[j]);
                }
            }
        }
    }

    public class RoundStat : IRoundStat
    {
        public IMapStat mapStat { get; }

        public int order { get; }
        public string name { get; }
        public int roundScore { get; }

        public RoundStat(IMapStat mapStat, int order, IRoundSetting roundSetting)
        {
            this.mapStat = mapStat;
            this.order = order;
            name = roundSetting.name;
            roundScore = roundSetting.roundScore;
        }
    }

    public class PlayerStat : IPlayerStat
    {
        public IMapStat mapStat { get; }

        public int order { get; }
        public Player player { get; }

        public PlayerStat(IMapStat mapStat, int order, Player player)
        {
            this.mapStat = mapStat;
            this.order = order;
            this.player = player;
        }
    }

    public class RoundPlayerStat : IRoundPlayerStat
    {
        public IMapStat mapStat { get; }
        public IRoundStat roundStat { get; }
        public IPlayerStat playerStat { get; }

        public ObjectPrefabInfo selectedObjectPrefab { get; set; }
        public bool won { get; set; }

        public List<IPlayerHealthChange> healthChangeList = new List<IPlayerHealthChange>();
        private IReadOnlyList<IPlayerHealthChange> _healthChanges;
        IReadOnlyList<IPlayerHealthChange> IRoundPlayerStat.healthChanges => _healthChanges;

        public List<IPlayerScoreChange> scoreChangeList = new List<IPlayerScoreChange>();
        private IReadOnlyList<IPlayerScoreChange> _scoreChanges;
        IReadOnlyList<IPlayerScoreChange> IRoundPlayerStat.scoreChanges => _scoreChanges;

        public RoundPlayerStat(IMapStat mapStat, IRoundStat roundStat, IPlayerStat playerStat)
        {
            this.mapStat = mapStat;
            this.roundStat = roundStat;
            this.playerStat = playerStat;
            _healthChanges = new ReadOnlyCollection<IPlayerHealthChange>(healthChangeList);
            _scoreChanges = new ReadOnlyCollection<IPlayerScoreChange>(scoreChangeList);
        }
    }
}
