using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace APlusOrFail.Maps
{
    using Objects;
    using Components.AutoResizeCamera;

    public class MapStat : IMapStat
    {
        public string name { get; }
        IReadOnlyList<IRoundSetting> IMapSetting.roundSettings => roundStats;
        public MapArea mapArea { get; }
        public AutoResizeCamera camera { get; }
        
        public IReadOnlyList<IRoundStat> roundStats { get; }
        IReadOnlyList<IReadonlyRoundStat> IReadOnlyMapStat.roundStats => roundStats;
        public int roundCount => roundStats.Count;

        public IReadOnlyList<IPlayerStat> playerStats { get; }
        IReadOnlyList<IReadOnlyPlayerStat> IReadOnlyMapStat.playerStats => playerStats;
        public int playerCount => playerStats.Count;

        private IRoundPlayerStat[,] roundPlayerStats;
        public IRoundPlayerStat GetRoundPlayerStat(int roundOrder, int playerOrder) => roundPlayerStats[roundOrder, playerOrder];
        IReadOnlyRoundPlayerStat IReadOnlyMapStat.GetRoundPlayerStat(int roundOrder, int playerOrder) => GetRoundPlayerStat(roundOrder, playerOrder);

        public int currentRound { get; set; } = -1;

        public MapStat(IMapSetting mapSetting)
        {
            name = mapSetting.name;
            mapArea = mapSetting.mapArea;
            camera = mapSetting.camera;

            List<IRoundStat> roundStats = new List<IRoundStat>(mapSetting.roundSettings.Count);
            for (int i = 0; i < mapSetting.roundSettings.Count; ++i)
            {
                roundStats.Add(new RoundStat(this, i, mapSetting.roundSettings[i]));
            }
            this.roundStats = new ReadOnlyCollection<IRoundStat>(roundStats);

            List<IPlayerStat> playerStats = new List<IPlayerStat>(Player.players.Count);
            for (int j = 0; j < Player.players.Count; ++j)
            {
                playerStats.Add(new PlayerStat(this, j, Player.players[j]));
            }
            this.playerStats = new ReadOnlyCollection<IPlayerStat>(playerStats);

            roundPlayerStats = new RoundPlayerStat[mapSetting.roundSettings.Count, Player.players.Count];
            for (int i = 0; i < mapSetting.roundSettings.Count; ++i)
            {
                for (int j = 0; j < Player.players.Count; ++j)
                {
                    roundPlayerStats[i, j] = new RoundPlayerStat(this, roundStats[i], playerStats[j]);
                }
            }
        }
    }

    public class RoundStat : IRoundStat
    {
        public IMapStat mapStat { get; }
        IReadOnlyMapStat IReadonlyRoundStat.mapStat => mapStat;
        public ObjectGridPlacer spawnArea { get; }

        public int order { get; }
        public string name { get; }
        public int roundScore { get; }
        public IReadOnlyList<ObjectPrefabInfo> usableObjects { get; }
        public RoundState state { get; set; } = RoundState.None;


        public RoundStat(IMapStat mapStat, int order, IRoundSetting roundSetting)
        {
            this.mapStat = mapStat;
            this.order = order;
            name = roundSetting.name;
            roundScore = roundSetting.roundScore;
            usableObjects = new ReadOnlyCollection<ObjectPrefabInfo>(roundSetting.usableObjects.ToList());
            spawnArea = roundSetting.spawnArea;
        }
    }

    public class PlayerStat : IPlayerStat
    {
        public IMapStat mapStat { get; }
        IReadOnlyMapStat IReadOnlyPlayerStat.mapStat => mapStat;

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
        IReadOnlyMapStat IReadOnlyRoundPlayerStat.mapStat => mapStat;

        public IRoundStat roundStat { get; }
        IReadonlyRoundStat IReadOnlyRoundPlayerStat.roundStat => roundStat;

        public IPlayerStat playerStat { get; }
        IReadOnlyPlayerStat IReadOnlyRoundPlayerStat.playerStat => playerStat;

        public ObjectPrefabInfo selectedObjectPrefab { get; set; }
        public bool won { get; set; }

        public IList<IPlayerHealthChange> healthChanges { get; } = new List<IPlayerHealthChange>();
        private IReadOnlyList<IPlayerHealthChange> _readonlyHealthChanges;
        IReadOnlyList<IPlayerHealthChange> IReadOnlyRoundPlayerStat.healthChanges => _readonlyHealthChanges;

        public IList<IPlayerScoreChange> scoreChanges { get; } = new List<IPlayerScoreChange>();
        private IReadOnlyList<IPlayerScoreChange> _readonlyScoreChanges;
        IReadOnlyList<IPlayerScoreChange> IReadOnlyRoundPlayerStat.scoreChanges => _readonlyScoreChanges;

        public RoundPlayerStat(IMapStat mapStat, IRoundStat roundStat, IPlayerStat playerStat)
        {
            this.mapStat = mapStat;
            this.roundStat = roundStat;
            this.playerStat = playerStat;
            _readonlyHealthChanges = new ReadOnlyCollection<IPlayerHealthChange>(healthChanges);
            _readonlyScoreChanges = new ReadOnlyCollection<IPlayerScoreChange>(scoreChanges);
        }
    }
}
